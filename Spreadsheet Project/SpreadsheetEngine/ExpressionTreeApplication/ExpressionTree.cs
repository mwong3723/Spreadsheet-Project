using SpreadsheetEngine.ExpressionTreeApplication.OperatorNodes;
using SpreadsheetEngine.ExpressionTreeApplication;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.

// ReSharper disable once CheckNamespace
namespace SpreadsheetEngine.ExpressionTree
{
    public class ExpressionTree
    {
        //Fields
        /// <summary>
        /// Expression that was given by user like "1+2+3"
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once InconsistentNaming
        public string expression;
        
        /// <summary>
        /// root of out tree with abstract node for later use to create different types of nodes
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once InconsistentNaming
        public Node? root;
        
        /// <summary>
        /// A Dictionary to store pairs of data with our keys being new variables and the values being
        /// values of double type
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once InconsistentNaming
        public Dictionary<string, double> variables = new Dictionary<string, double>();

        /// <summary>
        /// Stack used to hold post fix expression
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public Stack<Node> postFixExpressionStack = new Stack<Node>();
        
        /// <summary>
        /// Stack to hold operators to later use for calculations 
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public Stack<Node> operatorStack = new Stack<Node>();


        private readonly OperatorNodeFactory operatorNodeFactory;

        //Constructor
        /// <summary>
        /// Constructor method that sets our expression to newExpression and root to null
        /// </summary>
        /// <param name="newExpression">The expression given when creating new object</param>
        public ExpressionTree(string newExpression)
        {
            this.variables.Clear();
            this.operatorNodeFactory = new OperatorNodeFactory();
            this.ConvertToPostfixExpression(newExpression);
            if (newExpression != "")
            {
                this.root = this.postFixExpressionStack.Pop();
            }
            this.root = this.CreateTree(this.root);
            this.expression = newExpression;
        }
    
        /// <summary>
        /// Adds a new variable to our dictionary with our key being the variableName and our value as variableValue
        /// </summary>
        /// <param name="variableName">given key for our dictionary</param>
        /// <param name="variableValue">given value pair to our key</param>
        public void SetVariables(string variableName, double variableValue)
        {
            this.variables[variableName] = variableValue;
        }

        public List<string> GetVariableNames()
        {
            return [..this.variables.Keys];
        }
        
        /// <summary>
        /// Here we have an overloaded Evaluate() Function that calls our private Evaluate() function
        /// </summary>
        /// <returns>The result of our expression</returns>
        public double Evaluate()
        {
            return Evaluate(this.root);
        }

        /// <summary>
        /// Overloaded private Evaluate() function that handles the logic of our evaluate function
        /// </summary>
        /// <param name="root">The root of our expression tree</param>
        /// <param name="currentNode">The current node we are when moving through the tree</param>
        /// <returns>The result of our expression</returns>
        private double Evaluate(Node currentNode)
        {
            // If our current node is an operator we evaluate left and right nodes
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (currentNode != null && currentNode is OperatorNode)
            {
                OperatorNode tempNode = (OperatorNode)currentNode;
                return tempNode.Evaluate(this.Evaluate(tempNode.Left), this.Evaluate(tempNode.Right));
            }
    
            // If our current node is a Variable node we return its Name
            if (currentNode != null && currentNode is VariableNode)
            {
                VariableNode tempNode = (VariableNode)currentNode;
                return this.variables[tempNode.Name];
            }
            
            // If our current node is a Constant Node we return its value
            if (currentNode != null && currentNode is ConstantNode)
            {
                ConstantNode tempNode = (ConstantNode)currentNode;
                return tempNode.Value;
            }

            return 0;
        }
    
        /// <summary>
        /// Call this method in our constructor that will take the new expression and create
        /// a new tree based on that expression
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private Node CreateTree(Node currentNode)
        {
            if (currentNode is OperatorNode)
            {
                OperatorNode tempNode = (OperatorNode)currentNode;
                tempNode.Left = this.CreateTree(this.postFixExpressionStack.Pop());
                tempNode.Right = this.CreateTree(this.postFixExpressionStack.Pop());
            }

            return currentNode;
        }

        /// <summary>
        /// When called in constructor we convert our given expression from user to post fix expression in our post
        /// fix expression stack
        /// </summary>
        /// <param name="newExpression">Incoming expression from  user</param>
        private void ConvertToPostfixExpression(string newExpression)
        {
            this.CompileExpression(newExpression);
            // After we compile our expression we then loop and add all out operators to our stack so they are near the
            // top and there should be no parentheses left
            // ReSharper disable once UseMethodAny.0
            while (this.operatorStack.Count() > 0)
            {
                this.postFixExpressionStack.Push(this.operatorStack.Pop());
            }
        }
        /// <summary>
        /// Compiles our new expression to post fix
        /// </summary>
        /// <param name="newExpression">Incoming expression from user</param>
        private void CompileExpression(string newExpression)
        {
            //  Check to see if our newExpression is not empty
            if (newExpression.Length != 0)
            {
                // Holds the current index position in our string
                int currentIndex = 0;
                // String to hold sub string of parsed string
                string subString = string.Empty;

                for (int i = 0; i < newExpression.Length; i++)
                {
                    currentIndex = i;
                    // We loop until we find the first operator in our expression i.e. "-,+,/,*"
                    while (currentIndex < newExpression.Length &&
                           !operatorNodeFactory.IsValidOperator(newExpression[currentIndex].ToString()))
                    {
                        currentIndex++;
                    }
                    // If our current index position in our string is an operator we set it to our substring
                    if (currentIndex == i)
                    {
                        subString = newExpression[i].ToString();
                    }
                    // else we find the correct substring
                    else
                    {
                        subString = newExpression.Substring(i, currentIndex - i);
                        if (currentIndex - i > 1)
                        {
                            i += currentIndex - i;
                            i--;
                        }
                    }

                    // If our current symbol is an operator
                    if (operatorNodeFactory.IsValidOperator(subString[0].ToString()))
                    {
                        OperatorNode newNode = operatorNodeFactory.CreateOperatorNode(subString[0].ToString());
                        
                        // If new node is a left parentheses we push it onto the operator stack
                        if (newNode.Operator == '(')
                        {
                            this.operatorStack.Push(newNode);
                        }
                    
                        // If we encounter right parentheses we look through operator stock and pop values until we reach
                        // left parentheses then pop that left parentheses our
                        else if (newNode.Operator == ')')
                        {
                            while ((char)this.operatorStack.Peek().GetType().GetProperty("Operator")
                                       .GetValue(this.operatorStack.Peek()) != '(')
                            {
                                this.postFixExpressionStack.Push(this.operatorStack.Pop());
                            }

                            this.operatorStack.Pop();
                        }
                        
                        // If incoming symbol is an operator and our operator stack is empty or a left parentheses we
                        // push the new symbol onto the operator stack
                        else if (this.operatorStack.Count == 0 || (char)this.operatorStack.Peek().GetType()
                                     .GetProperty("Operator").GetValue(this.operatorStack.Peek()) == '(')
                            
                        {
                            this.operatorStack.Push(newNode);
                        }
                        
                        // Incoming symbol is an operator and has a higher or equal Precedence than the top operator in the stack
                        // and is right associative. Then we push this new operator onto the stack
                        else if (newNode.Precedence > (ushort)this.operatorStack.Peek().GetType().GetProperty("Precedence")
                                     .GetValue(this.operatorStack.Peek()) || (newNode.Precedence == (ushort)this
                                     .operatorStack
                                     .Peek().GetType().GetProperty("Precedence").GetValue(this.operatorStack.Peek())))
                        {
                            // If we have two operators with same precedence we will read left to right
                            if(newNode.Precedence == (ushort)this
                                   .operatorStack
                                   .Peek().GetType().GetProperty("Precedence").GetValue(this.operatorStack.Peek()))
                            {
                                this.postFixExpressionStack.Push(this.operatorStack.Pop());
                            }
                            this.operatorStack.Push(newNode);   
                        }
                        
                        // Incoming symbol is an operator and has a lower or equal Precedence than the top operator in the stack
                        // and is left associative. Then we pop from our stack until our condition is false and push the new 
                        // symbol onto our operator stack
                        else if(newNode.Precedence < (ushort)this.operatorStack.Peek().GetType().GetProperty("Precedence")
                                    .GetValue(this.operatorStack.Peek()) || (newNode.Precedence == (ushort)this.operatorStack.Peek()
                                .GetType().GetProperty("Precedence").GetValue(this.operatorStack.Peek())))

                        {
                            while (this.operatorStack.Count() > 0 && ((OperatorNode)this.operatorStack.Peek()).Operator
                                   != '(' && ((OperatorNode)this.operatorStack.Peek()).Precedence >= newNode.Precedence)
                            {
                                this.postFixExpressionStack.Push(this.operatorStack.Pop());
                            }
                            this.operatorStack.Push(newNode);
                        }
                    }
                    
                    //We Check if our expression is only a Constant number Ex. 10
                    double constantNumber;
                    if (double.TryParse(subString, out constantNumber))
                    {
                        ConstantNode newNode = new ConstantNode();
                        newNode.Value = constantNumber;
                        this.postFixExpressionStack.Push(newNode);
                    }
                    //Lastly we check if our expression is only a   Variable
                    else if (!operatorNodeFactory.IsValidOperator(subString[0].ToString()))
                    {
                        VariableNode newNode = new VariableNode();
                        newNode.Name = subString;
                        SetVariables(newNode.Name, 0);
                        this.postFixExpressionStack.Push(newNode);
                    }
                }
            }
        }
    }
}

