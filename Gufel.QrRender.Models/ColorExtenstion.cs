using System.Drawing;
using System.Globalization;

namespace Gufel.QrRender.Models;

public static class ColorExtenstion
{
    public static Color FromHex(this string color)
    {
        if (string.IsNullOrEmpty(color))
            return Color.Black;

        var hex = color.Replace("#", string.Empty);
        var h = NumberStyles.HexNumber;

        var r = int.Parse(hex.Substring(0, 2), h);
        var g = int.Parse(hex.Substring(2, 2), h);
        var b = int.Parse(hex.Substring(4, 2), h);
        var a = 255;

        if (hex.Length == 8)
        {
            a = int.Parse(hex.Substring(6, 2), h);
        }

        return Color.FromArgb(a, r, g, b);
    }

    public static Color FromHtml(this string hexFormat)
    {
        return ColorTranslator.FromHtml(hexFormat);
    }

    public static string ToHtmlRgb(this Color color)
    {
        return $"rgb({color.R}, {color.G}, {color.B})";
    }
}