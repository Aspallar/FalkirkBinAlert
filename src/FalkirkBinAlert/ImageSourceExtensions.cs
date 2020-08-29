using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FalkirkBinAlert
{
    internal static class ImageSourceExtensions
    {
        public static Icon ToSystemDrawingIcon(this ImageSource imageSource)
        {
            Icon icon;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Position = 0;
                var bitmap = (Bitmap)System.Drawing.Image.FromStream(stream);
                icon = Icon.FromHandle(bitmap.GetHicon());
            }
            return icon;
        }
    }
}
