using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PdfTemplateLib
{
    public class Result
    {
        public string Section { get; set; }
        public string Row { get; set; }
        public string Seat { get; set; }
        public string Barcode { get; set; }
    }

    public class Template
    {
        class TemplateLine
        {
            public int LineNo { get; set; }
            public string Pattern { get; set; }
        }        

        List<TemplateLine> lines;

        public Template()
        {
            lines = new List<TemplateLine>();
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
                        this.lines.Add(new TemplateLine { LineNo = line_no, Pattern = pattern });
                    }
                }
            }

            return true;
        }

        public Result Process (string[] pdfLines)
        {
            Result results = new Result();

            foreach (TemplateLine line in this.lines)
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
                            results.Section = groups[groupname].Value;
                            break;
                        case "seat":
                            results.Section = groups[groupname].Value;
                            break;
                        case "barcode":
                            results.Barcode = groups[groupname].Value;
                            break;
                    }
                }
            }

            return results;
        }
    }

    public class Processor
    {
        public List<Result> ProcessPDF (string pdfPath, string templatePath)
        {
            List<Result> results = null;
            Template template = new Template();

            if (template.Read(templatePath))
            {
                results = new List<Result>();

                using (PdfReader reader = new PdfReader(pdfPath))
                {
                    for (int i = 1; i < reader.NumberOfPages)
                    {
                        string pdfText = PdfTextExtractor.GetTextFromPage(reader, i);
                        string[] pdfLines = pdfText.Split('\n');
                        results.Add(template.Process(pdfLines));
                    }
                }
            }

            return results;
        }

        public List<Result> ProcessPDF (string pdfPath, Template template)
        {
            List<Result> results = null;
            results = new List<Result>();

            using (PdfReader reader = new PdfReader(pdfPath))
            {
                for (int i = 1; i < reader.NumberOfPages)
                {
                    string pdfText = PdfTextExtractor.GetTextFromPage(reader, i);
                    string[] pdfLines = pdfText.Split('\n');
                    results.Add(template.Process(pdfLines));
                }
            }

            return results;
        }
    }
}
