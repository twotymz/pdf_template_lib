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
                    this.commands.Add(new Command { Code = Command.Opcode.OP_REGEX, Args = pattern });
                }
            }
            else
            {
                Command.Opcode opcode;
                switch (tokens[0])
                {                    
                    case "headliner":
                        opcode = Command.Opcode.OP_HEADLINER;
                        break;
                    case "venue":
                        opcode = Command.Opcode.OP_VENUE;
                        break;
                    case "date":
                        opcode = Command.Opcode.OP_DATE;
                        break;
                    case "time":
                        opcode = Command.Opcode.OP_TIME;
                        break;
                    case "section":
                        opcode = Command.Opcode.OP_SECTION;
                        break;
                    case "row":
                        opcode = Command.Opcode.OP_ROW;
                        break;
                    case "path":
                        opcode = Command.Opcode.OP_PATH;
                        break;
                    case "axs":
                        opcode = Command.Opcode.OP_AXS;
                        break;
                    case "rotate":
                        opcode = Command.Opcode.OP_ROTATE;
                        break;
                    case "save_bitmap":
                        opcode = Command.Opcode.OP_SAVE_BITMAP;
                        break;
                    case "crop":
                        opcode = Command.Opcode.OP_CROP;
                        break;
                    case "rectangle":
                        opcode = Command.Opcode.OP_RECTANGLE;
                        break;
                    case "greyscale":
                        opcode = Command.Opcode.OP_GREYSCALE;
                        break;
                    case "reset" :
                        opcode = Command.Opcode.OP_RESET;
                        break;
                    case "text" :
                        opcode = Command.Opcode.OP_TEXT;
                        break;
                    case "save_text" :
                        opcode = Command.Opcode.OP_SAVE_TEXT;
                        break;
                    case "ocr" :
                        opcode = Command.Opcode.OP_OCR;
                        break;
                    case "dpi" :
                        opcode = Command.Opcode.OP_DPI;
                        break;
                }
            }
        }
    }
}
