using SpreadsheetEngine.ExpressionTreeApplication;
using SpreadsheetEngine.ExpressionTreeApplication.OperatorNodes;

// ReSharper disable once CheckNamespace
namespace SpreadsheetEngine.Test
{
    public class ExpressionTreeTests
    {
        // ----------------------------------------------------------------------------------------------------------
        /*
         *  HW 5 tests
         */
        // ----------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Constructor test for our expression tree testing if when passed no parameters all values are set tp
        /// default values for attributes 
        /// </summary>
        [Test]
        public void ExpressionTreeConstructorTest()
        {
            ExpressionTree.ExpressionTree testTree = new ExpressionTree.ExpressionTree("test");
            Assert.That(testTree.expression, Is.EqualTo("test"));
            Assert.That(testTree.root, Is.Not.Null);
            Assert.That(testTree.variables, Is.Not.Null);
        }

        /// <summary>
        /// This test will call our SetVariable function then test if our value was properly set by
        /// accessing our public dictionary field in our tree object
        /// </summary>
        [Test]
        public void ExpressionTreeSetVariablesTest()
        {
            ExpressionTree.ExpressionTree testTree = new ExpressionTree.ExpressionTree("Hello");
            testTree.SetVariables("A", 10);
            Assert.That(testTree.variables["A"], Is.EqualTo(10));
        }

        /// <summary>
        /// This test creates a tree object with an expression then calls evaluate to see if properly evaluated
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluate()
        {
            ExpressionTree.ExpressionTree testTree = new ExpressionTree.ExpressionTree("1+2+2");
            Assert.That(testTree.Evaluate(), Is.EqualTo(5));
        }
        
        /// <summary>
        /// This test creates a tree object with no expression then calls evaluate to see if returns null like it should
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateNull()
        {
            ExpressionTree.ExpressionTree testTree = new ExpressionTree.ExpressionTree("");
            Assert.That(testTree.Evaluate(), Is.EqualTo(0));
        }
        // ----------------------------------------------------------------------------------------------------------
        /*
         *  HW 6  tests
         */
        // ----------------------------------------------------------------------------------------------------------


        /// <summary>
        /// This Test will test the precedence of an equation that has parentheses
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateOneSetParentheses()
        {
            ExpressionTree.ExpressionTree testTree = new ExpressionTree.ExpressionTree("(2+1)*3");
            Assert.That(testTree.Evaluate(), Is.EqualTo(9));
        }
        
        /// <summary>
        /// Given an Unnecessary amount of parentheses we still ignore the rest and focus on the only set that matters
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateUnnecessaryParentheses()
        {
            ExpressionTree.ExpressionTree testTree = new ExpressionTree.ExpressionTree("((((1+2))))");
            Assert.That(testTree.Evaluate(), Is.EqualTo(3));
        }
        
        /// <summary>
        /// If given equation is only a set of parentheses we should evaluate to null and not 0 as parentheses are not
        /// a set of variables
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateOnlyParentheses()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var expressionTree = new ExpressionTree.ExpressionTree("()");
            });
        }

        /// <summary>
        /// If accidentally only inputted one parentheses should result in null or throw an error to user that is
        /// not acceptable 
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateOneParentheses()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var expressionTree = new ExpressionTree.ExpressionTree("(1+2+6");
            });
        }
        
        /// <summary>
        /// Testing the precedence of an equation with no parentheses to see if correctly evaluated
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluatePrecedenceOrder()
        {
            var testTree = new ExpressionTree.ExpressionTree("3+10/5");
            Assert.That(testTree.Evaluate(), Is.EqualTo(5));
        }
        
        /// <summary>
        /// Given an equation without setting its values, variables should be set to 0 automatically
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateVariablesNotSet()
        {
            var testTree = new ExpressionTree.ExpressionTree("Hello+World");
            Assert.That(testTree.Evaluate(), Is.EqualTo(0));
        }
        
        /// <summary>
        /// Testing Multi-Character values to simulate potential Spreed sheet application formula 
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateMultiCharacterValues()
        {
            ExpressionTree.ExpressionTree testTree = new ExpressionTree.ExpressionTree("B3-A7+E1");
            testTree.SetVariables("B3",10);
            testTree.SetVariables("A7",3);
            testTree.SetVariables("E1",1);
            Assert.That(testTree.Evaluate(), Is.EqualTo(8));
        }
        
        // ----------------------------------------------------------------------------------------------------------
        /*
         *  HW 7 tests
         */
        // ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This create node test unlike the others is testing a new refactored version of create node that uses
        /// reflection to dynamically populate handled expressions
        /// </summary>
        [Test]
        public void ExpressionTreeGetVariableNames()
        {
            //ExpressionTree.ExpressionTree testTree = new ExpressionTree.ExpressionTree("=B2");
            //List<string>variableNames = testTree.GetVariableNames();
            //Assert.That(variableNames[0], Is.EqualTo("B2"));
        }
        
    }
}
