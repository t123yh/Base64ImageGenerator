using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Base64ImageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: <InputFileName> <Ratio> <OutputHtmlPath>\nRatio is a float number. e.g. 0.5\nThe base64 string will be put into standard output stream.");
                return;
            }

            double ratio = double.Parse(args[1]);
            Bitmap newImage;
            using (var image = Image.FromFile(args[0]))
            {
                // Consolas font ratio: 1:2
                newImage = ResizeImage(image, (int)(image.Width * ratio), (int)(image.Height * ratio / 2));
            }
            //newImage.Save("D:\\test.png", ImageFormat.Png);
            byte[] resultBytes;
            using (FileStream fs = new FileStream(args[2], FileMode.OpenOrCreate, FileAccess.Write))
            using (MemoryStream ms = new MemoryStream(newImage.Height * newImage.Width * 3))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine("<html>");
                writer.WriteLine(Properties.Resources.Header);
                writer.WriteLine("<body>");
                writer.WriteLine("<div class=\"asciiimg\">");
                ms.Write(BitConverter.GetBytes(newImage.Width), 0, 4);
                ms.Write(BitConverter.GetBytes(newImage.Height), 0, 4);
                for (int y = 0; y < newImage.Height; y++)
                {
                    for (int x = 0; x < newImage.Width; x++)
                    {
                        Color color = newImage.GetPixel(x, y);
                        ms.WriteByte(color.R);
                        ms.WriteByte(color.G);
                        ms.WriteByte(color.B);
                        //ms.WriteByte((byte)(x % 255));
                        //ms.WriteByte((byte)(y % 255));
                        //ms.WriteByte(0);
                        writer.Write("<span style=\"color: #{0:X2}{1:X2}{2:X2}\">#</span>", color.R, color.G, color.B);
                    }
                    writer.WriteLine("<br>");
                }
                writer.WriteLine("</div></html>");
                resultBytes = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(resultBytes, 0, resultBytes.Length);
            }
            Console.WriteLine(Convert.ToBase64String(resultBytes));

        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
