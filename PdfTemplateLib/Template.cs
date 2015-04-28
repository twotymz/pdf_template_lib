using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace PdfTemplateLib
{
    public class Template
    {
        List<Command> commands;

        public List<Command> Commands { get { return commands; } }

        public Template()
        {
            commands = new List<Command>();
        }

        public bool Read (string templatePath)
        {            
            try
            {
                String text = File.ReadAllText(templatePath);
                SetText(text);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        public void SetText (string text)
        {
            string[] lines = Regex.Split(text, "\n");

            foreach (string l in lines)
            {
                AddLine(l);
            }
        }
        
        public void AddLine (string line)
        {
            if (line.Length == 0 || line[0] == '#')
            {
                return;
            }

            string[] tokens = line.Split();
            int line_no;

            // This is a line definition.
            if (int.TryParse(tokens[0], out line_no))
            {
                if (tokens.Length - 1 > 0)
                {
                    string pattern = string.Join(" ", tokens, 1, tokens.Length - 1);
                    this.commands.Add(new Command { Instruction = tokens[0], Args = pattern });
                }
            }
            else
            {
                string val = string.Join(" ", tokens, 1, tokens.Length - 1);
                switch (tokens[0])
                {                    
                    case "headliner":
                    case "venue":
                    case "date":
                    case "time":
                    case "section":
                    case "row":
                    case "path":
                    case "axs":
                    case "rotate":
                    case "save_bitmap":
                    case "crop":
                    case "rectangle":
                    case "greyscale":
                    case "reset" :
                    case "text" :
                    case "save_text" :
                    case "ocr" :
                    case "dpi" :
                        this.commands.Add(new Command { Instruction = tokens[0], Args = val });
                        break;
                }
            }
        }
    }
}
