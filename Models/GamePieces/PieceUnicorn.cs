﻿using FairyChess.Enums;

using Svg;


namespace FairyChess.Models.GamePieces;

public class PieceUnicorn : GamePiece
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
    public PieceUnicorn((int x, int y) position, PieceColor color)
        : base(position, color) { }

    static PieceUnicorn()
    {
        SvgDocument svgWhite = Program.OpenSVG("unicornW");
        SvgDocument svgBlack = Program.OpenSVG("unicornB");

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
            delta switch
            {
                (-1 or 1, 0) => EatOrMove(cell.HeldPiece),
                (0, -1 or 1) => EatOrMove(cell.HeldPiece),

                (var a and > 0, var b) when a + 1 == b
                    => CanMoveTo(end.x - 1, end.y - 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,
                (var a and > 0, var b) when a + 1 == -b
                    => CanMoveTo(end.x - 1, end.y + 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,

                (var a and < 0, var b) when a - 1 == b
                    => CanMoveTo(end.x + 1, end.y + 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,
                (var a and < 0, var b) when a - 1 == -b
                    => CanMoveTo(end.x + 1, end.y - 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,

                (var a and < 0, var b) when a + 1 == b
                    => CanMoveTo(end.x + 1, end.y + 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,
                (var a and < 0, var b) when a + 1 == -b
                    => CanMoveTo(end.x + 1, end.y - 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,

                (var a and > 0, var b) when a - 1 == b
                    => CanMoveTo(end.x - 1, end.y - 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,
                (var a and > 0, var b) when a - 1 == -b
                    => CanMoveTo(end.x - 1, end.y + 1) is MoveClass.Move
                           ? EatOrMove(cell.HeldPiece)
                           : MoveClass.None,

                _ => MoveClass.None,
            };

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

        return delta switch
               {
                   (-1 or 1, 0) => true,
                   (0, -1 or 1) => true,

                   (var a and > 0, var b) when a + 1 == b
                       => CanCaptureTo(end.x - 1, end.y - 1)
                       && GameBoard.Main[end.x - 1, end.y - 1].HeldPiece is null,
                   (var a and > 0, var b) when a + 1 == -b
                       => CanCaptureTo(end.x - 1, end.y + 1)
                       && GameBoard.Main[end.x - 1, end.y + 1].HeldPiece is null,

                   (var a and < 0, var b) when a - 1 == b
                       => CanCaptureTo(end.x + 1, end.y + 1)
                       && GameBoard.Main[end.x + 1, end.y + 1].HeldPiece is null,
                   (var a and < 0, var b) when a - 1 == -b
                       => CanCaptureTo(end.x + 1, end.y - 1)
                       && GameBoard.Main[end.x + 1, end.y - 1].HeldPiece is null,

                   (var a and < 0, var b) when a + 1 == b
                       => CanCaptureTo(end.x + 1, end.y + 1)
                       && GameBoard.Main[end.x + 1, end.y].HeldPiece is null,
                   (var a and < 0, var b) when a + 1 == -b
                       => CanCaptureTo(end.x + 1, end.y - 1)
                       && GameBoard.Main[end.x + 1, end.y - 1].HeldPiece is null,

                   (var a and > 0, var b) when a - 1 == b
                       => CanCaptureTo(end.x - 1, end.y - 1)
                       && GameBoard.Main[end.x - 1, end.y - 1].HeldPiece is null,
                   (var a and > 0, var b) when a - 1 == -b
                       => CanCaptureTo(end.x - 1, end.y + 1)
                       && GameBoard.Main[end.x - 1, end.y + 1].HeldPiece is null,

                   _ => false,
               };
    }
    #endregion
}