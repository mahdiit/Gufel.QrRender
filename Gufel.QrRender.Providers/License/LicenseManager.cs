using System.Text;
using Gufel.EcdKey;

namespace Gufel.QrRender.Providers.License;

public static class LicenseManager
{
    private static bool _isValid;
    private static DateTimeOffset? _expireDateTime;
    private static readonly EcdSignKey PublicKey = EcdSignKey.CreateFromJson(Properties.Resources.PublicKey);

    public static void Validate()
    {
        if (_isValid) return;

        throw new InvalidLicenseException();
    }


    private static void Validate(string? license)
    {
        if (string.IsNullOrEmpty(license))
            throw new InvalidLicenseException();

        var dataPart = license.Split('.');
        if (dataPart.Length != 2)
            throw new InvalidLicenseException();

        var content = EcdTools.FromUrlSafeBase64(dataPart[0]);
        var signature = EcdTools.FromUrlSafeBase64(dataPart[1]);

        _isValid = EcdSignKey.VerifyData(content, signature, PublicKey);
        if (!_isValid) throw new InvalidLicenseException();

        var dataContent = Encoding.UTF8.GetString(content);
        _expireDateTime = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(dataContent));

        _isValid = _expireDateTime >= DateTimeOffset.UtcNow;
    }

    public static void SetLicense(LicenseType licenseType, string? license = null)
    {
        if (licenseType == LicenseType.OpenSource)
        {
            Console.WriteLine("This application using open-source license of Gufel QrRender, use commercial license for private use");
            _isValid = true;
            return;
        }

        Validate(license);
        _isValid = true;
    }
}

public class InvalidLicenseException() : Exception("Invalid license, please set valid license");

public enum LicenseType
{
    OpenSource = 1000,
    Commercial = 2000
}