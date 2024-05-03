
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace SpreadsheetEngine.ExpressionTreeApplication
{
    public class VariableNode : Node
    {
        /// <summary>
        /// Name Property for our node so we can get and set a name when we add a new
        /// Variable
        /// </summary>
        public string Name { get; set; }
    }
}

