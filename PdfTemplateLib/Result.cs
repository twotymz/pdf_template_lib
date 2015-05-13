using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTemplateLib
{
    public class Result
    {
        public int Page { get; set; }
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
}
