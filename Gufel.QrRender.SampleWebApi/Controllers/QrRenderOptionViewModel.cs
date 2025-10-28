using System.ComponentModel.DataAnnotations;
using Gufel.QrRender.Models;
using QRCoder;

namespace Gufel.QrRender.SampleWebApi.Controllers
{
    public record QrRenderByOption
    {
        [Required]
        public string Payload { get; init; }

        [Required]
        public QrRenderOptionViewModel Option { get; init; }

        [Required]
        public UiExportType ExportType { get; init; }

        [Required]
        public UiServiceType ServiceType { get; init; }

        [Required]
        public QRCodeGenerator.ECCLevel Level { get; init; }
    }

    public record QrRenderOptionViewModel
    {
        [Required]
        [Range(0, 21)]
        public byte Body { get; init; }

        [Required]
        [Range(128, 5000)]
        public int BoxSize { get; init; }

        public int? QuietZone { get; init; }

        [Required]
        public QrEyesViewModel Eyes { get; init; }

        [Required]
        public QrColorsViewModel Colors { get; init; }
    }

    public record QrEyesViewModel
    {
        [Required]
        public QrEyeViewModel UpperLeft { get; init; }

        [Required]
        public QrEyeViewModel UpperRight { get; init; }

        [Required]
        public QrEyeViewModel LowerLeft { get; init; }

        public bool HasGradient { get; init; }
    }

    public record QrEyeViewModel
    {
        [Required]
        [RegularExpression(@"^#?(?:[0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$")]
        public string FrameColor { get; init; }

        [Required]
        [RegularExpression(@"^#?(?:[0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$")]
        public string BallColor { get; init; }

        [Required]
        [Range(0, 14)]
        public byte Frame { get; init; }

        [Required]
        [Range(0, 17)]
        public byte Ball { get; init; }
    }

    public class QrColorsViewModel
    {
        [Required]
        [RegularExpression(@"^#?(?:[0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$")]
        public string Body { get; init; }

        [Required]
        [RegularExpression(@"^#?(?:[0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$")]
        public string Background { get; init; }


        [RegularExpression(@"^#?(?:[0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$")]
        public string? Gradient { get; init; }

        [Required]
        public QrColorType BackgroundType { get; set; }
    }
}
