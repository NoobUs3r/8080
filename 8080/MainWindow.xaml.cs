using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _8080
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateRegWindows();
            UpdateConBitWindows();
            UpdateProgramCounterWindow();
            UpdateStackPointerWindow();
        }

        DataTable dt;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt = new DataTable("memoryTable");

            string[] columnHeaders = new string[] {"\\", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                                   "A", "B", "C", "D", "E", "F"};

            int cols = columnHeaders.Length;
            int rows = 16;

            for (int i = 0; i < cols; i++)
            {
                DataColumn col = new DataColumn(columnHeaders[i], typeof(string));
                dt.Columns.Add(col);
            }
            
            for (int j = 0; j < rows; j++)
            {
                DataRow dr = dt.NewRow();

                for (int m = 0; m < cols; m++)
                {
                    if (m == 0)
                        dr[m] = j.ToString("X");
                }

                dt.Rows.Add(dr);
            }

            memoryTable.ItemsSource = dt.DefaultView;
            UpdateMemoryWindow();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            ClearButton_Click(sender, e);
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory(CodeBox.Text);
            Chip.programCounter = 0;

            if (parserMessage != "Success")
            {
                MessageBox.Show(parserMessage);
                return;
            }

            while (Chip.programCounter < 65535 )
            {
                string executerMessage = CodeParser.ExecuteFromMemoryOnCounter();

                if (executerMessage != "Success")
                {
                    MessageBox.Show(executerMessage);
                    return;
                }

                if (Chip.memory[Chip.programCounter] == 118) // HLT
                {
                    Chip.programCounter++;
                    break;
                }
            }

            UpdateRegWindows();
            UpdateConBitWindows();
            UpdateProgramCounterWindow();
            UpdateStackPointerWindow();
            UpdateMemoryWindow();
        }

        public void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearRegValues();
            UpdateRegWindows();
            ClearConBitValues();
            UpdateConBitWindows();
            ClearProgramCounterValue();
            UpdateProgramCounterWindow();
            ClearStackPointerValue();
            UpdateStackPointerWindow();
            ClearMemoryValues();
            UpdateMemoryWindow();
            ClearCurrentStepRowValue();
        }

        int currentStepRow = 0;

        public void NextStepButton_Click(object sender, RoutedEventArgs e)
        {
            int totalNumberOfRows = CodeBox.Text.Split("\n").Length;

            if (currentStepRow == totalNumberOfRows || totalNumberOfRows == 0)
                return;

            ExecuteCodeUntilStepRow(sender, e, ++currentStepRow);
        }

        public void PreviousStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentStepRow == 0)
                return;

            ExecuteCodeUntilStepRow(sender, e, --currentStepRow);
        }

        private void ExecuteCodeUntilStepRow(object sender, RoutedEventArgs e, int ogCurrentStepRow)
        {
            string[] rows = CodeBox.Text.Split("\n");
            string codeToExecute = string.Empty;

            for (int i = 0; i < currentStepRow; i++)
            {
                codeToExecute += rows[i];
                codeToExecute += "\n";
            }

            ClearButton_Click(sender, e);
            currentStepRow = ogCurrentStepRow;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory(codeToExecute);
            Chip.programCounter = 0;

            if (parserMessage != "Success")
            {
                MessageBox.Show(parserMessage);
                return;
            }

            for (int i = 0; i < currentStepRow; i++)
            {
                if (rows[i].StartsWith(";"))
                    continue;

                string executerMessage = CodeParser.ExecuteFromMemoryOnCounter();

                if (executerMessage != "Success")
                {
                    MessageBox.Show(executerMessage);
                    return;
                }

                if (Chip.memory[Chip.programCounter] == 118) // HLT
                {
                    Chip.programCounter++;
                    break;
                }
            }

            UpdateRegWindows();
            UpdateConBitWindows();
            UpdateProgramCounterWindow();
            UpdateStackPointerWindow();
            UpdateMemoryWindow();
        }

        private void UpdateRegWindows()
        {
            for (int i = 0; i < Chip.registers.Count; i++)
            {
                string reg = Chip.registers.ElementAt(i).Key;
                string hexValue = Chip.registers[reg].ToString("X");

                if (hexValue.Length > 2)
                    hexValue = hexValue.Substring(hexValue.Length - 2);

                if (reg == "A")
                    regA_Value.Text = hexValue;
                else if (reg == "B")
                    regB_Value.Text = hexValue;
                else if (reg == "C")
                    regC_Value.Text = hexValue;
                else if (reg == "D")
                    regD_Value.Text = hexValue;
                else if (reg == "E")
                    regE_Value.Text = hexValue;
                else if (reg == "H")
                    regH_Value.Text = hexValue;
                else if (reg == "L")
                    regL_Value.Text = hexValue;
            }
        }

        private void UpdateConBitWindows()
        {
            conBitCarry_Value.Text = Chip.conditionalBits["CarryBit"].ToString();
            conBitAuxCarry_Value.Text = Chip.conditionalBits["AuxiliaryCarryBit"].ToString();
            conBitSign_Value.Text = Chip.conditionalBits["SignBit"].ToString();
            conBitZero_Value.Text = Chip.conditionalBits["ZeroBit"].ToString();
            conBitParity_Value.Text = Chip.conditionalBits["ParityBit"].ToString();
        }

        private void UpdateProgramCounterWindow()
        {
            string hexValue = Chip.programCounter.ToString("X");

            if (hexValue.Length > 4)
                hexValue = hexValue.Substring(hexValue.Length - 4);

            programCounter_Value.Text = hexValue;
        }

        private void UpdateStackPointerWindow()
        {
            string hexValue = Chip.stackPointer.ToString("X");

            if (hexValue.Length > 4)
                hexValue = hexValue.Substring(hexValue.Length - 4);

            stackPointer_Value.Text = hexValue;
        }

        private void UpdateMemoryWindow()
        {
            int address = memoryWindowAddressStart;
            int rowHeader = memoryWindowRowHeaderStart;
            int hex10000 = 65536;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    // Last row must be empty
                    if (address == hex10000)
                    {
                        dt.Rows[i][j] = "-";
                        continue;
                    }

                    if (j == 0)
                    {
                        string hexRowHeaderValue = rowHeader.ToString("X");

                        if (hexRowHeaderValue.Length == 1)
                            hexRowHeaderValue = "00" + hexRowHeaderValue;
                        else if (hexRowHeaderValue.Length == 2)
                            hexRowHeaderValue = "0" + hexRowHeaderValue;

                        dt.Rows[i][j] = hexRowHeaderValue;
                        rowHeader++;
                    }
                    else
                    {
                        string hexValue = Chip.memory[address].ToString("X");

                        if (hexValue.Length > 2)
                            hexValue = hexValue.Substring(hexValue.Length - 2);

                        dt.Rows[i][j] = hexValue;
                        address++;
                    }
                }
            }
        }

        private void ClearRegValues()
        {
            Chip.registers["A"] = 0;
            Chip.registers["B"] = 0;
            Chip.registers["C"] = 0;
            Chip.registers["D"] = 0;
            Chip.registers["E"] = 0;
            Chip.registers["H"] = 0;
            Chip.registers["L"] = 0;
        }

        private void ClearConBitValues()
        {
            Chip.conditionalBits["CarryBit"] = false;
            Chip.conditionalBits["AuxiliaryCarryBit"] = false;
            Chip.conditionalBits["SignBit"] = false;
            Chip.conditionalBits["ZeroBit"] = false;
            Chip.conditionalBits["ParityBit"] = false;
        }

        private void ClearProgramCounterValue()
        {
            Chip.programCounter = 0;
        }

        private void ClearStackPointerValue()
        {
            Chip.stackPointer = 65535;
        }

        private void ClearMemoryValues()
        {
            for (int i = 0; i < Chip.memory.Length; i++)
            {
                Chip.memory[i] = 0;
            }
        }

        int memoryWindowAddressStart = 0;
        int memoryWindowRowHeaderStart = 0;

        private void UpdateMemoryRowsButton_Click(object sender, RoutedEventArgs e)
        {
            int hexFF0 = 4080;
            string fullAddress = memoryTableStart_Value.Text;
            string rowAddress = fullAddress;

            if (!Int32.TryParse(fullAddress, System.Globalization.NumberStyles.HexNumber, null, out _))
            {
                MessageBox.Show("ERROR: Invalid memory window address format");
                return;
            }

            // Last char is column
            if (rowAddress.Length > 1)
                rowAddress = rowAddress[0..^1];

            int rowAddress_Int = Int32.Parse(rowAddress, System.Globalization.NumberStyles.HexNumber);
            int fullAddress_Int = Int32.Parse(fullAddress, System.Globalization.NumberStyles.HexNumber);

            if (rowAddress_Int > hexFF0)
            {
                rowAddress_Int = hexFF0;
                memoryTableStart_Value.Text = "FF0";
            }
            else if (rowAddress_Int < 0)
            {
                rowAddress_Int = 0;
                memoryTableStart_Value.Text = "0";
            }

            fullAddress = rowAddress_Int.ToString("X");

            if (!fullAddress.StartsWith("0"))
                fullAddress += "0";

            memoryWindowAddressStart = Int32.Parse(fullAddress, System.Globalization.NumberStyles.HexNumber);
            memoryWindowRowHeaderStart = rowAddress_Int;
            UpdateMemoryWindow();
        }

        private void ClearCurrentStepRowValue()
        {
            currentStepRow = 0;
        }
    }
}
