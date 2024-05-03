namespace SpreadsheetEngine.ExpressionTreeApplication.OperatorNodes
{
    internal class MultiplyNode : OperatorNode
    {
        // ReSharper disable once ConvertToPrimaryConstructor
        public MultiplyNode() : base('*')
        {
        }
        
        public static readonly string operatorSymbol = "*";

        // Value of importance when comparing with other operators
        public override ushort Precedence { get; } = 1;
        public override double Evaluate(double leftNode, double rightNode)
        {
            return rightNode * leftNode;
        }
    }
}

