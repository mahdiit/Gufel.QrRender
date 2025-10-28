using Gufel.QrRender.Models;
using Gufel.QrRender.Models.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Gufel.QrRender.Providers.Storage;

public sealed class LogoLoader(ILogoRepository logoRepository) : ILogoLoader
{
    private static QrLogo ImageLoad(byte[] imageData)
    {
        using var image = Image.Load(imageData);
        var mimeType = GetMimeTypeFromImageSharpFormat(image.Metadata.DecodedImageFormat);
        if (string.IsNullOrEmpty(mimeType))
            throw new Exception("Logo image MimeType is not found, please support valid image file");

        byte[] imgData;
        using (var ms = new MemoryStream())
        {
            image.Save(ms, image.Metadata.DecodedImageFormat ?? throw new InvalidOperationException("Image format could not be determined"));
            imgData = ms.ToArray();
        }

        return new QrLogo()
        {
            Data = "data:" + mimeType + ";base64," + Convert.ToBase64String(imgData),
            IsSvg = false,
            LogoSize = new System.Drawing.SizeF(image.Width, image.Height)
        };
    }

    private static string GetMimeTypeFromImageSharpFormat(IImageFormat? format)
    {
        if (format == null) return string.Empty;

        var formatName = format.DefaultMimeType?.ToLowerInvariant();
        return formatName switch
        {
            "image/jpeg" or "image/jpg" => "image/jpeg",
            "image/png" => "image/png",
            "image/gif" => "image/gif",
            "image/bmp" => "image/bmp",
            "image/webp" => "image/webp",
            "image/tiff" => "image/tiff",
            _ => format.DefaultMimeType ?? string.Empty
        };
    }

    private static QrLogo SvgLoad(string svgContent)
    {
        var cl = new CultureInfo("en-US");
        var doc = new XmlDocument();
        doc.LoadXml(svgContent);

        if (doc.DocumentElement == null)
            throw new ArgumentException("Svg document element is missing", svgContent);

        System.Drawing.SizeF logoSize;
        if (doc.DocumentElement.HasAttribute("viewBox"))
        {
            var viewBoxAttr = doc.DocumentElement.Attributes["viewBox"];
            var viewBoxData = viewBoxAttr?.Value.Split(' ');
            if (viewBoxData == null || viewBoxData.Length != 4)
                throw new ArgumentException("Svg width or height is missing", svgContent);

            logoSize = new System.Drawing.SizeF(
                Convert.ToSingle(viewBoxData[2], cl),
                Convert.ToSingle(viewBoxData[3], cl));
        }
        else if (doc.DocumentElement.HasAttribute("width")
                 && doc.DocumentElement.HasAttribute("height"))
        {
            var widthValue = doc.DocumentElement.Attributes["width"]?.Value;
            var heightValue = doc.DocumentElement.Attributes["height"]?.Value;

            logoSize = new System.Drawing.SizeF(Convert.ToSingle(widthValue, cl), Convert.ToSingle(heightValue, cl));
        }
        else
            throw new ArgumentException("Svg width or height is missing", svgContent);

        return new QrLogo()
        {
            Data = doc.DocumentElement.InnerXml,
            IsSvg = true,
            LogoSize = logoSize
        };
    }

    public QrLogo? Load(string name)
    {
        var data = logoRepository.Load(name);

        return data.IsSvg
             ? SvgLoad(Encoding.UTF8.GetString(data.Content))
             : ImageLoad(data.Content);
    }
}