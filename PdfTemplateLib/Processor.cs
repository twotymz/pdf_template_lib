using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Ghostscript.NET.Rasterizer;
using Tesseract;

namespace PdfTemplateLib
{
    public class Processor : IDisposable
    {
        string[] text = null;
        Bitmap bitmap = null;
        int dpiX = 300;
        int dpiY = 300;

        public string[] Text { get { return text;  } }
        public Bitmap Bitmap { get { return bitmap; } }

        public void Dispose ()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
            }
        }

        public List<Result> ProcessPDF(string pdfPath, string templatePath)
        {
            Template template = new Template();
            
            if (template.Read(templatePath))
            {
                return ProcessPDF(pdfPath, template);
            }

            return null;
        }

        public List<Result> ProcessPDF(string pdfPath, Template template)
        {
            List<Result> results = new List<Result>();
            int numPages = 0;

            using (PdfReader reader = new PdfReader(pdfPath))
            {
                numPages = reader.NumberOfPages;
            }

            for (int page = 1; page <= numPages; ++page)
            {
                Result result = ProcessPDFPage(pdfPath, template, page);
                results.Add(result);
            }

            return results;
        }

        public Result ProcessPDFPage (string pdfPath, Template template, int page)
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
            }

            text = null;
            bitmap = null;

            Result result = new Result();
            result.Page = page;

            foreach (Command command in template.Commands)
            {
                int line_no;

                if (int.TryParse(command.Instruction, out line_no))
                {
                    extractText(pdfPath, page);
                    if (text != null)
                    {
                        processRegex(text[line_no], command.Args, ref result);
                    }
                }
                else
                {
                    switch (command.Instruction)
                    {
                        case "path":
                            processRegex(pdfPath, command.Args, ref result);
                            break;

                        case "headliner":
                            result.Headliner = command.Args;
                            break;

                        case "venue":
                            result.Venue = command.Args;
                            break;

                        case "date":
                            result.Date = command.Args;
                            break;

                        case "time":
                            result.Time = command.Args;
                            break;

                        case "section":
                            result.Section = command.Args;
                            break;

                        case "row":
                            result.Row = command.Args;
                            break;

                        case "axs":
                            {
                                Result r = Axs.Process(pdfPath, page);
                                result.Barcode = r.Barcode;
                            }
                            break;

                        case "rotate":
                            getBitmap(pdfPath, page);
                            BitmapProcessor.Rotate(ref bitmap, command.ToInt());
                            break;

                        case "save_bitmap":
                            getBitmap(pdfPath, page);
                            bitmap.Save(command.Args);
                            break;

                        case "crop":
                            {
                                getBitmap(pdfPath, page);
                                Bitmap newBitmap = BitmapProcessor.Crop(bitmap, command.ToRect());
                                bitmap.Dispose();
                                bitmap = newBitmap;
                            }
                            break;

                        case "rectangle":
                            getBitmap(pdfPath, page);
                            BitmapProcessor.Rectangle(ref bitmap, command.ToRect());
                            break;

                        case "greyscale":
                            {
                                getBitmap(pdfPath, page);
                                Bitmap newBitmap = BitmapProcessor.GreyScale(bitmap);
                                bitmap.Dispose();
                                bitmap = newBitmap;
                            }  break;

                        case "reset" :
                            if (bitmap != null)
                            {
                                bitmap.Dispose();
                            }
                            text = null;
                            bitmap = null;
                            dpiX = 300;
                            dpiY = 300;
                            break;

                        case "text" :
                            extractText(pdfPath, page);
                            break;

                        case "save_text" :
                            extractText(pdfPath, page);
                            if (text != null)
                            {
                                System.IO.File.WriteAllText(command.Args, string.Join("\n", text));
                            }
                            break;

                        case "ocr" :
                            getBitmap(pdfPath, page);
                            extractText(pdfPath, page);
                            break;

                        case "dpi":
                            {
                                int x, y;
                                string[] args = command.Args.Split(' ');
                                if (int.TryParse(args[0], out x))
                                {
                                    if (int.TryParse(args[1], out y))
                                    {
                                        dpiX = x;
                                        dpiY = y;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            return result;
        }

        private void extractText(string pdfPath, int page)
        {
            if (text != null)
            {
                return;
            }

            string pdfText = null;

            if (bitmap == null)
            {
                using (PdfReader reader = new PdfReader(pdfPath)) 
                {
                    pdfText = PdfTextExtractor.GetTextFromPage(reader, page);
                }
            }
            else
            {
                try
                {
                    string tessDataPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "tessdata");
                    using (var engine = new TesseractEngine(@".\tessdata", "eng", EngineMode.Default))
                    {
                        using (var img = Pix.LoadTiffFromMemory(BitmapProcessor.ToTiffBytes(bitmap)))
                        {
                            using (var tessPage = engine.Process(img))
                            {
                                pdfText = tessPage.GetText();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }               
            }

            if (pdfText != null)
            {
                text = pdfText.Split('\n');
            }
        }

        private void processRegex (string line, string pattern, ref Result result)
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
                        result.Headliner = groups[groupname].Value;
                        break;
                    case "venue":
                        result.Venue = groups[groupname].Value;
                        break;
                    case "date":
                        result.Date = groups[groupname].Value;
                        break;
                    case "time":
                        result.Time = groups[groupname].Value;
                        break;
                    case "section":
                        result.Section = groups[groupname].Value;
                        break;
                    case "row":
                        result.Row = groups[groupname].Value;
                        break;
                    case "seat":
                        result.Seat = groups[groupname].Value;
                        break;
                    case "barcode":
                        result.Barcode = groups[groupname].Value;
                        break;
                    case "confnum":
                        result.ConfNumber = groups[groupname].Value;
                        break;
                }
            }
        }

        private void getBitmap (string pdfPath, int page)
        {
            if (bitmap == null)
            {
                using (var rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.Open(pdfPath);
                    bitmap = (Bitmap) rasterizer.GetPage(dpiX, dpiY, page);
                }  
            }
        }
    }
}
