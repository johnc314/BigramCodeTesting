using System;
using System.Windows.Forms;
using ParsingService.Services;

namespace BigramParsingJtc
{
    public partial class main_form : Form
    {
        public main_form()
        {
            InitializeComponent();
        }

        private async void Parse_button_click(object sender, EventArgs e)
        {
            var fileInput = FileInput_radio.Checked;
            var textInput = TextInput_radio.Checked;
            var input = Input_text.Text;

            var output = "";
            if (string.IsNullOrWhiteSpace(input))
            {
                output = "Please provide input before attempting to parse";
            }
            else if (fileInput)
            {

                try
                {
                    if (!System.IO.File.Exists(input))//consider moving file handling to it's own service
                    {
                        output = "File could not be found.";
                    }
                    //possibly check file permissions
                    //if this was a public job I'd want to put in file sniffing functionality to respond to that in a more controlled fashion
                    else
                    {
                        output = await BigramParsing.ParseFile(input);
                    }
                }
                catch (Exception ex)
                {
                    output = "Could not process the file.\r\nSee error message below.\r\n" + ex.Message;
                }
            }
            else if (textInput)
            {
                output = await ParsingService.Services. BigramParsing.ParseText(input);
            }

            Output_text.Text = output;
        }

        private void FileInput_radio_CheckedChanged(object sender, EventArgs e)
        {
            Input_text.Text = "";
            SelectFile_button.Visible = true;
        }

        private void TextInput_radio_CheckedChanged(object sender, EventArgs e)
        {
            Input_text.Text = "";
            SelectFile_button.Visible = false;
        }

        private void SelectFile_button_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;

                    Input_text.Text = filePath;
                }
            }
        }
    }
}
