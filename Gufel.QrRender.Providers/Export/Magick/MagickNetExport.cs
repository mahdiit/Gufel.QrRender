using ImageMagick;

namespace Gufel.QrRender.Providers.Export.Magick;

public static class MagickNetExport
{
    public static void SvgToPng(string svgPath, string pngPath, int? dpi = null)
    {
        var settings = new MagickReadSettings()
        {
            Format = MagickFormat.Svg,
            Density = dpi == null ? null : new Density(dpi.Value)
        };

        using var image = new MagickImage(svgPath, settings);
        image.Write(pngPath);
    }

    public static void SvgToPdf(string svgPath, string pdfPath, int? dpi = null)
    {
        var settings = new MagickReadSettings
        {
            Format = MagickFormat.Svg,
            Density = dpi == null ? null : new Density(dpi.Value)
        };

        using var image = new MagickImage(svgPath, settings);
        image.Write(pdfPath);
    }
}