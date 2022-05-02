using FairyChess.Enums;

using Svg;


namespace FairyChess.Models.GamePieces;

public class PiecePawn : GamePiece
{
    #region Variables
    private static readonly Image WhiteImage;
    private static readonly Image BlackImage;
    #endregion

    #region Properties
    public override int Value
        => 1;

    public override Image ImageWhite
        => WhiteImage;

    public override Image ImageBlack
        => BlackImage;
    #endregion

    #region Constructors
    public PiecePawn((int x, int y) position, PieceColor color)
        : base(position, color) { }

    static PiecePawn()
    {
        SvgDocument svgWhite = Program.OpenSVG("pawnW");
        SvgDocument svgBlack = Program.OpenSVG("pawnB");

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
        GameCell  cell = MainForm.Board[x, y];
        MoveClass output;
        switch (delta)
        {
            // White - Move
            case (0, 1) when cell.HeldPiece is null
                          && Color is PieceColor.White:
            case (0, 2) when cell.HeldPiece is null
                          && start.y <= 1
                          && Color is PieceColor.White
                          && CanMoveTo(x, y - 1) is MoveClass.Move:

            // Black - Move
            case (0, -1) when cell.HeldPiece is null
                           && Color is PieceColor.Black:
            case (0, -2) when cell.HeldPiece is null
                           && start.y >= 6
                           && Color is PieceColor.Black
                           && CanMoveTo(x, y + 1) is MoveClass.Move:
                output = MoveClass.Move;

                break;

            // White & Black - Capture
            case (-1 or 1, 1) when cell.HeldPiece?.Color is PieceColor.Black
                                && Color is PieceColor.White:
            case (-1 or 1, -1) when cell.HeldPiece?.Color is PieceColor.White
                                 && Color is PieceColor.Black:
                output = MoveClass.Capture;

                break;

            // White & Black - En Passant
            case (-1 or 1, 1) when cell.HeldPiece is null
                                && MainForm.Board[x, y - 1].HeldPiece is
                                       PiecePawn { Color: PieceColor.Black, TurnSinceMoved: 1 }
                                && Color is PieceColor.White:
            case (-1 or 1, -1) when cell.HeldPiece is null
                                 && MainForm.Board[x, y + 1].HeldPiece is
                                        PiecePawn { Color: PieceColor.White, TurnSinceMoved: 1 }
                                 && Color is PieceColor.Black:
                output = MoveClass.EnPassant;

                break;

            // Unable to move
            default:
                output = MoveClass.None;

                break;
        }

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
                   (-1 or 1, 1)
                       when Color is PieceColor.White => true,
                   (-1 or 1, -1)
                       when Color is PieceColor.Black => true,

                   _ => false,
               };
    }
    #endregion
}