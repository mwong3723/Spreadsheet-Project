namespace SpreadsheetEngine.ExpressionTreeApplication.OperatorNodes
{
    internal class RightParenthesesNode : OperatorNode
    {
        // ReSharper disable once ConvertToPrimaryConstructor
        public RightParenthesesNode() : base(')')
        {
        }
        
        public static readonly string operatorSymbol = ")";
        
        // Value of importance when comparing with other operators
        public override ushort Precedence { get; } = 2;
        public override double Evaluate(double leftNode, double rightNode)
        {
            return leftNode;
        }
    }
}

