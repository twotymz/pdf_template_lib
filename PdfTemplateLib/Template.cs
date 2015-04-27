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
        List<Command> text_commands;
        List<Command> transform_commands;

        public Template()
        {
            text_commands = new List<Command>();
            transform_commands = new List<Command>();
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
                if (l.Length == 0 || l[0] == '#')
                {
                    continue;
                }

                string[] tokens = l.Split();
                int line_no;

                // This is a line definition.
                if (int.TryParse(tokens[0], out line_no))
                {
                    if (tokens.Length - 1 > 0)
                    {
                        string pattern = string.Join(" ", tokens, 1, tokens.Length - 1);
                        this.text_commands.Add(new Command { Instruction = tokens[0], Args = pattern });
                    }
                }
                else
                {
                    string val = string.Join(" ", tokens, 1, tokens.Length - 1);
                    switch (tokens[0])
                    {
                        case "headliner" :
                        case "venue" :
                        case "date" :
                        case "time" :
                        case "section" :
                        case "row" :
                        case "path" :
                        case "axs" :
                            this.text_commands.Add(new Command { Instruction = tokens[0], Args = val });
                            break;
                    }
                }
            }
        }

        public Result Run (string pdfPath)
        {
            Result results = new Result();

            foreach (Command command in this.text_commands)
            {
                int line_no;

                if (int.TryParse(command.Instruction, out line_no))
                {
                    process_regex(pdfLines[line_no], command.Args, ref results);
                }
                else
                {
                    switch (command.Instruction)
                    {
                        case "path" :
                            process_regex(pdfPath, command.Args, ref results);
                            break;
                        case "headliner" :
                            results.Headliner = command.Args;
                            break;
                        case "venue" :
                            results.Venue = command.Args;
                            break;
                        case "date" :
                            results.Date = command.Args;
                            break;
                        case "time" :
                            results.Time = command.Args;
                            break;
                        case "section" :
                            results.Section = command.Args;
                            break;
                        case "row" :
                            results.Row = command.Args;
                            break;
                    }
                }
            }

            return results;
        }

        private void process_regex(string line, string pattern, ref Result results)
        {
            Regex regex = new Regex(pattern);
            GroupCollection groups = regex.Match(line).Groups;
            foreach (string groupname in regex.GetGroupNames())
            {
                if (groups[groupname].Value.Length == 0)
                {
                    continue;
                }

                switch (groupname)
                {
                    case "headliner":
                        results.Headliner = groups[groupname].Value;
                        break;
                    case "venue":
                        results.Venue = groups[groupname].Value;
                        break;
                    case "date":
                        results.Date = groups[groupname].Value;
                        break;
                    case "time":
                        results.Time = groups[groupname].Value;
                        break;
                    case "section":
                        results.Section = groups[groupname].Value;
                        break;
                    case "row":
                        results.Row = groups[groupname].Value;
                        break;
                    case "seat":
                        results.Seat = groups[groupname].Value;
                        break;
                    case "barcode":
                        results.Barcode = groups[groupname].Value;
                        break;
                    case "confnum":
                        results.ConfNumber = groups[groupname].Value;
                        break;
                }
            }
        }
    }
}
