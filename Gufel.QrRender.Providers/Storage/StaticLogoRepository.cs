namespace Gufel.QrRender.Providers.Storage;

public class StaticLogoRepository : ILogoRepository
{
    public (byte[] Content, bool IsSvg) Load(string name)
    {
        var content = (byte[])Properties.Resources.ResourceManager.GetObject(name.Replace(".", ""))!;
        return (content, true);
    }
}