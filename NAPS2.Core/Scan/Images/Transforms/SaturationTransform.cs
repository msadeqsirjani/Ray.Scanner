using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NAPS2.Scan.Images.Transforms
{
    [Serializable]
    public class SaturationTransform : Transform
    {
        public int Saturation { get; set; }

        public override Bitmap Perform(Bitmap bitmap)
        {
            var saturationAdjusted = Saturation / 1000.0 + 1;

            EnsurePixelFormat(ref bitmap);
            int bytesPerPixel;
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    bytesPerPixel = 3;
                    break;
                case PixelFormat.Format32bppArgb:
                    bytesPerPixel = 4;
                    break;
                default:
                    return bitmap;
            }

            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var stride = Math.Abs(data.Stride);
            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    int r = Marshal.ReadByte(data.Scan0 + stride * y + x * bytesPerPixel);
                    int g = Marshal.ReadByte(data.Scan0 + stride * y + x * bytesPerPixel + 1);
                    int b = Marshal.ReadByte(data.Scan0 + stride * y + x * bytesPerPixel + 2);

                    var c = Color.FromArgb(255, r, g, b);
                    ColorHelper.ColorToHsl(c, out var h, out var s, out var v);

                    s = Math.Min(s * saturationAdjusted, 1);

                    c = ColorHelper.ColorFromHsl(h, s, v);

                    Marshal.WriteByte(data.Scan0 + stride * y + x * bytesPerPixel, c.R);
                    Marshal.WriteByte(data.Scan0 + stride * y + x * bytesPerPixel + 1, c.G);
                    Marshal.WriteByte(data.Scan0 + stride * y + x * bytesPerPixel + 2, c.B);
                }
            }
            bitmap.UnlockBits(data);

            return bitmap;
        }

        public override bool IsNull => Saturation == 0;
    }
}
