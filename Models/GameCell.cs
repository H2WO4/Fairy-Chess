using FairyChess.Enums;
using FairyChess.Models.GamePieces;


namespace FairyChess.Models;

/// <summary>
/// Represents a cell of the game grid
/// </summary>
public class GameCell : PictureBox
{
    public const int CELL_SIZE   = 96;
    public const int NUM_ROWS    = 8;
    public const int NUM_COLUMNS = 10;

    #region Properties
    /// <summary>
    /// Determines if this is a light- or dark-colored cell
    /// </summary>
    public CellColor Color { get; }

    private (int x, int y) Position { get; }

    /// <summary>
    /// What <see cref="GamePiece"/> is currently on this cell
    /// </summary>
    public GamePiece? HeldPiece { get; set; }
    #endregion

    #region Constructors
    public GameCell(int x, int y)
    {
        Position = (x, y);
        Color = (x + y) % 2 == 0
                    ? CellColor.Light
                    : CellColor.Dark;

        Click       += OnClick;
        DoubleClick += OnDoubleClick;
    }

    static GameCell()
    {
        CheckForIllegalCrossThreadCalls = false;
    }
    #endregion

    #region Methods
    private void UpdateImage()
    {
        Image = HeldPiece?.Color switch
                {
                    PieceColor.White => HeldPiece.ImageWhite,
                    PieceColor.Black => HeldPiece.ImageBlack,

                    _ => null,
                };
    }

    private void OnClick(object? sender, EventArgs args)
    {
        if (MainForm.Board.SelectedCell is null)
            return;

        GameCell currCell = MainForm.Board.SelectedCell;
        switch (currCell.HeldPiece?.CanMoveTo(Position.x, Position.y))
        {
            case MoveClass.Move or MoveClass.Capture:
                MovePiece(currCell);

                break;

            case MoveClass.Castle:
                break;
        }

        ResetCellColors();
    }

    private void MovePiece(GameCell currCell)
    {
        // If it is a capture, remove the capture piece from the list
        if (HeldPiece != null)
            MainForm.Board.Pieces.Remove(HeldPiece);

        // Change positions
        HeldPiece          = currCell.HeldPiece!;
        currCell.HeldPiece = null;
        HeldPiece.Position = Position;

    #if !DEBUG_2
        // Change turn
        MainForm.Board.Turn = MainForm.Board.Turn is PieceColor.White
                                  ? PieceColor.Black
                                  : PieceColor.White;
    #endif

        // Increment TurnSinceMoved
        foreach (GamePiece piece in MainForm.Board.Pieces)
            if (piece.TurnSinceMoved > 0)
                piece.TurnSinceMoved++;

        if (HeldPiece.TurnSinceMoved is 0)
            HeldPiece.TurnSinceMoved = 1;

        if (HeldPiece is PiecePawn { Color: PieceColor.White, Position.y: 7 }
                      or PiecePawn { Color: PieceColor.Black, Position.y: 0 })
        {
            MainForm.Board.Pieces.Remove(HeldPiece);
            _ = new PieceQueen(Position, HeldPiece.Color);
        }

        // Clear the now out-of-date cache
        GamePiece.ResetCache();

        UpdateImage();
        currCell.UpdateImage();

    #if !DEBUG_2
        // Reverse the board for the opposite player
        FlipBoard();
    #endif
    }

    private void OnDoubleClick(object? sender, EventArgs args)
    {
        if (MainForm.Board.Turn != HeldPiece?.Color)
            return;

        for (var x = 0; x < NUM_COLUMNS; x++)
        for (var y = 0; y < NUM_ROWS; y++)
        {
            GameCell cell = MainForm.Board[x, y];

            MoveClass moveClass = HeldPiece.CanMoveTo(x, y);
            CellColor cellColor = cell.Color;
            cell.BackColor = GetCellColor(moveClass, cellColor);
        }

        BackColor = Color is CellColor.Light
                        ? Colors.SelectLight
                        : Colors.SelectDark;

        MainForm.Board.SelectedCell = this;
    }

    /// <summary>
    /// Flips the board's cells
    /// </summary>
    private static void FlipBoard()
    {
        var threads = new Thread[NUM_COLUMNS * NUM_ROWS];

        for (var x = 0; x < NUM_COLUMNS; x++)
        for (var y = 0; y < NUM_ROWS; y++)
        {
            GameCell cell = MainForm.Board[x, y];
            threads[x + NUM_COLUMNS * y] = new Thread(() => cell.Top = CELL_SIZE * NUM_ROWS - cell.Top);
        }

        for (var i = 0; i < threads.Length; i++)
            threads[i].Start();

        for (var i = 0; i < threads.Length; i++)
            threads[i].Join();
    }

    /// <summary>
    /// Recolors each cell back to its original color
    /// </summary>
    private static void ResetCellColors()
    {
        for (var x = 0; x < NUM_COLUMNS; x++)
        for (var y = 0; y < NUM_ROWS; y++)
        {
            GameCell cell = MainForm.Board[x, y];
            cell.BackColor = GetCellColor(MoveClass.None, cell.Color);
        }
    }

    /// <summary>
    /// Determine the color to apply on a cell
    /// </summary>
    /// <param name="moveClass">The type of movement possible toward this cell</param>
    /// <param name="cellColor">The color of the original cell</param>
    /// <returns>The new color to apply</returns>
    /// <exception cref="ArgumentOutOfRangeException">If moveClass is set to an impossible value</exception>
    private static Color GetCellColor(MoveClass moveClass, CellColor cellColor)
    {
        switch (moveClass)
        {
            case MoveClass.None:
                return cellColor == CellColor.Light
                           ? Colors.NeutralLight
                           : Colors.NeutralDark;

            case MoveClass.Move:
                return cellColor == CellColor.Light
                           ? Colors.MoveLight
                           : Colors.MoveDark;

            case MoveClass.Capture:
            case MoveClass.EnPassant:
                return cellColor == CellColor.Light
                           ? Colors.CaptureLight
                           : Colors.CaptureDark;

            case MoveClass.Castle:
                return cellColor == CellColor.Light
                           ? Colors.CastleLight
                           : Colors.CastleDark;

            default:
                throw new ArgumentOutOfRangeException(nameof(moveClass));
        }
    }
    #endregion
}