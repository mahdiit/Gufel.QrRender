namespace Gufel.QrRender.Models;

public class QrRenderOption(int boxSize = 512)
{
    public byte Body { get; set; } = 0;

    public int BoxSize { get; init; } = boxSize;

    public int QuietZone { get; set; } = Convert.ToInt32(Math.Floor(boxSize * 0.07));

    public QrEyes Eyes { get; init; } = new QrEyes();

    public QrColors Colors { get; init; } = new QrColors();

    public int ViewWidth => BoxSize + QuietZone * 2;

    public QrLogo? Logo { get; set; }
}