using System.Reflection;
using System.Text;
using SpreadsheetEngine;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace SpreadsheetEngine.Test
{
    public class CellTests
    {
        
        // ----------------------------------------------------------------------------------------------------------
        /*
         *  HW 4 tests
         */
        // ----------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// This constructor test will see if our spreadsheet class properly created our rows
        /// </summary>
        [Test]
        public void SpreadsheetConstructorRowTest()
        {
            Spreadsheet sheet = new Spreadsheet(10, 10);
            Assert.AreEqual(sheet.GetCell(0, 0).RowIndex, 0);
            Assert.AreEqual(sheet.GetCell(9, 9).RowIndex, 9);
        }
        
        /// <summary>
        /// This constructor test will see if our spreadsheet class properly created our columns
        /// </summary>
        [Test]
        public void SpreadsheetConstructorColumnTest()
        {
            Spreadsheet sheet = new Spreadsheet(10, 10);
            Assert.AreEqual(sheet.GetCell(0, 0).ColumnIndex, 0);
            Assert.AreEqual(sheet.GetCell(9, 9).ColumnIndex, 9);
        }
        
        /// <summary>
        /// This constructor test will see if our spreadsheet class has default empty values for text property
        /// </summary>
        [Test]
        public void SpreadsheetConstructorTextTest(){
            Spreadsheet sheet = new Spreadsheet(10, 10);
            Assert.AreEqual(sheet.GetCell(0, 0).Text, "");
        }

        /// <summary>
        /// This constructor test will see if when we instantiate our cell class if we properly set our rows
        /// by calling our RowIndex property.
        /// </summary>
        [Test]
        public void CellConstructorRowTest()
        {
            Cell cell = new InitalizeCell(3, 3);
            Assert.AreEqual(cell.RowIndex,3);
        }
        
        /// <summary>
        /// This constructor test will see if when we instantiate our cell class if we properly set our columns
        /// by calling our ColumnIndex property.
        /// </summary>
        [Test]
        public void CellConstructorColumnTest()
        {
            Cell cell = new InitalizeCell(3, 3);
            Assert.AreEqual(cell.ColumnIndex,3);
        }

        /// <summary>
        /// This constructor test will see if when we instantiate our cell class if we properly set our text field
        /// by calling our Text property.
        /// </summary>
        [Test]
        public void CellConstructorTextTest()
        {
            Cell cell = new InitalizeCell(3, 3);
            Assert.AreEqual(cell.Text, "");
        }
        
        // ----------------------------------------------------------------------------------------------------------
        /*
         *  HW 7 tests
         */
        // ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// We are testing if we create a cell and give it no formula then both its Text and Value property are the same
        /// </summary>
        [Test]
        public void CellValueMatchesText()
        {
            Spreadsheet spreadsheet = new Spreadsheet(1, 1);
            spreadsheet.GetCell(0, 0).Text = "test";
            Assert.That(spreadsheet.GetCell(0,0).Value, Is.EqualTo("test"));
        }

        /// <summary>
        /// In this test we create two cells one referencing the other and test the value and text property of the cell
        /// that is referencing the other
        /// </summary>
        [Test]
        public void CellValueHasFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet(2, 2);
            spreadsheet.GetCell(0, 0).Text = "Hello World";
            spreadsheet.GetCell(1, 1).Text = "=A1";
            Assert.That(spreadsheet.GetCell(1,1).Value, Is.EqualTo("Hello World"));
            Assert.That(spreadsheet.GetCell(1,1).Text, Is.EqualTo("=A1"));
        }

        /// <summary>
        /// Testing when we reference a cell and change that referenced cell will the cell update.
        /// </summary>
        [Test]
        public void CellUpdateReference()
        {
            var spreadsheet = new Spreadsheet(2, 2);
            spreadsheet.GetCell(0, 0).Value = "test";
            spreadsheet.GetCell(1, 1).Value = "test2";
            spreadsheet.GetCell(1, 1).Text = "=A1";
            Assert.That(spreadsheet.GetCell(1,1).Value, Is.EqualTo("test"));
        }
        
        // ----------------------------------------------------------------------------------------------------------
        /*
         *  HW 8 tests
         */
        // ----------------------------------------------------------------------------------------------------------

        /*
        [Test]
        public void TestCellDefaultColorProperty()
        {
            Cell cell = new InitalizeCell(2, 2);
            Assert.That(cell.BGColor, Is.EqualTo(0xFFFFFFFF));
        }
        
        [Test]
        public void TestCellNewColorProperty()
        {
            Cell cell = new InitalizeCell(2, 2);
            cell.BGColor = 0xFF2D00;
            Assert.That(cell.BGColor, Is.EqualTo(0xFF2D00));
        }
        */
        
        
        // ----------------------------------------------------------------------------------------------------------
        /*
         *  HW 9 tests
         */
        // ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// testing our SaveStream function
        /// </summary>
        [Test]
        public void SaveXmlTest()
        {
            string actualXml;
            var spreadsheet = new Spreadsheet(2, 2);
            spreadsheet.GetCell(0, 0).BackgroundColor = 0xFF8000FF;
            spreadsheet.GetCell(0,0).Text = "=B1";

            var stream = new MemoryStream();
            spreadsheet.SaveStream(stream);
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                actualXml = reader.ReadToEnd();
            }
            Assert.That(actualXml, 
                Is.EqualTo("<?xml version=\"1.0\" encoding=\"utf-8\"?><spreadsheet><cell name=\"A1\"><bgcolor>FF8000FF</bgcolor><text>=B1</text></cell></spreadsheet>"));
        }

        /// <summary>
        /// Testing our LoadStream function
        /// </summary>
        [Test]
        public void LoadXmlTest()
        {
            var spreadsheet = new Spreadsheet(4,4);
            var loadedXml =
                "<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><spreadsheet><cell name=\\\"C1\\\"><bgcolor>FF8000FF</bgcolor><text>Hi</text></cell></spreadsheet>";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(loadedXml));

            spreadsheet.LoadStream(stream);
            Assert.That(spreadsheet.GetCell(2,0).Text, Is.EqualTo("Hi"));
            Assert.That(spreadsheet.GetCell(2,0).BackgroundColor, Is.EqualTo(0xFF8000FF));
        }
        
        // ----------------------------------------------------------------------------------------------------------
        /*
         *  HW 10 tests
         */
        // ----------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Testing when a bad cell name is referenced like "=6+Cell*27" or "=Ba" and should display
        /// "!(bad reference)" in our cell
        /// </summary>
        [Test]
        public void BadCellNameReference()
        {
            var spreadSheet = new Spreadsheet(2, 2);
            spreadSheet.GetCell(0, 0).Text = "=6+Cell*27";
            Assert.That(spreadSheet.GetCell(0,0).Value, Is.EqualTo("!(bad reference)"));
        }
        
        /// <summary>
        /// Testing when referencing a cell that is out of bounds like "=A61123" and should display
        /// "!(out of bounds reference)"
        /// </summary>
        [Test]
        public void OutOfBoundsCellReference()
        {
            var spreadSheet = new Spreadsheet(2, 2);
            spreadSheet.GetCell(0, 0).Text = "=A4";
            Assert.That(spreadSheet.GetCell(0,0).Value, Is.EqualTo("!(out of bounds reference)"));
        }
        
        /// <summary>
        /// Testing when a cell references its self and should display
        /// "!(self reference)"
        /// </summary>
        [Test]
        public void CellSelfReference()
        {
            var spreadSheet = new Spreadsheet(2, 2);
            spreadSheet.GetCell(0, 0).Text = "=A1";
            Assert.That(spreadSheet.GetCell(0,0).Value, Is.EqualTo("!(self reference)"));
        }

        /// <summary>
        /// Testing when two cells reference each other A1:"=B1*2" and B1:"=A1*4" and should display
        /// "!(circular reference)" in either one or both cells
        /// </summary>
        [Test]
        public void CircularReferenceTwoCells()
        {
            var spreadSheet = new Spreadsheet(2, 2);
            spreadSheet.GetCell(0, 0).Text = "=B2*2";
            spreadSheet.GetCell(1, 1).Text = "=A1*3";
            Assert.That(spreadSheet.GetCell(0,0).Value, Is.EqualTo("!(circular reference)"));
        }
        
        /// <summary>
        /// Testing when four cells reference each other A1:"=B1*2",B1:"=C1*4", C1:"=D1*3", and D1:"=A1*9" and should display
        /// "!(circular reference)" in either one or all cells
        /// </summary>
        [Test]
        public void CircularReferenceFourCells()
        {
            var spreadSheet = new Spreadsheet(10, 10);
            spreadSheet.GetCell(0, 0).Text = "=B1*2";
            spreadSheet.GetCell(0, 1).Text = "=C1*3";
            spreadSheet.GetCell(0, 2).Text = "=D1";
            spreadSheet.GetCell(0, 3).Text = "=A1*5";
            Assert.That(spreadSheet.GetCell(0,0).Value, Is.EqualTo("!(circular reference)"));
        }
        
    }    
}

