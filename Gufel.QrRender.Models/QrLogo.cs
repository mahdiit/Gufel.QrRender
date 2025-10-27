using System.Drawing;

namespace Gufel.QrRender.Models;

public record QrLogo
{
    public string Data { get; init; }

    public bool IsSvg { get; init; }

    public SizeF LogoSize { get; init; }
}