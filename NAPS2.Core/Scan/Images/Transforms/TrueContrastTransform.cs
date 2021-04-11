using System;
using System.Drawing;

namespace NAPS2.Scan.Images.Transforms
{
    public class TrueContrastTransform : Transform
    {
        public int Contrast { get; set; }

        public override Bitmap Perform(Bitmap bitmap)
        {
            // convert +/-1000 input range to a logarithmic scaled multiplier
            var contrastAdjusted = (float)Math.Pow(2.718281f, Contrast / 500.0f);
            // see http://docs.rainmeter.net/tips/colormatrix-guide/ for offset & matrix calculation
            var offset = (1.0f - contrastAdjusted) / 2.0f;

            EnsurePixelFormat(ref bitmap);
            UnsafeImageOps.ChangeContrast(bitmap, contrastAdjusted, offset);
            return bitmap;
        }

        public override bool IsNull => Contrast == 0;
    }
}
