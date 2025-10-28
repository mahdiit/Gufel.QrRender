using Gufel.QrRender.Models;
using Gufel.QrRender.Models.Storage;
using Gufel.QrRender.Providers;
using Gufel.QrRender.Providers.Export.Inkscape;
using Gufel.QrRender.Providers.Export.Magick;
using Gufel.QrRender.Providers.Export.Skia;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.ComponentModel.DataAnnotations;

namespace Gufel.QrRender.SampleWebApi.Controllers
{
    /// <summary>
    /// Controller for QR code related operations
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class QrCodeController(IConfiguration configuration, IResourceRepository resourceStorage)
        : ControllerBase
    {
        /// <summary>
        /// Gets the INKSCAPE_PATH configuration value
        /// </summary>
        /// <returns>The INKSCAPE_PATH configuration value or 'Not Found' if not set</returns>
        [HttpGet(Name = "GetByTemplateId")]
        public async Task<IActionResult> GetByTemplateId(
            [Range(1, 19)] int templateId,
            [Required] UiExportType exportType,
            [Required] UiServiceType serviceType,
            [Required] string payload,
            [Required] QRCodeGenerator.ECCLevel level)
        {
            using var qrCodeData = QRCodeGenerator.GenerateQrCode(payload, level);
            using var svgRenderer = new SvgQrCode(resourceStorage);
            svgRenderer.SetQRCodeData(qrCodeData);

            var svgContent = svgRenderer.GetGraphic(templateId: templateId).SvgContent;
            return await RenderAndReturnResult($"output-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}", svgContent, exportType, serviceType);
        }

        [HttpPost(Name = "GetByOption")]
        public async Task<IActionResult> GetByOption(QrRenderByOption request)
        {
            var renderOption = Cast(request.Option);
            using var qrCodeData = QRCodeGenerator.GenerateQrCode(request.Payload, request.Level);
            using var svgRenderer = new SvgQrCode(resourceStorage);
            svgRenderer.SetQRCodeData(qrCodeData);

            var svgContent = svgRenderer.GetGraphic(option: renderOption).SvgContent;
            return await RenderAndReturnResult($"output-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}", svgContent, request.ExportType, request.ServiceType);
        }

        private static QrRenderOption Cast(QrRenderOptionViewModel requestOption)
        {
            return new QrRenderOption
            {
                Body = requestOption.Body,
                BoxSize = requestOption.BoxSize,
                QuietZone = requestOption.QuietZone.GetValueOrDefault(Convert.ToInt32(Math.Floor(requestOption.BoxSize * 0.07))),
                Colors = new QrColors
                {
                    Body = requestOption.Colors.Body.FromHtml(),
                    Background = requestOption.Colors.Background.FromHtml(),
                    BackgroundType = requestOption.Colors.BackgroundType,
                    Gradient = requestOption.Colors.Gradient?.FromHtml()
                },
                Eyes = new QrEyes
                {
                    HasGradient = requestOption.Eyes.HasGradient,
                    LowerLeft = Cast(requestOption.Eyes.LowerLeft),
                    UpperLeft = Cast(requestOption.Eyes.UpperLeft),
                    UpperRight = Cast(requestOption.Eyes.UpperRight)
                }
            };
        }

        private static QrEye Cast(QrEyeViewModel eye)
        {
            return new QrEye
            {
                Ball = eye.Ball,
                BallColor = eye.BallColor.FromHtml(),
                Frame = eye.Frame,
                FrameColor = eye.FrameColor.FromHtml()
            };
        }

        private async Task<IActionResult> RenderAndReturnResult(string svgFileName, string svgContent, UiExportType exportType, UiServiceType serviceType)
        {
            var dir = new DirectoryInfo("wwwroot/_temp");
            var svgPath = Path.Combine(dir.FullName, $"{svgFileName}.svg");

            await System.IO.File.WriteAllTextAsync(svgPath, svgContent, HttpContext.RequestAborted);

            if (exportType == UiExportType.Svg)
            {
                return File(System.IO.File.Open(svgPath, FileMode.Open), "image/svg+xml", "output.svg");
            }

            var fileName = exportType == UiExportType.Png ? $"{svgFileName}.png" : $"{svgFileName}.pdf";
            var exportFile = Path.Combine(dir.FullName, fileName);

            switch (serviceType)
            {
                case UiServiceType.Inkscape when configuration["INKSCAPE_PATH"] == null:
                    return NotFound(new { Error = "INKSCAPE_PATH variable not found" });
                case UiServiceType.Inkscape:
                    {
                        var inkscape = new InkscapeExportBuilder(configuration["INKSCAPE_PATH"]!);
                        inkscape.ExportType([exportType == UiExportType.Pdf ? InkscapeExportTypes.pdf : InkscapeExportTypes.png]);
                        inkscape.InputFile(svgPath);
                        await inkscape.Execute();
                        break;
                    }
                case UiServiceType.MagicNet when exportType == UiExportType.Pdf:
                    MagickNetExport.SvgToPdf(svgPath, exportFile);
                    break;
                case UiServiceType.MagicNet:
                    MagickNetExport.SvgToPng(svgPath, exportFile);
                    break;
                case UiServiceType.SkiaSharp when exportType == UiExportType.Pdf:
                    SkiaSharpExport.SvgToPdf(svgPath, exportFile);
                    break;
                case UiServiceType.SkiaSharp:
                    SkiaSharpExport.SvgToPng(svgPath, exportFile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceType), serviceType, null);
            }

            var contentType = exportType == UiExportType.Png ? "image/png" : "application/pdf";
            return File(System.IO.File.Open(exportFile, FileMode.Open), contentType, fileName);
        }
    }
}
