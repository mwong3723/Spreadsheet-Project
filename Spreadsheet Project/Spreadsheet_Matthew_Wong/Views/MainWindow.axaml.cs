using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Avalonia.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using HW4_Spreadsheet_Application.ViewModels;
using SpreadsheetEngine;
using ReactiveUI;
#pragma warning disable CS8602 // Dereference of a possibly null reference.


namespace HW4_Spreadsheet_Application.Views;


public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        
        DataContextChanged += (sender, args) =>
        {
            if (DataContext is MainWindowViewModel viewModel)
                InitializeDataGrid(this.SpreadsheetDataGrid);
        };

        this.WhenActivated(d =>
        {
            var mainWindowViewModel = this.ViewModel;
            if (mainWindowViewModel is null) return;

            d(mainWindowViewModel.LoadFile.RegisterHandler(this.DoOpenFile));
            d(mainWindowViewModel.SaveFile.RegisterHandler(this.DoSaveFile));
        });
    }
    
    public void InitializeDataGrid(DataGrid dataGrid)
    {
        // initialize A to Z columns headers since these are indexed this is not a behavior supported by default
        var columnCount = 'Z' - 'A' + 1;
        foreach (var columnIndex in Enumerable.Range(0, columnCount))
        {
            // for each column we will define the header text and
            // the binding to use
            var columnHeader = (char) ('A' + columnIndex);
            var columnTemplate = new DataGridTemplateColumn()
            {
                Header = columnHeader,
                CellStyleClasses = {"SpreadsheetCellClass"},CellTemplate = new FuncDataTemplate<RowViewModel>((_, _) =>
                    new TextBlock
                    {
                        [!TextBlock.TextProperty] = new Binding($"[{columnIndex}].Value"),
                        TextAlignment = TextAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = Thickness.Parse("5,0,5,0")
                    }),
                CellEditingTemplate = new FuncDataTemplate<RowViewModel>((_, _) => new TextBox()),
            };
            this.SpreadsheetDataGrid.Columns.Add(columnTemplate);
        }
        
        SpreadsheetDataGrid.PreparingCellForEdit += (_, args) =>
        {
            if (args.EditingElement is not TextBox textInput) return;
            var rowIndex = args.Row.GetIndex();
            var colIndex = args.Column.DisplayIndex;
            textInput.Text = (this.DataContext as MainWindowViewModel).GetCell(rowIndex, colIndex).Text;
        };

        SpreadsheetDataGrid.CellEditEnding += (_, args) =>
        {
            if (args.EditingElement is not TextBox textInput) return;

            var rowIndex = args.Row.GetIndex();
            var columnIndex = args.Column.DisplayIndex;

            if (this.DataContext is MainWindowViewModel model)
            {
                var newInputtedText = textInput.Text;

                if (newInputtedText == model.GetCell(rowIndex, columnIndex).Text) return;

                model.EditCellText(rowIndex, columnIndex, newInputtedText ?? throw new InvalidOperationException());
            }
        };

        
        SpreadsheetDataGrid.CellPointerPressed += (_, args) =>
        {
            var rowIndex = args.Row.GetIndex();
            var colIndex = args.Column.DisplayIndex;
            var multipleSelection = args.PointerPressedEventArgs.KeyModifiers != KeyModifiers.None;
            
            if (multipleSelection == false)
            {
                (this.DataContext as MainWindowViewModel).SelectCell(rowIndex,colIndex);
                var dataContext = this.DataContext;
                if (dataContext != null)
                {
                    this.SpreadsheetColorPicker.Color = Color.FromUInt32(((MainWindowViewModel)dataContext)
                        .GetCell(rowIndex, colIndex).BackgroundColor);
                }
            }
            else
            {
                (this.DataContext as MainWindowViewModel).ToggleCellSelection(rowIndex,colIndex);
            }
        };
        

        SpreadsheetDataGrid.BeginningEdit += (_, args) =>
        {
            var rowIndex = args.Row.GetIndex();
            var colIndex = args.Column.DisplayIndex;
            var cell = (this.DataContext as MainWindowViewModel).GetCell(rowIndex, colIndex);
            if (false == cell.CanEdit)
            {
                args.Cancel = true;
            }
            else
            {
                (this.DataContext as MainWindowViewModel).ResetSelection();
            }
        };
        
    }

    private void LoadingRow(object sender, DataGridRowEventArgs dataGridRowEventArgs)
    {
        dataGridRowEventArgs.Row.Header = dataGridRowEventArgs.Row.GetIndex() + 1;
    }

    private void OnColorChanged(object sender, ColorChangedEventArgs colorChangedEventArgs)
    {
        if (this.DataContext is not MainWindowViewModel mainWindowViewModel) return;
        if (sender is ColorPicker colorPicker &&
            mainWindowViewModel.SelectedCellsDifferentColors(colorPicker.Color.ToUInt32()))
        {
            mainWindowViewModel.ChangeSelectedCellColor(colorPicker.Color.ToUInt32());
        }
    }

    private static FilePickerFileType Xml { get; } = new("Xml")
    {
        Patterns = new[] { "*.xml" },
        AppleUniformTypeIdentifiers = new[] { "public.xml" },
        MimeTypes = new[] { "xml/*" },
    };
    
    private async Task DoOpenFile(InteractionContext<Unit, IStorageFile> interactionContext)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        Console.Write(topLevel);
        
        var fileTypes = new List<FilePickerFileType> { Xml };

        if (topLevel != null)
        {
            var file = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Text File",
                AllowMultiple = false,
                FileTypeFilter = fileTypes,
            });

            interactionContext.SetOutput(file.Count == 1 ? file[0] : null);
        }
    }

    private async Task DoSaveFile(InteractionContext<Unit, IStorageFile> interactionContext)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var fileTypes = new List<FilePickerFileType> { Xml };
        if (topLevel is not null)
        {
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Text File",
                FileTypeChoices = fileTypes,
            });
            interactionContext.SetOutput(file);
        }
    }
    
}