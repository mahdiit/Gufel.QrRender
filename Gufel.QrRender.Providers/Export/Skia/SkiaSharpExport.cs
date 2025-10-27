using SkiaSharp;
using Svg.Skia;

namespace Gufel.QrRender.Providers.Export.Skia;

public static class SkiaSharpExport
{
    public static void SvgToPng(string svgPath, string pngPath)
    {
        using var svg = new SKSvg();
        svg.Load(svgPath);

        using var bitmap = new SKBitmap(Convert.ToInt32(svg.Picture.CullRect.Width), Convert.ToInt32(svg.Picture.CullRect.Height));
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawPicture(svg.Picture);
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes(pngPath, data.ToArray());
    }

    public static void SvgToPdf(string svgPath, string pdfPath)
    {
        using var svg = new SKSvg();
        svg.Load(svgPath);

        using var document = SKDocument.CreatePdf(pdfPath);
        using var canvas = document.BeginPage(svg.Picture.CullRect.Width, svg.Picture.CullRect.Height);

        canvas.DrawPicture(svg.Picture);
        document.EndPage();
        document.Close();
    }
}