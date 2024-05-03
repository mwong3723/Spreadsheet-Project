namespace SpreadsheetEngine
{
    public class ChangeColorCommand : ICommand
    {
        private readonly List<Cell> cellList;

        private readonly uint color;

        private readonly Dictionary<Cell, uint> previousColors;

        public ChangeColorCommand(List<Cell> newCellList, uint newColor)
        {
            this.cellList = newCellList;
            this.color = newColor;

            this.previousColors = new Dictionary<Cell, uint>();
            foreach (var cell in newCellList)
            {
                this.previousColors.Add(cell, cell.BackgroundColor);
            }
        }

        public string GetCommandTitle()
        {
            return "background color change";
        }

        public void Execute()
        {
            foreach (var cell in this.cellList)
            {
                cell.BackgroundColor = this.color;
            }
        }

        public void Undo()
        {
            var cellColors = this.previousColors;
            if (cellColors is null) throw new InvalidOperationException();
            foreach (var cellsColor in cellColors)
            {
                cellsColor.Key.BackgroundColor = cellsColor.Value;
            }
        }
    }
}

