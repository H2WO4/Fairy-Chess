using FairyChess.Enums;


namespace FairyChess.Models;

/// <summary>
/// Represents a cell of the game grid
/// </summary>
public class GameCell : PictureBox
{
    #region Properties
    /// <summary>
    /// Determines if this is a light- or dark-colored cell
    /// </summary>
    public CellColor Color { get; }

    public (int x, int y) Position { get; }

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
    public void UpdateImage()
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
        GameCell? selectedCell = GameBoard.Main.SelectedCell;

        if (selectedCell is null)
            return;

        if (selectedCell.HeldPiece is null)
        {
            GameBoard.Main.SelectedCell = null;

            return;
        }

        switch (selectedCell.HeldPiece.CanMoveTo(Position.x, Position.y))
        {
            case MoveClass.Move:
            case MoveClass.Capture:
                selectedCell.HeldPiece.MoveTo(this);

                break;

            case MoveClass.Castle:
                break;
            
            case MoveClass.EnPassant:
                break;
        }

        ResetCellColors();
        GameBoard.Main.SelectedCell = null;
    }

    private void OnDoubleClick(object? sender, EventArgs args)
    {
        if (GameBoard.Main.Turn != HeldPiece?.Color)
        {
            GameBoard.Main.SelectedCell = null;

            return;
        }

        for (var x = 0; x < GameBoard.NUM_COLUMNS; x++)
        for (var y = 0; y < GameBoard.NUM_ROWS; y++)
        {
            GameCell cell = GameBoard.Main[x, y];

            MoveClass moveClass = HeldPiece.CanMoveTo(x, y);
            CellColor cellColor = cell.Color;
            cell.BackColor = GetCellColor(moveClass, cellColor);
        }

        BackColor = Color is CellColor.Light
                        ? Colors.SelectLight
                        : Colors.SelectDark;

        GameBoard.Main.SelectedCell = this;
    }

    /// <summary>
    /// Recolors each cell back to its original color
    /// </summary>
    private static void ResetCellColors()
    {
        for (var x = 0; x < GameBoard.NUM_COLUMNS; x++)
        for (var y = 0; y < GameBoard.NUM_ROWS; y++)
        {
            GameCell cell = GameBoard.Main[x, y];
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