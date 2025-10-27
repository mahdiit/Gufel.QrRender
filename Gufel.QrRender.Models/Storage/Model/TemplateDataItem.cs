namespace Gufel.QrRender.Models.Storage.Model;

public record TemplateDataItem
{
    public int Code { get; init; }
    public string BodyColor { get; init; }
    public string FrameColor { get; init; }
    public string BallColor { get; init; }
    public string BackgroundColor { get; init; }
    public int QuietZonePixel { get; init; }
    public byte Frame { get; init; }
    public byte Ball { get; init; }
    public byte Body { get; init; }
    public string Logo { get; init; }
    public string Name { get; init; }
    public string BodyColorGradient { get; init; }
    public byte? GradientOnEyes { get; init; }
    public byte? GradientType { get; init; }
}