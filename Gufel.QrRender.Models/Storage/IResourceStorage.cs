using Gufel.QrRender.Models.Storage.Model;

namespace Gufel.QrRender.Models.Storage;

public interface IResourceStorage
{
    GraphicDataItem GetGraphic(string id, QrGraphicType graphicType);

    QrRenderOption GetTemplate(int templateId);
}