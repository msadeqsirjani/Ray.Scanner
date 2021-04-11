using NAPS2.Util;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace NAPS2.Scan.Images
{
    internal static class ImageScaleHelper
    {
        public static Bitmap ScaleImage(Image original, double scaleFactor)
        {
            var realWidth = original.Width / scaleFactor;
            var realHeight = original.Height / scaleFactor;

            var horizontalRes = original.HorizontalResolution / scaleFactor;
            var verticalRes = original.VerticalResolution / scaleFactor;

            var result = new Bitmap((int)realWidth, (int)realHeight, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, 0, 0, (int)realWidth, (int)realHeight);
                result.SafeSetResolution((float)horizontalRes, (float)verticalRes);
                return result;
            }
        }
    }
}