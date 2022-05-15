using FairyChess.Enums;
using FairyChess.Models;
using FairyChess.Models.GamePieces;


namespace FairyChess;

public partial class GameBoard : Form
{
    public const int CELL_SIZE = 96;
    public const int NUM_ROWS  = 8;
#if !DEBUG_2
    public const int NUM_COLUMNS = 10;
#endif
#if DEBUG_2
    public const int NUM_COLUMNS = 8;
#endif

    #region Variables
    /// <summary>
    /// Stores all cells of the grid
    /// </summary>
    private readonly GameCell[] _grid = new GameCell[NUM_COLUMNS * NUM_ROWS];

    /// <summary>
    /// Pieces currently in play
    /// </summary>
    public readonly List<GamePiece> Pieces = new();
    #endregion

    #region Properties
    /// <summary>
    /// Whose color is allowed to move
    /// </summary>
    public PieceColor Turn { get; set; }

    /// <summary>
    /// The currently selected cell, if there is one
    /// </summary>
    public GameCell? SelectedCell { get; set; }

    /// <summary>
    /// The currently used board Form
    /// </summary>
    public static GameBoard Main { get; }
    #endregion

    #region Constructors
    private GameBoard()
    {
        InitializeComponent();

        Turn = PieceColor.White;
    }

    static GameBoard()
    {
        Main = new GameBoard();

        InitClassicBoard();
    }
    #endregion

    #region Methods
    private static void InitClassicBoard()
    {
    #if !DEBUG_2
        for (var x = 0; x < NUM_COLUMNS; x++)
        {
            if (x is 4 or 5)
                continue;

            _ = new PiecePawn((x, 1), PieceColor.White);
            _ = new PiecePawn((x, NUM_ROWS - 2), PieceColor.Black);
        }

        _ = new PieceGuard((4, 1), PieceColor.White);
        _ = new PieceGuard((4, NUM_ROWS - 2), PieceColor.Black);
        _ = new PieceGuard((5, 1), PieceColor.White);
        _ = new PieceGuard((5, NUM_ROWS - 2), PieceColor.Black);

        _ = new PieceRook((0, 0), PieceColor.White);
        _ = new PieceRook((0, NUM_ROWS - 1), PieceColor.Black);
        _ = new PieceRook((NUM_COLUMNS - 1, 0), PieceColor.White);
        _ = new PieceRook((NUM_COLUMNS - 1, NUM_ROWS - 1), PieceColor.Black);

        _ = new PieceKnight((1, 0), PieceColor.White);
        _ = new PieceKnight((1, NUM_ROWS - 1), PieceColor.Black);
        _ = new PieceKnight((NUM_COLUMNS - 2, 0), PieceColor.White);
        _ = new PieceKnight((NUM_COLUMNS - 2, NUM_ROWS - 1), PieceColor.Black);

        _ = new PieceBishop((2, 0), PieceColor.White);
        _ = new PieceBishop((2, NUM_ROWS - 1), PieceColor.Black);
        _ = new PieceBishop((NUM_COLUMNS - 3, 0), PieceColor.White);
        _ = new PieceBishop((NUM_COLUMNS - 3, NUM_ROWS - 1), PieceColor.Black);

        _ = new PieceChancellor((3, 0), PieceColor.White);
        _ = new PieceChancellor((3, NUM_ROWS - 1), PieceColor.Black);
        _ = new PieceCardinal((6, 0), PieceColor.White);
        _ = new PieceCardinal((6, NUM_ROWS - 1), PieceColor.Black);

        _ = new PieceQueen((4, 0), PieceColor.White);
        _ = new PieceQueen((4, NUM_ROWS - 1), PieceColor.Black);
        _ = new PieceKing((5, 0), PieceColor.White);
        _ = new PieceKing((5, NUM_ROWS - 1), PieceColor.Black);
    #endif

    #if DEBUG_2
        _ = new PieceDragon((0, 0), PieceColor.White);
        _ = new PieceUnicorn((7, 7), PieceColor.White);
    #endif
    }

    /// <summary>
    /// Changes the turn to the opposite player
    /// </summary>
    public static void ChangeTurn()
    {
        Main.Turn = Main.Turn is PieceColor.White
                        ? PieceColor.Black
                        : PieceColor.White;
    }

    /// <summary>
    /// Flips the board's cells
    /// </summary>
    public static void FlipBoard()
    {
        var threads = new Thread[NUM_COLUMNS * NUM_ROWS];

        for (var x = 0; x < NUM_COLUMNS; x++)
        for (var y = 0; y < NUM_ROWS; y++)
        {
            GameCell cell = Main[x, y];
            threads[x + NUM_COLUMNS * y] = new Thread(() => cell.Top = CELL_SIZE * NUM_ROWS - cell.Top);
        }

        for (var i = 0; i < threads.Length; i++)
            threads[i].Start();

        for (var i = 0; i < threads.Length; i++)
            threads[i].Join();
    }

    public GameCell this[int x, int y]
        => _grid[x + NUM_COLUMNS * y];
    #endregion
}