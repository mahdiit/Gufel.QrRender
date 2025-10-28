using System.Text.Json;
using Gufel.QrRender.Models;
using Gufel.QrRender.Models.Storage;
using Gufel.QrRender.Models.Storage.Model;

namespace Gufel.QrRender.Providers.Storage;

public class StaticResourceStorage
    (ILogoLoader logoStorage)
    : IResourceStorage
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
    };

    private readonly IReadOnlyList<GraphicDataItem> _graphics = JsonSerializer.Deserialize<IReadOnlyList<GraphicDataItem>>(Properties.Resources.GraphicData, Options)!;

    private readonly IReadOnlyList<TemplateDataItem> _templates = JsonSerializer.Deserialize<IReadOnlyList<TemplateDataItem>>(Properties.Resources.Template, Options)!;

    public GraphicDataItem GetGraphic(string id, QrGraphicType graphicType)
    {
        var result = _graphics?
            .FirstOrDefault(x =>
                x.GraphicID == id
                && x.GraphicTypeCode == (byte)graphicType);

        if (result == null)
            throw new Exception("element not found GraphicID=" + id);

        return result;
    }

    public QrRenderOption GetTemplate(int templateId)
    {
        var template = _templates?.FirstOrDefault(x => x.Code == templateId);
        if (template == null)
            throw new Exception("template not found " + templateId);

        return TemplateAsOption(template);
    }

    private QrRenderOption TemplateAsOption(TemplateDataItem template)
    {
        var result = new QrRenderOption(2000);

        if (!string.IsNullOrEmpty(template.BackgroundColor))
            result.Colors.Background = template.BackgroundColor.FromHtml();

        if (!string.IsNullOrEmpty(template.BodyColor))
            result.Colors.Body = template.BodyColor.FromHtml();

        if (!string.IsNullOrEmpty(template.BallColor))
            result.Eyes.SetBallColor(template.BallColor.FromHtml());

        if (!string.IsNullOrEmpty(template.FrameColor))
            result.Eyes.SetFrameColor(template.FrameColor.FromHtml());

        result.Eyes.SetBall(template.Ball);
        result.Body = template.Body;
        result.Eyes.SetFrame(template.Frame);

        if (!string.IsNullOrEmpty(template.BodyColorGradient))
        {
            result.Colors.BackgroundType = (QrColorType)template.GradientType
                .GetValueOrDefault((byte)QrColorType.LinearGradient);

            result.Colors.Gradient = template.BodyColorGradient.FromHtml();
            result.Eyes.HasGradient = template.GradientOnEyes.GetValueOrDefault() == 1;
        }

        if (template.QuietZonePixel >= 0)
            result.QuietZone = template.QuietZonePixel;

        if (string.IsNullOrEmpty(template.Logo)) return result;

        result.Logo = logoStorage.Load(template.Logo);

        return result;
    }
}