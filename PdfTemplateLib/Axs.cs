using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTemplateLib
{
    class Axs
    {
        static public Result Process (string pdfPath, int page)
        {
            Template template = new Template();
            Processor processor = new Processor();

            template.AddLine("rotate 270");
            template.AddLine("crop 1904 350 336 43");
            template.AddLine("0 ^(?<barcode>\\d+)$");

            return processor.ProcessPDFPage(pdfPath, template, page);
        }
    }
}
