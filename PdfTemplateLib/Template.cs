﻿using System;
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
        string defSection;
        string defRow;
        string defSeat;

        public Template()
        {
            lines = new List<Line>();
            defSection = null;
            defRow = null;
        }

        public bool Read (string templatePath)
        {            
            string[] lines;

            try
            {
                lines = File.ReadAllLines(templatePath);
            }
            catch(Exception)
            {
                return false;
            }

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
                    // This is a processing directive
                    if (tokens[0] == "section")
                    {
                        defSection = string.Join(" ", tokens, 1, tokens.Length - 1);
                    }
                    else if (tokens[0] == "row")
                    {
                        defRow = string.Join(" ", tokens, 1, tokens.Length - 1);
                    }
                }
            }

            return true;
        }

        public Result Parse (string[] pdfLines)
        {
            Result results = new Result();

            foreach (Line line in this.lines)
            {
                Regex regex = new Regex(line.Pattern);
                GroupCollection groups = regex.Match(pdfLines[line.LineNo]).Groups;
                foreach (string groupname in regex.GetGroupNames())
                {
                    switch (groupname)
                    {
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
                        case "confnum" :
                            results.ConfNumber = groups[groupname].Value;
                            break;
                    }
                }
            }

            if (results.Section == null && defSection != null)
            {
                results.Section = defSection;
            }

            if (results.Row == null && defRow != null)
            {
                results.Row = defRow;
            }

            return results;
        }
    }
}
