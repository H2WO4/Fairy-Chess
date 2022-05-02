using FairyChess.Enums;
using FairyChess.Models;

namespace FairyChess;

partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components      = new System.ComponentModel.Container();
        AutoScaleMode   = System.Windows.Forms.AutoScaleMode.Font;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MinimizeBox     = false;
        MaximizeBox     = false;
        ClientSize      = new System.Drawing.Size(GameCell.CELL_SIZE * (GameCell.NUM_COLUMNS + 1), 
                                                  GameCell.CELL_SIZE * (GameCell.NUM_ROWS + 1));
        BackColor = Color.FromArgb(22, 21, 18);
        Text      = "Fairy Chess";
        Icon      = new Icon($"{Program.ProjectPath}Images/knightW.ico");

        // Create the grid
        for (var x = 0; x < GameCell.NUM_COLUMNS; x++)
        for (var y = 0; y < GameCell.NUM_ROWS; y++)
        {
            var cell = new GameCell(x, y);
            cell.Parent    = this;
            cell.Size      = new Size(GameCell.CELL_SIZE, GameCell.CELL_SIZE);
            cell.Left      = x * GameCell.CELL_SIZE + GameCell.CELL_SIZE - GameCell.CELL_SIZE / 2;
            cell.Top       = (GameCell.NUM_ROWS - y - 1) * GameCell.CELL_SIZE + GameCell.CELL_SIZE - GameCell.CELL_SIZE / 2;
            cell.BackColor = cell.Color is CellColor.Light
                                 ? Colors.NeutralLight
                                 : Colors.NeutralDark;

            _grid[x + GameCell.NUM_COLUMNS * y] = cell;
        }
    }
}