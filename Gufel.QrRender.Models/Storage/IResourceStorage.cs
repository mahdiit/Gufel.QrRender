using Gufel.QrRender.Models.Storage.Model;

namespace Gufel.QrRender.Models.Storage;

public interface IResourceRepository
{
    GraphicDataItem GetGraphic(string id, QrGraphicType graphicType);

    QrRenderOption GetTemplate(int templateId);
}