using FairyChess.Enums;

using Svg;


namespace FairyChess.Models.GamePieces;

public class PieceGuard : GamePiece
{
    #region Variables
    private static readonly Image WhiteImage;
    private static readonly Image BlackImage;
    #endregion

    #region Properties
    public override int Value
        => 0;

    public override Image ImageWhite
        => WhiteImage;

    public override Image ImageBlack
        => BlackImage;
    #endregion

    #region Constructors
    public PieceGuard((int x, int y) position, PieceColor color)
        : base(position, color) { }

    static PieceGuard()
    {
        SvgDocument svgWhite = Program.OpenSVG("guardW");
        SvgDocument svgBlack = Program.OpenSVG("guardB");

        WhiteImage = svgWhite.Draw(GameBoard.CELL_SIZE, GameBoard.CELL_SIZE);
        BlackImage = svgBlack.Draw(GameBoard.CELL_SIZE, GameBoard.CELL_SIZE);
    }
    #endregion

    #region Methods
    public override MoveClass CanMoveTo(int x, int y)
    {
        // If result is cached
        if (MoveCache.ContainsKey((this, x, y)))
            return MoveCache[(this, x, y)];

        // Set up the variables
        (int x, int y) start = Position,
                       end   = (x, y),
                       delta = (end.x - start.x, end.y - start.y);

        // Determines the result
        GameCell cell = GameBoard.Main[end.x, end.y];
        MoveClass output =
            delta is (-1 or 0 or 1, -1 or 0 or 1)
                ? EatOrMove(cell.HeldPiece)
                : MoveClass.None;

        output = output is not MoveClass.None && CheckSafe(end.x, end.y)
                     ? output
                     : MoveClass.None;

        // Cache the result
        MoveCache[(this, x, y)] = output;

        return output;
    }

    public override bool CanCaptureTo(int x, int y)
    {
        // Set up the variables
        (int x, int y) start = Position,
                       end   = (x, y),
                       delta = (end.x - start.x, end.y - start.y);

        return delta is (-1 or 0 or 1, -1 or 0 or 1);
    }
    #endregion
}