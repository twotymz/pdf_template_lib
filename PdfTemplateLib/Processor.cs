using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PdfTemplateLib
{
    public class Processor
    {
        public List<Result> ProcessPDF(string pdfPath, string templatePath)
        {
            //List<Result> results = null;
            //Template template = new Template();
            
            //if (template.Read(templatePath))
            //{
            //    results = ProcessPDF(pdfPath);
            //}

            //return results;
        }

        public List<Result> ProcessPDF(string pdfPath, Template template)
        {
            //List<Result> results = template.Run(pdfPath);
            //return results;
        }
    }
}
