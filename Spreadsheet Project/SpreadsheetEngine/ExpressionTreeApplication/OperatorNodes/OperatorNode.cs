#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace SpreadsheetEngine.ExpressionTreeApplication.OperatorNodes
{
    public abstract class OperatorNode : Node
    {
        /// <summary>
        /// Operator property to get and set new operators like +,-,/,*
        /// </summary>
        public char Operator { get; set; }
        
        /// <summary>
        /// Left pointer to next node
        /// </summary>
        public Node Left { get; set; }
        /// <summary>
        /// Right pointer to next node
        /// </summary>
        public Node Right { get; set; }

        /// <summary>
        /// Value to determine the precedence/importance of a operator
        /// </summary>
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public abstract ushort Precedence { get; }
        
        

        /// <summary>
        /// Constructor for OperatorNode class
        /// </summary>
        /// <param name="operatorCharacter"></param>
        // ReSharper disable once PublicConstructorInAbstractClass
        public OperatorNode(char operatorCharacter)
        {
            this.Operator = operatorCharacter;
            this.Left = this.Right = null;
        }

        /// <summary>
        ///  Abstract Evaluate Class for other sub classes to override and implement their own
        /// </summary>
        public abstract double Evaluate(double leftNode, double rightNode);
    }
}