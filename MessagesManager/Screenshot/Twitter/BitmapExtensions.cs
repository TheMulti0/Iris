using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace MessagesManager
{
    internal static class BitmapExtensions
    {
        public static Bitmap ToBitmap(this byte[] data)
        {
            using var ms = new MemoryStream(data);
                 
            return new Bitmap(ms);
        }
        
        public static byte[] ToByteArray(this Bitmap bitmap, ImageFormat format)
        {
            using var ms = new MemoryStream();
            
            bitmap.Save(ms, format);
            
            return ms.ToArray();
        }
             
        public static Bitmap Crop(this Bitmap b, Rectangle r)
        {
            var newBitmap = new Bitmap(r.Width, r.Height);
            using Graphics g = Graphics.FromImage(newBitmap);
            g.DrawImage(b, -r.X, -r.Y);
                 
            return newBitmap;
        }
             
        public static Bitmap RoundCorners(this Bitmap bitmap, int cornerRadius)
        {
            cornerRadius *= 2;
            var newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            var gp = new GraphicsPath();
                 
            gp.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
            gp.AddArc(0 + newBitmap.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
            gp.AddArc(0 + newBitmap.Width - cornerRadius, 0 + newBitmap.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            gp.AddArc(0, 0 + newBitmap.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
     
            using Graphics g = Graphics.FromImage(newBitmap);
                 
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SetClip(gp);
            g.DrawImage(bitmap, Point.Empty);
                 
            return newBitmap;
        }
    }
}