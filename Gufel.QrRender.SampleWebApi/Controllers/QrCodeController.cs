using Gufel.QrRender.Models.Storage;
using Gufel.QrRender.Providers;
using Gufel.QrRender.Providers.Export.Inkscape;
using Gufel.QrRender.Providers.Export.Magick;
using Gufel.QrRender.Providers.Export.Skia;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace Gufel.QrRender.SampleWebApi.Controllers
{
    /// <summary>
    /// Controller for QR code related operations
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class QrCodeController(IConfiguration configuration, IResourceStorage resourceStorage)
        : ControllerBase
    {
        /// <summary>
        /// Gets the INKSCAPE_PATH configuration value
        /// </summary>
        /// <returns>The INKSCAPE_PATH configuration value or 'Not Found' if not set</returns>
        [HttpGet(Name = "Get")]
        public async Task<IActionResult> Get(
            int templateId,
            UiExportType exportType,
            UiServiceType serviceType,
            string payload,
            QRCodeGenerator.ECCLevel level)
        {
            using var qrCodeData = QRCodeGenerator.GenerateQrCode(payload, level);
            using var svgRenderer = new SvgQrCode(resourceStorage);
            svgRenderer.SetQRCodeData(qrCodeData);

            var dir = new DirectoryInfo("wwwroot/_temp");
            var svgPath = Path.Combine(dir.FullName, "output.svg");
            await System.IO.File.WriteAllTextAsync(svgPath, svgRenderer.GetGraphic(templateId: templateId).SvgContent, HttpContext.RequestAborted);

            if (exportType == UiExportType.Svg)
            {
                return File(System.IO.File.Open(svgPath, FileMode.Open), "image/svg+xml", "output.svg");
            }

            var fileName = exportType == UiExportType.Png ? "output.png" : "output.pdf";
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

    public enum UiExportType
    {
        Pdf = 1,
        Svg = 2,
        Png = 3
    }

    public enum UiServiceType
    {
        Inkscape = 1,
        SkiaSharp = 2,
        MagicNet = 3
    }
}
