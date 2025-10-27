using System.Drawing;

namespace Gufel.QrRender.Models;

public class QrColors
{
    public Color Body { get; set; } = Color.Black;

    public Color Background { get; set; } = Color.White;

    public Color? Gradient { get; set; }

    public QrColorType BackgroundType { get; set; } = QrColorType.Solid;
}