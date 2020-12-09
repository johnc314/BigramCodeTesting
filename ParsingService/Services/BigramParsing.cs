using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ParsingService.Models;

namespace ParsingService.Services
{

    //I'd want to break this up more to make it more SOLIDr but 
    //I'm treating this like a quick server side job that can be refactored in the future.
    public static class BigramParsing
    {
        public async static Task<string> ParseText(string input)
        {
            var existingHistograms = new List<HistogramEntry>();
            string[] words = SplitText(input);
            List<HistogramEntry> histograms = await ParseHistograms(existingHistograms, words, "");
            string result = RenderOutput(histograms);
            return result;
        }
        //This method was exceptionally faster in testing although it is much more memory intensive.
        public async static Task<string> ParseFile(string input)
        {
            var existingHistograms = new List<HistogramEntry>();
            var file = System.IO.File.ReadAllText(input);
            string[] words = SplitText(file);
            List<HistogramEntry> histograms = await ParseHistograms(existingHistograms, words, "");
            string result = RenderOutput(histograms);
            return result;
        }
        //slower but a much leaner footprint
        [Obsolete]
        public async static Task<string> ParseFileStream(string path)
        {

            var lastWord = "";
            List<HistogramEntry> histograms = new List<HistogramEntry>();
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            var line = await file.ReadLineAsync();
            while (line != null)
            {

                string[] words = SplitText(line);
                histograms = await ParseHistograms(histograms, words, lastWord);
                lastWord = words.LastOrDefault();
                line = file.ReadLine();
            }
            var result = RenderOutput(histograms);
            return result;
        }

        private static string[] SplitTextRegex(string input)
        {
            var words = Regex.Split(input.ToLower(), @"\W|\s|\n|\r|\t|\W|\e|\f", 
                RegexOptions.Multiline, 
                Regex.InfiniteMatchTimeout);

            return words;
        }
        private static string[] SplitText(string input)
        {
            char[] splitChars = (@"`~!@#$%^&*()_+-{}:<>?,./;'[]" + "\" \t\r\n").ToCharArray();
            var words = input.ToLower().Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            return words;
        }

        private static string RenderOutput(List<HistogramEntry> histograms)
        {
            var result = "";
            foreach (var histogram in histograms)
            {
                result += $"{histogram.Bigram}: {histogram.Count}\r\n";
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                result = "Input not long enough to create a bigram.";
            }

            return result;
        }

        private async static Task<List<HistogramEntry>> ParseHistograms(IEnumerable<HistogramEntry> existingHistograms, string[] words, string lastWord)
        {
            var bigrams = new List<string>();
            if (!string.IsNullOrWhiteSpace(lastWord) && words.Count() > 0)
            {
                bigrams.Add($"{lastWord} {words[0]}");
            }

            for (int i = 1; i < words.Count(); i++)
            {
                bigrams.Add($"{words[i - 1]} {words[i]}");
            }

            var histograms = bigrams
                .GroupBy(b => b)
                .Select(b => new HistogramEntry { Bigram = b.Key, Count = b.Count() })
                .Union(existingHistograms)
                .GroupBy(b=> b.Bigram)
                .Select(b => new HistogramEntry { Bigram = b.Key, Count = b.Sum(c=> c.Count) })
                .OrderByDescending(b => b.Count)
                .ThenBy(b => b.Bigram)
                .ToList();
            return histograms;
        }

    }
}
