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
                    return "ERROR: Invalid MOV operands";

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
            return ConcatRegisters("H", "L");
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

            int ogRegA = Chip.registers["A"];
            Chip.registers["A"] += value;

            if (!IsValueInOneByteRange(Chip.registers["A"]))
            {
                if (Chip.registers["A"] < 0)
                    Chip.registers["A"] += 256; // Registers can't hold negative numbers
                else
                    Chip.registers["A"] -= 256;
            }

            SetConditionalBits("ADD", ogRegA, value, Chip.registers["A"]);
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

            int ogRegA = Chip.registers["A"];
            Chip.registers["A"] += value;

            if (!IsValueInOneByteRange(Chip.registers["A"]))
            {
                if (Chip.registers["A"] < 0)
                    Chip.registers["A"] += 256; // Registers can't hold negative numbers
                else
                    Chip.registers["A"] -= 256;
            }

            SetConditionalBits("ADD", ogRegA, value, Chip.registers["A"]);
            return "Success";
        }

        public static string INR_Instr(string reg)
        {
            bool ogCarryBit = Chip.conditionalBits["CarryBit"];

            if (reg == "M")
            {
                int mAddress = GetM();
                int ogMemoryValue = Chip.memory[mAddress];

                if (Chip.memory[mAddress] == 255)
                    Chip.memory[mAddress] = 0;
                else
                    Chip.memory[mAddress]++;

                SetConditionalBits("ADD", ogMemoryValue, 1, Chip.memory[mAddress]);
            }
            else
            {
                int ogRegValue = Chip.registers[reg];

                if (Chip.registers[reg] == 255)
                    Chip.registers[reg] = 0;
                else
                    Chip.registers[reg]++;

                SetConditionalBits("ADD", ogRegValue, 1, Chip.registers[reg]);
            }

            Chip.conditionalBits["CarryBit"] = ogCarryBit; // Not affected by this instr
            return "Success";
        }
        public static string DCR_Instr(string reg)
        {
            bool ogCarryBit = Chip.conditionalBits["CarryBit"];

            if (reg == "M")
            {
                int mAddress = GetM();
                int ogMemoryValue = Chip.memory[mAddress];

                if (Chip.memory[mAddress] == 0)
                    Chip.memory[mAddress] = 255;
                else
                    Chip.memory[mAddress]--;

                SetConditionalBits("SUB", ogMemoryValue, 1, Chip.memory[mAddress]);
            }
            else
            {
                int ogRegValue = Chip.registers[reg];

                if (Chip.registers[reg] == 0)
                    Chip.registers[reg] = 255;
                else
                    Chip.registers[reg]--;

                SetConditionalBits("SUB", ogRegValue, 1, Chip.registers[reg]);
            }

            Chip.conditionalBits["CarryBit"] = ogCarryBit; // Not affected by this instr
            return "Success";
        }

        public static string DAA_Instr()
        {
            BitArray regABits = ConvertIntTo8BitArray(Chip.registers["A"]);
            BitArray regA4LowerBits = Get4LowerBits(regABits);
            int regA4LowerBitsInt = ConvertBitArrayToInt(regA4LowerBits);
            int ogRegA = Chip.registers["A"];

            if (regA4LowerBitsInt > 9 || Chip.conditionalBits["AuxiliaryCarryBit"])
            {
                Chip.registers["A"] += 6;
                SetConditionalBits("ADD", ogRegA, 6, Chip.registers["A"]);
            }

            BitArray regABitsUpdated = ConvertIntTo8BitArray(Chip.registers["A"]);
            BitArray regA4UpperBits = Get4UpperBits(regABitsUpdated);
            int regA4UpperBitsInt = ConvertBitArrayToInt(regA4UpperBits);

            if (regA4UpperBitsInt > 9 || Chip.conditionalBits["CarryBit"])
            {
                int ogRegA2 = Chip.registers["A"];
                int sixToBeAddedTo4UpperBits = 96; // 01100000
                bool ogAuxiliaryCarryBit = Chip.conditionalBits["AuxiliaryCarryBit"];
                Chip.registers["A"] += sixToBeAddedTo4UpperBits;

                if (!IsValueInOneByteRange(Chip.registers["A"]))
                {
                    if (Chip.registers["A"] < 0)
                        Chip.registers["A"] += 256; // Registers can't hold negative numbers
                    else
                        Chip.registers["A"] -= 256;
                }

                SetConditionalBits("ADD", ogRegA2, sixToBeAddedTo4UpperBits, Chip.registers["A"]);
                Chip.conditionalBits["AuxiliaryCarryBit"] = ogAuxiliaryCarryBit; // AuxiliarCarry since we set it above
            }

            return "Success";
        }

        public static string STAX_Instr(string reg)
        {
            string regNext = GetNextDictKey(reg, Chip.registers);
            int address = ConcatRegisters(reg, regNext);
            Chip.memory[address] = Chip.registers["A"];
            return "Success";
        }

        public static string LDAX_Instr(string reg)
        {
            string regNext = GetNextDictKey(reg, Chip.registers);
            int address = ConcatRegisters(reg, regNext);
            Chip.registers["A"] = Chip.memory[address];
            return "Success";
        }

        public static string SUB_Instr(string operand)
        {
            int value = -1;

            if (Chip.registers.ContainsKey(operand))
                value = Chip.registers[operand];
            else
            {
                int mAddress = GetM();
                value = Chip.memory[mAddress];
            }

            int ogRegA = Chip.registers["A"];
            Chip.registers["A"] -= value;

            if (!IsValueInOneByteRange(Chip.registers["A"]))
            {
                if (Chip.registers["A"] < 0)
                    Chip.registers["A"] += 256; // Registers can't hold negative numbers
                else
                    Chip.registers["A"] -= 256;
            }

            SetConditionalBits("SUB", ogRegA, value, Chip.registers["A"]);
            return "Success";
        }

        public static string ANA_Instr(string reg)
        {
            int value = -1;

            if (Chip.registers.ContainsKey(reg))
                value = Chip.registers[reg];
            else
            {
                int mAddress = GetM();
                value = Chip.memory[mAddress];
            }

            BitArray regAArray = ConvertIntTo8BitArray(Chip.registers["A"]);
            BitArray valueArray = ConvertIntTo8BitArray(value);
            BitArray andResult = LogicAndBitArrays(regAArray, valueArray);
            int andResultInt = ConvertBitArrayToInt(andResult);
            int ogRegA = Chip.registers["A"];

            Chip.registers["A"] = andResultInt;
            SetConditionalBits("AND", ogRegA, value, Chip.registers["A"]);
            return "Success";
        }

        public static string XRA_Instr(string reg)
        {
            int value = -1;

            if (Chip.registers.ContainsKey(reg))
                value = Chip.registers[reg];
            else
            {
                int mAddress = GetM();
                value = Chip.memory[mAddress];
            }

            BitArray regAArray = ConvertIntTo8BitArray(Chip.registers["A"]);
            BitArray valueArray = ConvertIntTo8BitArray(value);
            BitArray andResult = LogicXORBitArrays(regAArray, valueArray);
            int andResultInt = ConvertBitArrayToInt(andResult);
            int ogRegA = Chip.registers["A"];

            Chip.registers["A"] = andResultInt;
            SetConditionalBits("XOR", ogRegA, value, Chip.registers["A"]);
            return "Success";
        }

        public static string ORA_Instr(string reg)
        {
            int value = -1;

            if (Chip.registers.ContainsKey(reg))
                value = Chip.registers[reg];
            else
            {
                int mAddress = GetM();
                value = Chip.memory[mAddress];
            }

            BitArray regAArray = ConvertIntTo8BitArray(Chip.registers["A"]);
            BitArray valueArray = ConvertIntTo8BitArray(value);
            BitArray andResult = LogicORBitArrays(regAArray, valueArray);
            int andResultInt = ConvertBitArrayToInt(andResult);
            int ogRegA = Chip.registers["A"];

            Chip.registers["A"] = andResultInt;
            SetConditionalBits("OR", ogRegA, value, Chip.registers["A"]);
            return "Success";
        }

        public static string CMP_Instr(string reg)
        {
            int ogRegA = Chip.registers["A"];
            int value = Chip.registers[reg];
            SUB_Instr(reg);
            Chip.registers["A"] = ogRegA;
            SetConditionalBits("SUB", ogRegA, value, Chip.registers["A"]);
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

            SetConditionalCarryBit(operation, operand1Bits, operand2Bits);
            SetConditionalAuxiliaryCarryBit(operation, operand1Bits, operand2Bits);
            SetConditionalSignBit(resultBits);
            SetConditionalParityBit(resultBits);
            SetConditionalZeroBit(result);
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

            if (isNegative)
                return ConvertBitArrayToTwosComplement(eightBitArray);

            return eightBitArray;
        }

        private static BitArray Get4LowerBits(BitArray array)
        {
            BitArray fourBitArray = new BitArray(4);

            for (int i = 0; i < 4; i++)
            {
                fourBitArray.Set(i, array[i]);
            }

            return fourBitArray;
        }

        private static BitArray Get4UpperBits(BitArray array)
        {
            BitArray fourBitArray = new BitArray(4);

            for (int i = 0; i < 4; i++)
            {
                fourBitArray.Set(i, array[i + 4]);
            }

            return fourBitArray;
        }

        private static void SetConditionalSignBit(BitArray array)
        {
            Chip.conditionalBits["SignBit"] = array[7];
        }

        private static void SetConditionalAuxiliaryCarryBit(string operation, BitArray regAArray, BitArray array)
        {
            BitArray regAFourBitArray = Get4LowerBits(regAArray);
            BitArray fourBitArray2 = Get4LowerBits(array);

            if (operation == "ADD")
            {
                bool carry = false;

                for (int i = 0; i < 4; i++)
                {
                    if ((regAFourBitArray[i] == true && fourBitArray2[i] == true) ||
                        ((regAFourBitArray[i] == true || fourBitArray2[i] == true) && carry))
                        carry = true;
                    else carry = false;
                }

                Chip.conditionalBits["AuxiliaryCarryBit"] = carry;
            }
            else if (operation == "SUB")
            {
                BitArray fourBitArray2TwosComplement = ConvertBitArrayToTwosComplement(fourBitArray2);
                bool carry = false;

                for (int i = 0; i < 4; i++)
                {
                    if ((regAFourBitArray[i] == true && fourBitArray2TwosComplement[i] == true) ||
                        ((regAFourBitArray[i] == true || fourBitArray2TwosComplement[i] == true) && carry))
                        carry = true;
                    else carry = false;
                }

                Chip.conditionalBits["AuxiliaryCarryBit"] = carry;
            }
            else if (operation == "ADD" || operation == "XOR" || operation == "OR")
                Chip.conditionalBits["AuxiliaryCarryBit"] = false;
            // Add other operations
        }

        private static void SetConditionalCarryBit(string operation, BitArray regAArray, BitArray array2)
        {
            if (operation == "ADD")
            {
                bool carry = false;

                for (int i = 0; i < 8; i++)
                {
                    if ((regAArray[i] == true && array2[i] == true) ||
                        ((regAArray[i] == true || array2[i] == true) && carry))
                        carry = true;
                    else carry = false;
                }

                Chip.conditionalBits["CarryBit"] = carry;
            }
            else if (operation == "SUB")
            {
                BitArray array2TwosComplement = ConvertBitArrayToTwosComplement(array2);
                bool carry = false;

                for (int i = 0; i < 8; i++)
                {
                    if ((regAArray[i] == true && array2TwosComplement[i] == true) ||
                        ((regAArray[i] == true || array2TwosComplement[i] == true) && carry))
                        carry = true;
                    else carry = false;
                }

                Chip.conditionalBits["CarryBit"] = !carry;
            }
            else if (operation == "ADD" || operation == "XOR" || operation == "OR")
                Chip.conditionalBits["CarryBit"] = false;

            // Add other operations
        }

        private static void SetConditionalZeroBit(int value)
        {
            Chip.conditionalBits["ZeroBit"] = value == 0 || value == 256; // 256 is 11111111 + 1, should result in 0
        }

        private static void SetConditionalParityBit(BitArray array)
        {
            int count = 0;

            for (int i = 0; i < 8; i++)
            {
                if (array[i] == true)
                    count++;
            }

            Chip.conditionalBits["ParityBit"] = count % 2 == 0;
        }

        public static int ConvertBitArrayToInt(BitArray bitArray)
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

        private static BitArray ConvertBitArrayToTwosComplement(BitArray array)
        {
            return ConvertBitArrayToOnesComplement(array, true);
        }

        public static BitArray ConvertBitArrayToOnesComplement(BitArray array, bool carry = false)
        {
            BitArray twosComplement = new BitArray(array.Length);

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

        private static BitArray ConcatBitArrays(BitArray upperBitsArray, BitArray lowerBitsArray)
        {
            BitArray concatArray = new BitArray(upperBitsArray.Length + lowerBitsArray.Length);

            for (int i = 0; i < concatArray.Length; i++)
            {
                if (i <= lowerBitsArray.Length)
                    concatArray[i] = upperBitsArray[i];
                else
                    concatArray[i] = lowerBitsArray[i];
            }

            return concatArray;
        }

        private static int ConcatRegisters(string regUpperBits, string regLowerBits)
        {
            int upperBits = Chip.registers[regUpperBits] * 16;
            int lowerBits = Chip.registers[regLowerBits];
            return upperBits + lowerBits;
        }

        public static BitArray LogicAndBitArrays(BitArray array1, BitArray array2)
        {
            BitArray fin = new BitArray(array1.Length);

            for (int i = 0; i < fin.Length; i++)
            {
                if (array1[i] && array2[i])
                    fin[i] = true;
            }

            return fin;
        }

        public static BitArray LogicXORBitArrays(BitArray array1, BitArray array2)
        {
            BitArray fin = new BitArray(array1.Length);

            for (int i = 0; i < fin.Length; i++)
            {
                if (array1[i] != array2[i])
                    fin[i] = true;
            }

            return fin;
        }

        public static BitArray LogicORBitArrays(BitArray array1, BitArray array2)
        {
            BitArray fin = new BitArray(array1.Length);

            for (int i = 0; i < fin.Length; i++)
            {
                if (array1[i] || array2[i])
                    fin[i] = true;
            }

            return fin;
        }
    }
}
