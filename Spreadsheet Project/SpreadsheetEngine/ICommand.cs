namespace SpreadsheetEngine
{
    public interface ICommand
    {
        public string GetCommandTitle();

        public void Execute();

        public void Undo();
        
    }
}
