using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
//Author: Sarthak Jain
//Class: CS 3500 Fall 2020
//Prof.: Prof. Daniel Kopta
namespace SS
{

    // PARAGRAPHS 2 and 3 modified for PS5.
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        //A dictionary to keep track of the cell's associated with the name in the spreadsheet
        private Dictionary<String, Cell> dictionary;
        //A graph to keep track of dependecies of cells in the spreadsheet
        private DependencyGraph graph;

        //id of the clients map to a cell
        // private Dictionary<int, Cell> clients;

        //client ids 
        // private HashSet<int> numOfClients;

       

        /// <summary>
        /// Initializes the instance variables
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            //Initializes the dictionary, graph, and sets changed to false
            dictionary = new Dictionary<string, Cell>();
            // clients = new Dictionary<int, Cell>();
            graph = new DependencyGraph();
            Changed = false;
            // numOfClients = new HashSet<int>();
        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            //Initializes the dictionary, graph, and sets changed to false
            dictionary = new Dictionary<string, Cell>();
            // clients = new Dictionary<int, Cell>();
            graph = new DependencyGraph();
            Changed = false;
            // numOfClients = new HashSet<int>();
        }
        /// <summary>
        /// You should add a four-argument constructor to the Spreadsheet class.
        /// It should allow the user to provide a string representing a path to a file (first parameter), 
        /// a validity delegate (second parameter), a normalization delegate (third parameter), and
        /// a version (fourth parameter). It should read a saved spreadsheet from the file (see the Save method)
        /// and use it to construct a new spreadsheet. The new spreadsheet should use the provided validity 
        /// delegate, normalization delegate, and version.
        /// </summary> 
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            //Initializes the dictionary, graph, and sets changed to false
            dictionary = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
            // clients = new Dictionary<int, Cell>();
            Changed = false;
            // numOfClients = new HashSet<int>();
            //Trys to get the data from a previous spreadsheet
            try
            {
                //Uses the XMLReader to read the file associated with the filePath
                using (XmlReader xmlReader = XmlReader.Create(filePath))
                {
                    //Creates a cellName and contents variable to store the data from the file
                    String cellName = "";
                    String contents = "";
                    //Runs while xmlReader is not empty
                    while (xmlReader.Read())
                    {
                        //Checks if the item is a starting tag
                        if (xmlReader.IsStartElement())
                        {
                            //If the reader finds the name as spreadsheet, then goes in
                            if (xmlReader.Name.Equals("spreadsheet"))
                            {
                                //Checks if the versions match or not
                                if (xmlReader["version"] != Version)
                                {
                                    throw new SpreadsheetReadWriteException("Version's don't match");
                                }
                            }
                            //If the reader finds the name as equal to the cellName, then goes in
                            else if (xmlReader.Name.Equals("name"))
                            {
                                //Reads and sets the value of the cellName accordingly
                               xmlReader.Read(); 
                                cellName = xmlReader.Value;

                            }
                            else if (xmlReader.Name.Equals("contents"))
                            {
                                //Reads and sets the value of the contents accordingly
                                xmlReader.Read();
                                contents = xmlReader.Value;
                                //Sets the contents of the cell using the cellName and content found into the new spreadsheet
                                SetContentsOfCell(cellName, contents);
                            }
                        }
                    }
                }
            }
            //Catches all possible Exceptions and throws a SpreadsheetReadWriteException accordingly
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("The reason for the error was " + e.Message);
            }


        }
        // ADDED FOR PS5
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Sets the number of clients
        /// </summary>
        // public void SetNumOfClients(HashSet<int> num)
        // {
        //     numOfClients = num;
        // }

        /// <summary>
        /// Gets the number of clients
        /// </summary>
        // public HashSet<int> GetNumOfClients()
        // {
        //     return numOfClients;
        // }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {

            String normalizedName = Normalize(name);
            //Checks if the name of the cell is null
            if (normalizedName == null)
            {
                throw new InvalidNameException();
            }

            //Checks if the name of the cell is valid
            if (!Valid(normalizedName))
            {
                throw new InvalidNameException();
            }

            //If the dictionary contains the key name, then it goes into this condition
            if (dictionary.ContainsKey(normalizedName))
            {
                //Returns the contents of the cell associated with the name
                return dictionary[normalizedName].GetContents();
            }
            else
            {
                //Returns an empty string if the dictionary doesn't contain the key
                return "";
            }
        }

        // public Dictionary<int, Cell> GetClients()
        // {
        //     return clients;
        // }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            //Normalizes the name
            String normalizedName = Normalize(name);
            //Checks if the name of the cell is null
            if (normalizedName == null)
            {
                throw new InvalidNameException();
            }

            //Checks if the name of the cell is valid
            if (!Valid(normalizedName))
            {
                throw new InvalidNameException();
            }

            //If the dictionary contains the key name, then it goes into this condition
            if (dictionary.ContainsKey(normalizedName))
            {
                //Returns the contents of the cell associated with the name
                return dictionary[normalizedName].GetValue();
            }
            else
            {
                //Returns an empty string if the dictionary doesn't contain the key
                return "";
            }
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //Returns all the keys in the dictionary
            return dictionary.Keys;
        }
        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            //Has the versionInfo set as null
            String versionInfo = null;
            try
            {
                //Tries reading the XML file at the given filepath
                using (XmlReader xmlReader = XmlReader.Create(filename))
                {
                    //A loop that runs while the file has items in it
                    while (xmlReader.Read())
                    {
                        //If the xmlReader name is Spreadsheet and the nodeType is an element, then goes in here 
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "spreadsheet")
                        {
                            //If the xmlReader has attributes, then gets the attribute associated with version and breaks out of the loop
                            if (xmlReader.HasAttributes)
                            {
                                versionInfo = xmlReader.GetAttribute("version");
                                break;
                            }
                        }
                    }
                }
            }
            //Catches all possible Exceptions and throws a SpreadsheetReadWriteException accordingly
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("The reason for the error was " + e.Message);
            }
            //Return the returnStr if needed
            return versionInfo;

        }
        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            //Creates the settings of the XMLWriterSettings
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            try
            {
                //Uses the XmlWriter to create an XML file using the settings and the fileName 
                //provided where the XML will be stored
                using (XmlWriter xml = XmlWriter.Create(filename, settings))
                {
                    //Starts writing the document
                    xml.WriteStartDocument();

                    //Starts the spreadsheet
                    xml.WriteStartElement("spreadsheet");

                    //Writes the version as an Attribute of the spreadsheet
                    xml.WriteAttributeString("version", Version);

                    foreach (String cellName in dictionary.Keys)
                    {
                        //Starts the cell
                        xml.WriteStartElement("cell");

                        //Writes the cellName within the name tag
                        xml.WriteElementString("name", cellName);

                        //If the contents of the cellName is a double, then it gets the
                        //contents of the cell and stores it within the contents tag
                        if (dictionary[cellName].GetContents() is Double d)
                        {
                            xml.WriteElementString("contents", dictionary[cellName].GetContents().ToString());
                        }
                        //If the contents of the cellName is a Formula, then it gets the
                        //contents of the cell and stores it within the contents tag, with
                        //the equals sign in front of the start of the contents to indicate this
                        //is a formula
                        else if (dictionary[cellName].GetContents() is Formula f)
                        {
                            xml.WriteElementString("contents", "=" + dictionary[cellName].GetContents().ToString());
                        }
                        //If the contents of the cellName is a string, then it gets the
                        //contents of the cell and stores it within the contents tag
                        else if (dictionary[cellName].GetContents() is String s)
                        {
                            xml.WriteElementString("contents", dictionary[cellName].GetContents().ToString());
                        }
                        //Ends the cell
                        xml.WriteEndElement();
                    }
                    //Ends the spreadsheet
                    xml.WriteEndElement();
                    //Ends the document
                    xml.WriteEndDocument();

                    //Sets changed to false
                    Changed = false;
                }
            }
            catch (Exception e)
            {
                //Throws a SpreadsheetReadWriteException if any exception is encountered
                throw new SpreadsheetReadWriteException("The reason for the error was " + e.Message);
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            //Normalizes the name
            String normalizedName = Normalize(name);
            //Creates a new cell, with the name and number being passed
            Cell c = new Cell(normalizedName, number);

            //Sets the contents of the cell with the number
            c.SetContents(number);

            //Sets the value of the cell which should be the number
            c.SetValue(Lookup);

            //If the dictionary contains the key name, then it goes here
            if (dictionary.ContainsKey(normalizedName))
            {
                //Sets the value associated with the name in the dictionary with the new Cell
                dictionary[normalizedName] = c;

                //Replaces the dependees of the name, with a new HashSet
                graph.ReplaceDependees(normalizedName, new HashSet<string>());
            }
            else
            {
                //Adds the cell to the dictionary
                dictionary.Add(normalizedName, c);

                //Replaces the dependees of the name, with a new HashSet
                graph.ReplaceDependees(normalizedName, new HashSet<string>());
            }

            //Gets the orders of the cell recalculated and stores it into a list
            List<string> recalculatedList = new List<string>(GetCellsToRecalculate(normalizedName));

            //Recalculates the values of all the cells
            RecalculateValues(recalculatedList);

            //Sets changed to true
            Changed = true;

            //Returns the list of cells recalculated
            return recalculatedList;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            //Normalizes the name
            String normalizedName = name;

            //Creates a new Cell, with the name and text being passed
            Cell c = new Cell(normalizedName, text);

            //Sets the contents and value of the cell
            c.SetContents(text);
            c.SetValue(Lookup);

            //If the dictionary contains the key name, then it goes here
            if (dictionary.ContainsKey(normalizedName))
            {
                //Sets the value associated with the name in the dictionary with the new Cell
                dictionary[normalizedName] = c;
                //If the contents of the cell is empty, then it removes it from the dictionary
                if ((string)c.GetContents() == "")
                {
                    dictionary.Remove(normalizedName);
                }
                //Replaces the dependees of the name
                graph.ReplaceDependees(normalizedName, new HashSet<string>());
            }
            else
            {
                //If the contents of the cell is not null, then it adds the cell to the dictionary
                if ((string)c.GetContents() != "")
                {
                    dictionary.Add(normalizedName, c);
                }

                //Replaces the dependees of the name
                graph.ReplaceDependees(normalizedName, new HashSet<string>());
            }

            //Gets the orders of the cell recalculated and stores it into a list
            List<string> recalculatedList = new List<string>(GetCellsToRecalculate(normalizedName));

            //Recalculates the values of all the cells
            RecalculateValues(recalculatedList);

            //Sets changed to true
            Changed = true;

            //Returns the list of cells recalculated
            return recalculatedList;
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            //Normalizes the name of the Cell
            String normalizedName = Normalize(name);


            //Creates a new Cell passing the name and formula
            Cell c = new Cell(normalizedName, formula);

            //Sets the contents of the cell with the formula
            c.SetContents(formula);
            //Sets the value of the Cell which should be a double if the formula is valid
            //or a FormulaError if the formula is invalid
            c.SetValue(Lookup);

            //A boolean to keep track if a new Entry is made
            bool newEntry = false;
            //Gets the previous dependees of the cell before
            HashSet<string> dependees = new HashSet<string>();
            //A Cell to store the prevCell
            Cell prevCell = null;
            //If the dictionary contains the key name, then goes here
            if (dictionary.ContainsKey(normalizedName))
            {
                //Gets the prevCell
                prevCell = dictionary[normalizedName];
                //Replaces it with the new cell
                dictionary[normalizedName] = c;

                //Gets the prevDependees of the graph
                dependees = new HashSet<string>(graph.GetDependees(normalizedName));
                //Replaces the dependees with the variables of the formula
                graph.ReplaceDependees(normalizedName, formula.GetVariables());
            }
            else
            {
                //Adds the cell to the dictionary
                dictionary.Add(normalizedName, c);

                //Gets the prevDependees of the graph
                dependees = new HashSet<string>(graph.GetDependees(normalizedName));

                //Replaces the dependees with the variables of the formula
                graph.ReplaceDependees(normalizedName, formula.GetVariables());

                //Sets the newEntry boolean to true
                newEntry = true;
            }

            //Creates a new list to be returned
            List<string> recalculatedList;
            try
            {
                //Gets the orders of the cell recalculated and stores it into a list
                recalculatedList = new List<string>(GetCellsToRecalculate(normalizedName));

                //Recalculates the values of all the cells
                RecalculateValues(recalculatedList);
            }
            //Goes in here if we encounter a CircularException
            catch (CircularException ce)
            {

                //If a new Cell was added, we remove it from the dictionary
                if (newEntry == true)
                {
                    dictionary.Remove(normalizedName);
                }
                //Otherwise, we revert the value set to the value before in the Cell
                else
                {
                    dictionary[normalizedName] = prevCell;
                }
                //Replace the dependees of the name to it's original form
                graph.ReplaceDependees(normalizedName, dependees);

                //Throws a CircularException
                throw new CircularException();
            }


            //Sets changed to true
            Changed = true;

            //Returns the list of cells recalculated
            return recalculatedList;

        }

        // ADDED FOR PS5
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            //Checks if the contents are null
            if (content == null)
            {
                throw new ArgumentNullException();
            }
            //Normalizes the name
            String normalizedName = Normalize(name);
           
           
            //Checks if the normalizedName is null
            if (normalizedName == null)
            {
                throw new InvalidNameException();
            }
            
            //Checks if the normalName is invalid
            if (!Valid(name))
            {
                throw new InvalidNameException();
            }

            //Checks if the normalizedName is invalid
            if (!Valid(normalizedName))
            {
                throw new InvalidNameException();
            }
            //If the content could be parsed to a double, then it goes into the SetCellContents with
            //the name and double parameter
            if (Double.TryParse(content, out double parsedNum))
            {
                return SetCellContents(normalizedName, parsedNum);
            }
            //If the content is not null, and starts with an equals sign, then it goes into the 
            //SetCellContents with the name and formula parameter
            else if (content != "" && content[0] == '=')
            {
                //Gets the formula as a string
                String formulaAsStr = content.Substring(1);
                //Converts the string to a Formula
                Formula formula = new Formula(formulaAsStr, Normalize, IsValid);
                //Returns the SetCellContents method with the normalizedName and formula
                return SetCellContents(normalizedName, formula);
            }
            else
            {
                //Otherwise assumes anything else is a string, and will go into the SetCellContents with
                //the string parameter
                return SetCellContents(normalizedName, content);
            }

        }

        /// <summary>
        /// A helper method that does the recalculations of the cell
        /// </summary>
        /// <param name="list"></param>
        private void RecalculateValues(IEnumerable<string> list)
        {
            //Iterates through the list
            foreach (string s in list)
            {
                //If the dictionary contains a key, then it sets it's new 
                //value using the lookup delegate
                if (dictionary.ContainsKey(s))
                {
                    dictionary[s].SetValue(Lookup);
                }
            }
        }


        ///<summary>
        ///A helper method that checks if the Cell is valid according to the rules of the Abstract
        ///Spreadsheet in that the starting character has to be a letter, and the characters after that 
        ///should be either a letter or number, and once a number is seen, we should only have numbers 
        ///after. Also checks if the name is valid according to the validator
        /// </summary>
        private bool Valid(String name)
        {
            //Normalizes the name given
            String normalizedName = Normalize(name);
            //Returns true if the cell name is valid, and the validator thinks it is valid
            return Cell.IsValid(normalizedName) && IsValid(normalizedName);
        }



        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //Gets the dependents of the name from the graph
            return graph.GetDependents(name);
        }
        /// <summary>
        /// A private method that acts as a delegate doing the looking up when setting the values of Cell's
        /// </summary>
        private double Lookup(String name)
        {
            //Normalizes the name
            String newName = Normalize(name);

            //Checks if the dictionary contains the key newName
            if (dictionary.ContainsKey(newName))
            {
                //If the value of the key is a double, then it returns the value
                if (dictionary[newName].GetValue() is double)
                {
                    return (double)dictionary[newName].GetValue();
                }
            }
            //Throws an exception if can't find anything
            throw new ArgumentException();
        }

   

        /// <summary>
        /// A private class to represent a Cell
        /// </summary>
        public class Cell
        {
            //A string representing the cellName
            private String cellName;
            //An Object representing the contents of the cell
            private Object contents;
            //An Object representing the value of the cell
            private Object value;

            /// <summary>
            /// Empty constructor
            /// </summary>
            public Cell()
            {
                cellName = "A1";
                contents = "";
                value = "";
            }

            /// <summary>
            /// Constructor with just cellName
            /// </summary>
            public Cell(string cell)
            {
                cellName = cell;
                contents = "";
                value = "";
            }

            /// <summary>
            /// The constructor creating a new Cell
            /// </summary>
            public Cell(String cell, Object o)
            {
                //If the object is a formula, then goes here
                if (o is Formula formula)
                {
                    //Intializes the cellName and the contents
                    cellName = cell;
                    contents = formula;
                }
                //If the object is a double, then goes here
                else if (o is Double d)
                {
                    //Intializes the cellName, the contents and the value
                    cellName = cell;
                    contents = d;
                    value = d;
                }
                //If the object is a string, then goes here
                else if (o is String s)
                {
                    //Intializes the cellName, the contents and the value
                    cellName = cell;
                    contents = s;
                    value = s;
                }
            }
            /// <summary>
            /// Returns the cell name
            /// </summary>
            public string GetCellName()
            {
                return cellName;
            }

            /// <summary>
            /// Sets the cell name
            /// </summary>
            public void SetCellName(string name)
            {
                cellName = name;
            }


            /// <summary>
            /// Gets the value of the cell
            /// </summary>
            public object GetValue()
            {
                return value;
            }


            /// <summary>
            /// Gets the contents of the cell
            /// </summary>
            public Object GetContents()
            {
                return contents;
            }

            /// <summary>
            /// Sets the contents of the cell
            /// </summary>
            public void SetContents(Object obj)
            {
                //If the object is a Formula, then goes in here to set the contents
                if (obj is Formula)
                {
                    contents = (Formula)obj;
                }
                //If the object is a Double, then goes in here to set the contents
                else if (obj is Double)
                {
                    contents = (Double)obj;
                }
                //If the object is a String, then goes in here to set the contents
                else if (obj is String)
                {
                    contents = (string)obj;
                }
            }
            /// <summary>
            /// Sets the value of the Cell
            /// </summary>
            public void SetValue(Func<string, double> lookup)
            {
                //If the contents is a String, then sets the value as the string
                if (GetContents() is String s)
                {
                    value = s;
                }
                //If the contents is a Double, then sets the value as the double
                else if (GetContents() is Double d)
                {
                    value = d;
                }
                //If the contents is a Formula, then it evaluates the formula using the lookup delegate
                else if (GetContents() is Formula f)
                {
                    value = f.Evaluate(lookup);
                }
            }

            ///<summary>
            /// A string is a valid cell name if and only if:
            ///   (1) its first character is a letter
            ///   (2) its remaining characters are letters and/or digits. Once a digit is seen, letter should
            ///   not exist after
            /// </summary>
            public static bool IsValid(String name)
            {
                return Regex.IsMatch(name, @"^[a-zA-Z]+(\d)+$");
            }

       
        }
    }
}