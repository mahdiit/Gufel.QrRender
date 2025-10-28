namespace Gufel.QrRender.Providers.Storage;

public interface ILogoRepository
{
    (byte[] Content, bool IsSvg) Load(string name);
}