using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PdfTemplateLib
{
    class Processor
    {
        public List<Result> ProcessPDF(string pdfPath, string templatePath)
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
                        results.Add(template.Parse(pdfLines));
                    }
                }
            }

            return results;
        }

        public List<Result> ProcessPDF(string pdfPath, Template template)
        {
            List<Result> results = null;
            results = new List<Result>();

            using (PdfReader reader = new PdfReader(pdfPath))
            {
                for (int i = 1; i < reader.NumberOfPages)
                {
                    string pdfText = PdfTextExtractor.GetTextFromPage(reader, i);
                    string[] pdfLines = pdfText.Split('\n');
                    results.Add(template.Parse(pdfLines));
                }
            }

            return results;
        }
    }
}
