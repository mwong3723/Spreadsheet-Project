using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace SpreadsheetEngine.ExpressionTreeApplication.OperatorNodes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class OperatorNodeFactory
    {
        /*
        private static readonly Dictionary<char, Func<OperatorNode>> OperatorsDictionary =
            new Dictionary<char, Func<OperatorNode>>
            {
                {'+', () => new AddNode()},
                {'-', () => new MinusNode()},
                {'*', () => new MultiplyNode()},
                {'/', () => new DivideNode()},
                {'(', () => new LeftParenthesesNode()},
                {')', () => new RightParenthesesNode()},

            };
        */

        /// <summary>
        /// Dictionary that holds all our operator types and a lambda function to call to institute a new Node
        /// depending on the type
        /// </summary>
        public static Dictionary<string, Type> OperatorsDictionary;
        
        /// <summary>
        ///  This is our Delegate function that executes when a new operator is found 
        /// </summary>
        private delegate void OnOperator(string op, Type type);
        
        /// <summary>
        /// Constructor to instantiate a new Dictionary and populates that Dictionary with operators
        /// </summary>
        public OperatorNodeFactory()
        {
            OperatorNodeFactory.OperatorsDictionary = new Dictionary<string, Type>();
            TraverseAvailableOperators((op,type)=>OperatorsDictionary.Add(op,type));
        }

        /// <summary>
        /// Populates our dictionary with new operators
        /// </summary>
        /// <param name="onOperator"></param>
        private void TraverseAvailableOperators(OnOperator onOperator)
        {
            Type operatorNodeType = typeof(OperatorNode);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                IEnumerable<Type> operatorTypes =
                    assembly.GetTypes().Where(type => type.IsSubclassOf(operatorNodeType));
                
                foreach (var type in operatorTypes)
                {
                    //PropertyInfo operatorField = type.GetProperty("Operator");
                    FieldInfo operatorField = type.GetField("operatorSymbol");
                    
                    if (operatorField != null)
                    {
                        object value = operatorField.GetValue(type);
                        if (value is string operatorSymbol)
                        {
                            onOperator(operatorSymbol, type);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates new operator node based on the given operator type
        /// </summary>
        /// <param name="op">inputted operator type</param>
        /// <returns>new Operator node</returns>
        /// <exception cref="Exception"></exception>
        public OperatorNode CreateOperatorNode(string op)
        {
            if (OperatorNodeFactory.OperatorsDictionary.TryGetValue(op, out var operatorType))
            {
                return (OperatorNode)Activator.CreateInstance(operatorType);
            }

            throw new Exception("Unhandled Operator");
        }
        
        
        /// <summary>
        /// Factory method to check that out inputted operator was valid i.e. "+,-,*,/"
        /// </summary>
        /// <param name="op">inputted operator</param>
        /// <returns>true if operator is in our dictionary else false</returns>
        public bool IsValidOperator(string op)
        {
            return OperatorsDictionary.ContainsKey(op);
        }
        
    }
}
