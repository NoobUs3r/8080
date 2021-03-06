using System;
using System.Data;
using System.Windows;

namespace _8080
{
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
            Chip.ProgramCounter = 0;

            if (parserMessage != "Success")
            {
                MessageBox.Show(parserMessage);
                ClearButton_Click(sender, e);
                return;
            }

            while (Chip.ProgramCounter < 65535 && !Chip.IsHalted)
            {
                if (Chip.IsAddressDefineByte(Chip.ProgramCounter))
                {
                    Chip.ProgramCounter++;
                    continue;
                }

                string executerMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                instructionsToBeExecuted++;

                if (executerMessage != "Success")
                {
                    MessageBox.Show(executerMessage);
                    ClearButton_Click(sender, e);
                    return;
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
            Chip.ProgramCounter = 0;
            UpdateProgramCounterWindow();
            Chip.StackPointer = 65535;
            UpdateStackPointerWindow();
            ClearMemoryValues();
            UpdateMemoryWindow();
            Instructions.ClearLabelMemoryAddress();
            instructionsToBeExecuted = 0;
            areInstructionsWrittenToMemory = false;
            Chip.IsHalted = false;
        }

        int instructionsToBeExecuted = 0;
        bool areInstructionsWrittenToMemory = false;

        public void NextStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (instructionsToBeExecuted == 65535 || Chip.IsHalted)
                return;

            instructionsToBeExecuted++;

            if (!areInstructionsWrittenToMemory)
            {
                areInstructionsWrittenToMemory = true;
                string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory(CodeBox.Text);
                Chip.ProgramCounter = 0;

                if (parserMessage != "Success")
                {
                    MessageBox.Show(parserMessage);
                    ClearButton_Click(sender, e);
                    return;
                }
            }

            ExecuteCodeUntilStepRow(sender, e);
        }

        public void PreviousStepButton_Click(object sender, RoutedEventArgs e)
        {
            int ogInstructionsToBeExecuted = --instructionsToBeExecuted;
            ClearButton_Click(sender, e);
            instructionsToBeExecuted = ogInstructionsToBeExecuted;

            if (instructionsToBeExecuted <= 0)
                return;

            areInstructionsWrittenToMemory = true;
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory(CodeBox.Text);
            Chip.ProgramCounter = 0;

            if (parserMessage != "Success")
            {
                MessageBox.Show(parserMessage);
                ClearButton_Click(sender, e);
                return;
            }

            for (int i = 0; i < instructionsToBeExecuted; i++)
            {
                if (Chip.IsHalted)
                    break;

                ExecuteCodeUntilStepRow(sender, e);
            }
        }

        private void ExecuteCodeUntilStepRow(object sender, RoutedEventArgs e)
        {
            if (Chip.IsAddressDefineByte(Chip.ProgramCounter))
            {
                Chip.ProgramCounter++;
                return;
            }

            string executerMessage = InstrExecuter.ExecuteFromMemoryOnCounter();

            if (executerMessage != "Success")
            {
                MessageBox.Show(executerMessage);
                ClearButton_Click(sender, e);
                return;
            }

            UpdateRegWindows();
            UpdateConBitWindows();
            UpdateProgramCounterWindow();
            UpdateStackPointerWindow();
            UpdateMemoryWindow();
        }

        private void UpdateRegWindows()
        {
            regA_Value.Text = Chip.GetRegister("A").ToString("X");
            regB_Value.Text = Chip.GetRegister("B").ToString("X");
            regC_Value.Text = Chip.GetRegister("C").ToString("X");
            regD_Value.Text = Chip.GetRegister("D").ToString("X");
            regE_Value.Text = Chip.GetRegister("E").ToString("X");
            regH_Value.Text = Chip.GetRegister("H").ToString("X");
            regL_Value.Text = Chip.GetRegister("L").ToString("X");
        }

        private void UpdateConBitWindows()
        {
            conBitCarry_Value.Text = Chip.GetConditionalBit("CarryBit").ToString();
            conBitAuxCarry_Value.Text = Chip.GetConditionalBit("AuxiliaryCarryBit").ToString();
            conBitSign_Value.Text = Chip.GetConditionalBit("SignBit").ToString();
            conBitZero_Value.Text = Chip.GetConditionalBit("ZeroBit").ToString();
            conBitParity_Value.Text = Chip.GetConditionalBit("ParityBit").ToString();
        }

        private void UpdateProgramCounterWindow()
        {
            string hexValue = Chip.ProgramCounter.ToString("X");

            if (hexValue.Length > 4)
                hexValue = hexValue.Substring(hexValue.Length - 4);

            programCounter_Value.Text = hexValue;
        }

        private void UpdateStackPointerWindow()
        {
            string hexValue = Chip.StackPointer.ToString("X");

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
                        string hexValue = Chip.GetMemory(address).ToString("X");

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
            Chip.SetRegister("A", 0);
            Chip.SetRegister("B", 0);
            Chip.SetRegister("C", 0);
            Chip.SetRegister("D", 0);
            Chip.SetRegister("E", 0);
            Chip.SetRegister("H", 0);
            Chip.SetRegister("L", 0);
        }

        private void ClearConBitValues()
        {
            Chip.SetConditionalBit("CarryBit", false);
            Chip.SetConditionalBit("AuxiliaryCarryBit", false);
            Chip.SetConditionalBit("SignBit", false);
            Chip.SetConditionalBit("ZeroBit", false);
            Chip.SetConditionalBit("ParityBit", false);
        }

        private void ClearMemoryValues()
        {
            for (int i = 0; i < 65535; i++)
            {
                Chip.SetMemory(i, 0);
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

        private void CodeSamplesList_DropDownClosed(object sender, EventArgs e)
        {
            string sampleName = CodeSamplesList.Text;
            CodeBox.Text = CodeSamples.GetCodeSample(sampleName);
            ClearButton_Click(sender, new RoutedEventArgs());
        }
    }
}
