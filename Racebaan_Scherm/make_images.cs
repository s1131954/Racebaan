using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Racebaan_Scherm
{
    public static class make_images
    {
        private static Dictionary<string, Bitmap> file = new Dictionary<string, Bitmap>();

        public static Bitmap returnBitmap(string s)
        {
            if (!file.ContainsKey(s))
            {
                if (s == "")
                {
                    file[s] = createEmpty(70, 70);
                }
                file[s] = new Bitmap(s);
            }
            return (Bitmap)file[s].Clone();
        }

        public static void clear()
        {
            file.Clear();
        }

        public static Bitmap createEmpty(int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            Graphics gfx = Graphics.FromImage(b);
            SolidBrush brush = new SolidBrush(System.Drawing.Color.Green);
            gfx.FillRectangle(brush, 0, 0, width, height);
            return new Bitmap(width, height, gfx);
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
