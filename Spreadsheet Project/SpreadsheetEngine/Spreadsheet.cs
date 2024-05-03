using System.ComponentModel;
using System.Globalization;
using System.Net.Mime;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Xml;
#pragma warning disable CS8602 // Dereference of a possibly null reference.


namespace SpreadsheetEngine
{
    public class Spreadsheet
    {
        //Events
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Cell?[,] _sheet;

        //Properties
        private int RowIndex { get; }
        private int ColumnIndex { get; }

        private readonly CommnadInvoker commandInvoker;

        //Fields
        private List<char> operatorList = new List<char>() {'+','-','*','/','(',')'};


    /// <summary>
        /// Constructor for our Spreadsheet class that sets default values to whatever is given
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public Spreadsheet(int row, int column)
        {
            this.RowIndex = row;
            this.ColumnIndex = column;
            this._sheet = new Cell[row, column];
            this.commandInvoker = new CommnadInvoker();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    this._sheet[i, j] = new InitalizeCell(i, j);
                    this._sheet[i, j].PropertyChanged += this.OnCellPropertyChanged;
                }
            }
        }


        // Methods
        
        /// <summary>
        /// Returns a certain cell when given valid row and column indexes
        /// </summary>
        /// <param name="row">inputted row</param>
        /// <param name="column">inputted column</param>
        /// <returns>Targeted Cell</returns>
        public Cell? GetCell(int row, int column)
        {
            if (row <= this.RowIndex && column <= this.ColumnIndex)
            {
                return this._sheet[row, column];
            }
            return null;
        }

        /// <summary>
        /// Returns a certain cell when given valid row and column indexes
        /// </summary>
        /// <param name="cell">Cell that was inputeed</param>
        /// <returns>TargetedCell</returns>
        /// <exception cref="ArgumentException"></exception>
        private Cell GetCell(string cell)
        {
            var column = cell[0] - 'A';
            var row = int.Parse(cell.Substring(1, cell.Length - 1)) - 1;
            if (row < 0 || row >= this.RowIndex || column < 0 || column >= this.ColumnIndex)
            {
                throw new ArgumentException("Cell is out of range");
            }

#pragma warning disable CS8603 // Possible null reference return.
            return this.GetCell(row, column);
#pragma warning restore CS8603 // Possible null reference return.
        }
        
       
        
        /// <summary>
        /// Whenever we change our cell we need to properly update our UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCellPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            var cell = (Cell)sender;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (cell != null && e.PropertyName == nameof(Cell.Text))
            {
                // for checking if cell starts with "=" we use function .StartsWith() over indexing because
                // it will not through an error when deleting
                if (cell.Text.StartsWith("=") && cell.Text.Length > 1)
                {
                    cell.Value = this.EvaluateTree(cell.Text.Substring(1,cell.Text.Length - 1), cell);
                }
                else
                {
                    cell.Value = cell.Text;
                }
            } 
        }


        /// <summary>
        /// Builds an expression tree based on the expression inputted in the cell
        /// </summary>
        /// <param name="expression">String expression inputted Ex. "=A1+A2"</param>
        /// <param name="sender">The Cell that was changed Ex."=A1"</param>
        /// <returns>The evaluated value of the tree in string format</returns>
        /// <exception cref="Exception"></exception>
        
        private string EvaluateTree(string expression, Cell sender)
        {
            // We unsubscribe from all of the cells referenced by the sender cell 
            foreach (var referencedCell in sender.ReferencedCellList)
            {
                this.GetCell(referencedCell).PropertyChanged -= sender.OnReferencedCellChanged;
                sender.ReferencedCellList.Remove(referencedCell);
            }

            // Use a try and catch method instead if and else so that we can give our own exception if we cannot build our tree 
            // and prevent our spreadsheet from showing a error to user
            try
            {
                // new expresssion tree based on current formula in the cell
                var tree = new ExpressionTree.ExpressionTree(expression);
                // A list of Cells that is referenced in a cell
                // Ex. "=A2-B1" in our list is A2 and B1
                var referencedCellList = tree.GetVariableNames();
                var exceptionString = CheckReference(referencedCellList, sender);
                //  we found a problematic reference
                if (exceptionString != "")
                {
                    throw new Exception(exceptionString);
                }
                //Check if our formula is just a single cell Ex. "=A1"
                if (referencedCellList.Count > 0 && expression == referencedCellList.First())
                {
                    // First subscribe to the referenced cell
                    this.SubscribeToReferencedCells(sender, referencedCellList.First());
                    // Get the first variable/cell in our cell Ex. "A1"
                    var referencedValue = this.GetCell(referencedCellList.First()).Value;
                    // If our string is not empty we return our string else return 0
                    if (!string.IsNullOrEmpty(referencedValue))
                    {
                        return referencedValue;
                    }
                    else
                    {
                        return "0";
                    }
                }

                // Loop through our cell list that was in our formula Ex. "=A1-A2" List:A1,A2
                foreach (var referencedCell in referencedCellList)
                {
                    // First subscribe to the referenced cell
                    this.SubscribeToReferencedCells(sender, referencedCell);
                    // Get the targeted cell's value and store in a string
                    string cellStringValue = this.GetCell(referencedCell).Value;
                    
                    // We first try to convert our targeted cell's value to a double and then
                    // set our value in our tree's dictionary to store both the cell and value
                    if (double.TryParse(cellStringValue, out var cellDoubleValue))
                    {
                        tree.SetVariables(referencedCell, cellDoubleValue);
                    }
                    // if our string is empty no value was set so we set it to 0
                    else if (string.IsNullOrEmpty(cellStringValue))
                    {
                        tree.SetVariables(referencedCell, 0);
                    }
                    // throw exception if our expression is invalid like an incomplete or invalid expression
                    // Ex. "=10-", "=A1+B", and more
                    else
                    {
                        throw new Exception("Invalid Expression");
                    }
                }
                // Dont know why I have to do this rider suggested it
                return tree.Evaluate().ToString(CultureInfo.InvariantCulture);
            }
            catch(Exception e)
            {
                sender.Value = e.Message;
                return e.Message;
            }
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Cell that is being changed</param>
        /// <param name="reference">The Value of a variable</param>
        private void SubscribeToReferencedCells(Cell sender, string reference)
        {
            if (sender.ReferencedCellList.Contains(reference))
            {
                return;
            }

            this.GetCell(reference).PropertyChanged += sender.OnReferencedCellChanged;
            sender.ReferencedCellList.Add(reference);
        }


        public void EditCellText(int rowIndex, int columnIndex, string textInput)
        {
            var cell = this.GetCell(rowIndex, columnIndex);
            this.commandInvoker.EditCellText(cell,textInput);
        }

        public void ChangeCellColor(List<Cell> selectedCell, uint newColor)
        {
            this.commandInvoker.ChangeCellColor(selectedCell, newColor);
        }

        public ICommand TryGetUndoCommand()
        {
            return this.commandInvoker.TryGetUndoCommand();
        }

        public ICommand TryGetRedoCommand()
        {
            return this.commandInvoker.TryGetRedoCommand();
        }

        public void Undo()
        {
            this.commandInvoker.Undo();
        }

        public void Redo()
        {
            this.commandInvoker.Redo();
        }
        
        public void SaveStream(Stream stream)
        {
            using var newXmlWriter = XmlWriter.Create(stream);
            newXmlWriter.WriteStartElement("spreadsheet");
            for (var i = 0; i < this.RowIndex; i++)
            {
                for (var j = 0; j < this.ColumnIndex; j++)
                {
                    var currentCell = this._sheet[i, j];
                    if (currentCell is not { cellEdited: true })
                    {
                        continue;
                    }
                    
                    newXmlWriter.WriteStartElement("cell");
                    newXmlWriter.WriteAttributeString("name", currentCell.GetName());
                    
                    newXmlWriter.WriteStartElement("bgcolor");
                    newXmlWriter.WriteString(currentCell.BackgroundColor.ToString("X"));
                    newXmlWriter.WriteEndElement();
                    
                    newXmlWriter.WriteStartElement("text");
                    newXmlWriter.WriteString(currentCell.Text);
                    newXmlWriter.WriteEndElement();
                    
                    newXmlWriter.WriteEndElement();
                }
            }
        }

        
        public void LoadStream(Stream stream)
        {
            this.ResetCells();

            using var newXmlReader = XmlReader.Create(stream);
            while (newXmlReader.Read())
            {
                if (newXmlReader.NodeType != XmlNodeType.Element || newXmlReader.Name != "cell")
                {
                    continue;
                }

                var cellName = newXmlReader.GetAttribute("name");
                string cellBGColor = null;
                string cellText = null;
                
                newXmlReader.ReadStartElement();

                while (newXmlReader.NodeType != XmlNodeType.EndElement)
                {
                    if (newXmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (newXmlReader.Name == "bgcolor")
                        {
                            cellBGColor = newXmlReader.ReadElementContentAsString();
                        }
                        if (newXmlReader.Name == "text")
                        {
                            cellText = newXmlReader.ReadElementContentAsString();
                        }
                        else
                        {
                            newXmlReader.Skip();
                        }
                    }
                    else
                    {
                        newXmlReader.Read();
                    }
                }

                if (cellName is null)
                {
                    continue;
                }

                var currentCell = this.GetCell(cellName);
                if (cellBGColor is not null)
                {
                    var backgroundColor = uint.Parse(cellBGColor, NumberStyles.HexNumber);
                    currentCell.BackgroundColor = backgroundColor;
                }

                if (cellText is not null)
                {
                    currentCell.Text = cellText;
                }
            }

            this.commandInvoker.ClearStacks();
        }
    
        public void ResetCells()
        {
            if (this._sheet is null) return;
            foreach (var currentCell in this._sheet)
            {
                if(currentCell is null) continue;
                currentCell.Text = string.Empty;
                currentCell.cellEdited = false;
                currentCell.BackgroundColor = 0xFFFFFFFF;
            }
        }

        /// <summary>
        /// Checks the current cell's text to make sure there is no bad reference if so handles accordingly 
        /// </summary>
        /// <param name="cellText"></param>
        /// <param name="currentCell"></param>
        public string CheckReference(List<string> variableList, Cell currentCell)
        {
            foreach (var variable in variableList)
            {
                if (variable == currentCell.GetName())
                {
                    return "!(self reference)";
                }
                else if (!OutOfBoundsReference(variable))
                {
                    return "!(out of bounds reference)";
                }
                else if (!BadReference(variable, variableList))
                {
                    return "!(bad reference)";
                }
                else if (CircularReference(variable, currentCell.GetName(), currentCell.ReferencedCellList))
                {
                    return "!(circular reference)";
                }
            }
            return "";
        }
        
        /// <summary>
        /// Checks if the variable passed was in bounds of our spreadsheet
        /// </summary>
        /// <param name="variableName">targeted cell passed</param>
        /// <returns>boolean if either true or false</returns>
        public bool OutOfBoundsReference(string variableName)
        {
            // The column character Ex. A,B,C
            char column = variableName[0];
            // The row character Ex.1-50
            int row, columnValue;
            int.TryParse(variableName.Substring(1), out row);

            columnValue = Convert.ToInt32(column) - 65;
            // If our targeted cell is out of bounds return false
            if (columnValue < 0 || columnValue > this.ColumnIndex || row < 0 || row > this.RowIndex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if all our variables in our formula is a valid variable
        /// </summary>
        /// <param name="variableName">Targeted cell passed</param>
        /// <param name="variableList">List of all our variables in our formula</param>
        /// <returns></returns>
        public bool BadReference(string variableName, List<string> variableList)
        {
            // The column character Ex. A,B,C
            char column;
            // The row character Ex.1-50
            int row;
            // loops through all variables in our list
            foreach (var variable in variableList)
            {
                column = variable[0];
                // Checks if the first character is valid i.e A-Z
                if (column > 'A' || column < 'Z')
                {
                    // Tries to convert the rest of our string into a value if it cannot it returns zero
                    int.TryParse(variableName.Substring(1), out row);
                    if (row == 0 || row > 50 || row < 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        /// <summary>
        /// Checks for a circular reference in our spreadsheet
        /// </summary>
        /// <param name="currentVariable"></param>
        /// <param name="currentCellName"></param>
        /// <param name="cellList"></param>
        /// <returns></returns>
        public bool CircularReference(string currentVariable, string currentCellName, HashSet<string> cellList)
        {
            // If the current variable is already visited then its a ciruclar reference
            if (cellList.Contains(currentVariable))
            {
                return true;
            }

            // Add our current variable to our hashset
            cellList.Add(currentVariable);
            //We get the cell that we referenced and check to see if not null
            var referencedCell = this.GetCell(currentVariable);
            if (referencedCell == null)
            {
                return false;
            }

            // Loops through the referncedCell's list of cells it is referencing
            foreach (var variable in referencedCell.ReferencedCellList)
            {
                if (variable == currentCellName || CircularReference(variable, currentCellName, cellList))
                {
                    return true;
                }
            }

            // Remove the vistied cell when we are backtracking
            cellList.Remove(currentVariable);
            return false;
        }
    }
}
