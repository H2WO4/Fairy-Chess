using System.Diagnostics;

using FairyChess.Enums;
using FairyChess.Models.GamePieces;


namespace FairyChess.Models;

/// <summary>
/// Represent a chess piece
/// </summary>
public abstract class GamePiece
{
    #region Variables
    protected static readonly Dictionary<(GamePiece, int, int), MoveClass> MoveCache = new();
    #endregion

    #region Properties
    /// <summary>
    /// Where the piece is located on the board
    /// </summary>
    public (int x, int y) Position { get; set; }

    /// <summary>
    /// How long has it been since the piece first move
    /// </summary>
    /// <remarks>
    /// Used for castling and en passant
    /// </remarks>
    public int TurnSinceMoved { get; set; }

    /// <summary>
    /// What color the piece is
    /// </summary>
    public PieceColor Color { get; }

    /// <summary>
    /// The image to use for white pieces
    /// </summary>
    public abstract Image ImageWhite { get; }

    /// <summary>
    /// The image to use for black pieces
    /// </summary>
    public abstract Image ImageBlack { get; }

    /// <summary>
    /// How much the piece is worth
    /// </summary>
    public abstract int Value { get; }
    #endregion

    #region Constructor
    protected GamePiece((int x, int y) position, PieceColor color)
    {
        Position = position;
        Color    = color;

        GameCell cell = MainForm.Board[position.x, position.y];

        cell.HeldPiece = this;
        cell.Image = color == PieceColor.White
                         ? ImageWhite
                         : ImageBlack;

        MainForm.Board.Pieces.Add(this);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Determines if the piece can move to a certain position on the grid
    /// </summary>
    /// <param name="x">The end X position</param>
    /// <param name="y">The end Y position</param>
    /// <returns>The type of movement possible to perform</returns>
    public abstract MoveClass CanMoveTo(int x, int y);

    /// <summary>
    /// Determines if the piece can potentially capture a certain position
    /// </summary>
    /// <param name="x">The end X position</param>
    /// <param name="y">The end Y position</param>
    public abstract bool CanCaptureTo(int x, int y);

    /// <summary>
    /// Determines if the movement is a move, capture, or nothing, based on the piece on the square
    /// </summary>
    /// <param name="piece">The piece on the square</param>
    /// <returns>The type of movement that applies</returns>
    protected MoveClass EatOrMove(GamePiece? piece)
    {
        return piece switch
               {
                   null                          => MoveClass.Move,
                   { } when piece.Color != Color => MoveClass.Capture,
                   _                             => MoveClass.None,
               };
    }

    protected bool CheckSafe(int x, int y)
    {
        GameCell cell1 = MainForm.Board[Position.x, Position.y];
        GameCell cell2 = MainForm.Board[x, y];

        GamePiece? king = MainForm.Board.Pieces
                                 .OfType<PieceKing>()
                                 .FirstOrDefault(piece => piece.Color == Color);

        if (king is null)
            return true;

        GamePiece? temp = cell2.HeldPiece;
        cell1.HeldPiece = null;
        cell2.HeldPiece = this;
        bool output = MainForm.Board.Pieces
                              .Where(piece => piece.Color != Color)
                              .Where(piece => piece != temp)
                              .Any(piece => piece.CanCaptureTo(king.Position.x, king.Position.y));

        cell1.HeldPiece = this;
        cell2.HeldPiece = temp;

        return !output;
    }

    /// <summary>
    /// Empties the cache
    /// </summary>
    public static void ResetCache()
        => MoveCache.Clear();
    #endregion
}