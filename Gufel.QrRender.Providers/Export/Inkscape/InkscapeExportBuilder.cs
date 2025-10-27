using CliWrap;
using System.Text;

namespace Gufel.QrRender.Providers.Export.Inkscape;

/// <summary>
/// https://inkscape.org/doc/inkscape-man.html
/// </summary>
public class InkscapeExportBuilder(string inkscapePath)
{
    private string? _outputFilePath;
    private bool _overWrite = true;
    private int? _dpi;
    private int? _width;
    private int? _height;
    private float? _margin;
    private InkscapePdfVersion? _pdfVersion;
    private InkscapeExportTypes[]? _exportTypes;
    private string? _backgroundColor;
    private float? _backgroundOpacity;
    private InkscapePngColorMode? _pngColorMode;
    private string? _inputFilePath;

    /// <summary>
    /// Overwrite input file (otherwise add '_out' suffix if type doesn't change)
    /// </summary>
    /// <param name="overwrite">default is true</param>
    /// <returns></returns>
    public InkscapeExportBuilder Overwrite(bool overwrite)
    {
        _overWrite = overwrite;
        return this;
    }

    public InkscapeExportBuilder InputFile(string path)
    {
        _inputFilePath = path;
        return this;
    }

    /// <summary>
    ///  Output file name (defaults to input filename; file type is guessed from extension if present; use '-' to write to stdout)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public InkscapeExportBuilder OutputFile(string path)
    {
        _outputFilePath = path;
        return this;
    }

    /// <summary>
    ///  Resolution for bitmaps and rasterized filters; default is 96
    /// </summary>
    /// <param name="dpi"></param>
    /// <returns></returns>
    public InkscapeExportBuilder Dpi(int dpi)
    {
        _dpi = dpi;
        return this;
    }

    /// <summary>
    /// Bitmap width in pixels (overrides --export-dpi)
    /// </summary>
    /// <param name="width"></param>
    /// <returns></returns>
    public InkscapeExportBuilder Width(int width)
    {
        _width = width;
        return this;
    }

    /// <summary>
    /// Bitmap height in pixels (overrides --export-dpi)
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    public InkscapeExportBuilder Height(int height)
    {
        _height = height;
        return this;
    }

    /// <summary>
    ///  Margin around export area: units of page size for SVG, mm for PS/PDF
    /// </summary>
    /// <param name="margin"></param>
    /// <returns></returns>
    public InkscapeExportBuilder Margin(float margin)
    {
        _margin = margin;
        return this;
    }

    /// <summary>
    ///  Background color for exported bitmaps (any SVG color string) for example "#ff007f" or "rgb(255, 0, 128)"
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public InkscapeExportBuilder Background(string color)
    {
        _backgroundColor = color;
        return this;
    }

    /// <summary>
    /// PDF version (1.4 or 1.5); default is 1.5
    /// </summary>
    /// <param name="pdfVersion"></param>
    /// <returns></returns>
    public InkscapeExportBuilder PdfVersion(InkscapePdfVersion pdfVersion)
    {
        _pdfVersion = pdfVersion;
        return this;
    }

    /// <summary>
    /// Background opacity for exported bitmaps (0.0 to 1.0, or 1 to 255)
    /// </summary>
    /// <param name="opacity"></param>
    /// <returns></returns>
    public InkscapeExportBuilder BackgroundOpacity(float opacity)
    {
        _backgroundOpacity = opacity;
        return this;
    }

    /// <summary>
    /// Color mode (bit depth and color type) for exported bitmaps
    /// </summary>
    /// <param name="colorMode"></param>
    /// <returns></returns>
    public InkscapeExportBuilder PngColorMode(InkscapePngColorMode colorMode)
    {
        _pngColorMode = colorMode;
        return this;
    }

    /// <summary>
    /// File type(s) to export: [svg,png,ps,eps,pdf,emf,wmf,xaml]
    /// </summary>
    /// <param name="exportTypes"></param>
    /// <returns></returns>
    public InkscapeExportBuilder ExportType(InkscapeExportTypes[] exportTypes)
    {
        _exportTypes = exportTypes;
        return this;
    }

    public string Build(Dictionary<string, string>? otherParam = null)
    {
        var str = new StringBuilder();

        if (_exportTypes != null)
            str.Append($"--export-type={string.Join(",", _exportTypes.Select(x => x.ToString()))} ");

        if (!string.IsNullOrEmpty(_outputFilePath))
            str.Append($"--export-filename=\"{_outputFilePath}\" ");

        if (_overWrite)
            str.Append("--export-overwrite ");

        if (_dpi.HasValue)
            str.Append($"--export-dpi={_dpi} ");

        if (_width.HasValue)
            str.Append($"--export-width={_width} ");

        if (_height.HasValue)
            str.Append($"--export-height={_height} ");

        if (_margin.HasValue)
            str.Append($"--export-margin={_margin} ");

        if (_pdfVersion != null)
            str.Append($"--export-pdf-version={(_pdfVersion == InkscapePdfVersion.v1_5 ? "1.5" : "1.4")} ");

        if (!string.IsNullOrEmpty(_backgroundColor))
            str.Append($"--export-background={_backgroundColor} ");

        if (_backgroundOpacity.HasValue)
            str.Append($"--export-background-opacity={_backgroundOpacity} ");

        if (_pngColorMode != null)
            str.Append($"--export-png-color-mode={_pngColorMode} ");

        if (otherParam != null)
        {
            foreach (var keyValue in otherParam)
            {
                str.Append($"{keyValue.Key}={keyValue.Value} ");
            }
        }

        str.Append($"\"{_inputFilePath}\"");
        return str.ToString();
    }

    public async Task<CommandResult> Execute(Dictionary<string, string>? otherParam = null)
    {
        var cli = Cli.Wrap(inkscapePath);
        var command = Build(otherParam);
        return await cli.WithArguments(command).ExecuteAsync();
    }
}