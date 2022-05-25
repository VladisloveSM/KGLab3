using System.Drawing;

namespace ImageWrapper;

public sealed class ColorConverter
{
    public static (short Hue, float Saturation, float Value) ToHsv(Color color)
    {
        var r = color.R / 255f;
        var g = color.G / 255f;
        var b = color.B / 255f;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var d = max - min;

        var realH = 0f;

        if (Math.Abs(max - r) <= float.Epsilon)
        {
            realH = 60 * ((g - b) / d % 6);
        }
        else if (Math.Abs(max - g) <= float.Epsilon)
        {
            realH = 60 * ((b - r) / d + 2);
        }
        else if (Math.Abs(max - b) <= float.Epsilon)
        {
            realH = 60 * ((r - g) / d + 4);
        }

        var h = (short)Math.Round(realH);
        var s = max == 0 ? 0 : d / max;

        return new(h, s, max);
    }

    public static Color ToRgb((short Hue, float Saturation, float Value) color)
    {
        var c = color.Value * color.Saturation;
        var x = c * (1 - Math.Abs(color.Hue / 60f % 2 - 1));
        var m = color.Value - c;

        var (realR, realG, realB) = (0f, 0f, 0f);

        if (color.Hue < 60)
        {
            (realR, realG, realB) = (c, x, 0);
        }
        else if (color.Hue < 120)
        {
            (realR, realG, realB) = (x, c, 0);
        }
        else if (color.Hue < 180)
        {
            (realR, realG, realB) = (0, c, x);
        }
        else if (color.Hue < 240)
        {
            (realR, realG, realB) = (0, x, c);
        }
        else if (color.Hue < 300)
        {
            (realR, realG, realB) = (x, 0, c);
        }
        else if (color.Hue < 360)
        {
            (realR, realG, realB) = (c, 0, x);
        }

        var (r, g, b) = ((byte)Math.Round((realR + m) * 255),
            (byte)Math.Round((realG + m) * 255),
            (byte)Math.Round((realB + m) * 255));

        return Color.FromArgb(r, g, b);
    }
}