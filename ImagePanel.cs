using System;
using System.Drawing;
using System.Windows.Forms;

namespace DualScreenDemo
{
    public class ImagePanel : Panel
    {
        public ImagePanel()
        {
            this.DoubleBuffered = true; // 减少闪烁
            this.Size = new Size(1201, 496); // 设置大小
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        public void SetBackgroundImageFromFile(string filePath, Rectangle cropArea)
        {
            Image image = Image.FromFile(filePath);
            Bitmap bmp = new Bitmap(cropArea.Width, cropArea.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(image,
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    cropArea,
                    GraphicsUnit.Pixel);
            }

            this.BackgroundImage = bmp;
        }
    }
}