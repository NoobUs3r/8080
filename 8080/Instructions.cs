using System;
using System.Collections.Generic;
using System.Collections;

namespace _8080
{
    public class Instructions
    {
        private static Dictionary<string, string> opLabPartOrFullCode = new Dictionary<string, string>
        {
            { "CMC", "00111111" },
            { "STC", "00110111" },
            { "CMA", "00101111" },
            { "DAA", "00100111" },
            { "NOP", "00000000" },
            { "ADI", "11000110" },
            { "ACI", "11001110" },
            { "SUI", "11010110" },
            { "SBI", "11011110" },
            { "ANI", "11100110" },
            { "XRI", "11101110" },
            { "ORI", "11110110" },
            { "CPI", "11111110" },
            { "HLT", "01110110" },
            { "XCHG", "11101011" },
            { "XTHL", "11100011" },
            { "SPHL", "11111001" },
            { "PCHL", "11101001" },
            { "STAX", "0010" },
            { "LDAX", "1010" },
            { "PUSH", "0101" },
            { "POP", "0001" },
            { "DAD", "1001" },
            { "INX", "0011" },
            { "DCX", "1011" },
            { "INR", "100" },
            { "DCR", "101" },
            { "ADD", "000" },
            { "ADC", "001" },
            { "SUB", "010" },
            { "SBB", "011" },
            { "ANA", "100" },
            { "XRA", "101" },
            { "ORA", "110" },
            { "CMP", "111" },
            { "SHLD", "00" },
            { "LHLD", "01" },
            { "JMP", "000" },
            { "JNC", "010" },
            { "JNZ", "000" },
            { "JPE", "101" },
            { "JPO", "100" },
            { "JC", "011" },
            { "JZ", "001" },
            { "JM", "111" },
            { "JP", "110" },
            { "CALL", "001" },
            { "CNC", "010" },
            { "CNZ", "000" },
            { "CPE", "101" },
            { "CPO", "100" },
            { "CC", "011" },
            { "CZ", "001" },
            { "CM", "111" },
            { "CP", "110" },
            { "RET", "001" },
            { "RNC", "010" },
            { "RNZ", "000" },
            { "RPE", "101" },
            { "RPO", "100" },
            { "RC", "011" },
            { "RZ", "001" },
            { "RM", "111" },
            { "RP", "110" },
            { "RLC", "00" },
            { "RRC", "01" },
            { "RAL", "10" },
            { "RAR", "11" },
            { "STA", "10" },
            { "LDA", "11" },
            { "MOV", "" },
            { "MVI", "" },
            { "LXI", "" },
            { "DB", "" },
            { "DW", "" },
            { "DS", "" }
        };

        public static bool DoesInstructionExist(string instr)
        {
            return opLabPartOrFullCode.ContainsKey(instr);
        }

        public static string GetOpLabPartOrFullCode(string opLab)
        {
            return opLabPartOrFullCode[opLab];
        }

        public static bool IsOpLabNoOperand(string opLab)
        {
            if (opLab == "CMC" || opLab == "STC" || opLab == "CMA" || opLab == "DAA" || opLab == "NOP" ||
                opLab == "XCHG" || opLab == "XTHL" || opLab == "SPHL" || opLab == "PCHL" || opLab == "HLT")
                return true;

            return false;
        }

        public static bool IsOpLabRegOrMemToAcc(string opLab)
        {
            if (opLab == "ADD" || opLab == "ADC" || opLab == "SUB" || opLab == "SBB" || opLab == "ANA" ||
                opLab == "XRA" || opLab == "ORA" || opLab == "CMP")
                return true;

            return false;
        }

        public static bool IsOpLabRotateAcc(string opLab)
        {
            if (opLab == "RLC" || opLab == "RRC" || opLab == "RAL" || opLab == "RAR")
                return true;

            return false;
        }

        public static bool IsOpLabImmediateInstr(string opLab)
        {
            if (opLab == "ADI" || opLab == "ACI" || opLab == "SUI" || opLab == "SBI" ||
                opLab == "ANI" || opLab == "XRI" || opLab == "ORI" || opLab == "CPI")
                return true;

            return false;
        }

        public static bool IsOpLabDirectAddressing(string opLab)
        {
            if (opLab == "STA" || opLab == "LDA" || opLab == "SHLD" || opLab == "LHLD")
                return true;

            return false;
        }

        public static bool IsOpLabJump(string opLab)
        {
            if (opLab == "JMP" || opLab == "JC" || opLab == "JNC" || opLab == "JZ" || opLab == "JNZ" ||
                opLab == "JM" || opLab == "JP" || opLab == "JPE" || opLab == "JPO")
                return true;

            return false;
        }

        public static bool IsOpLabCall(string opLab)
        {
            if (opLab == "CALL" || opLab == "CC" || opLab == "CNC" || opLab == "CZ" || opLab == "CNZ" ||
                opLab == "CM" || opLab == "CP" || opLab == "CPE" || opLab == "CPO")
                return true;

            return false;
        }

        public static bool IsOpLabReturn(string opLab)
        {
            if (opLab == "RET" || opLab == "RC" || opLab == "RNC" || opLab == "RZ" || opLab == "RNZ" ||
                opLab == "RM" || opLab == "RP" || opLab == "RPE" || opLab == "RPO")
                return true;

            return false;
        }

        private static Dictionary<string, int> labelMemoryAddress = new Dictionary<string, int>();

        public static string SetLabelMemoryAddress(string label, int memoryAddress)
        {
            if (labelMemoryAddress.ContainsKey(label))
                return "ERROR: Label already exists";

            labelMemoryAddress.Add(label, memoryAddress);
            return "Success";
        }

        public static int GetLabelMemoryAddress(string label)
        {
            if (!labelMemoryAddress.ContainsKey(label))
                return -1;

            return labelMemoryAddress[label];
        }

        public static void ClearLabelMemoryAddress()
        {
            labelMemoryAddress.Clear();
        }

        public static string CMC_Instr()
        {
            if (Chip.GetConditionalBit("CarryBit") == false)
                Chip.SetConditionalBit("CarryBit", true);
            else
                Chip.SetConditionalBit("CarryBit", false);

            return "Success";
        }

        public static string STC_Instr()
        {
            if (Chip.GetConditionalBit("CarryBit") == false)
                Chip.SetConditionalBit("CarryBit", true);

            return "Success";
        }

        public static string CMA_Instr()
        {
            BitArray regA = Instructions.ConvertIntTo8BitArray(Chip.GetRegister("A"));
            BitArray regAOnesComplement = Instructions.ConvertBitArrayToOnesComplement(regA);
            Chip.SetRegister("A", Instructions.ConvertBitArrayToInt(regAOnesComplement));
            return "Success";
        }

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
                    Chip.SetMemory(mAddress, Chip.GetRegister(operand2));
                else
                    Chip.SetRegister(operand1, Chip.GetMemory(mAddress));
            }
            else
                Chip.SetRegister(operand1, Chip.GetRegister(operand2));

            return "Success";
        }

        public static int GetM()
        {
            return ConcatRegisters("H", "L");
        }

        public static string MVI_Instr(string operand1, int operand2)
        {
            if (operand1 == "M")
            {
                int mAddress = GetM();
                Chip.SetMemory(mAddress, operand2);
            }
            else
                Chip.SetRegister(operand1, operand2);
            
            return "Success";
        }

        public static bool IsValueOperandFormatValid(string operand)
        {
            if (operand.EndsWith("H"))
            {
                if (Char.IsDigit(operand[0]))
                    return true;
                else if (operand.Length > 2)
                    if (operand[0] == '-' && Char.IsDigit(operand[1]))
                        return true;

                return false;
            }
            else
                return false;
        }

        public static int ConvertValueOperandToDecimal(string operand, ref string errorMessage)
        {
            if (operand.EndsWith("H"))
                return ConvertHexToDecimal(operand[..^1], ref errorMessage);
            else
            {
                errorMessage = "ERROR: Invalid format";
                return -1;
            }
        }

        public static string LXI_Instr(string operand1, int highValue, int lowValue)
        {
            if (operand1 == "SP")
            {
                int concatValue = Concat8BitIntValues(highValue, lowValue);
                Chip.StackPointer = concatValue;
            }
            else
            {
                string nextRegister = Chip.GetNextRegister(operand1);
                Chip.SetRegister(operand1, highValue);
                Chip.SetRegister(nextRegister, lowValue);
            }

            return "Success";
        }

        public static int[] Extract8BitHighAndLowValues(int value)
        {
            int high = value / 256;
            int low = value - high * 256;
            return new int[] { high, low };
        }

        public static string LDA_Instr(int address)
        {
            int addressValue = Chip.GetMemory(address);
            Chip.SetRegister("A", addressValue);
            return "Success";
        }

        public static string STA_Instr(int address)
        {
            int regA = Chip.GetRegister("A");
            Chip.SetMemory(address, regA);
            return "Success";
        }

        public static string LHLD_Instr(int address)
        {
            if (address == 65535)
                return "ERROR: Invalid LHLD address";

            int nextAddress = address + 1;
            int lowAddressValue = Chip.GetMemory(address);
            int highAddressValue = Chip.GetMemory(nextAddress);

            Chip.SetRegister("L", lowAddressValue);
            Chip.SetRegister("H", highAddressValue);
            return "Success";
        }

        public static string SHLD_Instr(int address)
        {
            if (address == 65535)
                return "ERROR: Invalid SHLD address";

            int nextAddress = address + 1;
            int lValue = Chip.GetRegister("L");
            int hValue = Chip.GetRegister("H");

            Chip.SetMemory(address, lValue);
            Chip.SetMemory(nextAddress, hValue);
            return "Success";
        }

        public static string XCHG_Instr()
        {
            int temp = Chip.GetRegister("H");
            Chip.SetRegister("H", Chip.GetRegister("D"));
            Chip.SetRegister("D", temp);

            temp = Chip.GetRegister("L");
            Chip.SetRegister("L", Chip.GetRegister("E"));
            Chip.SetRegister("E", temp);
            return "Success";
        }

        public static string XTHL_Instr()
        {
            int temp = Chip.GetRegister("H");
            Chip.SetRegister("H", Chip.GetMemory(Chip.StackPointer + 1));
            Chip.SetMemory(Chip.StackPointer + 1, temp);

            temp = Chip.GetRegister("L");
            Chip.SetRegister("L", Chip.GetMemory(Chip.StackPointer));
            Chip.SetMemory(Chip.StackPointer, temp);
            return "Success";
        }

        public static string ADI_Instr(int value)
        {
            if (!IsValueInOneByteRange(value))
                value = NormalizeToOneByteValue(value);

            int ogRegA = Chip.GetRegister("A");
            int result = Chip.GetRegister("A") + value;

            if (!IsValueInOneByteRange(result))
                result = NormalizeToOneByteValue(result);

            Chip.SetRegister("A", result);
            SetConditionalBits("ADD", ogRegA, value, Chip.GetRegister("A"));
            return "Success";
        }

        public static string INR_Instr(string reg)
        {
            bool ogCarryBit = Chip.GetConditionalBit("CarryBit");

            if (reg == "M")
            {
                int mAddress = GetM();
                int ogMemoryValue = Chip.GetMemory(mAddress);
                int result = Chip.GetMemory(mAddress) + 1;

                if (!IsValueInOneByteRange(result))
                    result = NormalizeToOneByteValue(result);

                Chip.SetMemory(mAddress, result);
                SetConditionalBits("ADD", ogMemoryValue, 1, Chip.GetMemory(mAddress));
            }
            else
            {
                int ogRegValue = Chip.GetRegister(reg);
                int result = Chip.GetRegister(reg) + 1;

                if (!IsValueInOneByteRange(result))
                    result = NormalizeToOneByteValue(result);

                Chip.SetRegister(reg, result);
                SetConditionalBits("ADD", ogRegValue, 1, Chip.GetRegister(reg));
            }

            Chip.SetConditionalBit("CarryBit", ogCarryBit); // Not affected by this instr
            return "Success";
        }
        public static string DCR_Instr(string reg)
        {
            bool ogCarryBit = Chip.GetConditionalBit("CarryBit");

            if (reg == "M")
            {
                int mAddress = GetM();
                int ogMemoryValue = Chip.GetMemory(mAddress);
                int result = Chip.GetMemory(mAddress) - 1;

                if (!IsValueInOneByteRange(result))
                    result = NormalizeToOneByteValue(result);
                
                Chip.SetMemory(mAddress, result);
                SetConditionalBits("SUB", ogMemoryValue, 1, Chip.GetMemory(mAddress));
            }
            else
            {
                int ogRegValue = Chip.GetRegister(reg);
                int result = Chip.GetRegister(reg) - 1;

                if (!IsValueInOneByteRange(result))
                    result = NormalizeToOneByteValue(result);

                Chip.SetRegister(reg, result);
                SetConditionalBits("SUB", ogRegValue, 1, Chip.GetRegister(reg));
            }

            Chip.SetConditionalBit("CarryBit", ogCarryBit); // Not affected by this instr
            return "Success";
        }

        public static string DAA_Instr()
        {
            BitArray regABits = ConvertIntTo8BitArray(Chip.GetRegister("A"));
            BitArray regA4LowerBits = Get4LowerBits(regABits);
            int regA4LowerBitsInt = ConvertBitArrayToInt(regA4LowerBits);
            int ogRegA = Chip.GetRegister("A");

            if (regA4LowerBitsInt > 9 || Chip.GetConditionalBit("AuxiliaryCarryBit"))
            {
                Chip.SetRegister("A", Chip.GetRegister("A") + 6);
                SetConditionalBits("ADD", ogRegA, 6, Chip.GetRegister("A"));
            }

            BitArray regABitsUpdated = ConvertIntTo8BitArray(Chip.GetRegister("A"));
            BitArray regA4UpperBits = Get4UpperBits(regABitsUpdated);
            int regA4UpperBitsInt = ConvertBitArrayToInt(regA4UpperBits);

            if (regA4UpperBitsInt > 9 || Chip.GetConditionalBit("CarryBit"))
            {
                bool ogAuxiliaryCarryBit = Chip.GetConditionalBit("AuxiliaryCarryBit");
                int ogRegA2 = Chip.GetRegister("A");
                int sixToBeAddedTo4UpperBits = 96; // 01100000
                int result = Chip.GetRegister("A") + sixToBeAddedTo4UpperBits;

                if (!IsValueInOneByteRange(result))
                    result = NormalizeToOneByteValue(result);

                Chip.SetRegister("A", result);
                SetConditionalBits("ADD", ogRegA2, sixToBeAddedTo4UpperBits, Chip.GetRegister("A"));
                Chip.SetConditionalBit("AuxiliaryCarryBit", ogAuxiliaryCarryBit); // AuxiliarCarry since we set it above
            }

            return "Success";
        }

        public static string STAX_Instr(string reg)
        {
            string regNext = Chip.GetNextRegister(reg);
            int address = ConcatRegisters(reg, regNext);
            Chip.SetMemory(address, Chip.GetRegister("A"));
            return "Success";
        }

        public static string LDAX_Instr(string reg)
        {
            string regNext = Chip.GetNextRegister(reg);
            int address = ConcatRegisters(reg, regNext);
            Chip.SetRegister("A", Chip.GetMemory(address));
            return "Success";
        }

        public static string SUI_Instr(int value)
        {
            if (!IsValueInOneByteRange(value))
                value = NormalizeToOneByteValue(value);

            int ogRegA = Chip.GetRegister("A");
            int result = Chip.GetRegister("A") - value;

            if (!IsValueInOneByteRange(result))
                result = NormalizeToOneByteValue(result);

            Chip.SetRegister("A", result);
            SetConditionalBits("SUB", ogRegA, value, Chip.GetRegister("A"));
            return "Success";
        }

        public static string ANI_Instr(int value)
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.GetRegister("A"));
            BitArray valueArray = ConvertIntTo8BitArray(value);
            BitArray andResult = LogicAndBitArrays(regAArray, valueArray);
            int andResultInt = ConvertBitArrayToInt(andResult);
            int ogRegA = Chip.GetRegister("A");

            Chip.SetRegister("A", andResultInt);
            SetConditionalBits("AND", ogRegA, value, Chip.GetRegister("A"));
            return "Success";
        }

        public static string XRI_Instr(int value)
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.GetRegister("A"));
            BitArray valueArray = ConvertIntTo8BitArray(value);
            BitArray andResult = LogicXORBitArrays(regAArray, valueArray);
            int andResultInt = ConvertBitArrayToInt(andResult);
            int ogRegA = Chip.GetRegister("A");

            Chip.SetRegister("A", andResultInt);
            SetConditionalBits("XOR", ogRegA, value, Chip.GetRegister("A"));
            return "Success";
        }

        public static string ORI_Instr(int value)
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.GetRegister("A"));
            BitArray valueArray = ConvertIntTo8BitArray(value);
            BitArray andResult = LogicORBitArrays(regAArray, valueArray);
            int andResultInt = ConvertBitArrayToInt(andResult);
            int ogRegA = Chip.GetRegister("A");

            Chip.SetRegister("A", andResultInt);
            SetConditionalBits("OR", ogRegA, value, Chip.GetRegister("A"));
            return "Success";
        }

        public static string CPI_Instr(int value)
        {
            int ogRegA = Chip.GetRegister("A");
            SUI_Instr(value);
            Chip.SetRegister("A", ogRegA);
            SetConditionalBits("SUB", ogRegA, value, Chip.GetRegister("A"));
            return "Success";
        }

        public static string RLC_Instr()
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.GetRegister("A"));
            BitArray newArray = new BitArray(8);
            Chip.SetConditionalBit("CarryBit", regAArray[7]);
            newArray[0] = regAArray[7];

            for (int i = 1; i < 8; i++)
            {
                newArray[i] = regAArray[i - 1];
            }

            Chip.SetRegister("A", ConvertBitArrayToInt(newArray));
            return "Success";
        }

        public static string RRC_Instr()
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.GetRegister("A"));
            BitArray newArray = new BitArray(8);
            Chip.SetConditionalBit("CarryBit", regAArray[0]);
            newArray[7] = regAArray[0];

            for (int i = 6; i >= 0; i--)
            {
                newArray[i] = regAArray[i + 1];
            }

            Chip.SetRegister("A", ConvertBitArrayToInt(newArray));
            return "Success";
        }

        public static string RAL_Instr()
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.GetRegister("A"));
            BitArray newArray = new BitArray(8);
            newArray[0] = Chip.GetConditionalBit("CarryBit");
            Chip.SetConditionalBit("CarryBit", regAArray[7]);

            for (int i = 1; i < 8; i++)
            {
                newArray[i] = regAArray[i - 1];
            }

            Chip.SetRegister("A", ConvertBitArrayToInt(newArray));
            return "Success";
        }

        public static string RAR_Instr()
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.GetRegister("A"));
            BitArray newArray = new BitArray(8);
            newArray[7] = Chip.GetConditionalBit("CarryBit");
            Chip.SetConditionalBit("CarryBit", regAArray[0]);

            for (int i = 6; i >= 0; i--)
            {
                newArray[i] = regAArray[i + 1];
            }

            Chip.SetRegister("A", ConvertBitArrayToInt(newArray));
            return "Success";
        }

        public static string PUSH_Instr(string reg)
        {
            if (Chip.StackPointer < 2)
                return "ERROR: Stack pointer value is too low";

            if (reg == "PSW")
            {
                Chip.SetMemory(--Chip.StackPointer, Chip.GetRegister("A"));

                BitArray conditionalBitsArray = new BitArray(8);
                conditionalBitsArray[0] = Chip.GetConditionalBit("CarryBit");
                conditionalBitsArray[1] = true;
                conditionalBitsArray[2] = Chip.GetConditionalBit("ParityBit");
                conditionalBitsArray[4] = Chip.GetConditionalBit("AuxiliaryCarryBit");
                conditionalBitsArray[6] = Chip.GetConditionalBit("ZeroBit");
                conditionalBitsArray[7] = Chip.GetConditionalBit("SignBit");
                Chip.SetMemory(--Chip.StackPointer, ConvertBitArrayToInt(conditionalBitsArray));
                return "Success";
            }
            else
            {
                string regNext = Chip.GetNextRegister(reg);

                Chip.SetMemory(--Chip.StackPointer, Chip.GetRegister(reg));
                Chip.SetMemory(--Chip.StackPointer, Chip.GetRegister(regNext));
                return "Success";
            }
        }

        public static string POP_Instr(string reg)
        {
            if (Chip.StackPointer > 65533)
                return "ERROR: Stack pointer value is too high";

            if (reg == "PSW")
            {
                BitArray conditionalBitsArray = ConvertIntTo8BitArray(Chip.GetMemory(Chip.StackPointer++));
                Chip.SetConditionalBit("CarryBit", conditionalBitsArray[0]);
                Chip.SetConditionalBit("ParityBit", conditionalBitsArray[2]);
                Chip.SetConditionalBit("AuxiliaryCarryBit", conditionalBitsArray[4]);
                Chip.SetConditionalBit("ZeroBit", conditionalBitsArray[6]);
                Chip.SetConditionalBit("SignBit", conditionalBitsArray[7]);

                Chip.SetRegister("A", Chip.GetMemory(Chip.StackPointer++));
                return "Success";
            }
            else
            {
                string regNext = Chip.GetNextRegister(reg);

                Chip.SetRegister(regNext, Chip.GetMemory(Chip.StackPointer++));
                Chip.SetRegister(reg, Chip.GetMemory(Chip.StackPointer++));
                return "Success";
            }
        }

        public static string DAD_Instr(string reg)
        {
            int hLRegPairValue = ConcatRegisters("H", "L");
            int regPairValue = -1;

            if (reg == "SP")
            {
                if (Chip.StackPointer == 0)
                    return "ERROR: StackPointer address is too low for DAD operation";

                int spHighValue = Chip.GetMemory(Chip.StackPointer);
                int spLowValue = Chip.GetMemory(Chip.StackPointer - 1);
                regPairValue = Concat8BitIntValues(spHighValue, spLowValue);
            }
            else
            {
                string regNext = Chip.GetNextRegister(reg);
                regPairValue = ConcatRegisters(reg, regNext);
            }

            int result = hLRegPairValue + regPairValue;

            if (!IsValueInTwoBytesRange(result))
            {
                result = NormalizeTwoByteValue(result);
                Chip.SetConditionalBit("CarryBit", true);
            }

            int[] highLowBytes = Extract8BitHighAndLowValues(result);
            Chip.SetRegister("H", highLowBytes[0]);
            Chip.SetRegister("L", highLowBytes[1]);
            return "Success";
        }

        public static string INX_Instr(string reg)
        {
            if (reg == "SP")
            {
                int stackValue = Chip.StackPointer + 1;

                if (!IsValueInTwoBytesRange(stackValue))
                    stackValue = NormalizeTwoByteValue(stackValue);

                Chip.StackPointer = stackValue;
            }
            else
            {
                string regNext = Chip.GetNextRegister(reg);
                int regPairValue = ConcatRegisters(reg, regNext);
                int result = regPairValue + 1;

                if (!IsValueInTwoBytesRange(result))
                    result = NormalizeTwoByteValue(result);

                int[] highLowBytes = Extract8BitHighAndLowValues(result);

                Chip.SetRegister(reg, highLowBytes[0]);
                Chip.SetRegister(regNext, highLowBytes[1]);
            }

            return "Success";
        }
        public static string DCX_Instr(string reg)
        {
            if (reg == "SP")
            {
                int stackValue = Chip.StackPointer - 1;

                if (!IsValueInTwoBytesRange(stackValue))
                    stackValue = NormalizeTwoByteValue(stackValue);

                Chip.StackPointer = stackValue;
            }
            else
            {
                string regNext = Chip.GetNextRegister(reg);
                int regPairValue = ConcatRegisters(reg, regNext);
                int result = regPairValue - 1;

                if (!IsValueInTwoBytesRange(result))
                    result = NormalizeTwoByteValue(result);

                int[] highLowBytes = Extract8BitHighAndLowValues(result);

                Chip.SetRegister(reg, highLowBytes[0]);
                Chip.SetRegister(regNext, highLowBytes[1]);
            }

            return "Success";
        }

        public static string CALL_Instr(int nextAddress, int jumpAddress)
        {
            int[] nextAddressHighLowByteValues = Instructions.Extract8BitHighAndLowValues(nextAddress);
            int nextAddressHighByteValue = nextAddressHighLowByteValues[0];
            int nextAddressLowByteValue = nextAddressHighLowByteValues[1];

            Chip.SetMemory(--Chip.StackPointer, nextAddressHighByteValue);
            Chip.SetMemory(--Chip.StackPointer, nextAddressLowByteValue);
            Chip.ProgramCounter = jumpAddress;
            return "Succes";
        }

        public static string RET_Instr()
        {
            int nextAddressLowByteValue = Chip.GetMemory(Chip.StackPointer++);
            int nextAddressHighByteValue = Chip.GetMemory(Chip.StackPointer++);
            Chip.ProgramCounter = Instructions.Concat8BitIntValues(nextAddressHighByteValue, nextAddressLowByteValue);
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

            string operand1 = operands[0].Trim();
            string operand2 = operands[1].Trim();
            return new string[] { operand1, operand2 };
        }

        private static int ConvertHexToDecimal(string hex, ref string errorMessage)
        {
            if (Int32.TryParse(hex.ToString(), System.Globalization.NumberStyles.HexNumber, null, out int value))
                return value;

            errorMessage = "ERROR: Invalid hex value";
            return -1;
        }

        public static bool IsValueInOneByteRange(int value)
        {
            if (value < 0 || value > 255)
                return false;

            return true;
        }

        public static bool IsValueInTwoBytesRange(int value)
        {
            if (value < 0 || value > 65535)
                return false;

            return true;
        }

        public static int NormalizeToOneByteValue(int value)
        {
            if (value < 0)
                return value += 256;

            return value -= 256;
        }

        public static int NormalizeTwoByteValue(int value)
        {
            if (value < 0)
                return value += 65536;

            return value -= 65536;
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
            Chip.SetConditionalBit("SignBit", array[7]);
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

                Chip.SetConditionalBit("AuxiliaryCarryBit", carry);
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

                Chip.SetConditionalBit("AuxiliaryCarryBit", carry);
            }
            else if (operation == "ADD" || operation == "XOR" || operation == "OR")
                Chip.SetConditionalBit("AuxiliaryCarryBit", false);
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

                Chip.SetConditionalBit("CarryBit", carry);
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

                Chip.SetConditionalBit("CarryBit", !carry);
            }
            else if (operation == "ADD" || operation == "XOR" || operation == "OR")
                Chip.SetConditionalBit("CarryBit", false);

            // Add other operations
        }

        private static void SetConditionalZeroBit(int value)
        {
            Chip.SetConditionalBit("ZeroBit", value == 0 || value == 256); // 256 is 11111111 + 1, should result in 0
        }

        private static void SetConditionalParityBit(BitArray array)
        {
            int count = 0;

            for (int i = 0; i < 8; i++)
            {
                if (array[i] == true)
                    count++;
            }

            Chip.SetConditionalBit("ParityBit", count % 2 == 0);
        }

        public static int ConvertBitArrayToInt(BitArray bitArray)
        {
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
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

        private static int ConcatRegisters(string regUpperBits, string regLowerBits)
        {
            int upperBits = Chip.GetRegister(regUpperBits);
            int lowerBits = Chip.GetRegister(regLowerBits);
            return Concat8BitIntValues(upperBits, lowerBits);
        }

        public static int Concat8BitIntValues(int intUpperBits, int intLowerBits)
        {
            intUpperBits *= 256;
            return intUpperBits + intLowerBits;
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

        public static int Convert8BitIntToTwosComplement(int value)
        {
            value *= -1;
            BitArray valueArray = Instructions.ConvertIntTo8BitArray(value);
            BitArray valueArrayTwosComplement = Instructions.ConvertBitArrayToOnesComplement(valueArray, true);
            return Instructions.ConvertBitArrayToInt(valueArrayTwosComplement);
        }
    }
}
