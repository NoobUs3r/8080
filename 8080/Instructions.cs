using System;
using System.Collections.Generic;
using System.Linq;

namespace _8080
{
    class Instructions
    {
        public static string MOV_Instr(string operand1, string operand2)
        {
            int mAddress = GetM();

            if (operand1 == "M" || operand2 == "M")
            {
                if (operand1 == "M" && operand2 == "M")
                    return "Success";

                if (!IsValueInOneByteRange(mAddress))
                    return "ERROR: Invalid M hex address";

                if (operand1 == "M")
                    Chip.memory[mAddress] = Chip.registers.GetValueOrDefault(operand2);
                else
                    Chip.registers[operand1] = Chip.memory[mAddress];
            }
            else
                Chip.registers[operand1] = Chip.registers.GetValueOrDefault(operand2);

            return "Success";
        }

        public static bool IsOperandValid(string operand)
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

        public static string MVI_Instr(string operand1, int operand2)
        {
            Chip.registers[operand1] = operand2;
            return "Success";
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

        public static string LXI_Instr(string operand1, int highValue, int lowValue)
        {
            string nextRegister = GetNextDictKey(operand1, Chip.registers);

            Chip.registers[operand1] = highValue;
            Chip.registers[nextRegister] = lowValue;
            return "Success";
        }

        public static int[] ExtractHighAndLowValues(int value)
        {
            int high = value / 16;
            int low = value - high * 16;
            return new int[] { high, low };
        }

        public static string LDA_Instr(int address)
        {
            int addressValue = Chip.memory[address];
            Chip.registers["A"] = addressValue;
            return "Success";
        }

        public static string STA_Instr(int address)
        {
            int regA = Chip.registers["A"];
            Chip.memory[address] = regA;
            return "Success";
        }

        public static string LHLD_Instr(int address)
        {
            int nextAddress = address + 1;
            int lowAddressValue = Chip.memory[address];
            int highAddressValue = Chip.memory[nextAddress];

            Chip.registers["L"] = lowAddressValue;
            Chip.registers["H"] = highAddressValue;
            return "Success";
        }

        public static string SHLD_Instr(int address)
        {
            int nextAddress = address + 1;
            int lValue = Chip.registers["L"];
            int hValue = Chip.registers["H"];

            Chip.memory[address] = lValue;
            Chip.memory[nextAddress] = hValue;
            return "Success";
        }

        public static string[] SplitOperands(string text, ref string errorMessage)
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

        public static bool IsValueInOneByteRange(int value)
        {
            if (value < 0 || value > 255)
                return false;

            return true;
        }
    }
}
