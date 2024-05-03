using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpreadsheetEngine
{
    public abstract class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// Notifies when a property of this class changes
        /// </summary>
        //Events
        public event PropertyChangedEventHandler? PropertyChanged;

        // Fields

        protected string text;
        protected string value;
        protected uint backgroundColor;
        

        //Properties
        /// <summary>
        /// gets the row index of a cell
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// gets the column index of a cell
        /// </summary>
        public int ColumnIndex { get; }

        public uint BackgroundColor
        {
            get => this.backgroundColor;
            set
            {
                this.backgroundColor = value;
                this.OnPropertyChanged();
                this.cellEdited = true;
            }
        }

        public bool cellEdited { get; set; }


        /// <summary>
        /// HashSet of referenced Cells
        /// </summary>
        public HashSet<string> ReferencedCellList { get; }
        
        protected abstract void SetValue(string newValue);

        /// <summary>
        /// Constructor for our Cell class that sets default values when instantiated
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        protected Cell(int rowIndex, int columnIndex)
        {
            this.RowIndex = rowIndex;
            this.ColumnIndex = columnIndex;
            this.BackgroundColor = 0xFFFFFFFF;
            this.text = string.Empty;
            this.value = string.Empty;
            this.ReferencedCellList = new HashSet<string>();
            this.cellEdited = false;
        }

        /// <summary>
        /// Gets or sets the text of the cell.
        /// </summary>
        public string Text
        {
            get => this.text;
            set
            {
                this.SetCellValue(ref this.text, value);
                this.cellEdited = true;
            }
        }

        /// <summary>
        /// Gets or sets the value of the cell.
        /// </summary>
        public virtual string Value
        {
            get => this.value;
            set
            {
                this.SetValue(value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the string name of our targeted cell
        /// </summary>
        /// <returns>string name of our targeted cell</returns>
        public string GetName()
        {
            var column = (char)(this.ColumnIndex + 'A');
            var row = this.RowIndex + 1;
            return ("" + column + row);
        }
        /// <summary>
        /// When we change our referenced cells value we call this function to call out Property Change function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnReferencedCellChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(nameof(this.Text));
        }
        
        /// <summary>
        /// Calls Spreadsheet function that invokes the event handler when we change a cell
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            //Invokes our PropertyChangedEvent if PropertyChanged is not null
            // We can achieve the same implementation using an if statement where
            // if(PropertyChanged != null) PropertyChanged(this, ...)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
            
        /// <summary>
        /// Sets the value or our field and raises the property changed event
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private bool SetCellValue<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            return false;
        }
    
    }


    /// <summary>
    ///  Helper function for spreadsheet class to Initialize a cell later on in
    ///  Spreadsheet class
    /// </summary>
    public class InitalizeCell : Cell
    {
        public InitalizeCell(int rowCount, int columnCount) : base(rowCount,columnCount)
        {
        }
        
        protected override void SetValue(string newValue)
        {
            this.value = newValue;
        }
    }

    
}

