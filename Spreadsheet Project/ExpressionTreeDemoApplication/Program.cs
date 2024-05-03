using SpreadsheetEngine.ExpressionTree;

namespace ExpressionTreeDemoApplication
{
    public abstract class Program
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        static void Main()
        {
            var choice = 0;
            string? currentExpression = null;
            var tree = new ExpressionTree(string.Empty);
            
            while (choice != 4)
            {
                if (currentExpression == null)
                {
                    Console.WriteLine("Menu:");
                }
                else
                {
                    Console.WriteLine("Menu(current expression=\"{0}\"):", currentExpression);
                }
                Console.WriteLine("1 = Enter a new expression");
                Console.WriteLine("2 = Set a variable value");
                Console.WriteLine("3 = Evaluate Tree");
                Console.WriteLine("4 = Quit");
                choice = int.Parse(s: Console.ReadLine() ?? throw new InvalidOperationException());

                switch (choice)
                {
                    case 1: // Enter Expression
                        Console.WriteLine("Enter a new Expression:");
                        currentExpression = Console.ReadLine();
                        if (currentExpression != null) tree = new ExpressionTree(currentExpression);
                        
                        break;
                    case 2: // Set Variable
                        Console.WriteLine("Enter a variable name:");
                        string newVariableName = Console.ReadLine() ?? throw new InvalidOperationException();
                        Console.WriteLine("Enter a variable value:");
                        double newVariableValue = double.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                        tree.SetVariables(newVariableName,newVariableValue);
                        break;
                    case 3: // Evaluate
                        Console.WriteLine("{0} = {1}", currentExpression, tree.Evaluate());
                        //Console.WriteLine("Evaluate");
                        break;
                    case 4: // Quit
                        Console.WriteLine("Bye Bye");
                        break;
                    default:
                        Console.WriteLine("Wrong Input");
                        break;
                }
            }
        }
    }
}


