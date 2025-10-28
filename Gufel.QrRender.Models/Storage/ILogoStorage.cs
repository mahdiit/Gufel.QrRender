namespace Gufel.QrRender.Models.Storage;

public interface ILogoLoader
{
    QrLogo? Load(string name);
}