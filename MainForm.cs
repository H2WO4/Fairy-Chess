using System.Collections;

using FairyChess.Enums;
using FairyChess.Models;
using FairyChess.Models.GamePieces;


namespace FairyChess;

public partial class MainForm : Form
{
    #region Variables
    /// <summary>
    /// Stores all cells of the grid
    /// </summary>
    private readonly GameCell[] _grid = new GameCell[GameCell.NUM_COLUMNS * GameCell.NUM_ROWS];

    public readonly List<GamePiece> Pieces = new();
    #endregion

    #region Properties
    public PieceColor Turn { get; set; }

    public GameCell? SelectedCell { get; set; }

    public static MainForm Board { get; }
    #endregion

    #region Constructors
    private MainForm()
    {
        InitializeComponent();

        Turn = PieceColor.White;
    }

    static MainForm()
    {
        Board = new MainForm();

        InitClassicBoard();
    }
    #endregion

    #region Methods
    private static void InitClassicBoard()
    {
        /*for (var x = 0; x < GameCell.NUM_COLUMNS; x++)
        {
            if (x is 4 or 5)
                continue;

            _ = new PiecePawn((x, 1), PieceColor.White);
            _ = new PiecePawn((x, 6), PieceColor.Black);
        }

        _ = new PieceGuard((4, 1), PieceColor.White);
        _ = new PieceGuard((4, 6), PieceColor.Black);
        _ = new PieceGuard((5, 1), PieceColor.White);
        _ = new PieceGuard((5, 6), PieceColor.Black);*/

        /*_ = new PieceRook((0, 0), PieceColor.White);
        _ = new PieceRook((0, 7), PieceColor.Black);
        _ = new PieceRook((GameCell.NUM_COLUMNS - 1, 0), PieceColor.White);
        _ = new PieceRook((GameCell.NUM_COLUMNS - 1, 7), PieceColor.Black);

        _ = new PieceKnight((1, 0), PieceColor.White);
        _ = new PieceKnight((1, 7), PieceColor.Black);
        _ = new PieceKnight((GameCell.NUM_COLUMNS - 2, 0), PieceColor.White);
        _ = new PieceKnight((GameCell.NUM_COLUMNS - 2, 7), PieceColor.Black);

        _ = new PieceBishop((2, 0), PieceColor.White);
        _ = new PieceBishop((2, 7), PieceColor.Black);
        _ = new PieceBishop((GameCell.NUM_COLUMNS - 3, 0), PieceColor.White);
        _ = new PieceBishop((GameCell.NUM_COLUMNS - 3, 7), PieceColor.Black);

        _ = new PieceChancellor((3, 0), PieceColor.White);
        _ = new PieceChancellor((3, 7), PieceColor.Black);
        _ = new PieceCardinal((6, 0), PieceColor.White);
        _ = new PieceCardinal((6, 7), PieceColor.Black);

        _ = new PieceQueen((4, 0), PieceColor.White);
        _ = new PieceQueen((4, 7), PieceColor.Black);
        _ = new PieceKing((5, 0), PieceColor.White);
        _ = new PieceKing((5, 7), PieceColor.Black);*/

        _ = new PieceDragon((0, 0), PieceColor.White);
        _ = new PieceUnicorn((GameCell.NUM_COLUMNS - 1, 0), PieceColor.White);
    }

    public GameCell this[int x, int y]
        => _grid[x + GameCell.NUM_COLUMNS * y];
    #endregion
}