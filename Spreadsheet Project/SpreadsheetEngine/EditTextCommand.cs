namespace SpreadsheetEngine
{
    public class EditTextCommand : ICommand
    {
        private readonly Cell cell;

        private readonly string textInput;

        private readonly string previousText;

        public EditTextCommand(Cell newCell, string newTextInput)
        {
            this.cell = newCell;
            this.textInput = newTextInput;
            this.previousText = newCell.Text;
        }

        public string GetCommandTitle()
        {
            return "text change";
        }

        public void Execute()
        {
            this.cell.Text = this.textInput;
        }

        public void Undo()
        {
            this.cell.Text = this.previousText;
        }
    }
}

