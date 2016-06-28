using AddStrip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AddingStrip
{
    /// <summary> class AddStripForm
	/// class to display a windows form as the user interface to perform caculations
	/// </summary>
    /// <Author>Mitchell Yuan</Author>
    /// <Date>06/06/2016</Date>
    public partial class AddStripForm : Form
    {
        Calculation theCalculation;
        CalcLine currentCalcLine;
        bool validCalcuLine, firstLine;

        /// <summary> constructor AddStripForm
		/// Open a windows form according to the design.
		/// </summary>
        public AddStripForm()
        {
            InitializeComponent();
            theCalculation = new Calculation(lstDisplay); //Create a new Calculation object when program launched
        }

        /// <summary> method mnuNew_Click
		/// When user clicks on "New" in the menu, starts a new calculation and displays a blank listbox and blank text boxes.
        /// If the calculation has been modified, the user will be given a chance to save their modifications before a new 
        /// calculation is started.
		/// </summary>
        private void mnuNew_Click(object sender, EventArgs e)
        {
            // Call the method  ModificationChecking() to give user a chance to save their modifications if the calculation 
            // has been modified.
            ModificationChecking();

            theCalculation.Clear();
            txtInput.Text = "";
            txtSelect.Text = "";
        }

        /// <summary> method mnuOpen_Click
		/// When user clicks on "Open" in the menu, displays an OpenDialogBox and lets the user pick a previously created .cal file. 
        /// The data in the file is read and used to create a new Calculation object and display its calculation lines.
        /// If the calculation has been modified, the user will be given a chance to save their modifications before a previous 
        /// calculation is loaded.
		/// </summary>
        private void mnuOpen_Click(object sender, EventArgs e)
        {
            // Call the method  ModificationChecking() to give user a chance to save their modifications if the calculation 
            // has been modified.
            ModificationChecking();

            txtInput.Text = "";
            txtSelect.Text = "";

            // Displays an OpenDialogBox and lets the user pick a previously created.cal file to open in the program.
            if (dlgOpenCalc.ShowDialog() == DialogResult.OK)
            {
                this.Text = dlgOpenCalc.FileName + " - AddStrip"; //Set the Form window title
                if (!theCalculation.LoadFromFile(dlgOpenCalc.FileName))
                    MessageBox.Show("There was a problem reading from the file selected, please try another file.", "Problem Reading from file");
            }
        }

        /// <summary> method mnuSave_Click
		/// When user clicks on "Save" in the menu, save current caculation to a file.
        /// If the “Adding strip” data has not been saved before, Save runs the Save As option. 
        /// If the “Adding strip” data is for an existing “Adding strip” data file, the data in the Calculation object 
        /// is saved back to that file without displaying a SaveDialogBox.
		/// </summary>
        private void mnuSave_Click(object sender, EventArgs e)
        {
            // If the calculation has been given a file name, save back to that file.
            if (theCalculation.CalcFilename != "")
            {
                theCalculation.SaveToFile(theCalculation.CalcFilename);
            }

            // If the calculation has not been saved before, call SaveAs() method.
            else
            {
                SaveAs();
            }
        }

        /// <summary> method mnuSaveAs_Click
        /// When user clicks on "Save" in the menu, call SaveAs() method.
		/// </summary>
        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        /// <summary> method SaveAs
		/// Displays a SaveDialogBox with either the name of the opened file or if it is a new file being saved, 
        /// gives a default name “Calculation1.cal”. The data in the Calculation object is saved as a text file 
        /// using the filename the user chooses.
		/// </summary>
        private void SaveAs()
        {
            // If current calculation is loaded from a .cal file, set the default file name in SaveDialogBox as the file's name.
            if (theCalculation.CalcFilename != "")
            {
                dlgSaveCalc.FileName = theCalculation.CalcFilename.Substring(8); // Use Substring(8) to cut the string "C:\TEMP\" before the bare file name.
            }

            // Display the SaveDialogBox. If user clicks on "OK" button, call the SaveToFile() method to save current caculation
            // to a file with the filename given in the SaveDialogBox.
            if (dlgSaveCalc.ShowDialog() == DialogResult.OK)
            {
                if (!theCalculation.SaveToFile(dlgSaveCalc.FileName))
                    MessageBox.Show("There was a problem writing to the file selected, please try another file.", "Problem Writing to file");
            }
        }

        /// <summary> method mnuPrint_Click
		/// When user clicks on "Print" in the menu, display a print preview form to display the printout first 
        /// and then print if the user chooses to do so.
		/// </summary>
        private void mnuPrint_Click(object sender, EventArgs e)
        {
            dlgPrintPreview.ShowDialog();
        }

        /// <summary> method printCalculation_PrintPage
		/// Event handler of printDocument object. Define the printout
		/// </summary>
        private void printCalculation_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font textFont = new Font("Arial", 10, FontStyle.Regular);
            Font resultFont = new Font("Arial", 10, FontStyle.Bold);
            Brush myBrush = new SolidBrush(Color.Black);
            int lineSoFar = 0;
            int lineHeight = textFont.Height;
            int leftMargin = e.MarginBounds.Left;
            int topMargin = e.MarginBounds.Top;

            // Print a line for each item in ListBox
            for (int i = 0; i < lstDisplay.Items.Count; i++)
            {
                string strToPrint = lstDisplay.Items[i].ToString();

                // If the line is a total or subtotal, use resultFont style
                if ((strToPrint[0] == '#') || (strToPrint[0] == '='))
                {
                    g.DrawString(strToPrint, resultFont, myBrush, leftMargin, topMargin + (lineSoFar * lineHeight));
                    lineSoFar++;
                }

                // If the line is a ordinary line, use textFont style
                else
                {
                    g.DrawString(lstDisplay.Items[i].ToString(), textFont, myBrush, leftMargin, topMargin + (lineSoFar * lineHeight));
                    lineSoFar++;
                }
            }
        }

        /// <summary> method mnuExit_Click
		/// When user clicks on "Exit" in the menu, closes the program.
        /// If the calculation has been modified, the user will be given a chance to save their modifications before the program is closed.
		/// </summary>
        private void mnuExit_Click(object sender, EventArgs e)
        {
            // Call the method  ModificationChecking() to give user a chance to save their modifications if the calculation 
            // has been modified.
            ModificationChecking();

            this.Close();
        }

        /// <summary> method AddStripForm_FormClosing
        /// When user clicks on "X" in the top right corner of the form, Call the method  ModificationChecking() to give user 
        /// a chance to save their modifications before the form is closed.
        /// </summary>
        private void AddStripForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ModificationChecking();
        }

        /// <summary> method ModificationChecking
        /// Check whether current calculation has been modified. If yes, display a dialog to give user a chance to save the modifications.
        /// </summary>
        private void ModificationChecking()
        {
            if (theCalculation.IsModified)
            {
                DialogResult result = MessageBox.Show("Do you wish to save the existing Calculation before exiting?", "Save existing file?", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    // If user selects to save the modifications, save back to previous file.
                    if (theCalculation.CalcFilename != "")
                    {
                        theCalculation.SaveToFile(theCalculation.CalcFilename);
                    }

                    // If the calculation is a new one, save to a new file.
                    else
                    {
                        if (dlgSaveCalc.ShowDialog() == DialogResult.OK)
                        {
                            if (!theCalculation.SaveToFile(dlgSaveCalc.FileName))
                                MessageBox.Show("There was a problem writing to the file selected, please try another file.", "Problem Writing to file");
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }

        /// <summary> method txtInput_KeyPress
        /// When user presses a key on the keyboard, process the data according to the data entry policies.
        /// </summary>
        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            string inputStr = txtInput.Text;
            int calcLength = txtInput.TextLength;
            validCalcuLine = true;

            // Mark the first calculation
            if (lstDisplay.Items.Count == 0)
            {
                firstLine = true;
            }
            else
            {
                firstLine = false;
            }

            // If the program has just started and no calculations have begun then the Enter key is not allowed.
            if (calcLength == 0)
            {
                if ((e.KeyChar == 13) && firstLine)
                {
                    MessageBox.Show("No caculation available. Enter key is not allowed.", "Empty caculation");
                }
            }

            // If the calculation has begun, perform data validation and entry.
            else
            {
                // If users presses an operator or Enter, perform the data validation.
                if (IsOperator(e.KeyChar) || (e.KeyChar == 13))
                {
                    CalcLineInvalidation(inputStr, calcLength);

                    // If the entry passes data validation, add the entry to a the Calculation.
                    if (validCalcuLine)
                    {
                        AddCalcLine(e.KeyChar);
                    }
                }
            }
        }

        /// <summary> method IsOperator
        /// Return a bool variable indicating whether the character ch is an operator or not.
        /// </summary>
        private bool IsOperator(char ch)
        {
            if ((ch == '+') ||
                (ch == '-') ||
                (ch == '*') ||
                (ch == '/') ||
                (ch == '#') ||
                (ch == '='))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> method IsNumber
        /// Return a bool variable indicating whether the character ch is a number or not.
        /// </summary>
        private bool IsNumber(char ch)
        {
            if ((ch >= 48) && (ch <= 57))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> method CalcLineInvalidation
        /// Validate the string str.
        /// </summary>
        private void CalcLineInvalidation(string str, int strLength)
        {
            // If current calculation is the first calculation, perform the first operator validation, format validation and number validation.
            if (firstLine)
            {
                // Perform total and subtotal validation.
                if (!SumUpValidation(str))
                {
                    validCalcuLine = false;
                }

                // If pass total and subtotal validation, perform first operator validation
                else if (!FirstOpValidation(str))
                {
                    validCalcuLine = false;
                }

                // If pass first operator validation, perform calculation format validation.
                else if (!FormatValidation(str))
                {
                    validCalcuLine = false;
                }

                // If pass format validation, perform number validation at last.
                else
                {
                    NumberValidation(str, strLength);
                }
            }

            // If current calculation is not the first line, only perform format validation and number validation.
            else
            {
                // Perform format validation first.
                if (!FormatValidation(str))
                {
                    validCalcuLine = false;
                }

                // If pass format validation, perform number validation.
                else
                {
                    NumberValidation(str, strLength);
                }
            }
        }

        /// <summary> method SumUpValidation
        /// Return a bool variable to indicate whether the first character of string str is "#" or "=".
        /// </summary>
        private bool SumUpValidation(string str)
        {
            if ((str[0] == '#') || (str[0] == '='))
            {
                MessageBox.Show("No caculation available. So no subtotal or total can be displayed.", "Invalid Request");
                txtInput.Text = "";
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary> method FirstOpValidation
        /// Return a bool variable to indicate whether the first character of string str is "*" or "/".
        /// </summary>
        private bool FirstOpValidation(string str)
        {
            if ((str[0] == '*') || (str[0] == '/'))
            {
                MessageBox.Show("First data entry must use either '+' or '-' as the beginning operator.", "Invalid First Data Entry");
                txtInput.Text = "";
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary> method FormatValidation
        /// Return a bool variable to indicate whether the first character of string str is an operator or a number.
        /// </summary>
        private bool FormatValidation(string str)
        {
            if ((!IsOperator(str[0])) && (!IsNumber(str[0])))
            {
                MessageBox.Show("Incorrect data format.", "Invalid Data Format");
                txtInput.Text = "";
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary> method NumberValidation
        /// Check whether every character after the first character in string str is a number. If any character is not a number, 
        /// mark the validation variable as false and return.
        /// </summary>
        private void NumberValidation(string str, int strLength)
        {
            for (int i = 1; i < strLength; i++)
            {
                if (!IsNumber(str[i]))
                {
                    MessageBox.Show("Please enter a number after the beginning operator.", "Invalid Number Value");
                    validCalcuLine = false;
                    txtInput.Text = "";
                    return;
                }
            }
        }

        /// <summary> method AddCalcLine
        /// Convert the text in the upper textbox into a CalcLine object and add the object to the Calculation object.
        /// </summary>
        private void AddCalcLine(char ch)
        {
            currentCalcLine = new CalcLine(txtInput.Text);
            theCalculation.Add(currentCalcLine);
            txtInput.Text = "";

            // If the character ch is "#" or "=", create a new CalcLine object from ch and add to the Calculation object.
            if ((ch == '#') || (ch == '='))
            {
                currentCalcLine = new CalcLine(ch.ToString());
                theCalculation.Add(currentCalcLine);
            }

            // If the character ch is ENTER, create a new CalcLine object from "=" and add to the Calculation object.
            else if (ch == 13)
            {
                currentCalcLine = new CalcLine("=");
                theCalculation.Add(currentCalcLine);
            }
        }

        /// <summary> method lstDisplay_SelectedIndexChanged
        /// When user clicks on an item in ListBox, display the text in the lower textbox.
        /// </summary>
        private void lstDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = lstDisplay.SelectedIndex;
            string selectedCalc = theCalculation.Find(n).ToString();
            string subText = " ";
            int substringAt = selectedCalc.IndexOf(subText);

            // If there's a space in the selected item, remove the space from the string.
            if (substringAt > 0)
            {
                string part1 = selectedCalc.Substring(0, substringAt);
                string part2 = selectedCalc.Substring(substringAt + subText.Length);
                txtSelect.Text = part1 + part2;
            }
            else
            {
                txtSelect.Text = selectedCalc;
            }
        }

        /// <summary> method btnUpdate_Click
        /// When user clickes on "Update" button, validate the data in the lower textbox and update the Calculation object and the listbox.
        /// </summary>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string selectStr = txtSelect.Text;
            int n = lstDisplay.SelectedIndex;
            validCalcuLine = true;

            // If the selected line in the listbox is the first line, mark the firstline.
            if (n == 0)
            {
                firstLine = true;
            }
            else
            {
                firstLine = false;
            }

            // Validate the data in the lower textbox.
            CalcLineInvalidation(selectStr, selectStr.Length);

            // If the data in the lower textbox is valid, update the Calculation object and the listbox.
            if (validCalcuLine)
            {
                currentCalcLine = new CalcLine(txtSelect.Text);
                theCalculation.Replace(currentCalcLine, n);
                lstDisplay.SelectedIndex = n;
            }
        }

        /// <summary> method btnDelete_Click
        /// When user clickes on "Delete" button, the program will ask if the user wants to delete the selected line and 
        /// if the user responds Yes then the selected line will be removed.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int n = lstDisplay.SelectedIndex;
            if (MessageBox.Show("Are you sure you want to delete this line?", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                theCalculation.Delete(n);
                txtSelect.Text = "";
            }
        }

        /// <summary> method btnInsert_Click
        /// When user clickes on "Insert" button, validate the data in the lower textbox and insert a new line in the calculation 
        /// immediately before the selected line.
        /// </summary>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            string insertStr = txtSelect.Text;
            int n = lstDisplay.SelectedIndex;
            validCalcuLine = true;

            // If the selected line in the listbox is the first line, mark the firstline.
            if (n == 0)
            {
                firstLine = true;
            }
            else
            {
                firstLine = false;
            }

            // Validate the data in the lower textbox.
            CalcLineInvalidation(insertStr, insertStr.Length);

            // If the data in the lower textbox is valid, insert a new line in the calculation immediately before the selected line.
            if (validCalcuLine)
            {
                currentCalcLine = new CalcLine(insertStr);
                theCalculation.Insert(currentCalcLine, n);
                lstDisplay.SelectedIndex = n;
            }
        }
    }
}
