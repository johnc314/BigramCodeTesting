using System;
using System.Linq;
using ParsingService.Services;

namespace BigramConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("No input provided.");
                Console.ReadLine();
                return;
            }
            if (args.Count() > 1)
            {
                Console.WriteLine("Please use quotes to contain input.");
                Console.ReadLine();
                return;
            }

            var input = args.First();

            var output = "";
            try { 

                if (!System.IO.File.Exists(input)) //consider moving file handling to it's own service
                {
                    output = BigramParsing.ParseText(input).GetAwaiter().GetResult();
                }
                ////possibly check file permissions
                else
                {
                    output = BigramParsing.ParseFile(input).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                output = "Could not process the file.\r\nSee error message below.\r\n" + ex.Message;
            }

            Console.WriteLine(output);
            Console.ReadLine();
        }
    }
}
