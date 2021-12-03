using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            if (!(Chip.registers.ContainsKey(operand1) || operand1 == "M") || // Check if arg1 is register or M
                !(Chip.registers.ContainsKey(operand2) || operand2 == "M")) // Check if arg2 is register or M
            {
                errorMessage = "ERROR: Invalid MOV operands";
                return;
            }

            string hexAddressString = GetM();
            int hexAddressDecimal = -1;

            if (operand1 == "M" || operand2 == "M")
            {
                if (operand1 == "M" && operand2 == "M")
                    return;

                if (!CheckIfHexAddressValid(hexAddressString))
                {
                    errorMessage = "ERROR: Invalid M hex address";
                    return;
                }

                hexAddressString = hexAddressString[..^1];
                hexAddressDecimal = ConvertHexToDecimal(hexAddressString);

                if (operand1 == "M")
                    Chip.memory[hexAddressDecimal] = Chip.registers.GetValueOrDefault(operand2);
                else
                    Chip.registers[operand1] = Chip.memory[hexAddressDecimal];
            }
            else
                Chip.registers[operand1] = Chip.registers.GetValueOrDefault(operand2);
        }

        private static string GetM()
        {
            string h = Chip.registers["H"];
            string l = Chip.registers["H"];

            if (h.Length == 2 && h.StartsWith("0"))
                h = h[^1..];

            if (l.Length == 2 && l.StartsWith("0"))
                l = l[^1..];

            return h + l + "H";
        }

        public static void MVI_Instr(string text, ref string errorMessage)
        {
            string[] operands = SplitOperands(text, ref errorMessage);
            string operand1 = operands[0];
            string operand2 = operands[1];

            if (errorMessage != string.Empty)
                return;

            if (!Chip.registers.ContainsKey(operand1) || // First operand must be register
                !CheckIfHex(operand2)) // Second operand must be hex
            {
                errorMessage = "ERROR: Invalid MVI operands";
                return;
            }

            Remove_H_FromEndIfExists(operand2);
            AppendZeroToBeginningIfSingleChar(operand2);

            Chip.registers[operand1] = operand2;
        }

        public static string AppendZeroToBeginningIfSingleChar(string str)
        {
            if (str.Length == 1)
                str = "0" + str;

            return str;
        }

        public static string Remove_H_FromEndIfExists(string str)
        {
            if (str.EndsWith("H"))
                str = str[..^1];

            return str;
        }

        public static void LXI_Instr(string text, ref string errorMessage)
        {
            string[] operands = SplitOperands(text, ref errorMessage);
            string operand1 = operands[0] == "M" ? "H" : operands[0];
            string operand2 = operands[1];

            if (errorMessage != string.Empty)
                return;

            string[] hexValues = SplitHexValues(operand2);

            if ((operand1 != "B" && operand1 != "D" && operand1 != "H") || // First operand must be B, D or H
                hexValues.Length == 0) // Error during SplitHexValues
            {
                errorMessage = "ERROR: Invalid LXI operands";
                return;
            }

            string nextRegister = GetNextDictKey(operand1, Chip.registers);

            if (hexValues.Length == 1)
            {
                Chip.registers[operand1] = "00";

                if (hexValues[0].Length == 1)
                    Chip.registers[nextRegister] = "0" + hexValues[0];
                else
                    Chip.registers[nextRegister] = hexValues[0];
            }
            else
            {
                if (hexValues[0].Length == 1)
                    Chip.registers[operand1] = "0" + hexValues[0];
                else
                    Chip.registers[operand1] = hexValues[0];

                if (hexValues[1].Length == 1)
                    Chip.registers[nextRegister] = "0" + hexValues[1];
                else
                    Chip.registers[nextRegister] = hexValues[1];
            }
        }

        public static void LDA_Instr(string text, ref string errorMessage)
        {
            if (!CheckIfHex(text))
            {
                errorMessage = "ERROR: Invalid operand";
                return;
            }

            string hexString = text[..^1];
            int hexAddress = ConvertHexToDecimal(hexString);

            if (hexAddress < 0 || hexAddress > 255)
            {
                errorMessage = "ERROR: Invalid hex address";
                return;
            }

            string hexValueString = Chip.memory[hexAddress];

            Chip.registers["A"] = hexValueString;
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

        private static string[] SplitHexValues(string text)
        {
            if (!CheckIfHex(text))
                return new string[0];

            text = text[..^1];

            string firstHex = string.Empty;
            string secondHex = string.Empty;

            if (text.Length <= 2)
                return new string[] { text };

            return new string[] { text[..2], text[2..] };
        }

        private static bool CheckIfHex(IEnumerable<char> chars)
        {
            if (chars.Last() != 'H' || chars.Count() < 2 || chars.Count() > 5)
                return false;

            bool isHex;

            for (int i = 0; i < chars.Count() - 1; i++)
            {
                char c = chars.ElementAt(i);

                isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!isHex)
                    return false;
            }

            return true;
        }

        private static string GetNextDictKey(string key, Dictionary<string, string> dict)
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
            return int.Parse(hex.ToString(), System.Globalization.NumberStyles.HexNumber);
        }

        private static string ConvertDecimalToHex(int dec)
        {
            return dec.ToString("X");
        }

        private static bool CheckIfHexAddressValid(string hexAddressString)
        {
            if (!CheckIfHex(hexAddressString))
            {
                return false;
            }

            hexAddressString = hexAddressString[..^1];
            int hexAddress = ConvertHexToDecimal(hexAddressString);

            if (hexAddress < 0 || hexAddress > 255)
            {
                return false;
            }

            return true;
        }
    }
}
