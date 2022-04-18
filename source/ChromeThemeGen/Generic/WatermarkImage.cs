using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Text;

namespace ChromeThemeGen.Generic
{
    public class WatermarkImage
    {
        public enum WatermarkDirection
        {
            BottomLeftToTopRight,
            TopLeftToBottomRight

        }

        public static void Watermark(Bitmap bmp, WatermarkDirection direction, Brush txtBrush, String text, string fontName, float fontSize)
        {
            Debug.Assert(OperatingSystem.IsWindows());

            if (!string.IsNullOrEmpty(text))
            {
                using Graphics g = Graphics.FromImage(bmp);
                using Font font = new(fontName, fontSize);
                using SolidBrush shadowBrush = new(Color.FromArgb(120, 0, 0, 0));

                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                SizeF size = g.MeasureString(text, font);

                StringBuilder sb = new();

                //Add 40 times
                for (int i = 0; i < 100; i++)
                {
                    sb.Append($"{text}");
                }


                string textLine = sb.ToString();

                (int angel, int offset, int shadowOffsetX, int shadowOffsetY) modifier = direction == WatermarkDirection.BottomLeftToTopRight ? new(-45, 0,2,-2) : new(45, 0-bmp.Height,-2,-2);

                bool isAlt = false;

                g.RotateTransform(modifier.angel);
                for (int loop = 0; loop < (bmp.Height * 2); loop += (int)size.Height, isAlt = !isAlt)
                {
                    g.DrawString(textLine, font, shadowBrush, (-100 + (isAlt ? 0 : 0-(size.Width / 2)) - loop), (loop + modifier.offset));
                    g.DrawString(textLine, font, txtBrush, (-100 + (isAlt ? 0 : 0 - (size.Width / 2)) - loop) + modifier.shadowOffsetX, (loop + modifier.offset) + modifier.shadowOffsetY);
                }
            }
        }
    }
}
