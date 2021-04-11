using System;
using System.Drawing;

namespace NAPS2.Scan.Images.Transforms
{
    [Serializable]
    public class BrightnessTransform : Transform
    {
        public int Brightness { get; set; }

        public override Bitmap Perform(Bitmap bitmap)
        {
            var brightnessAdjusted = Brightness / 1000f;
            EnsurePixelFormat(ref bitmap);
            UnsafeImageOps.ChangeBrightness(bitmap, brightnessAdjusted);
            return bitmap;
        }

        public override bool IsNull => Brightness == 0;
    }
}
