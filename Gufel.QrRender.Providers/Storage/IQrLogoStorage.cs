namespace Gufel.QrRender.Providers.Storage;

public interface ILogoRepository
{
    (byte[] Content, bool IsSvg) Load(string name);
}

public record PhysicalLogoRepository(string Path) : ILogoRepository
{
    public (byte[] Content, bool IsSvg) Load(string name)
    {
        return (File.ReadAllBytes(System.IO.Path.Combine(Path, name)), name.EndsWith(".svg"));
    }
}