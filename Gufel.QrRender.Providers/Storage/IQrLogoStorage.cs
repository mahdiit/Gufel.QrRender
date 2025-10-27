namespace Gufel.QrRender.Providers.Storage;

public interface IQrLogoStorage
{
    string Path { get; }
}

public record QrLogoStorage(string Path) : IQrLogoStorage
{

}