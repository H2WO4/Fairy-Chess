using System.Reflection;

using Svg;


namespace FairyChess;

public static class Program
{
    private static readonly Assembly Asm = Assembly.GetExecutingAssembly();

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(MainForm.Board);
    }

    /// <summary>
    /// Get an embedded SVG image
    /// </summary>
    /// <param name="name">The name of the embedded resource</param>
    public static SvgDocument OpenSVG(in string name)
        => SvgDocument.Open<SvgDocument>(Asm.GetManifestResourceStream($"FairyChess.Images.{name}.svg")!);

    /// <summary>
    /// Get an embedded icon
    /// </summary>
    /// <param name="name">The name of the embedded resource</param>
    public static Icon OpenIcon(in string name)
        => new(Asm.GetManifestResourceStream($"FairyChess.Images.{name}.ico")!);
}