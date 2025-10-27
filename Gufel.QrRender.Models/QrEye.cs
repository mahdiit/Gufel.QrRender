using System.Drawing;

namespace Gufel.QrRender.Models;

public class QrEye
{
    public Color FrameColor { get; set; } = Color.Black;

    public Color BallColor { get; set; } = Color.Black;

    public byte Frame { get; set; }

    public byte Ball { get; set; }
}