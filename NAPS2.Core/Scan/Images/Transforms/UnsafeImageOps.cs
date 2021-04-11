using NAPS2.Util;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace NAPS2.Scan.Images.Transforms
{
    public static class UnsafeImageOps
    {
        public static unsafe void ChangeBrightness(Bitmap bitmap, float brightnessAdjusted)
        {
            var bytesPerPixel = GetBytesPerPixel(bitmap);

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var stride = Math.Abs(bitmapData.Stride);
            var h = bitmapData.Height;
            var w = bitmapData.Width;

            brightnessAdjusted *= 255;

            var data = (byte*)bitmapData.Scan0;
            PartitionRows(h, (start, end) =>
            {
                for (var y = start; y < end; y++)
                {
                    var row = data + stride * y;
                    for (var x = 0; x < w; x++)
                    {
                        var pixel = row + x * bytesPerPixel;
                        var r = *pixel;
                        var g = *(pixel + 1);
                        var b = *(pixel + 2);

                        var r2 = (int)(r + brightnessAdjusted);
                        var g2 = (int)(g + brightnessAdjusted);
                        var b2 = (int)(b + brightnessAdjusted);

                        r = (byte)(r2 < 0 ? 0 : r2 > 255 ? 255 : r2);
                        g = (byte)(g2 < 0 ? 0 : g2 > 255 ? 255 : g2);
                        b = (byte)(b2 < 0 ? 0 : b2 > 255 ? 255 : b2);

                        *pixel = r;
                        *(pixel + 1) = g;
                        *(pixel + 2) = b;
                    }
                }
            });

            bitmap.UnlockBits(bitmapData);
        }

        public static unsafe void ChangeContrast(Bitmap bitmap, float contrastAdjusted, float offset)
        {
            var bytesPerPixel = GetBytesPerPixel(bitmap);

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var stride = Math.Abs(bitmapData.Stride);
            var h = bitmapData.Height;
            var w = bitmapData.Width;

            offset *= 255;

            var data = (byte*)bitmapData.Scan0;
            PartitionRows(h, (start, end) =>
            {
                for (var y = start; y < end; y++)
                {
                    var row = data + stride * y;
                    for (var x = 0; x < w; x++)
                    {
                        var pixel = row + x * bytesPerPixel;
                        var r = *pixel;
                        var g = *(pixel + 1);
                        var b = *(pixel + 2);

                        var r2 = (int)(r * contrastAdjusted + offset);
                        var g2 = (int)(g * contrastAdjusted + offset);
                        var b2 = (int)(b * contrastAdjusted + offset);

                        r = (byte)(r2 < 0 ? 0 : r2 > 255 ? 255 : r2);
                        g = (byte)(g2 < 0 ? 0 : g2 > 255 ? 255 : g2);
                        b = (byte)(b2 < 0 ? 0 : b2 > 255 ? 255 : b2);

                        *pixel = r;
                        *(pixel + 1) = g;
                        *(pixel + 2) = b;
                    }
                }
            });

            bitmap.UnlockBits(bitmapData);
        }

        public static unsafe void HueShift(Bitmap bitmap, float hueShift)
        {
            var bytesPerPixel = GetBytesPerPixel(bitmap);

            hueShift /= 60;

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var stride = Math.Abs(bitmapData.Stride);
            var h = bitmapData.Height;
            var w = bitmapData.Width;

            var data = (byte*)bitmapData.Scan0;
            PartitionRows(h, (start, end) =>
            {
                for (var y = start; y < end; y++)
                {
                    var row = data + stride * y;
                    for (var x = 0; x < w; x++)
                    {
                        var pixel = row + x * bytesPerPixel;
                        var r = *pixel;
                        var g = *(pixel + 1);
                        var b = *(pixel + 2);

                        int max = Math.Max(r, Math.Max(g, b));
                        int min = Math.Min(r, Math.Min(g, b));

                        if (max == min)
                        {
                            continue;
                        }

                        var hue = 0.0f;
                        float delta = max - min;
                        if (r == max)
                        {
                            hue = (g - b) / delta;
                        }
                        else if (g == max)
                        {
                            hue = 2 + (b - r) / delta;
                        }
                        else if (b == max)
                        {
                            hue = 4 + (r - g) / delta;
                        }
                        hue += hueShift;
                        hue = (hue + 6) % 6;

                        var sat = (max == 0) ? 0 : 1f - (1f * min / max);
                        float val = max;

                        var hi = (int)Math.Floor(hue);
                        var f = hue - hi;

                        var v = (byte)(val);
                        var p = (byte)(val * (1 - sat));
                        var q = (byte)(val * (1 - f * sat));
                        var t = (byte)(val * (1 - (1 - f) * sat));

                        switch (hi)
                        {
                            case 0:
                                r = v;
                                g = t;
                                b = p;
                                break;
                            case 1:
                                r = q;
                                g = v;
                                b = p;
                                break;
                            case 2:
                                r = p;
                                g = v;
                                b = t;
                                break;
                            case 3:
                                r = p;
                                g = q;
                                b = v;
                                break;
                            case 4:
                                r = t;
                                g = p;
                                b = v;
                                break;
                            default:
                                r = v;
                                g = p;
                                b = q;
                                break;
                        }

                        *pixel = r;
                        *(pixel + 1) = g;
                        *(pixel + 2) = b;
                    }
                }
            });

            bitmap.UnlockBits(bitmapData);
        }

        public static unsafe void RowWiseCopy(Bitmap src, Bitmap dst, int x1, int y1, int w, int h)
        {
            var bitPerPixel = src.PixelFormat == PixelFormat.Format1bppIndexed;
            var bytesPerPixel = bitPerPixel ? 0 : GetBytesPerPixel(src);

            var srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, src.PixelFormat);
            var srcStride = Math.Abs(srcData.Stride);

            var dstData = dst.LockBits(new Rectangle(0, 0, dst.Width, dst.Height), ImageLockMode.ReadWrite, dst.PixelFormat);
            var dstStride = Math.Abs(dstData.Stride);

            if (bitPerPixel)
            {
                // 1bpp copy requires bit shifting
                var shift1 = x1 % 8;
                var shift2 = 8 - shift1;

                var bytes = (w + 7) / 8;
                var bytesExceptLast = bytes - 1;

                PartitionRows(h, (start, end) =>
                {
                    for (var y = start; y < end; y++)
                    {
                        var srcRow = (byte*)(srcData.Scan0 + srcStride * (y1 + y) + (x1 + 7) / 8);
                        var dstRow = (byte*)(dstData.Scan0 + dstStride * y);

                        for (var x = 0; x < bytesExceptLast; x++)
                        {
                            var srcPtr = srcRow + x;
                            var dstPtr = dstRow + x;
                            *dstPtr = (byte)((*srcPtr << shift1) | (*(srcPtr + 1) >> shift2));
                        }

                        if (w <= 0) continue;
                        {
                            var srcPtr = srcRow + bytesExceptLast;
                            var dstPtr = dstRow + bytesExceptLast;
                            if (shift2 == 8)
                            {
                                *dstPtr = *srcPtr;
                            }
                            else
                            {
                                *dstPtr = (byte)((*srcPtr << shift1) | (*(srcPtr + 1) >> shift2));
                            }
                            var mask = (byte)(0xFF << (w % 8));
                            *dstPtr = (byte)(*dstPtr & mask);
                        }
                    }
                });
            }
            else
            {
                // Byte-aligned copy is a bit simpler
                PartitionRows(h, (start, end) =>
                {
                    for (var y = start; y < end; y++)
                    {
                        var srcPtrB = (byte*)(srcData.Scan0 + srcStride * (y1 + y) + x1 * bytesPerPixel);
                        var dstPtrB = (byte*)(dstData.Scan0 + dstStride * y);
                        var srcPtrL = (long*)srcPtrB;
                        var dstPtrL = (long*)dstPtrB;
                        var len = w * bytesPerPixel;

                        // Copy via longs for better bandwidth
                        for (var i = 0; i < len / 8; i++)
                        {
                            *(dstPtrL + i) = *(srcPtrL + i);
                        }
                        // Then copy any leftovers one byte at a time
                        for (var i = len / 8 * 8; i < len; i++)
                        {
                            *(dstPtrB + i) = *(srcPtrB + i);
                        }
                    }
                });
            }

            src.UnlockBits(srcData);
            dst.UnlockBits(dstData);
        }

        private static int GetBytesPerPixel(Image bitmap)
        {
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                    return 4;
                case PixelFormat.Format24bppRgb:
                    return 3;
                default:
                    throw new ArgumentException("Unsupported pixel format: " + bitmap.PixelFormat);
            }
        }

        public static unsafe Bitmap ConvertTo1Bpp(Bitmap bitmap, int threshold)
        {
            var thresholdAdjusted = (threshold + 1000) * 255 / 2;
            var bytesPerPixel = GetBytesPerPixel(bitmap);

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var stride = Math.Abs(bitmapData.Stride);
            var data = (byte*)bitmapData.Scan0;
            var h = bitmapData.Height;
            var w = bitmapData.Width;

            var monoBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format1bppIndexed);
            var p = monoBitmap.Palette;
            p.Entries[0] = Color.Black;
            p.Entries[1] = Color.White;
            monoBitmap.Palette = p;
            var monoBitmapData = monoBitmap.LockBits(new Rectangle(0, 0, monoBitmap.Width, monoBitmap.Height), ImageLockMode.WriteOnly, monoBitmap.PixelFormat);
            var monoStride = Math.Abs(monoBitmapData.Stride);
            var monoData = (byte*)monoBitmapData.Scan0;

            PartitionRows(h, (start, end) =>
            {
                for (var y = start; y < end; y++)
                {
                    var row = data + stride * y;
                    for (var x = 0; x < w; x += 8)
                    {
                        byte monoByte = 0;
                        for (var k = 0; k < 8; k++)
                        {
                            monoByte <<= 1;
                            if (x + k >= w) continue;
                            var pixel = row + (x + k) * bytesPerPixel;
                            var r = *pixel;
                            var g = *(pixel + 1);
                            var b = *(pixel + 2);
                            // Use standard values for grayscale conversion to weight the RGB values
                            var luma = r * 299 + g * 587 + b * 114;
                            if (luma >= thresholdAdjusted)
                            {
                                monoByte |= 1;
                            }
                        }
                        *(monoData + y * monoStride + x / 8) = monoByte;
                    }
                }
            });

            bitmap.UnlockBits(bitmapData);
            monoBitmap.UnlockBits(monoBitmapData);
            monoBitmap.SafeSetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            return monoBitmap;
        }

        public static unsafe BitArray[] ConvertToBitArrays(Bitmap bitmap)
        {
            var bitPerPixel = bitmap.PixelFormat == PixelFormat.Format1bppIndexed;
            var bytesPerPixel = bitPerPixel ? 0 : GetBytesPerPixel(bitmap);

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var stride = Math.Abs(bitmapData.Stride);
            var data = (byte*)bitmapData.Scan0;
            var h = bitmap.Height;
            var w = bitmap.Width;

            var bitArrays = new BitArray[h];

            if (bitPerPixel)
            {
                PartitionRows(h, (start, end) =>
                {
                    for (var y = start; y < end; y++)
                    {
                        var outRow = new BitArray(w);
                        bitArrays[y] = outRow;
                        var row = data + stride * y;
                        for (var x = 0; x < w; x += 8)
                        {
                            var monoByte = *(row + x / 8);
                            for (var k = 7; k >= 0; k--)
                            {
                                if (x + k < w)
                                {
                                    outRow[x + k] = (monoByte & 1) == 0;
                                }
                                monoByte >>= 1;
                            }
                        }
                    }
                });
            }
            else
            {
                const int thresholdAdjusted = 140 * 1000;
                PartitionRows(h, (start, end) =>
                {
                    for (var y = start; y < end; y++)
                    {
                        var outRow = new BitArray(w);
                        bitArrays[y] = outRow;
                        var row = data + stride * y;
                        for (var x = 0; x < w; x++)
                        {
                            var pixel = row + x * bytesPerPixel;
                            var r = *pixel;
                            var g = *(pixel + 1);
                            var b = *(pixel + 2);
                            // Use standard values for grayscale conversion to weight the RGB values
                            var luma = r * 299 + g * 587 + b * 114;
                            outRow[x] = luma < thresholdAdjusted;
                        }
                    }
                });
            }

            bitmap.UnlockBits(bitmapData);

            return bitArrays;
        }

        private static void PartitionRows(int count, Action<int, int> action)
        {
            const int partitionCount = 1;
            var div = (count + partitionCount - 1) / partitionCount;

            var tasks = new Task[partitionCount];
            for (var i = 0; i < partitionCount; i++)
            {
                int start = div * i, end = Math.Min(div * (i + 1), count);
                tasks[i] = Task.Factory.StartNew(() => action(start, end));
            }
            Task.WaitAll(tasks);
        }
    }
}
