using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace _8080
{
    class Instructions
    {
        public static Dictionary<string, string> opLabPartOrFullCode = new Dictionary<string, string>
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
            { "RLC", "00" },
            { "RRC", "01" },
            { "RAL", "10" },
            { "RAR", "11" },
            { "STA", "10" },
            { "LDA", "11" },
        };

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
            if (operand1 == "M")
            {
                int mAddress = GetM();
                Chip.memory[mAddress] = operand2;
            }
            else
                Chip.registers[operand1] = operand2;
            
            return "Success";
        }

        public static bool IsValueOperandFormatValid(string operand)
        {
            if (operand.EndsWith("H"))
            {
                if (Char.IsDigit(operand[0]))
                    return true;

                return false;
            }
            else if (operand.EndsWith("D"))
            {
                operand = operand[..^1];

                if (Int32.TryParse(operand, out int value))
                    return true;

                return false;
            }
            else if (Int32.TryParse(operand, out int value))
                return true;
            // Add other checks for bites etc
            else
                return false;
        }

        public static int ConvertValueOperandToDecimalTwosComplement(string operand, ref string errorMessage)
        {
            if (operand.EndsWith("H"))
                return ConvertHexToDecimalTwosComplement(operand[..^1], ref errorMessage);
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
            if (operand1 == "SP")
            {
                int concatValue = Concat8BitIntValues(highValue, lowValue);
                Chip.stackPointer = concatValue;
            }
            else
            {
                string nextRegister = GetNextDictKey(operand1, Chip.registers);
                Chip.registers[operand1] = highValue;
                Chip.registers[nextRegister] = lowValue;
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
            if (address == 65535)
                return "ERROR: Invalid LHLD address";

            int nextAddress = address + 1;
            int lowAddressValue = Chip.memory[address];
            int highAddressValue = Chip.memory[nextAddress];

            Chip.registers["L"] = lowAddressValue;
            Chip.registers["H"] = highAddressValue;
            return "Success";
        }

        public static string SHLD_Instr(int address)
        {
            if (address == 65535)
                return "ERROR: Invalid SHLD address";

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
                Chip.registers["A"] = NormalizeOneByteValue(Chip.registers["A"]);

            SetConditionalBits("ADD", ogRegA, value, Chip.registers["A"]);
            return "Success";
        }

        public static string ADI_Instr(int value)
        {
            if (!IsValueInOneByteRangeTwosComplement(value))
                value = NormalizeOneByteValueTwosComplement(value);

            int ogRegA = Chip.registers["A"];
            Chip.registers["A"] += value;

            if (!IsValueInOneByteRangeTwosComplement(Chip.registers["A"]))
                Chip.registers["A"] = NormalizeOneByteValueTwosComplement(Chip.registers["A"]);

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
                Chip.memory[mAddress]++;

                if (!IsValueInOneByteRange(Chip.memory[mAddress]))
                    Chip.memory[mAddress] = NormalizeOneByteValue(Chip.memory[mAddress]);

                SetConditionalBits("ADD", ogMemoryValue, 1, Chip.memory[mAddress]);
            }
            else
            {
                int ogRegValue = Chip.registers[reg];
                Chip.registers[reg]++;

                if (!IsValueInOneByteRange(Chip.registers[reg]))
                    Chip.registers[reg] = NormalizeOneByteValue(Chip.registers[reg]);

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
                Chip.memory[mAddress]--;

                if (!IsValueInOneByteRange(Chip.memory[mAddress]))
                    Chip.memory[mAddress] = NormalizeOneByteValue(Chip.memory[mAddress]);

                SetConditionalBits("SUB", ogMemoryValue, 1, Chip.memory[mAddress]);
            }
            else
            {
                int ogRegValue = Chip.registers[reg];
                Chip.registers[reg]--;

                if (!IsValueInOneByteRange(Chip.registers[reg]))
                    Chip.registers[reg] = NormalizeOneByteValue(Chip.registers[reg]);

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
                    Chip.registers["A"] = NormalizeOneByteValue(Chip.registers["A"]);

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
                Chip.registers["A"] = NormalizeOneByteValue(Chip.registers["A"]);

            SetConditionalBits("SUB", ogRegA, value, Chip.registers["A"]);
            return "Success";
        }

        public static string SUI_Instr(int value)
        {
            if (!IsValueInOneByteRangeTwosComplement(value))
                value = NormalizeOneByteValueTwosComplement(value);

            int ogRegA = Chip.registers["A"];
            Chip.registers["A"] -= value;

            if (!IsValueInOneByteRangeTwosComplement(Chip.registers["A"]))
                Chip.registers["A"] = NormalizeOneByteValueTwosComplement(Chip.registers["A"]);

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

        public static string ANI_Instr(int value)
        {
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

        public static string XRI_Instr(int value)
        {
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

        public static string ORI_Instr(int value)
        {
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

        public static string CPI_Instr(int value)
        {
            int ogRegA = Chip.registers["A"];
            SUI_Instr(value);
            Chip.registers["A"] = ogRegA;
            SetConditionalBits("SUB", ogRegA, value, Chip.registers["A"]);
            return "Success";
        }

        public static string RLC_Instr()
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.registers["A"]);
            BitArray newArray = new BitArray(8);
            Chip.conditionalBits["CarryBit"] = regAArray[7];
            newArray[0] = regAArray[7];

            for (int i = 1; i < 8; i++)
            {
                newArray[i] = regAArray[i - 1];
            }

            Chip.registers["A"] = ConvertBitArrayToInt(newArray);
            return "Success";
        }

        public static string RRC_Instr()
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.registers["A"]);
            BitArray newArray = new BitArray(8);
            Chip.conditionalBits["CarryBit"] = regAArray[0];
            newArray[7] = regAArray[0];

            for (int i = 6; i >= 0; i--)
            {
                newArray[i] = regAArray[i + 1];
            }

            Chip.registers["A"] = ConvertBitArrayToInt(newArray);
            return "Success";
        }

        public static string RAL_Instr()
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.registers["A"]);
            BitArray newArray = new BitArray(8);
            newArray[0] = Chip.conditionalBits["CarryBit"];
            Chip.conditionalBits["CarryBit"] = regAArray[7];

            for (int i = 1; i < 8; i++)
            {
                newArray[i] = regAArray[i - 1];
            }

            Chip.registers["A"] = ConvertBitArrayToInt(newArray);
            return "Success";
        }

        public static string RAR_Instr()
        {
            BitArray regAArray = ConvertIntTo8BitArray(Chip.registers["A"]);
            BitArray newArray = new BitArray(8);
            newArray[7] = Chip.conditionalBits["CarryBit"];
            Chip.conditionalBits["CarryBit"] = regAArray[0];

            for (int i = 6; i >= 0; i--)
            {
                newArray[i] = regAArray[i + 1];
            }

            Chip.registers["A"] = ConvertBitArrayToInt(newArray);
            return "Success";
        }

        public static string PUSH_Instr(string reg)
        {
            if (Chip.stackPointer < 2)
                return "ERROR: Stack pointer value is too low";

            if (reg == "PSW")
            {
                Chip.memory[--Chip.stackPointer] = Chip.registers["A"];

                BitArray conditionalBitsArray = new BitArray(8);
                conditionalBitsArray[0] = Chip.conditionalBits["CarryBit"];
                conditionalBitsArray[1] = true;
                conditionalBitsArray[2] = Chip.conditionalBits["ParityBit"];
                conditionalBitsArray[4] = Chip.conditionalBits["AuxiliaryCarryBit"];
                conditionalBitsArray[6] = Chip.conditionalBits["ZeroBit"];
                conditionalBitsArray[7] = Chip.conditionalBits["SignBit"];
                Chip.memory[--Chip.stackPointer] = ConvertBitArrayToInt(conditionalBitsArray);
                return "Success";
            }
            else
            {
                string regNext = GetNextDictKey(reg, Chip.registers);

                Chip.memory[--Chip.stackPointer] = Chip.registers[reg];
                Chip.memory[--Chip.stackPointer] = Chip.registers[regNext];
                return "Success";
            }
        }
        public static string POP_Instr(string reg)
        {
            if (Chip.stackPointer > 65533)
                return "ERROR: Stack pointer value is too high";

            if (reg == "PSW")
            {
                BitArray conditionalBitsArray = ConvertIntTo8BitArray(Chip.memory[Chip.stackPointer++]);
                Chip.conditionalBits["CarryBit"] = conditionalBitsArray[0];
                Chip.conditionalBits["ParityBit"] = conditionalBitsArray[2];
                Chip.conditionalBits["AuxiliaryCarryBit"] = conditionalBitsArray[4];
                Chip.conditionalBits["ZeroBit"] = conditionalBitsArray[6];
                Chip.conditionalBits["SignBit"] = conditionalBitsArray[7];

                Chip.registers["A"] = Chip.memory[Chip.stackPointer++];
                return "Success";
            }
            else
            {
                string regNext = GetNextDictKey(reg, Chip.registers);

                Chip.registers[regNext] = Chip.memory[Chip.stackPointer++];
                Chip.registers[reg] = Chip.memory[Chip.stackPointer++];
                return "Success";
            }
        }

        public static string DAD_Instr(string reg)
        {
            int hLRegPairValue = ConcatRegisters("H", "L");
            int regPairValue = -1;

            if (reg == "SP")
            {
                if (Chip.stackPointer == 0)
                    return "ERROR: StackPointer address is too low for DAD operation";

                int spHighValue = Chip.memory[Chip.stackPointer];
                int spLowValue = Chip.memory[Chip.stackPointer - 1];
                regPairValue = Concat8BitIntValues(spHighValue, spLowValue);
            }
            else
            {
                string regNext = GetNextDictKey(reg, Chip.registers);
                regPairValue = ConcatRegisters(reg, regNext);
            }

            int result = hLRegPairValue + regPairValue;

            if (!IsValueInTwoBytesRange(result))
            {
                result = NormalizeTwoByteValue(result);
                Chip.conditionalBits["CarryBit"] = true;
            }

            int[] highLowBytes = Extract8BitHighAndLowValues(result);
            Chip.registers["H"] = highLowBytes[0];
            Chip.registers["L"] = highLowBytes[1];
            return "Success";
        }

        public static string INX_Instr(string reg)
        {
            int regPairValue = -1;
            string regNext = string.Empty;

            if (reg == "SP")
            {
                if (Chip.stackPointer == 0)
                    return "ERROR: StackPointer address is too low for INX operation";

                int spHighValue = Chip.memory[Chip.stackPointer];
                int spLowValue = Chip.memory[Chip.stackPointer - 1];
                regPairValue = Concat8BitIntValues(spHighValue, spLowValue);
            }
            else
            {
                regNext = GetNextDictKey(reg, Chip.registers);
                regPairValue = ConcatRegisters(reg, regNext);
            }

            int result = regPairValue + 1;

            if (!IsValueInTwoBytesRange(result))
                result = NormalizeTwoByteValue(result);

            int[] highLowBytes = Extract8BitHighAndLowValues(result);

            if (reg == "SP")
            {
                Chip.memory[Chip.stackPointer] = highLowBytes[0];
                Chip.memory[Chip.stackPointer - 1] = highLowBytes[1];
            }
            else
            {
                Chip.registers[reg] = highLowBytes[0];
                Chip.registers[regNext] = highLowBytes[1];
            }

            return "Success";
        }
        public static string DCX_Instr(string reg)
        {
            int regPairValue = -1;
            string regNext = string.Empty;

            if (reg == "SP")
            {
                if (Chip.stackPointer == 0)
                    return "ERROR: StackPointer address is too low for DCX operation";

                int spHighValue = Chip.memory[Chip.stackPointer];
                int spLowValue = Chip.memory[Chip.stackPointer - 1];
                regPairValue = Concat8BitIntValues(spHighValue, spLowValue);
            }
            else
            {
                regNext = GetNextDictKey(reg, Chip.registers);
                regPairValue = ConcatRegisters(reg, regNext);
            }

            int result = regPairValue - 1;

            if (!IsValueInTwoBytesRange(result))
                result = NormalizeTwoByteValue(result);

            int[] highLowBytes = Extract8BitHighAndLowValues(result);

            if (reg == "SP")
            {
                Chip.memory[Chip.stackPointer] = highLowBytes[0];
                Chip.memory[Chip.stackPointer - 1] = highLowBytes[1];
            }
            else
            {
                Chip.registers[reg] = highLowBytes[0];
                Chip.registers[regNext] = highLowBytes[1];
            }

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

        private static int ConvertHexToDecimalTwosComplement(string hex, ref string errorMessage)
        {
            if (Int32.TryParse(hex.ToString(), System.Globalization.NumberStyles.HexNumber, null, out int value))
            {
                if (!IsValueInOneByteRangeTwosComplement(value))
                    value = NormalizeOneByteValueTwosComplement(value);

                return value;
            }

            errorMessage = "ERROR: Invalid hex value";
            return -1;
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

        public static bool IsValueInTwoBytesRange(int value)
        {
            if (value < 0 || value > 65535)
                return false;

            return true;
        }

        public static bool IsValueInOneByteRangeTwosComplement(int value)
        {
            if (value < -128 || value > 127)
                return false;

            return true;
        }

        public static int NormalizeOneByteValue(int value)
        {
            if (value < 0)
                return value += 256;

            return value -= 256;
        }

        public static int NormalizeOneByteValueTwosComplement(int value)
        {
            if (value < -128)
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
            int upperBits = Chip.registers[regUpperBits];
            int lowerBits = Chip.registers[regLowerBits];
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
    }
}
