using Gufel.QrRender.Models;
using Gufel.QrRender.Models.Storage;
using QRCoder;
using System.Drawing;
using System.Globalization;
using System.Text;
using Gufel.QrRender.Providers.License;

namespace Gufel.QrRender.Providers;

public sealed class SvgQrCode(IResourceStorage storage)
    : AbstractQRCode, IDisposable
{
    private const string GraphicPosScale = @"<g transform=""translate({0},{1}) scale({2}, {2})"">{3}</g>";
    private const string GraphicRotateColor = "<g transform=\"{0}\" style=\"fill: {1};\">{2}</g>";
    private const string LogoHolder = "<g id=\"Logo\" transform=\"translate({0},{0})\"><image transform=\"\" width=\"{1}\" height=\"{1}\" xlink:href=\"{2}\" /></g>";
    private const string LinearGradientSvg = "<defs><linearGradient gradientTransform=\"rotate(90)\" id=\"grad\"><stop offset=\"5%\" stop-color=\"{0}\" /><stop offset=\"95%\" stop-color=\"{1}\" /></linearGradient>";
    private const string RadialGradientSvg = "<defs><radialGradient id=\"grad\"><stop offset=\"5%\" stop-color=\"{0}\" /><stop offset=\"95%\" stop-color=\"{1}\" /></radialGradient>";
    private const string GradientMaskPixel = "<rect x=\"0\" y=\"0\" width=\"{0}\" height=\"{1}\" fill=\"url(#grad)\" mask=\"url(#gmask)\" />";

    private static string SvgVal(double input)
    {
        return input.ToString(CultureInfo.InvariantCulture);
    }

    private static bool IsInEyeFrame(int x, int y, int count)
    {
        var isInLeftTop = x >= 0 && x <= 6 && y >= 0 && y <= 6;
        var isInLeftBottom = x >= 0 && x <= 6 && y >= (count - 7) && y <= count;
        var isInRightTop = x >= (count - 7) && x <= count && y >= 0 && y <= 6;

        return isInLeftTop || isInLeftBottom || isInRightTop;
    }

    public (string SvgContent, int BoxSize) GetGraphic(int templateId)
    {
        var option = storage.GetTemplate(templateId);
        return GetGraphic(option);
    }

    public (string SvgContent, int BoxSize) GetGraphic(QrRenderOption option)
    {
        ArgumentNullException.ThrowIfNull(QrCodeData);

        LicenseManager.Validate();

        var boxCount = QrCodeData.ModuleMatrix.Count - 8;
        var pixelsPerModule = option.BoxSize / (double)boxCount;

        //fix size
        var bestModuleSize = Convert.ToInt32(Math.Ceiling(pixelsPerModule));
        var boxSize = bestModuleSize * boxCount;
        pixelsPerModule = bestModuleSize;

        var svgSizeAttributes = $"width=\"{option.ViewWidth}\" height=\"{option.ViewWidth}\"";
        var svgFile = new StringBuilder($"""<svg version="1.1" baseProfile="full" shape-rendering="crispEdges" {svgSizeAttributes} xmlns="http://www.w3.org/2000/svg">""");

        svgFile.AppendLine($@"<rect x=""0"" y=""0"" width=""{option.ViewWidth}"" height=""{option.ViewWidth}"" fill=""{option.Colors.Background.ToHtmlRgb()}"" />");
        svgFile.AppendLine("<g transform=\"translate(" + option.QuietZone + "," + option.QuietZone + ")\" id=\"MainGraphicLayer\">");

        if (option.Colors.BackgroundType != QrColorType.Solid
            && option.Eyes.HasGradient
            && option.Colors.Gradient.HasValue)
        {
            svgFile.AppendLine(string.Format(option.Colors.BackgroundType == QrColorType.LinearGradient ? LinearGradientSvg : RadialGradientSvg,
                option.Colors.Body.ToHtmlRgb(),
                option.Colors.Gradient.Value.ToHtmlRgb()));
            svgFile.AppendLine("<mask id=\"gmask\">");
        }

        CreateEyeFrame(option, svgFile, pixelsPerModule, boxSize);

        CreateEyeBall(option, svgFile, pixelsPerModule, boxCount);

        if (option.Colors.BackgroundType != QrColorType.Solid
            && !option.Eyes.HasGradient
            && option.Colors.Gradient.HasValue)
        {
            svgFile.AppendLine(string.Format(option.Colors.BackgroundType == QrColorType.LinearGradient ? LinearGradientSvg : RadialGradientSvg,
                option.Colors.Body.ToHtmlRgb(),
                option.Colors.Gradient.Value.ToHtmlRgb()));
            svgFile.AppendLine("<mask id=\"gmask\">");
        }

        CreatePixel(option, svgFile, boxCount, pixelsPerModule);

        if (option.Colors.BackgroundType != QrColorType.Solid)
        {
            svgFile.AppendLine("</mask></defs>");
            svgFile.AppendLine(string.Format(GradientMaskPixel, boxSize, boxSize));
        }

        CreateLogo(option, boxCount, boxSize, pixelsPerModule, svgFile);

        svgFile.AppendLine("</g></svg>");

        return (svgFile.ToString(), boxSize);
    }

    private static void CreateLogo(QrRenderOption option, int boxCount, int boxSize, double pixelsPerModule, StringBuilder svgFile)
    {
        if (option.Logo == null) return;

        var logoWidth = ((double)boxCount / 4) * pixelsPerModule;
        var pos = (boxSize - logoWidth) / 2;
        if (option.Logo.IsSvg)
        {
            var scaleSize = logoWidth / option.Logo.LogoSize.Width;
            svgFile.AppendLine(string.Format(GraphicPosScale, pos, pos, SvgVal(scaleSize), option.Logo.Data));
        }
        else
        {
            svgFile.AppendLine(string.Format(LogoHolder, pos, pos, option.Logo.Data));
        }
    }

    private void CreatePixel(QrRenderOption option, StringBuilder svgFile, int boxCount, double pixelsPerModule)
    {
        var pixelColor = (option.Colors.BackgroundType == QrColorType.Solid) ? option.Colors.Body : Color.White;
        svgFile.AppendLine("<g id=\"PixelLayer\">");

        for (var yi = 4; yi < QrCodeData.ModuleMatrix.Count - 4; yi++)
        {
            for (var xi = 4; xi < QrCodeData.ModuleMatrix[yi].Count - 4; xi++)
            {
                if (IsInEyeFrame(xi - 4, yi - 4, boxCount) || !QrCodeData.ModuleMatrix[yi][xi]) continue;

                var x = (xi - 4) * pixelsPerModule;
                var y = (yi - 4) * pixelsPerModule;

                if (option.Body == 0)
                    svgFile.AppendLine($@"<rect x=""{x}"" y=""{y}"" width=""{pixelsPerModule}"" height=""{pixelsPerModule}"" fill=""{pixelColor.ToHtmlRgb()}"" />");
                else
                {
                    var graphicId = string.Format("{0}{1}{2}{3}",
                        QrCodeData.ModuleMatrix[yi - 1][xi] ? 1 : 0,
                        QrCodeData.ModuleMatrix[yi][xi - 1] ? 1 : 0,
                        QrCodeData.ModuleMatrix[yi + 1][xi] ? 1 : 0,
                        QrCodeData.ModuleMatrix[yi][xi + 1] ? 1 : 0);

                    var graphicItem = storage.GetGraphic(option.Body + "-" + graphicId, QrGraphicType.Body);
                    var scaleSize = Math.Round(pixelsPerModule / (double)graphicItem.GraphicSize, 3);

                    var pixel = string.Format(GraphicRotateColor, "", pixelColor.ToHtmlRgb(), graphicItem.GraphicData);
                    pixel = string.Format(GraphicPosScale, x, y, SvgVal(scaleSize), pixel);

                    svgFile.AppendLine(pixel);
                }
            }
        }

        svgFile.AppendLine("</g>");
    }

    private void CreateEyeFrame(QrRenderOption option, StringBuilder svgFile, double pixelsPerModule, int boxSize)
    {
        svgFile.AppendLine("<g id=\"EyeFrames\">");
        var eyeFrameSize = Math.Round(7 * pixelsPerModule, 2);

        var graphicItem = storage.GetGraphic(option.Eyes.UpperLeft.Frame.ToString(), QrGraphicType.Frame);

        var eyeFrameColor = option.Colors.BackgroundType != QrColorType.Solid && option.Eyes.HasGradient ? Color.White : option.Eyes.UpperLeft.FrameColor;
        var frame = string.Format(GraphicRotateColor, graphicItem.TopLeft, eyeFrameColor.ToHtmlRgb(), graphicItem.GraphicData);
        var scaleSize = Math.Round(eyeFrameSize / graphicItem.GraphicSize, 2);
        svgFile.AppendLine(string.Format(GraphicPosScale, 0, 0, SvgVal(scaleSize), frame));

        //top right
        graphicItem = storage.GetGraphic(option.Eyes.UpperRight.Frame.ToString(), QrGraphicType.Frame);
        eyeFrameColor = option.Colors.BackgroundType != QrColorType.Solid && option.Eyes.HasGradient ? Color.White : option.Eyes.UpperRight.FrameColor;
        frame = string.Format(GraphicRotateColor, graphicItem.TopRight, eyeFrameColor.ToHtmlRgb(), graphicItem.GraphicData);
        scaleSize = Math.Round(eyeFrameSize / graphicItem.GraphicSize, 2);
        svgFile.AppendLine(string.Format(GraphicPosScale, boxSize - eyeFrameSize, 0, SvgVal(scaleSize), frame));

        //bottom left
        graphicItem = storage.GetGraphic(option.Eyes.LowerLeft.Frame.ToString(), QrGraphicType.Frame);
        eyeFrameColor = option.Colors.BackgroundType != QrColorType.Solid && option.Eyes.HasGradient ? Color.White : option.Eyes.LowerLeft.FrameColor;
        frame = string.Format(GraphicRotateColor, graphicItem.BottomLeft, eyeFrameColor.ToHtmlRgb(), graphicItem.GraphicData);
        scaleSize = Math.Round(eyeFrameSize / graphicItem.GraphicSize, 2);
        svgFile.AppendLine(string.Format(GraphicPosScale, 0, boxSize - eyeFrameSize, SvgVal(scaleSize), frame));

        svgFile.AppendLine("</g>");
    }

    private void CreateEyeBall(QrRenderOption option, StringBuilder svgFile, double pixelsPerModule, int boxCount)
    {
        svgFile.AppendLine("<g id=\"EyeBall\">");
        var eyeFrameSize = Math.Round(3 * pixelsPerModule, 2);

        var graphicItem = storage.GetGraphic(option.Eyes.UpperLeft.Ball.ToString(), QrGraphicType.Ball);
        var scaleSize = Math.Round(eyeFrameSize / graphicItem.GraphicSize, 2);
        var xX = Math.Round(2 * pixelsPerModule, 2);
        var yY = Math.Round(2 * pixelsPerModule, 2);

        var eyeFrameColor = option.Colors.BackgroundType != QrColorType.Solid && option.Eyes.HasGradient ? Color.White : option.Eyes.UpperLeft.BallColor;
        var frame = string.Format(GraphicRotateColor, graphicItem.TopLeft, eyeFrameColor.ToHtmlRgb(), graphicItem.GraphicData);
        svgFile.AppendLine(string.Format(GraphicPosScale, xX, yY, SvgVal(scaleSize), frame));

        graphicItem = storage.GetGraphic(option.Eyes.UpperRight.Ball.ToString(), QrGraphicType.Ball);
        scaleSize = Math.Round(eyeFrameSize / graphicItem.GraphicSize, 2);
        xX = Math.Round((boxCount - 5) * pixelsPerModule, 2);
        yY = Math.Round(2 * pixelsPerModule, 2);
        eyeFrameColor = option.Colors.BackgroundType != QrColorType.Solid && option.Eyes.HasGradient ? Color.White : option.Eyes.UpperRight.BallColor;
        frame = string.Format(GraphicRotateColor, graphicItem.TopRight, eyeFrameColor.ToHtmlRgb(), graphicItem.GraphicData);
        svgFile.AppendLine(string.Format(GraphicPosScale, xX, yY, SvgVal(scaleSize), frame));

        graphicItem = storage.GetGraphic(option.Eyes.LowerLeft.Ball.ToString(), QrGraphicType.Ball);
        scaleSize = Math.Round(eyeFrameSize / graphicItem.GraphicSize, 2);
        xX = Math.Round(2 * pixelsPerModule, 2);
        yY = Math.Round((boxCount - 5) * pixelsPerModule, 2);
        eyeFrameColor = option.Colors.BackgroundType != QrColorType.Solid && option.Eyes.HasGradient ? Color.White : option.Eyes.LowerLeft.BallColor;
        frame = string.Format(GraphicRotateColor, graphicItem.BottomLeft, eyeFrameColor.ToHtmlRgb(), graphicItem.GraphicData);
        svgFile.AppendLine(string.Format(GraphicPosScale, xX, yY, SvgVal(scaleSize), frame));

        svgFile.AppendLine("</g>");
    }
}