using System.Reflection;
using System.Runtime.CompilerServices;

using Svg;


namespace FairyChess;

public static class Program
{
    public static readonly string   ProjectPath = SourcePath.Value;
    public static readonly Assembly Asm         = Assembly.GetExecutingAssembly();

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(MainForm.Board);
    }

    public static SvgDocument OpenSVG(in string name)
        => SvgDocument.Open<SvgDocument>(Asm.GetManifestResourceStream($"FairyChess.Images.{name}.svg")!);
}

#region Source Path
public static class SourcePath
{
    private const  string  RELATIVE_PATH = nameof(Program) + ".cs";
    private static string? _lazyValue;

    public static string Value
        => _lazyValue ??= CalculatePath();

    private static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null) //
        => callerFilePath ?? "";

    private static string CalculatePath()
    {
        string pathName = GetSourceFilePathName();

        return pathName[..^RELATIVE_PATH.Length];
    }
}
#endregion