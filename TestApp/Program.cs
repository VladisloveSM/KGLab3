using System.Drawing;
using System.Drawing.Imaging;

using ColorConverter = ImageWrapper.ColorConverter;

namespace TestApp;

internal class Program
{
    static void Main(string[] args)
    {
        ImageWrapper.ImageWrapper iw = new ImageWrapper.ImageWrapper("D:\\data\\TST.png");

        //var img = iw.Equalization(c => (byte)Math.Round(ColorConverter.ToHsv(c).Value * 255), (c, b) =>
        //{
        //    var t = ColorConverter.ToHsv(c);
        //    t.Value = b / 255f;
        //    return ColorConverter.ToRgb(t);
        //});
        //img.Save("D:\\file___NEW.png", ImageFormat.Png);

        var img2 = iw.MedianFilter(2, c => (byte)Math.Round(ColorConverter.ToHsv(c).Value * 255), (c, b) =>
        {
            var t = ColorConverter.ToHsv(c);
            t.Value = b / 255f;
            return ColorConverter.ToRgb(t);
        });
        img2.Save("D:\\file___MED2.png", ImageFormat.Png);
    }
}