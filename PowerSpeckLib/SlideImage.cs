using System;
using System.Drawing;
using System.IO;
using PowerSpeckUtilities;

namespace PowerSpeckLib
{
    public class SlideImage : SlideObject
    {
        public SlideImage(string imagePath, int x, int y, int width, int height)
        {
            Type = SlideObjectType.Image;
            Top = x;
            Left = y;

            // Get the image and calculate the size
            try
            {
                Image tmp;
                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    tmp = Image.FromStream(stream);

                if (width <= 0 && height <= 0)
                {
                    width = tmp.Width;
                    height = tmp.Height;
                }

                if (width > 0 && height > 0)
                {
                    if (width == tmp.Width && height == tmp.Height)
                        Image = tmp;
                    else
                    {
                        // Normal resize
                        var newImage = new Bitmap(width, height);
                        Graphics.FromImage(newImage).DrawImage(tmp, 0, 0, width, height);
                        Image = new Bitmap(newImage);
                    }
                }
                else
                    Image = Utilities.ScaleImage(tmp, width <= 0 ? height*10 : width, height <= 0 ? width*10 : height);
            }
            catch
            {
                Image = new Bitmap(Math.Max(1, width), Math.Max(1, height));
            }
        }

        public Image Image { get; set; }

        public override void Draw(Graphics graphics)
        {
            graphics.DrawImage(Image, Top, Left);
        }
    }
}