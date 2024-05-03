using ReactiveUI;
using SpreadsheetEngine;

namespace HW4_Spreadsheet_Application.ViewModels
{
    public class CellViewModel : ViewModelBase
    {
        //Fields
        protected readonly Cell cell;

        private bool canEdit;

        private bool isSelected;

        //Properties
        public Cell Cell
        {
            get => cell;
        }

        public bool IsSelected
        {
            get => isSelected;
            set => this.RaiseAndSetIfChanged(ref isSelected, value);
        }

        public bool CanEdit
        {
            get => canEdit;
            set => this.RaiseAndSetIfChanged(ref canEdit, value);
        }

        public string? Text
        {
            get => cell.Text;
            set
            {
                if (value != null)
                {
                    this.Cell.Text = value;
                }
            }
        }

        public string? Value => cell.Value;

        public virtual uint BackgroundColor
        {
            get => cell.BackgroundColor;
            set => cell.BackgroundColor = value;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cell"></param>
        public CellViewModel(Cell cell)
        {
            this.cell = cell;
            isSelected = false;
            canEdit = false;

            this.cell.PropertyChanged += (_, args) =>
            {
                this.RaisePropertyChanged(args.PropertyName);
            };
        }
        
    }
}

