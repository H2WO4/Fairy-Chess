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
    protected (int x, int y) Position { get; private set; }

    /// <summary>
    /// How long has it been since the piece first move
    /// </summary>
    /// <remarks>
    /// Used for castling and en passant
    /// </remarks>
    protected int TurnSinceMoved { get; private set; }

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

        GameCell cell = GameBoard.Main[position.x, position.y];

        cell.HeldPiece = this;
        cell.Image = color == PieceColor.White
                         // ReSharper disable once VirtualMemberCallInConstructor
                         ? ImageWhite
                         // ReSharper disable once VirtualMemberCallInConstructor
                         : ImageBlack;

        GameBoard.Main.Pieces.Add(this);
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
    /// Moves a piece to a new cell, eating any piece there
    /// </summary>
    /// <param name="newCell">The cell to move to</param>
    public void MoveTo(GameCell newCell)
    {
        GameCell oldCell = GameBoard.Main[Position.x, Position.y];

        // If it is a capture, remove the capture piece from the list
        if (newCell.HeldPiece != null)
            GameBoard.Main.Pieces.Remove(newCell.HeldPiece);

        // Change positions
        oldCell.HeldPiece = null;
        newCell.HeldPiece = this;
        Position          = newCell.Position;

    #if !DEBUG_2
        GameBoard.ChangeTurn();
    #endif

        // Increment TurnSinceMoved
        foreach (GamePiece piece in GameBoard.Main.Pieces)
            if (piece.TurnSinceMoved > 0)
                piece.TurnSinceMoved++;

        if (TurnSinceMoved is 0)
            TurnSinceMoved = 1;

        // Promotion
        if (this is PiecePawn { Color: PieceColor.White, Position.y: 7 }
                 or PiecePawn { Color: PieceColor.Black, Position.y: 0 })
        {
            GameBoard.Main.Pieces.Remove(this);
            _ = new PieceQueen(Position, Color);
        }

        // Clear the now out-of-date cache
        ResetCache();

        // Update the images
        oldCell.UpdateImage();
        newCell.UpdateImage();

    #if !DEBUG_2
        // Reverse the board for the opposite player
        GameBoard.FlipBoard();
    #endif
    }

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

    /// <summary>
    /// Check that the king not in check, even if the piece were to move to a new cell
    /// </summary>
    /// <param name="x">The x coordinate of the new cell</param>
    /// <param name="y">The y coordinate of the new cell</param>
    /// <returns>Whether the king is safe from check after the move</returns>
    protected bool CheckSafe(int x, int y)
    {
        GameCell cell1 = GameBoard.Main[Position.x, Position.y];
        GameCell cell2 = GameBoard.Main[x, y];

        GamePiece? king = GameBoard.Main.Pieces
                                   .OfType<PieceKing>()
                                   .FirstOrDefault(piece => piece.Color == Color);

        if (king is null)
            return true;

        GamePiece? temp = cell2.HeldPiece;
        cell1.HeldPiece = null;
        cell2.HeldPiece = this;
        bool output = GameBoard.Main.Pieces
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