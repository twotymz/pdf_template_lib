using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PdfTemplateLib
{
    class BitmapProcessor
    {
        public static void Rotate (ref Bitmap bmp, int degrees)
        {
            RotateFlipType flipType = RotateFlipType.RotateNoneFlipNone;

            switch (degrees)
            {
                case 90 :
                    flipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case 180 :
                    flipType = RotateFlipType.Rotate180FlipNone;
                    break;
                case 270 :
                    flipType = RotateFlipType.Rotate270FlipNone;
                    break;
            }

            bmp.RotateFlip(flipType);
        }

        public static Bitmap Crop (Bitmap bmp, Rectangle cropRect)
        {
            Bitmap output = new Bitmap(cropRect.Width, cropRect.Height);
            using (Graphics g = Graphics.FromImage(output))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, output.Width, output.Height), cropRect, GraphicsUnit.Pixel);
            }

            return output;
        }

        public static void Rectangle (ref Bitmap bmp, Rectangle rect)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Pen pen = new Pen (Color.Red);
                g.DrawRectangle(pen, rect);
            }
        }

        public static Bitmap GreyScale(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            Graphics g = Graphics.FromImage(newBitmap);

            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][] 
              {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
              });

            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                        0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            g.Dispose();
            return newBitmap;
        }

        public static byte[] ToTiffBytes (Bitmap input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.Save(ms, ImageFormat.Tiff);
                return ms.ToArray();
            }
        }
    }
}
