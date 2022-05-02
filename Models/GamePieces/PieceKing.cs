using FairyChess.Enums;

using Svg;


namespace FairyChess.Models.GamePieces;

public class PieceKing : GamePiece
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
    public PieceKing((int x, int y) position, PieceColor color)
        : base(position, color) { }

    static PieceKing()
    {
        SvgDocument svgWhite = Program.OpenSVG("kingW");
        SvgDocument svgBlack = Program.OpenSVG("kingB");

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
                (-1 or 0 or 1, -1 or 0 or 1)
                    when IsInCheck(end.x, end.y) is false
                    => EatOrMove(cell.HeldPiece),

                // TODO: Castling

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

        return delta is (-1 or 0 or 1, -1 or 0 or 1);
    }

    private bool IsInCheck(int x, int y)
    {
        GameCell cell1 = MainForm.Board[Position.x, Position.y];
        GameCell cell2 = MainForm.Board[x, y];

        GamePiece? temp = cell2.HeldPiece;
        cell1.HeldPiece = null;
        cell2.HeldPiece = this;
        bool output = MainForm.Board.Pieces
                              .Where(piece => piece.Color != Color)
                              .Any(piece => piece.CanCaptureTo(x, y));

        cell1.HeldPiece = this;
        cell2.HeldPiece = temp;

        return output;
    }
    #endregion
}