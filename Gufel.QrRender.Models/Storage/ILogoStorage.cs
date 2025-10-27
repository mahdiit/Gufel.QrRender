namespace Gufel.QrRender.Models.Storage;

public interface ILogoStorage
{
    QrLogo? Load(string name);
}