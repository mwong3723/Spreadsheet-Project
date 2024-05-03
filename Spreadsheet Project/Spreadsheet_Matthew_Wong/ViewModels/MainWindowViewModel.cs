using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ReactiveUI;
using SpreadsheetEngine;
namespace HW4_Spreadsheet_Application.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
                   
    //Fields
    private Spreadsheet _spreadsheet;
    public List<RowViewModel> _SpreadsheetData { get; set; }
    
    private readonly List<CellViewModel> _selectionCells = new();

    private string undoMenuItem;

    private string redoMenuItem;

    private bool undoAvaliable;

    private bool redoAvaliable;
    
    //Properties
    public string UndoMenuItem
    {
        get => this.undoMenuItem;
        set => this.RaiseAndSetIfChanged(ref this.undoMenuItem, value);
    }

    public string RedoMenuItem
    {
        get => this.redoMenuItem;
        set => this.RaiseAndSetIfChanged(ref this.redoMenuItem, value);
    }
    
    public bool UndoAvaliable
    {
        get => this.undoAvaliable;
        set => this.RaiseAndSetIfChanged(ref this.undoAvaliable, value);
    }

    public bool RedoAvaliable
    {
        get => this.redoAvaliable;
        set => this.RaiseAndSetIfChanged(ref this.redoAvaliable, value);
    }

    public Interaction<Unit, IStorageFile> LoadFile { get; }
    
    public Interaction<Unit, IStorageFile> SaveFile { get; }

    //Constructor
    public MainWindowViewModel()
    {
        this.undoMenuItem = "Undo";
        this.redoMenuItem = "Redo";
        this.undoAvaliable = false;
        this.redoAvaliable = false;

        this.LoadFile = new Interaction<Unit, IStorageFile>();
        this.SaveFile = new Interaction<Unit, IStorageFile>();
        InitializeSpreadsheet();
    }
    
    /// <summary>
    /// Not used in future homeworks but sets random cells to contain Hello World
    /// </summary>
    public void ExecuteDemo()
    {
        Random random = new Random();
        foreach (var rowIndex in Enumerable.Range(0, 50))
        {
            foreach (var columnIndex in Enumerable.Range(0, 'Z' - 'A' + 1))
            {
                var cell = this._spreadsheet?.GetCell(rowIndex, columnIndex);
                if (cell != null)
                {
                    // Set random cells to "Hello World!"
                    if (random.Next(10) < 5)
                    {
                        cell.Text = "Hello World!";
                    }
                }
            }
        }
        
        // This loop is after because when we enter first loop it will place some Hello Worlds
        // In the B Column so we can overwrite this after we run the first loop
        // Set column B cells to "This is cell B#"
        foreach (var rowIndex in Enumerable.Range(0, 50))
        {
            var cellB = this._spreadsheet?.GetCell(rowIndex, 1); // Since we want column B we set our column value to 1
            if (cellB != null)
            {
                cellB.Text = $"This is cell B{rowIndex + 1}";
            }
        }

        // Set column A cells to "=B#"
        foreach (var rowIndex in Enumerable.Range(0, 50))
        {
            var cellA = this._spreadsheet?.GetCell(rowIndex, 0); // Since we want column A we set our column value to 0
            if (cellA != null)
            {
                cellA.Text = $"=B{rowIndex + 1}";
            }
        }
    }
    
    private void InitializeSpreadsheet()
    {
        const int rowCount = 50;
        const int columnCount = 'Z' - 'A' + 1;
        
        this._spreadsheet = new Spreadsheet(rowCount, columnCount);
        this._SpreadsheetData = new List<RowViewModel>(rowCount);

        foreach (var rowIndex in Enumerable.Range(0, rowCount))
        {
            var columns = new List<CellViewModel>(columnCount);
            foreach (var columnIndex in Enumerable.Range(0, columnCount))
            {
                columns.Add(new CellViewModel(this._spreadsheet.GetCell(rowIndex,columnIndex)) ??
                            throw new InvalidOperationException());
            }

            this._SpreadsheetData.Add(new RowViewModel(columns));
        }
    }
    
    /// <summary>
    /// Handles when a cell is selected by user
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="columnIndex"></param>
    public void SelectCell(int rowIndex, int columnIndex)
    {
        var clickedCell = this.GetCell(rowIndex, columnIndex);
        var shouldEditCell = clickedCell.IsSelected;

        ResetSelection();
        
        _selectionCells.Add(clickedCell);
        clickedCell.IsSelected = true;
        if (shouldEditCell)
        {
            clickedCell.CanEdit = true;
        }
    }

    /// <summary>
    /// Adds or removes the cell selected from our list of selected cells
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="columnIndex"></param>
    public void ToggleCellSelection(int rowIndex, int columnIndex)
    {
        var clickedCell = this.GetCell(rowIndex, columnIndex);
        if (false == clickedCell.IsSelected)
        {
            _selectionCells.Add(clickedCell);
            clickedCell.IsSelected = true;
        }
        else
        {
            _selectionCells.Remove(clickedCell);
            clickedCell.IsSelected = false;
        }
    }

    /// <summary>
    /// removes and clears all the cells in our selected cell list
    /// </summary>
    public void ResetSelection()
    {
        foreach (var cell in _selectionCells)
        {
            cell.IsSelected = false;
            cell.CanEdit = false;
        }
        _selectionCells.Clear();
    }
    
    /// <summary>
    /// Gets a certain cell based off targeted indices
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="columnIndex"></param>
    /// <returns></returns>
    public CellViewModel GetCell(int rowIndex, int columnIndex)
    {
        if (this._SpreadsheetData is null) throw new NullReferenceException("Spreadsheet is null");
        if (rowIndex < 0 || rowIndex >= this._SpreadsheetData.Count
                         || columnIndex < 0 || columnIndex >= this._SpreadsheetData[0].Cells.Count)
        {
            throw new IndexOutOfRangeException("Tried to get out of bounds cell");
        }

        return this._SpreadsheetData[rowIndex].Cells[columnIndex];
    }

    /// <summary>
    /// calls the Edit cell text function from Spreadsheet class
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="columnIndex"></param>
    /// <param name="textInput"></param>
    public void EditCellText(int rowIndex, int columnIndex, string textInput)
    {
        this._spreadsheet.EditCellText(rowIndex, columnIndex, textInput);
        this.UpdateUndoAndRedoButtons();
    }

    /// <summary>
    /// Changes the cells color by calling ChangeCellColor from Spreadsheet class
    /// </summary>
    /// <param name="newColor"></param>
    public void ChangeSelectedCellColor(uint newColor)
    {
        List<Cell> cellList = this._selectionCells.Select(cellViewModel => cellViewModel.Cell).ToList();
        
        this._spreadsheet.ChangeCellColor(cellList, newColor);
        this.UpdateUndoAndRedoButtons();
    }

    /// <summary>
    /// Calls undo commando from Spreadsheet class
    /// </summary>
    public void UndoCommand()
    {
        this._spreadsheet.Undo();
        this.UpdateUndoAndRedoButtons();
    }

    /// <summary>
    /// Calls redo commando from Spreadsheet class
    /// </summary>
    public void RedoCommand()
    {
        this._spreadsheet.Redo();
        this.UpdateUndoAndRedoButtons();
    }

    public bool SelectedCellsDifferentColors(uint color)
    {
        foreach (var cell in this._selectionCells)
        {
            if (cell.BackgroundColor != color) return true;
        }

        return false;
    }
    
    private void UpdateUndoAndRedoButtons()
    {
        if (this._spreadsheet == null) throw new InvalidOperationException();               

        var undoCommand = this._spreadsheet.TryGetUndoCommand();
        var redoCommand = this._spreadsheet.TryGetRedoCommand();

        if (undoCommand == null)
        {
            this.UndoMenuItem = "Undo";
            this.UndoAvaliable = false;
        }
        else
        {
            this.UndoMenuItem = "Undo " + undoCommand.GetCommandTitle();
            this.UndoAvaliable = true;
        }
        if (redoCommand == null)
        {
            this.RedoMenuItem = "Redo";
            this.RedoAvaliable = false;
        }
        else
        {
            this.RedoMenuItem = "Redo " + redoCommand.GetCommandTitle();
            this.RedoAvaliable = true;
        }
    }

    public async void LoadFromFile()
    {
        var filePath = await this.LoadFile.Handle(default);
        if(filePath is null) return;

        await using var stream = await filePath.OpenReadAsync();
        this._spreadsheet.LoadStream(stream);
        this.UpdateUndoAndRedoButtons();
    }

    public async void SavetoFile()
    {
        var file = await this.SaveFile.Handle(default);
        if(file is null) return;

        await using var stream = await file.OpenWriteAsync();
        this._spreadsheet.SaveStream(stream);
    }
}