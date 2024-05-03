using System.Windows.Input;

namespace SpreadsheetEngine
{
    public class CommnadInvoker
    {
        private readonly Stack<ICommand> undoStack;

        private readonly Stack<ICommand> redoStack;

        public CommnadInvoker()
        {
            this.undoStack = new Stack<ICommand>();
            this.redoStack = new Stack<ICommand>();
        }

        public ICommand TryGetUndoCommand()
        {
            try
            {
                return this.undoStack.Peek();
            }
            catch
            {
                return null;
            }
        }

        public ICommand TryGetRedoCommand()
        {
            try
            {
                return this.redoStack.Peek();
            }
            catch
            {
                return null;
            }
        }

        public void EditCellText(Cell cell, string textInput)
        {
            var command = new EditTextCommand(cell, textInput);
            command.Execute();
            
            this.undoStack.Push(command);
            this.redoStack.Clear();
        }

        public void ChangeCellColor(List<Cell> cellList, uint color)
        {
            var command = new ChangeColorCommand(cellList, color);
            command.Execute();
            
            this.undoStack.Push(command);
            this.redoStack.Clear();
        }

        public void Undo()
        {
            var command = this.undoStack.Pop();
            command.Undo();
            this.redoStack.Push(command);

        }

        public void Redo()
        {
            var command = this.redoStack.Pop();
            
            command.Execute();
            this.undoStack.Push(command);
        }

        /// <summary>
        /// Clears both the redo and undo stakcs
        /// </summary>
        public void ClearStacks()
        {
            this.redoStack.Clear();
            this.undoStack.Clear();
        }
    }
}

