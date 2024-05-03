namespace SpreadsheetEngine.ExpressionTreeApplication.OperatorNodes
{
    internal class MinusNode : OperatorNode
    {
        // ReSharper disable once ConvertToPrimaryConstructor
        public MinusNode() : base('-')
        {
        }
        
        public static readonly string operatorSymbol = "-";
        
        // Value of importance when comparing with other operators
        public override ushort Precedence { get; } = 0;
        public override double Evaluate(double leftNode, double rightNode)
        {
            return rightNode - leftNode;
        }
    }
}

