using FairyChess.Enums;

using Svg;


namespace FairyChess.Models.GamePieces;

public class PieceQueen : GamePiece
{
    #region Variables
    private static readonly Image WhiteImage;
    private static readonly Image BlackImage;
    #endregion

    #region Properties
    public override int Value
        => 3;

    public override Image ImageWhite
        => WhiteImage;

    public override Image ImageBlack
        => BlackImage;
    #endregion

    #region Constructors
    public PieceQueen((int x, int y) position, PieceColor color)
        : base(position, color) { }

    static PieceQueen()
    {
        SvgDocument svgWhite = Program.OpenSVG("queenW");
        SvgDocument svgBlack = Program.OpenSVG("queenB");

        WhiteImage = svgWhite.Draw(GameCell.CELL_SIZE, GameCell.CELL_SIZE);
        BlackImage = svgBlack.Draw(GameCell.CELL_SIZE, GameCell.CELL_SIZE);
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
        GameCell cell = MainForm.Board[end.x, end.y];
        MoveClass output =
            delta switch
            {
                (-1 or 0 or 1, -1 or 0 or 1) => EatOrMove(cell.HeldPiece),

                var (a, b) when a == b && a > 0
                    => CanMoveTo(end.x - 1, end.y - 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,

                var (a, b) when a == b && a < 0
                    => CanMoveTo(end.x + 1, end.y + 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,

                var (a, b) when a == -b && a > 0
                    => CanMoveTo(end.x - 1, end.y + 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,

                var (a, b) when a == -b && a < 0
                    => CanMoveTo(end.x + 1, end.y - 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,

                (> 0, 0) => CanMoveTo(end.x - 1, end.y) is MoveClass.Move
                                ? EatOrMove(cell.HeldPiece)
                                : MoveClass.None,

                (< 0, 0) => CanMoveTo(end.x + 1, end.y) is MoveClass.Move
                                ? EatOrMove(cell.HeldPiece)
                                : MoveClass.None,

                (0, > 0) => CanMoveTo(end.x, end.y - 1) is MoveClass.Move
                                ? EatOrMove(cell.HeldPiece)
                                : MoveClass.None,

                (0, < 0) => CanMoveTo(end.x, end.y + 1) is MoveClass.Move
                                ? EatOrMove(cell.HeldPiece)
                                : MoveClass.None,

                _ => MoveClass.None,
            };

        output = CheckSafe(end.x, end.y)
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

        return delta switch
               {
                   (-1 or 0 or 1, -1 or 0 or 1) => true,

                   var (a, b) when a == b && a > 0
                       => CanCaptureTo(end.x - 1, end.y - 1)
                       && MainForm.Board[end.x - 1, end.y - 1].HeldPiece is null,

                   var (a, b) when a == b && a < 0
                       => CanCaptureTo(end.x + 1, end.y + 1)
                       && MainForm.Board[end.x + 1, end.y + 1].HeldPiece is null,

                   var (a, b) when a == -b && a > 0
                       => CanCaptureTo(end.x - 1, end.y + 1)
                       && MainForm.Board[end.x - 1, end.y + 1].HeldPiece is null,

                   var (a, b) when a == -b && a < 0
                       => CanCaptureTo(end.x + 1, end.y - 1)
                       && MainForm.Board[end.x + 1, end.y - 1].HeldPiece is null,

                   (> 0, 0) => CanCaptureTo(end.x - 1, end.y)
                            && MainForm.Board[end.x - 1, end.y].HeldPiece is null,

                   (< 0, 0) => CanCaptureTo(end.x + 1, end.y)
                            && MainForm.Board[end.x + 1, end.y].HeldPiece is null,

                   (0, > 0) => CanCaptureTo(end.x, end.y - 1)
                            && MainForm.Board[end.x, end.y - 1].HeldPiece is null,

                   (0, < 0) => CanCaptureTo(end.x, end.y + 1)
                            && MainForm.Board[end.x, end.y + 1].HeldPiece is null,

                   _ => false,
               };
    }
    #endregion
}