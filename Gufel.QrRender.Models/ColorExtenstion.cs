using System.Drawing;

namespace Gufel.QrRender.Models;

public static class ColorExtenstion
{
    public static Color FromHtml(this string hexFormat)
    {
        return ColorTranslator.FromHtml(hexFormat);
    }

    public static string ToHtmlRgb(this Color color)
    {
        return $"rgb({color.R}, {color.G}, {color.B})";
    }
}