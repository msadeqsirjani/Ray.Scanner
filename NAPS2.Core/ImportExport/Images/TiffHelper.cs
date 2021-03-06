using NAPS2.Scan.Images;
using NAPS2.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NAPS2.ImportExport.Images
{
    public class TiffHelper
    {
        private readonly ScannedImageRenderer scannedImageRenderer;

        public TiffHelper(ScannedImageRenderer scannedImageRenderer)
        {
            this.scannedImageRenderer = scannedImageRenderer;
        }

        public async Task<bool> SaveMultiPage(List<ScannedImage.Snapshot> snapshots, string location, TiffCompression compression, ProgressHandler progressCallback, CancellationToken cancelToken)
        {
            try
            {
                var codecInfo = GetCodecForString("TIFF");

                progressCallback(0, snapshots.Count);
                if (cancelToken.IsCancellationRequested)
                {
                    return false;
                }

                PathHelper.EnsureParentDirExists(location);

                if (snapshots.Count == 1)
                {
                    var encoderParameters = new EncoderParameters(1);
                    var encoder = Encoder.Compression;
                    using (var bitmap = await scannedImageRenderer.Render(snapshots[0]))
                    {
                        ValidateBitmap(bitmap);
                        var encoderParameter = new EncoderParameter(encoder, (long)GetEncoderValue(compression, bitmap));
                        encoderParameters.Param[0] = encoderParameter;
                        bitmap.Save(location, codecInfo, encoderParameters);
                    }
                }
                else if (snapshots.Count > 1)
                {
                    var encoderParams = new EncoderParameters(2);
                    var saveEncoder = Encoder.SaveFlag;
                    var compressionEncoder = Encoder.Compression;

                    File.Delete(location);
                    using (var bitmap0 = await scannedImageRenderer.Render(snapshots[0]))
                    {
                        ValidateBitmap(bitmap0);
                        encoderParams.Param[0] = new EncoderParameter(compressionEncoder, (long)GetEncoderValue(compression, bitmap0));
                        encoderParams.Param[1] = new EncoderParameter(saveEncoder, (long)EncoderValue.MultiFrame);
                        bitmap0.Save(location, codecInfo, encoderParams);

                        for (var i = 1; i < snapshots.Count; i++)
                        {
                            if (snapshots[i] == null)
                                break;

                            progressCallback(i, snapshots.Count);
                            if (cancelToken.IsCancellationRequested)
                            {
                                bitmap0.Dispose();
                                File.Delete(location);
                                return false;
                            }

                            using (var bitmap = await scannedImageRenderer.Render(snapshots[i]))
                            {
                                ValidateBitmap(bitmap);
                                encoderParams.Param[0] = new EncoderParameter(compressionEncoder, (long)GetEncoderValue(compression, bitmap));
                                encoderParams.Param[1] = new EncoderParameter(saveEncoder, (long)EncoderValue.FrameDimensionPage);
                                bitmap0.SaveAdd(bitmap, encoderParams);
                            }
                        }

                        encoderParams.Param[0] = new EncoderParameter(saveEncoder, (long)EncoderValue.Flush);
                        bitmap0.SaveAdd(encoderParams);
                    }
                }
                return true;


            }
            catch (Exception ex)
            {
                throw new Exception("Error saving TIFF", ex);
            }

        }

        private static EncoderValue GetEncoderValue(TiffCompression compression, Bitmap bitmap)
        {
            switch (compression)
            {
                case TiffCompression.None:
                    return EncoderValue.CompressionNone;
                case TiffCompression.Ccitt4:
                    return EncoderValue.CompressionCCITT4;
                case TiffCompression.Lzw:
                    return EncoderValue.CompressionLZW;
                default:
                    if (bitmap.PixelFormat == PixelFormat.Format1bppIndexed
                        && bitmap.Palette.Entries.Length == 2
                        && bitmap.Palette.Entries[0].ToArgb() == Color.Black.ToArgb()
                        && bitmap.Palette.Entries[1].ToArgb() == Color.White.ToArgb())
                    {
                        return EncoderValue.CompressionCCITT4;
                    }
                    else
                    {
                        return EncoderValue.CompressionLZW;
                    }
            }
        }

        private static void ValidateBitmap(Bitmap bitmap)
        {
            if (bitmap.PixelFormat != PixelFormat.Format1bppIndexed || bitmap.Palette.Entries.Length != 2 ||
                bitmap.Palette.Entries[0].ToArgb() != Color.White.ToArgb() ||
                bitmap.Palette.Entries[1].ToArgb() != Color.Black.ToArgb()) return;
            // Inverted palette (0 = white); some scanners may produce bitmaps like this
            // It won't encode properly in a TIFF, so we need to invert the encoding
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var stride = Math.Abs(data.Stride);
            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x += 8)
                {
                    var b = Marshal.ReadByte(data.Scan0 + y * stride + x / 8);
                    var bits = Math.Min(8, data.Width - x);
                    b ^= (byte)(0xFF << (8 - bits));
                    Marshal.WriteByte(data.Scan0 + y * stride + x / 8, b);
                }
            }
            bitmap.UnlockBits(data);
            bitmap.Palette.Entries[0] = Color.Black;
            bitmap.Palette.Entries[1] = Color.White;
        }

        private ImageCodecInfo GetCodecForString(string type)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            return info.FirstOrDefault(t => t.FormatDescription.Equals(type));
        }
    }
}
