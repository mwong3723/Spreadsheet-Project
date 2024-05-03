namespace SpreadsheetEngine.ExpressionTreeApplication
{
    public class ConstantNode : Node
    {
        /// <summary>
        /// Value Property of our constant node so we can set and get
        /// its value for example a value of 10
        /// </summary>
        public double Value { get; set; }
    }
}
