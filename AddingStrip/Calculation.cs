using AddStrip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AddingStrip
{
    /// <summary> class Calculation
	/// class to store CalcLine objects and handle add, find, replace, insert, delete, clear and
    /// file operations on the calculation
	/// </summary>
    /// <Author>Mitchell Yuan</Author>
    /// <Date>06/06/2016</Date>
    class Calculation
    {
        private ArrayList theCalcs;
        private ListBox lstDisplay;
        private bool isModified = false;    //Has the Calculation been modified since last save
        string calcFilename = "";
        double resultSoFar;

        /// <summary> constructor Calculation
		/// create a Calculation object which contains the reference to a listbox and a new
        /// ArrayList object
		/// </summary>
        public Calculation(ListBox lb)
        {
            lstDisplay = lb;
            theCalcs = new ArrayList();
        }

        public int Count //public property that gives access to the private theCalcs data field
        {
            get { return theCalcs.Count; }
        }
        public bool IsModified //Public property providing the filename for the saved Calculation.
        {
            get { return isModified; }
        }
        public string CalcFilename //Public property providing the filename for the saved Calculation.
        {
            get { return calcFilename; }
        }

        /// <summary> method Add
		/// Add a CalcLine object to the ArrayList then redisplay the calculations, and mark the
        /// Calculation as modified.
		/// </summary>
        public void Add(CalcLine cl)
        {
            theCalcs.Add(cl);
            Redisplay();
            isModified = true;
        }

        /// <summary> method Clear
		/// Clear the ArrayList and the listbox
		/// </summary>
        public void Clear()
        {
            theCalcs.Clear();
            lstDisplay.Items.Clear();
            calcFilename = "";
            isModified = false;
        }

        /// <summary> method Redisplay
		/// Clear the listbox and then for each line in the calculation, if the line is an ordinary
        /// calculation add the text version of the CalcLine to the listbox and calculate the result
        /// of the calculation so far. If the line is for a total or subtotal add the text for the 
        /// total or subtotal to the listbox. If the line is for a total, the result of the calculation
        /// so far is reset to zero.
		/// </summary>
        public void Redisplay()
        {
            lstDisplay.Items.Clear();
            resultSoFar = 0;

            // Go through each CalcLine object in the arrylist and implement operation 
            foreach(CalcLine cl in theCalcs)
            {
                string calcLine = cl.ToString();
                char ch = calcLine[0];

                // If the first character of calcLine is "=", add a new line to the Listbox to show the result so far
                // and reset the result to zero.
                if (ch == '=')
                {
                    lstDisplay.Items.Add((CalcLine)cl + " " + cl.NextResult(resultSoFar).ToString() + " << total");
                    resultSoFar = 0;
                }

                // If the first character of calcLine is "#", add a new line to the Listbox to show the result so far.
                else if (ch == '#')
                {
                    lstDisplay.Items.Add((CalcLine)cl + " " + cl.NextResult(resultSoFar).ToString() + " < subtotal");
                }

                // If the line is an ordinary calculation add the text version of the CalcLine to the listbox and calculate the result
                else
                {
                    lstDisplay.Items.Add((CalcLine)cl);
                    resultSoFar = cl.NextResult(resultSoFar);
                }
            }
        }

        /// <summary> method Find
		/// Return a reference to the nth CalcLine object in the ArrayList
		/// </summary>
        public CalcLine Find(int n)
        {
            return (CalcLine)theCalcs[n];
        }

        /// <summary> method Replace
		/// Replace the nth CalcLine object in the ArrayList with the newCalc object passed by parameter, 
        /// and then redisplay the calculations.
        /// Mark the caculation as modified
		/// </summary>
        public void Replace(CalcLine newCalc, int n)
        {
            theCalcs[n] = newCalc;
            Redisplay();
            isModified = true;
        }

        /// <summary> method Insert
		/// Insert the newCalc CalcLine object in the ArrayList immediately before the nth object, and then
        /// redisplay the calculations.
        /// Mark the caculation as modified
		/// </summary>
        public void Insert(CalcLine newCalc, int n)
        {
            theCalcs.Insert(n, newCalc);
            Redisplay();
            isModified = true;
        }

        /// <summary> method Delete
		/// Delete the nth CalcLine object in the ArrayList, and then redisplay the calculations.
        /// Mark the caculation as modified.
		/// </summary>
        public void Delete(int n)
        {
            theCalcs.RemoveAt(n);
            Redisplay();
            isModified = true;
        }

        /// <summary> method SaveToFile
		/// Save all the CalcLine objects in the ArrayList as lines of text in the given file.
        /// Return a bool variable giving the result.
		/// </summary>
        public bool SaveToFile(string filename)
        {
            // Create a new StreamWriter object to write data to the given file
            StreamWriter sw = new StreamWriter(filename);

            try
            {
                foreach(CalcLine cl in theCalcs)
                {
                    sw.Write(cl.ToString() + "\r\n");
                }

                // Initialize the modification status to false
                isModified = false;

                // Show a message box to tell user that the save operation has accomplished, and return true.
                MessageBox.Show("Your calculation has been saved successfully!", "Success");
                return true;
            }
            catch (Exception)
            {
                // If an exception occur, return false.
                return false;
            }
            finally
            {
                sw.Close(); // Close the StreamWriter object at the end of the method.
            }
        }

        /// <summary> method LoadFromFile
		/// Clear the ArrayList and then open the given file and convert the lines of the file 
        /// to a set of CalcLine objects held in the ArrayList. Then redisplay the calculations.
        /// Return a bool variable giving the result.
		/// </summary>
        public bool LoadFromFile(string filename)
        {
            // Create a new StreamReader object to read data from the given file
            StreamReader sr = new StreamReader(filename);
            string temp = sr.ReadLine();

            // Clear the ArrayList
            theCalcs.Clear();

            try
            {
                // If current line is not null, creat a new CalcLine object from current line and 
                // add the CalcLine object to ArrayList. Then read next line.
                while (temp != null)
                {
                    CalcLine cl = new CalcLine(temp);
                    theCalcs.Add(cl);
                    temp = sr.ReadLine();
                }

                // Redisplay the listbox with latest ArrayList and return true.
                Redisplay();
                calcFilename = filename;
                return true;
            }
            catch (Exception)
            {
                // If an exception occur, return false;
                return false;
            }
            finally
            {
                sr.Close(); // Close the StreamReader object at the end of the method.
            }
        }
    }
}
