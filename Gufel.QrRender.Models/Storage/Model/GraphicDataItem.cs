namespace Gufel.QrRender.Models.Storage.Model
{
    public record GraphicDataItem
    {
        public int Code { get; init; }
        public string GraphicID { get; init; }
        public byte GraphicTypeCode { get; init; }
        public string GraphicData { get; init; }
        public string TopLeft { get; init; }
        public string TopRight { get; init; }
        public string BottomLeft { get; init; }
        public int GraphicSize { get; init; }
    }
}
