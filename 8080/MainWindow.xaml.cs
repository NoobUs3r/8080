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
        }

        DataTable dt;

        private void Window_Loaded(object sender, RoutedEventArgs e)

        {
            dt = new DataTable("memoryTable");

            int cols = 17;
            int rows = 16;
            string[] columnHeaders = new string[] {"\\", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                                   "A", "B", "C", "D", "E", "F"};
            string[] rowHeaders = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                                "A", "B", "C", "D", "E", "F"};

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
                        dr[m] = rowHeaders[j];
                    else
                        dr[m] = " ";
                }

                dt.Rows.Add(dr);
            }

            memoryTable.ItemsSource = dt.DefaultView;
            UpdateMemoryWindow();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            CodeParser.ClearErrorMessage();
            CodeParser.CheckCodeForErrors(CodeBox.Text);

            if (CodeParser.errorMessage != string.Empty)
                MessageBox.Show(CodeParser.errorMessage);

            string instructionMethodMessage = ExecuteInstructionMethod(CodeParser.instruction, CodeParser.operands);
            
            if (instructionMethodMessage != "Success")
                MessageBox.Show(instructionMethodMessage);

            UpdateRegWindows();
            UpdateMemoryWindow();
        }
        public static string ExecuteInstructionMethod(string instr, string text)
        {
            string errorMessage = string.Empty;

            if (instr == "MOV")
            {
                string[] operands = Instructions.SplitOperands(text, ref errorMessage);
                string operand1 = operands[0];
                string operand2 = operands[1];

                if (errorMessage != string.Empty)
                    return errorMessage;
                else if (!Instructions.IsOperandValid(operand1) || !Instructions.IsOperandValid(operand2))
                    return "ERROR: Invalid MOV operands";

                return Instructions.MOV_Instr(operand1, operand2);
            }
            else if (instr == "MVI")
            {
                string[] operands = Instructions.SplitOperands(text, ref errorMessage);
                string operand1 = operands[0];
                string operand2 = operands[1];
                int operand2_Int = Instructions.ConvertValueOperandToDecimal(operand2);

                if (errorMessage != string.Empty)
                    return errorMessage;
                else if (!Instructions.IsValueOperandFormatValid(operand2))
                    return "ERROR: Invalid MVI operand value";
                else if (!Chip.registers.ContainsKey(operand1) || // First operand must be register
                         operand2_Int == -1 || // Second operand must have correct value
                         !Instructions.IsValueInOneByteRange(operand2_Int))
                    return "ERROR: Invalid MVI operands";

                return Instructions.MVI_Instr(operand1, operand2_Int);
            }
            else if (instr == "LXI")
            {
                string[] operands = Instructions.SplitOperands(text, ref errorMessage);
                string operand1 = operands[0] == "M" ? "H" : operands[0];
                string operand2 = operands[1];

                if (errorMessage != string.Empty)
                    return errorMessage;
                else if (!Instructions.IsValueOperandFormatValid(operand2))
                    return "ERROR: Invalid LXI operand format";

                int operand2_Int = Instructions.ConvertValueOperandToDecimal(operand2);
                int[] highAndLowValues = Instructions.ExtractHighAndLowValues(operand2_Int);
                int highValue = highAndLowValues[0];
                int lowValue = highAndLowValues[1];

                if (operand1 != "B" && operand1 != "D" && operand1 != "H" || // First operand must be B, D or H
                    !Instructions.IsValueInOneByteRange(highValue) || !Instructions.IsValueInOneByteRange(lowValue))
                {
                    return "ERROR: Invalid LXI operands";
                }

                return Instructions.LXI_Instr(operand1, highValue, lowValue);
            }
            else if (instr == "LDA")
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return "ERROR: Invalid LDA operand format";

                int address = Instructions.ConvertValueOperandToDecimal(text);

                if (!Instructions.IsValueInOneByteRange(address))
                    return "ERROR: Invalid LDA address";

                return Instructions.LDA_Instr(address);
            }
            else if (instr == "STA")
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return "ERROR: Invalid STA operand format";

                int address = Instructions.ConvertValueOperandToDecimal(text);
                return Instructions.STA_Instr(address);
            }
            else if (instr == "LHLD")
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return "ERROR: Invalid LHLD operand format";

                int address = Instructions.ConvertValueOperandToDecimal(text);
                int maxAddress = Chip.memory.Length - 1;

                if (address == maxAddress - 1)
                    return "ERROR: Invalid LHLD address";

                return Instructions.LHLD_Instr(address);
            }
            else if (instr == "SHLD")
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return "ERROR: Invalid SHLD operand format";

                int address = Instructions.ConvertValueOperandToDecimal(text);
                int maxAddress = Chip.memory.Length - 1;

                if (address == maxAddress - 1)
                    return "ERROR: Invalid SHLD address";

                return Instructions.SHLD_Instr(address);
            }
            else
                return "ERROR: Instruction not found";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearRegValues();
            UpdateRegWindows();
            ClearMemoryValues();
            UpdateMemoryWindow();
        }

        private void UpdateRegWindows()
        {
            regA_Value.Text = Chip.registers["A"].ToString();
            regB_Value.Text = Chip.registers["B"].ToString();
            regC_Value.Text = Chip.registers["C"].ToString();
            regD_Value.Text = Chip.registers["D"].ToString();
            regE_Value.Text = Chip.registers["E"].ToString();
            regH_Value.Text = Chip.registers["H"].ToString();
            regL_Value.Text = Chip.registers["L"].ToString();
        }

        private void UpdateMemoryWindow()
        {
            int m = 0;

            for (int i = 0; i < 16; i++)
            {
                for (int j = 1; j < 17; j++)
                {
                    dt.Rows[i][j] = Chip.memory[m];
                    m++;
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

        private void ClearMemoryValues()
        {
            for (int i = 0; i < Chip.memory.Length; i++)
            {
                Chip.memory[i] = 0;
            }
        }
    }
}
