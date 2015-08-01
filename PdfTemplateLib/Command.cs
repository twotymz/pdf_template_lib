using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PdfTemplateLib
{
    public class Command
    {
        public enum Opcode
        {
            OP_HEADLINER,       // "headliner" string
            OP_VENUE,           // "venue" string
            OP_DATE,            // "date" string
            OP_TIME,            // "time" string
            OP_SECTION,         // "section" string
            OP_ROW,             // "row" string
            OP_PATH,            // "path" string
            OP_AXS,             // "axs"
            OP_ROTATE,          // "rotate" integer
            OP_SAVE_BITMAP,     // "save_bitmap" string
            OP_CROP,            // "crop" integer integer integer integer
            OP_RECTANGLE,       // "rectange" integer integer integer integer
            OP_GREYSCALE,       // "greyscale"
            OP_RESET,           // "reset"
            OP_TEXT,            // "text"
            OP_SAVE_TEXT,       // "save_text" string
            OP_OCR,             // "ocr"
            OP_DPI,             // "dpi" integer integer
            OP_REGEX            // integer string
        }

        public Opcode Code { get; set; }
        public List<string> Args { get; set; }
    }
}
