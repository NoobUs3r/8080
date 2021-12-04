using System;
using System.Collections.Generic;
using System.Linq;

namespace _8080
{
    class Instructions
    {
        public static void MOV_Instr(string text, ref string errorMessage)
        {
            string[] operands = SplitOperands(text, ref errorMessage);
            string operand1 = operands[0];
            string operand2 = operands[1];

            if (errorMessage != string.Empty)
                return;

            if (!IsOperandValid(operand1) || !IsOperandValid(operand2)) 
            {
                errorMessage = "ERROR: Invalid MOV operands";
                return;
            }

            int mAddress = GetM();

            if (operand1 == "M" || operand2 == "M")
            {
                if (operand1 == "M" && operand2 == "M")
                    return;

                if (!IsAddressValid(mAddress))
                {
                    errorMessage = "ERROR: Invalid M hex address";
                    return;
                }

                if (operand1 == "M")
                    Chip.memory[mAddress] = Chip.registers.GetValueOrDefault(operand2);
                else
                    Chip.registers[operand1] = Chip.memory[mAddress];
            }
            else
                Chip.registers[operand1] = Chip.registers.GetValueOrDefault(operand2);
        }

        private static bool IsOperandValid(string operand)
        {
            if (Chip.registers.ContainsKey(operand) || operand == "M")
                return true;
            
            return false;
        }

        private static int GetM()
        {
            int h = Chip.registers["H"] * 16;
            int l = Chip.registers["H"];
            return h + l;
        }

        public static void MVI_Instr(string text, ref string errorMessage)
        {
            string[] operands = SplitOperands(text, ref errorMessage);
            string operand1 = operands[0];
            string operand2 = operands[1];

            if (errorMessage != string.Empty)
                return;

            if (!IsValueOperandFormatValid(operand2))
            {
                errorMessage = "ERROR: Invalid MVI operand value";
                return;
            }

            int operand2_Int = ConvertValueOperandToDecimal(operand2);

            if (!Chip.registers.ContainsKey(operand1) || // First operand must be register
                operand2_Int == -1 || // Second operand must have correct value
                !IsAddressValid(operand2_Int))
            {
                errorMessage = "ERROR: Invalid MVI operands";
                return;
            }

            Chip.registers[operand1] = operand2_Int;
        }

        public static bool IsValueOperandFormatValid(string operand)
        {
            if (operand.EndsWith("H"))
            {
                if (operand.StartsWith("0"))
                    return true;

                return false;
            }
            // Add other checks for bites, decimals etc
            else if (operand.EndsWith("D"))
            {
                operand = operand[..^1];

                if (Int32.TryParse(operand, out int value))
                    return true;

                return false;
            }
            else if (Int32.TryParse(operand, out int value))
                return true;
            else
                return false;
        }

        public static int ConvertValueOperandToDecimal(string operand)
        {
            if (operand.EndsWith("H"))
                return ConvertHexToDecimal(operand);
            else if (operand.EndsWith("D"))
            {
                operand = operand[..^1];

                if (Int32.TryParse(operand, out int value))
                    return value;

                return -1;
            }
            else if (Int32.TryParse(operand, out int value))
                return value; // Add other formats like bites etc
            else
                return -1;
        }

        public static void LXI_Instr(string text, ref string errorMessage)
        {
            string[] operands = SplitOperands(text, ref errorMessage);
            string operand1 = operands[0] == "M" ? "H" : operands[0];
            string operand2 = operands[1];

            if (errorMessage != string.Empty)
                return;

            if (!IsValueOperandFormatValid(operand2))
            {
                errorMessage = "ERROR: Invalid LXI operand format";
                return;
            }

            int operand2_Int = ConvertValueOperandToDecimal(operand2);
            int[] highAndLowValues = ExtractHighAndLowValues(operand2_Int);
            int highValue = highAndLowValues[0];
            int lowValue = highAndLowValues[1];

            if (operand1 != "B" && operand1 != "D" && operand1 != "H") // First operand must be B, D or H
            {
                errorMessage = "ERROR: Invalid LXI operands";
                return;
            }

            string nextRegister = GetNextDictKey(operand1, Chip.registers);

            Chip.registers[operand1] = highValue;
            Chip.registers[nextRegister] = lowValue;
        }

        public static int[] ExtractHighAndLowValues(int value)
        {
            int high = value / 16;
            int low = value - high * 16;
            return new int[] { high, low };
        }

        public static void LDA_Instr(string text, ref string errorMessage)
        {
            if (!IsValueOperandFormatValid(text))
            {
                errorMessage = "ERROR: Invalid LDA operand format";
                return;
            }

            int address = ConvertValueOperandToDecimal(text);

            if (!IsAddressValid(address))
            {
                errorMessage = "ERROR: Invalid LDA address";
                return;
            }

            int addressValue = Chip.memory[address];
            Chip.registers["A"] = addressValue;
        }

        private static string[] SplitOperands(string text, ref string errorMessage)
        {
            string[] operands = text.Split(",");

            if (operands.Length != 2)
            {
                errorMessage = "ERROR: Invalid number of operands";
                return new string[] { "", "" };
            }

            string operand1 = operands[0];
            string operand2 = operands[1];

            operand1 = CodeParser.RemoveSpacesFromBeginning(operand1);
            operand1 = CodeParser.RemoveSpacesFromEnd(operand1);
            operand2 = CodeParser.RemoveSpacesFromBeginning(operand2);
            operand2 = CodeParser.RemoveSpacesFromEnd(operand2);

            return new string[] { operand1, operand2 };
        }

        private static string GetNextDictKey(string key, Dictionary<string, int> dict)
        {
            for (int i = 0; i < dict.Count; i++)
            {
                if (dict.ElementAt(i).Key == key && i != dict.Count - 1)
                    return dict.ElementAt(++i).Key;
            }

            return dict.ElementAt(0).Key;
        }

        private static int ConvertHexToDecimal(string hex)
        {
            if (Int32.TryParse(hex.ToString(), System.Globalization.NumberStyles.HexNumber, null, out int value))
                return value;

            return -1;
        }

        private static bool IsAddressValid(int address)
        {
            if (address < 0 || address > 255)
                return false;

            return true;
        }
    }
}
