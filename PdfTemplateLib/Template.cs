using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace PdfTemplateLib
{
    public class Result
    {
        public string Headliner { get; set; }
        public string Venue { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Section { get; set; }
        public string Row { get; set; }
        public string Seat { get; set; }
        public string Barcode { get; set; }
        public string ConfNumber { get; set; }
    }

    public class Template
    {
        class Line
        {
            public int LineNo { get; set; }
            public string Pattern { get; set; }
        }
        
        List<Line> lines;
        string headliner;
        string venue;
        string date;
        string time;
        string section;
        string row;
        string path;

        public Template()
        {
            lines = new List<Line>();
            headliner = null;
            venue = null;
            date = null;
            time = null;
            section = null;
            row = null;
            path = null;
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
                        this.lines.Add(new Line { LineNo = line_no, Pattern = pattern });
                    }
                }
                else
                {
                    // Deal with processing directives.
                    string val = string.Join(" ", tokens, 1, tokens.Length - 1);
                    switch (tokens[0])
                    {
                        case "headliner":
                            headliner = val;
                            break;
                        case "venue":
                            venue = val;
                            break;
                        case "date":
                            date = val;
                            break;
                        case "time":
                            time = val;
                            break;
                        case "section":
                            section = val;
                            break;
                        case "row":
                            row = val;
                            break;
                        case "path":
                            path = val;
                            break;
                    }
                }
            }
        }

        public Result Run (string pdfPath, string[] pdfLines)
        {
            Result results = new Result();

            foreach (Line line in this.lines)
            {
                process_regex(pdfLines[line.LineNo], line.Pattern, ref results);
            }

            if (path != null)
            {
                process_regex(pdfPath, path, ref results);
            }

            // Use default values if specified and if we didn't get anything from parsing
            // the lines from the PDF.
            if (results.Headliner == null && headliner != null)
            {
                results.Headliner = headliner;
            }

            if (results.Venue == null && venue != null)
            {
                results.Venue = venue;
            }

            if (results.Date == null && date != null)
            {
                results.Date = date;
            }

            if (results.Time == null && time != null)
            {
                results.Time = time;
            }

            if (results.Section == null && section != null)
            {
                results.Section = section;
            }

            if (results.Row == null && row != null)
            {
                results.Row = row;
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
