using System;

namespace _8080
{
    public static class InstrLoader
    {
        public static string LoadInstructionToMemory(string instr, string text, bool storingLabels)
        {
            string errorMessage = string.Empty;

            if (Instructions.IsOpLabNoOperand(instr))
            {
                if (text != string.Empty)
                    return $"ERROR: Invalid {instr} operand";

                string instructionCode = Instructions.GetOpLabPartOrFullCode(instr);
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                return "Success";
            }
            else if (instr == "INR" || instr == "DCR")
            {
                if (!Chip.DoesRegCodePairExist(text))
                    return $"ERROR: Invalid {instr} operand";

                string regCode = Chip.GetRegCode(text);
                string instructionCode = "00" + regCode + Instructions.GetOpLabPartOrFullCode(instr);
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                return "Success";
            }
            else if (instr == "MOV")
            {
                string[] operands = Instructions.SplitOperands(text, ref errorMessage);
                string operand1 = operands[0];
                string operand2 = operands[1];

                if (errorMessage != string.Empty)
                    return errorMessage;
                else if (!Chip.DoesRegCodePairExist(operand1) || !Chip.DoesRegCodePairExist(operand2))
                    return "ERROR: Invalid MOV operands";

                string operand1Code = Chip.GetRegCode(operand1);
                string operand2Code = Chip.GetRegCode(operand2);
                string instructionCode = "01" + operand1Code + operand2Code;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                return "Success";
            }
            else if (instr == "STAX" || instr == "LDAX")
            {
                string bit = string.Empty;

                if (text == "B")
                    bit = "0";
                else if (text == "D")
                    bit = "1";
                else
                    return $"ERROR: Invalid {instr} operand";

                string instructionCode = "000" + bit + Instructions.GetOpLabPartOrFullCode(instr);
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                return "Success";
            }
            else if (Instructions.IsOpLabRegOrMemToAcc(instr))
            {
                if (!Chip.DoesRegCodePairExist(text))
                    return $"ERROR: Invalid {instr} operand";

                string regCode = Chip.GetRegCode(text);
                string instructionCode = "10" + Instructions.GetOpLabPartOrFullCode(instr) + regCode;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                return "Success";
            }
            else if (Instructions.IsOpLabRotateAcc(instr))
            {
                if (text != string.Empty)
                    return $"ERROR: Invalid {instr} operand";

                string instructionCode = "000" + Instructions.GetOpLabPartOrFullCode(instr) + "111";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                return "Success";
            }
            else if (instr == "PUSH" || instr == "POP")
            {
                string regPairCode = string.Empty;
                string opLabCode = Instructions.GetOpLabPartOrFullCode(instr);

                if (text == "B")
                    regPairCode = "00";
                else if (text == "D")
                    regPairCode = "01";
                else if (text == "H")
                    regPairCode = "10";
                else if (text == "PSW")
                    regPairCode = "11";
                else
                    return $"ERROR: Invalid {instr} operand";

                string instructionCode = "11" + regPairCode + opLabCode;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                return "Success";
            }
            else if (instr == "DAD" || instr == "INX" || instr == "DCX")
            {
                string regPairCode = string.Empty;

                if (text == "B")
                    regPairCode = "00";
                else if (text == "D")
                    regPairCode = "01";
                else if (text == "H")
                    regPairCode = "10";
                else if (text == "SP")
                    regPairCode = "11";
                else
                    return $"ERROR: Invalid {instr} operand";

                string opLabCode = Instructions.GetOpLabPartOrFullCode(instr);
                string instructionCode = "00" + regPairCode + opLabCode;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                return "Success";
            }
            else if (instr == "LXI")
            {
                string[] operands = Instructions.SplitOperands(text, ref errorMessage);
                string operand1 = operands[0];
                string operand2 = operands[1];

                if (!Instructions.IsValueOperandFormatValid(operand2))
                    return $"ERROR: Invalid {instr} operand format";

                int operand2_Int = Instructions.ConvertValueOperandToDecimal(operand2, ref errorMessage);

                if (errorMessage != string.Empty)
                    return errorMessage;
                else if (!Instructions.IsValueInTwoBytesRange(operand2_Int))
                    return "ERROR: Value is not in two bytes range";

                int[] highAndLowValues = Instructions.Extract8BitHighAndLowValues(operand2_Int);
                int highByteValue = highAndLowValues[0];
                int lowByteValue = highAndLowValues[1];
                string regPairCode = string.Empty;

                if (operand1 == "B")
                    regPairCode = "00";
                else if (operand1 == "D")
                    regPairCode = "01";
                else if (operand1 == "H")
                    regPairCode = "10";
                else if (operand1 == "SP")
                    regPairCode = "11";
                else
                    return $"ERROR: Invalid {instr} operand";

                if (!Instructions.IsValueInOneByteRange(highByteValue) || !Instructions.IsValueInOneByteRange(lowByteValue))
                    return $"ERROR: Invalid {instr} operands";

                string instructionCode = "00" + regPairCode + "0001";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                Chip.SetMemory(Chip.ProgramCounter++, lowByteValue);
                Chip.SetMemory(Chip.ProgramCounter++, highByteValue);
                return "Success";
            }
            else if (instr == "MVI")
            {
                string[] operands = Instructions.SplitOperands(text, ref errorMessage);
                string operand1 = operands[0];
                string operand2 = operands[1];

                if (errorMessage != string.Empty)
                    return errorMessage;
                else if (!Instructions.IsValueOperandFormatValid(operand2))
                    return "ERROR: Invalid MVI operand value";

                int operand2_Int = Instructions.ConvertValueOperandToDecimal(operand2, ref errorMessage);

                if (!Chip.DoesRegCodePairExist(operand1) || // First operand must be register
                    !Instructions.IsValueInOneByteRange(operand2_Int) ||
                    errorMessage != string.Empty)
                {
                    return "ERROR: Invalid MVI operands";
                }

                string regCode = Chip.GetRegCode(operand1);
                string instructionCode = "00" + regCode + "110";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                Chip.SetMemory(Chip.ProgramCounter++, operand2_Int);
                return "Success";
            }
            else if (Instructions.IsOpLabImmediateInstr(instr))
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return $"ERROR: Invalid {instr} operand format";

                int value = Instructions.ConvertValueOperandToDecimal(text, ref errorMessage);

                if (errorMessage != string.Empty)
                    return errorMessage;

                if (!Instructions.IsValueInOneByteRange(value))
                    return $"ERROR: Invalid {instr} value";

                if (value < 0)
                    value = Instructions.Convert8BitIntToTwosComplement(value);

                string instructionCode = Instructions.GetOpLabPartOrFullCode(instr);
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                Chip.SetMemory(Chip.ProgramCounter++, value);
                return "Success";
            }
            else if (Instructions.IsOpLabDirectAddressing(instr))
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return $"ERROR: Invalid {instr} operand format";

                int address = Instructions.ConvertValueOperandToDecimal(text, ref errorMessage);

                if (errorMessage != string.Empty)
                    return errorMessage;

                int[] highAndLowBytes = Instructions.Extract8BitHighAndLowValues(address);
                int highByteValue = highAndLowBytes[0];
                int lowByteValue = highAndLowBytes[1];

                if (!Instructions.IsValueInOneByteRange(highByteValue) || !Instructions.IsValueInOneByteRange(lowByteValue))
                    return $"ERROR: Invalid {instr} operands";

                string opLabCode = Instructions.GetOpLabPartOrFullCode(instr);
                string instructionCode = "001" + opLabCode + "010";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                Chip.SetMemory(Chip.ProgramCounter++, lowByteValue);
                Chip.SetMemory(Chip.ProgramCounter++, highByteValue);
                return "Success";
            }
            else if (Instructions.IsOpLabJump(instr))
            {
                int text_Int = -1;

                if (storingLabels)
                    text = "0H";

                if (!Instructions.IsValueOperandFormatValid(text))
                {
                    int labelMemoryAddress = Instructions.GetLabelMemoryAddress(text);

                    if (labelMemoryAddress == -1)
                        return $"ERROR: Invalid {instr} operand format";

                    text_Int = labelMemoryAddress;
                }
                else
                {
                    text_Int = Instructions.ConvertValueOperandToDecimal(text, ref errorMessage);

                    if (errorMessage != string.Empty)
                        return errorMessage;
                }

                int[] highAndLowValues = Instructions.Extract8BitHighAndLowValues(text_Int);
                int highByteValue = highAndLowValues[0];
                int lowByteValue = highAndLowValues[1];
                string JMP = "0";

                if (!Instructions.IsValueInOneByteRange(highByteValue) || !Instructions.IsValueInOneByteRange(lowByteValue))
                    return $"ERROR: Invalid {instr} operands";

                if (instr == "JMP")
                    JMP = "1";

                string opLabCode = Instructions.GetOpLabPartOrFullCode(instr);
                string instructionCode = "11" + opLabCode + "01" + JMP;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                Chip.SetMemory(Chip.ProgramCounter++, lowByteValue);
                Chip.SetMemory(Chip.ProgramCounter++, highByteValue);
                return "Success";
            }
            else if (Instructions.IsOpLabCall(instr))
            {
                int text_Int = -1;

                if (storingLabels)
                    text = "0H";

                if (!Instructions.IsValueOperandFormatValid(text))
                {
                    int labelMemoryAddress = Instructions.GetLabelMemoryAddress(text);

                    if (labelMemoryAddress == -1)
                        return $"ERROR: Invalid {instr} operand format";

                    text_Int = labelMemoryAddress;
                }
                else
                {
                    text_Int = Instructions.ConvertValueOperandToDecimal(text, ref errorMessage);

                    if (errorMessage != string.Empty)
                        return errorMessage;
                }

                int[] highAndLowValues = Instructions.Extract8BitHighAndLowValues(text_Int);
                int highByteValue = highAndLowValues[0];
                int lowByteValue = highAndLowValues[1];
                string CALL = "0";

                if (!Instructions.IsValueInOneByteRange(highByteValue) || !Instructions.IsValueInOneByteRange(lowByteValue))
                    return $"ERROR: Invalid {instr} operands";

                if (instr == "CALL")
                    CALL = "1";

                string opLabCode = Instructions.GetOpLabPartOrFullCode(instr);
                string instructionCode = "11" + opLabCode + "10" + CALL;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                Chip.SetMemory(Chip.ProgramCounter++, lowByteValue);
                Chip.SetMemory(Chip.ProgramCounter++, highByteValue);
                return "Success";
            }
            else if (Instructions.IsOpLabReturn(instr))
            {
                if (text != string.Empty)
                    return $"ERROR: Invalid {instr} operand";

                string RET = "0";

                if (instr == "RET")
                    RET = "1";

                string opLabCode = Instructions.GetOpLabPartOrFullCode(instr);
                string instructionCode = "11" + opLabCode + "00" + RET;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.SetMemory(Chip.ProgramCounter++, instructionCodeInt);
                return "Success";
            }
            else if (instr == "DB")
            {
                string[] operands = text.Split(",");

                for (int i = 0; i < operands.Length; i++)
                {
                    string operand = operands[i].Trim();

                    if (operand.StartsWith("\'") && operand.EndsWith("\'"))
                    {
                        operand = operand[1..^1];

                        foreach (char chr in operand)
                        {
                            int chr_Int = (int)chr;

                            if (chr_Int > 255 || chr_Int < 0)
                                return "ERROR: Invalid DB operand";

                            Chip.SetDefineByteMemoryAddress(Chip.ProgramCounter);
                            Chip.SetMemory(Chip.ProgramCounter++, chr_Int);
                        }
                    }
                    else if (Instructions.IsValueOperandFormatValid(operand))
                    {
                        int operand_Int = Instructions.ConvertValueOperandToDecimal(operand, ref errorMessage);

                        if (errorMessage != string.Empty)
                            return errorMessage;
                        else if (operand_Int > 127 || operand_Int < 0) // ASCII char high bit is always 0 
                            return "ERROR: Invalid DB operand";

                        Chip.SetDefineByteMemoryAddress(Chip.ProgramCounter);
                        Chip.SetMemory(Chip.ProgramCounter++, operand_Int);
                    }
                    else
                        return "ERROR: Invalid DB operand";
                }

                return "Success";
            }
            else if (instr == "DW")
            {
                string[] operands = text.Split(",");

                for (int i = 0; i < operands.Length; i++)
                {
                    string operand = operands[i].Trim();

                    if (Instructions.IsValueOperandFormatValid(operand))
                    {
                        int operand_Int = Instructions.ConvertValueOperandToDecimal(operand, ref errorMessage);

                        if (errorMessage != string.Empty)
                            return errorMessage;

                        int[] hightLowBytes = Instructions.Extract8BitHighAndLowValues(operand_Int);
                        int lowByte = hightLowBytes[0];
                        int highByte = hightLowBytes[1];

                        if (!Instructions.IsValueInOneByteRange(lowByte) || !Instructions.IsValueInOneByteRange(highByte))
                            return "ERROR: Invalid DB operand";

                        Chip.SetDefineByteMemoryAddress(Chip.ProgramCounter);
                        Chip.SetMemory(Chip.ProgramCounter++, highByte);
                        Chip.SetDefineByteMemoryAddress(Chip.ProgramCounter);
                        Chip.SetMemory(Chip.ProgramCounter++, lowByte);
                    }
                    else
                        return "ERROR: Invalid DB operand";
                }

                return "Success";
            }
            else if (instr == "DS")
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return "ERROR: Invalid DS operand";

                int operand_Int = Instructions.ConvertValueOperandToDecimal(text, ref errorMessage);

                if (errorMessage != string.Empty)
                    return errorMessage;

                for (int i = 0; i < operand_Int; i++)
                {
                    Chip.SetDefineByteMemoryAddress(Chip.ProgramCounter++);
                }

                return "Success";
            }

            return "ERROR: Invalid instruction";
        }

        private static int ConvertBinaryStringToInt(string binaryString)
        {
            return Convert.ToInt32(binaryString, 2);
        }
    }
}
