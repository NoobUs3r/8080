using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

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

        public static int GetM()
        {
            int h = Chip.registers["H"] * 16;
            int l = Chip.registers["L"];
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
                return ConvertHexToDecimal(operand[..^1]);
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

        public static string XCHG_Instr()
        {
            int tempRegD = Chip.registers["D"];
            int tempRegE = Chip.registers["E"];

            Chip.registers["D"] = Chip.registers["H"];
            Chip.registers["E"] = Chip.registers["L"];
            Chip.registers["H"] = tempRegD;
            Chip.registers["L"] = tempRegE;
            return "Success";
        }

        public static string ADD_Instr(string operand)
        {
            int value = -1;

            if (Chip.registers.ContainsKey(operand))
                value = Chip.registers[operand];
            else
            {
                int mAddress = GetM();
                value = Chip.memory[mAddress];
            }

            int result = Chip.registers["A"] + value;

            if (!IsValueInOneByteRange(result))
            {
                if (result < 0)
                    result = 0; // Registers can't hold negative numbers
                else
                    result = 255;
            }

            SetConditionalBits("ADD", Chip.registers["A"], value, result);
            Chip.registers["A"] = result;
            return "Success";
        }

        public static string ADI_Instr(int value)
        {
            if (!IsValueInOneByteRangeTwosComplement(value))
            {
                if (value < 0)
                    value = -128;
                else
                    value = 127;
            }

            int result = Chip.registers["A"] + value;

            if (!IsValueInOneByteRange(result))
            {
                if (result < 0)
                    result = 0; // Registers can't hold negative numbers
                else
                    result = 255;
            }

            SetConditionalBits("ADD", Chip.registers["A"], value, result);
            Chip.registers["A"] = result;
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

        public static bool IsValueInOneByteRangeTwosComplement(int value)
        {
            if (value < -128 || value > 127)
                return false;

            return true;
        }

        private static void SetConditionalBits(string operation, int operand1, int operand2, int result)
        {
            BitArray operand1Bits = ConvertIntTo8BitArray(operand1);
            BitArray operand2Bits = ConvertIntTo8BitArray(operand2);
            BitArray resultBits = ConvertIntTo8BitArray(result);

            /*if (BitConverter.IsLittleEndian)
            {
                ReverseBitArray(operand1Bits);
                ReverseBitArray(operand2Bits);
                ReverseBitArray(resultBits);
            }*/

            SetConditionalSignBit(resultBits);
            SetConditionalAuxiliaryCarryBit(operation, operand1Bits, operand2Bits);
            SetConditionalCarryBit(operation, operand1Bits, operand2Bits);
            SetConditionalZeroBit(operation, operand1, operand2);
            SetConditionalParityBit(resultBits);
        }

        public static BitArray ConvertIntTo8BitArray(int value)
        {
            bool isNegative = false;

            if (value < 0)
            {
                isNegative = true;
                value *= -1;
            }

            byte[] numberAsByte = new byte[] { (byte)value };
            BitArray valueArray = new BitArray(numberAsByte);
            BitArray eightBitArray = new BitArray(8);
            CopyBitArrayToBitArray(valueArray, ref eightBitArray);

            /*if (valueArray.Length < 8)
            {
                int bitsToAdd = 8 - valueArray.Length;
                int j = 0;

                for (int i = bitsToAdd; i < 8; i++)
                {
                    eightBitArray.Set(i, valueArray[j]);
                    j++;
                }

                return eightBitArray;
            }
            else
                valueArray.CopyTo(eightBitArray, 0);*/

            if (isNegative)
                return Convert8BitArrayToTwosComplement(eightBitArray);

            return eightBitArray;
        }

        private static BitArray Get4LowerBits(BitArray array)
        {
            BitArray fourBitArray = new BitArray(4);

            for (int i = 0; i < 4; i++)
            {
                fourBitArray.Set(i, array[i + 4]);
            }

            return fourBitArray;
        }

        private static void SetConditionalSignBit(BitArray value)
        {
            Chip.conditionalBits["SignBit"] = value[7];
        }

        private static void SetConditionalAuxiliaryCarryBit(string operation, BitArray array1, BitArray array2)
        {
            BitArray fourBitArray1 = Get4LowerBits(array1);
            BitArray fourBitArray2 = Get4LowerBits(array2);

            if (operation == "ADD")
            {
                bool carry = false;

                for (int i = 0; i < 4; i++)
                {
                    if ((fourBitArray1[i] == true && fourBitArray2[i] == true) ||
                        ((fourBitArray1[i] == true || fourBitArray2[i] == true) && carry))
                        carry = true;
                    else carry = false;
                }

                Chip.conditionalBits["AuxiliaryCarryBit"] = carry;
            }

            // Add other operations
        }

        private static void SetConditionalCarryBit(string operation, BitArray array1, BitArray array2)
        {
            if (operation == "ADD")
            {
                bool carry = false;

                for (int i = 0; i < 8; i++)
                {
                    if ((array1[i] == true && array2[i] == true) ||
                        ((array1[i] == true || array2[i] == true) && carry))
                        carry = true;
                    else carry = false;
                }

                Chip.conditionalBits["CarryBit"] = carry;
            }
            // Add other operations
        }

        private static void SetConditionalZeroBit(string operation, int operand1, int operand2)
        {
            if (operation == "ADD")
            {
                int result = operand1 + operand2;
                Chip.conditionalBits["ZeroBit"] = result == 0 || result == 256; // 256 is 11111111 + 1, should result in 0
            }
            // Add other operations
        }

        private static void SetConditionalParityBit(BitArray array)
        {
            int count = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == true)
                    count++;
            }

            Chip.conditionalBits["ParityBit"] = count % 2 == 0;
        }

        private static int GetIntFromBitArray(BitArray bitArray)
        {
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        public static void ReverseBitArray(BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }

        public static string ConvertIntTo8BinaryString(int value)
        {
            bool isNegative = false;

            if (value < 0)
            {
                isNegative = true;
                value *= -1;
            }

            int from = 10;
            int to = 2;
            string val = value.ToString();
            string binary = Convert.ToString(Convert.ToInt32(val, from), to);

            while (binary.Length < 8)
                binary = "0" + binary;

            if (isNegative)
                binary = Convert8BitStringToTwosComplement(binary);

            return binary;
        }

        private static string Convert8BitStringToTwosComplement(string str)
        {
            string twosComplement = string.Empty;
            bool carry = true;

            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] == '0')
                {
                    if (carry)
                        twosComplement = "0" + twosComplement;
                    else
                    {
                        twosComplement = "1" + twosComplement;
                        carry = false;
                    }
                }
                else
                {
                    if (carry)
                    {
                        twosComplement = "1" + twosComplement;
                        carry = false;
                    }
                    else
                        twosComplement = "0" + twosComplement;
                }
            }

            return twosComplement;
        }

        private static BitArray Convert8BitArrayToTwosComplement(BitArray array)
        {
            BitArray twosComplement = new BitArray(8);
            bool carry = true;

            for (int i = 0; i < twosComplement.Length; i++)
            {
                if (array[i] == false)
                {
                    if (carry)
                        twosComplement[i] = false;
                    else
                    {
                        twosComplement[i] = true;
                        carry = false;
                    }
                }
                else
                {
                    if (carry)
                    {
                        twosComplement[i] = true;
                        carry = false;
                    }
                    else
                        twosComplement[i] = false;
                }
            }

            return twosComplement;
        }

        private static void CopyBitArrayToBitArray(BitArray copyFromArray, ref BitArray copyToArray)
        {
            for (int i = 0; i < copyFromArray.Length; i++)
            {
                copyToArray[i] = copyFromArray[i];
            }
        }
    }
}
