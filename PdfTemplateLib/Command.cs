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
        public string Instruction { get; set; }
        public string Args { get; set; }

        public int ToInt ()
        {
            int v;
            if (int.TryParse(Args, out v))
            {
                return v;
            }

            return 0;
        }

        public Rectangle ToRect ()
        {
            string[] tokens = Args.Split(' ');
            if (tokens.Length == 4)
            {
                int x, y, w, h;
                if (int.TryParse(tokens[0], out x))
                {
                    if (int.TryParse(tokens[1], out y))
                    {
                        if (int.TryParse(tokens[2], out w))
                        {
                            if (int.TryParse(tokens[3], out h))
                            {
                                return new Rectangle(x, y, w, h);
                            }
                        }
                    }
                }
            }

            return new Rectangle(0, 0, 0, 0);
        }
    }
}
