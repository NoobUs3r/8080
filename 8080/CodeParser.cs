﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _8080
{
    public static class CodeParser
    {
        public static string instruction = string.Empty;
        public static string operands = string.Empty;

        public static string CheckCodeForErrorsAndWriteToMemory(string code)
        {
            string codeUppercase = code.ToUpper();
            string[] lines = codeUppercase.Split(new string[] { "\r\n", "\r", "\n" },
                                                 StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                line = RemoveCommentFromLine(line);
                line = RemoveSpacesFromEnd(line);
                string lineRemainingPart = line;
                string[] words = line.Split(" ");

                if (line == string.Empty)
                    continue;

                for (int j = 0; j < words.Length; j++)
                {
                    string word = words[j];
                    lineRemainingPart = RemoveFirstOccurenceFromString(word, lineRemainingPart);
                    lineRemainingPart = RemoveSpacesFromBeginning(lineRemainingPart);

                    // Check if LABEL
                    if (word.Contains(":"))
                    {
                        if (!IsLabelValid(word))
                            return "ERROR: Invalid label";

                        continue;
                    }
                    // Check if instruction
                    else if (FindInstructionMatch(word) == "NO MATCH")
                        return "ERROR: Invalid instruction";

                    // Execute
                    instruction = word;
                    operands = lineRemainingPart;

                    //string instructionMethodMessage = ExecuteInstructionMethod(CodeParser.instruction, CodeParser.operands);
                    string instructionLoadMessage = LoadInstructionToMemory(instruction, operands);

                    if (instructionLoadMessage != "Success")
                        return instructionLoadMessage;

                    /*string executionMessage = ExecuteFromMemoryOnCounter();

                    if (executionMessage != "Success")
                        return executionMessage;*/

                    break;
                }
            }

            return "Success";
        }

        private static string RemoveCommentFromLine(string line)
        {
            if (line.Contains(";"))
            {
                int semicolonIndex = line.IndexOf(";");
                line = line[0..semicolonIndex];
            }

            return line;
        }

        private static string RemoveFirstOccurenceFromString(string valueToRemove, string fullText)
        {
            int index = fullText.IndexOf(valueToRemove);
            string fullTextUpdated = (index < 0) ? fullText : fullText.Remove(index, valueToRemove.Length);
            return fullTextUpdated;
        }

        private static bool IsLabelValid(string word)
        {
            if (!word.Contains(":"))
                return false;

            int semicolonIndex = word.IndexOf(":");
            string afterSemicolon = string.Empty;
            string label = word.Substring(0, semicolonIndex);
            label = RemoveSpacesFromBeginning(label);
            
            if (semicolonIndex != word.Length)
                afterSemicolon = word[(semicolonIndex + 1)..];

            if (label == string.Empty ||
                label.Contains(" ") || // LABEL can't have spaces inside
                FindInstructionMatch(label) != "NO MATCH" ||
                Char.IsDigit(label[0]) ||
                afterSemicolon != string.Empty)
            {
                return false;
            }

            return true;
        }

        public static string RemoveSpacesFromBeginning(string line)
        {
            while (line.StartsWith(" "))
            {
                line = line[1..];
            }

            return line;
        }

        public static string RemoveSpacesFromEnd(string line)
        {
            while (line.EndsWith(" "))
            {
                line = line[0..^1];
            }

            return line;
        }

        private static string FindInstructionMatch(string str)
        {
            foreach (string instruction in Chip.instructions)
            {
                if (str == instruction)
                    return instruction;
            }

            return "NO MATCH";
        }

        /*public static string ExecuteInstructionMethod(string instr, string text)
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
                int[] highAndLowValues = Instructions.Extract8BitHighAndLowValues(operand2_Int);
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

                if (address == maxAddress - 1 || address < 0)
                    return "ERROR: Invalid SHLD address";

                return Instructions.SHLD_Instr(address);
            }
            else if (instr == "XCHG")
            {
                if (text != string.Empty)
                    return "ERROR: Invalid XCHG operands";

                return Instructions.XCHG_Instr();
            }
            else if (instr == "ADD")
            {
                if (!Chip.registers.ContainsKey(text))
                {
                    if (text != "M")
                        return "ERROR: Invalid ADD operands";

                    int maxAddress = Chip.memory.Length - 1;
                    int textInt = Instructions.GetM();

                    if (textInt > maxAddress || textInt < 0)
                        return "ERROR: Invalid ADD address";
                }

                return Instructions.ADD_Instr(text);
            }
            else if (instr == "ADI")
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return "ERROR: Invalid ADI operand format";

                int value = Instructions.ConvertValueOperandToDecimal(text);

                if (!Instructions.IsValueInOneByteRange(value))
                    return "ERROR: Invalid ADI value";

                return Instructions.ADI_Instr(value);
            }
            else
                return "ERROR: Instruction not found";
        }*/

        private static string LoadInstructionToMemory(string instr, string text)
        {
            string errorMessage = string.Empty;

            if (Instructions.IsOpLabNoOperand(instr))
            {
                if (text != string.Empty)
                    return $"ERROR: Invalid {instr} operand";

                string instructionCode = Instructions.opLabPartOrFullCode[instr];
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "INR" || instr == "DCR")
            {
                if (!Chip.RegCode.ContainsKey(text))
                    return $"ERROR: Invalid {instr} operand";

                string regCode = Chip.RegCode[text];
                string instructionCode = "00" + regCode + Instructions.opLabPartOrFullCode[instr];
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "MOV")
            {
                string[] operands = Instructions.SplitOperands(text, ref errorMessage);
                string operand1 = operands[0];
                string operand2 = operands[1];

                if (errorMessage != string.Empty)
                    return errorMessage;
                else if (!Chip.RegCode.ContainsKey(operand1) || !Chip.RegCode.ContainsKey(operand2))
                    return "ERROR: Invalid MOV operands";

                string operand1Code = Chip.RegCode[operand1];
                string operand2Code = Chip.RegCode[operand2];
                string instructionCode = "01" + operand1Code + operand2Code;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
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

                string instructionCode = "000" + bit + Instructions.opLabPartOrFullCode[instr];
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                return "Success";
            }
            else if (Instructions.IsOpLabRegOrMemToAcc(instr))
            {
                if (!Chip.RegCode.ContainsKey(text))
                    return $"ERROR: Invalid {instr} operand";

                string regCode = Chip.RegCode[text];
                string instructionCode = "10" + Instructions.opLabPartOrFullCode[instr] + regCode;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                return "Success";
            }
            else if (Instructions.IsOpLabRotateAcc(instr))
            {
                if (text != string.Empty)
                    return $"ERROR: Invalid {instr} operand";

                string instructionCode = "000" + Instructions.opLabPartOrFullCode[instr] + "111";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "PUSH" || instr == "POP")
            {
                string regPairCode = string.Empty;

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

                string opLabCode = Chip.RegCode[instr];
                string instructionCode = "11" + regPairCode + opLabCode;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
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

                string opLabCode = Chip.RegCode[text];
                string instructionCode = "00" + regPairCode + opLabCode;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "LXI")
            {
                string[] operands = Instructions.SplitOperands(text, ref errorMessage);
                string operand1 = operands[0];
                string operand2 = operands[1];

                if (errorMessage != string.Empty)
                    return errorMessage;
                else if (!Instructions.IsValueOperandFormatValid(operand2))
                    return $"ERROR: Invalid {instr} operand format";

                int operand2_Int = Instructions.ConvertValueOperandToDecimal(operand2);
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

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                Chip.memory[Chip.programCounter++] = lowByteValue;
                Chip.memory[Chip.programCounter++] = highByteValue;
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

                int operand2_Int = Instructions.ConvertValueOperandToDecimal(operand2);

                if (!Chip.RegCode.ContainsKey(operand1) || // First operand must be register
                    operand2_Int == -1 || // Second operand must have correct value
                    !Instructions.IsValueInOneByteRange(operand2_Int))
                {
                    return "ERROR: Invalid MVI operands";
                }

                string regCode = Chip.RegCode[operand1];
                string instructionCode = "00" + regCode + "110";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                Chip.memory[Chip.programCounter++] = operand2_Int;
                return "Success";
            }
            else if (Instructions.IsOpLabImmediateInstr(instr))
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return $"ERROR: Invalid {instr} operand format";

                int value = Instructions.ConvertValueOperandToDecimalTwosComplement(text, ref errorMessage);

                if (errorMessage != string.Empty)
                    return errorMessage;

                if (!Instructions.IsValueInOneByteRangeTwosComplement(value))
                    return $"ERROR: Invalid {instr} value";

                string instructionCode = Instructions.opLabPartOrFullCode[instr];
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                Chip.memory[Chip.programCounter++] = value;
                return "Success";
            }
            else if (Instructions.IsOpLabDirectAddressing(instr))
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return $"ERROR: Invalid {instr} operand format";

                int address = Instructions.ConvertValueOperandToDecimal(text);
                int[] highAndLowBytes = Instructions.Extract8BitHighAndLowValues(address);
                int highByteValue = highAndLowBytes[0];
                int lowByteValue = highAndLowBytes[1];

                if (!Instructions.IsValueInOneByteRange(highByteValue) || !Instructions.IsValueInOneByteRange(lowByteValue))
                    return $"ERROR: Invalid {instr} operands";

                string opLabCode = Instructions.opLabPartOrFullCode[instr];
                string instructionCode = "001" + opLabCode + "010";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                Chip.memory[Chip.programCounter++] = lowByteValue;
                Chip.memory[Chip.programCounter++] = highByteValue;
                return "Success";
            }
            else if (Instructions.IsOpLabJump(instr))
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return $"ERROR: Invalid {instr} operand format";

                int text_Int = Instructions.ConvertValueOperandToDecimal(text);
                int[] highAndLowValues = Instructions.Extract8BitHighAndLowValues(text_Int);
                int highByteValue = highAndLowValues[0];
                int lowByteValue = highAndLowValues[1];
                string JMP = "0";

                if (!Instructions.IsValueInOneByteRange(highByteValue) || !Instructions.IsValueInOneByteRange(lowByteValue))
                    return $"ERROR: Invalid {instr} operands";

                if (instr == "JMP")
                    JMP = "1";

                string opLabCode = Instructions.opLabPartOrFullCode[instr];
                string instructionCode = "11" + opLabCode + "01" + JMP;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                Chip.memory[Chip.programCounter++] = lowByteValue;
                Chip.memory[Chip.programCounter++] = highByteValue;
                return "Success";
            }
            else if (Instructions.IsOpLabCall(instr))
            {
                if (!Instructions.IsValueOperandFormatValid(text))
                    return $"ERROR: Invalid {instr} operand format";

                int text_Int = Instructions.ConvertValueOperandToDecimal(text);
                int[] highAndLowValues = Instructions.Extract8BitHighAndLowValues(text_Int);
                int highByteValue = highAndLowValues[0];
                int lowByteValue = highAndLowValues[1];
                string CALL = "0";

                if (!Instructions.IsValueInOneByteRange(highByteValue) || !Instructions.IsValueInOneByteRange(lowByteValue))
                    return $"ERROR: Invalid {instr} operands";

                if (instr == "CALL")
                    CALL = "1";

                string opLabCode = Instructions.opLabPartOrFullCode[instr];
                string instructionCode = "11" + opLabCode + "10" + CALL;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                Chip.memory[Chip.programCounter++] = lowByteValue;
                Chip.memory[Chip.programCounter++] = highByteValue;
                return "Success";
            }
            else if (Instructions.IsOpLabReturn(instr))
            {
                if (text != string.Empty)
                    return $"ERROR: Invalid {instr} operand";

                string RET = "0";

                if (instr == "RET")
                    RET = "1";

                string opLabCode = Instructions.opLabPartOrFullCode[instr];
                string instructionCode = "11" + opLabCode + "00" + RET;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter++] = instructionCodeInt;
                return "Success";
            }

            return "ERROR: Invalid instruction";
        }

        public static string ExecuteFromMemoryOnCounter()
        {
            int op = Chip.memory[Chip.programCounter];
            string opBinaryString = Instructions.ConvertIntTo8BinaryString(op);

            if (opBinaryString == "00111111") // CMC
            {
                if (Chip.conditionalBits["CarryBit"] == false)
                    Chip.conditionalBits["CarryBit"] = true;
                else
                    Chip.conditionalBits["CarryBit"] = false;

                Chip.programCounter++;
                return "Success";
            }
            else if (opBinaryString == "00110111") // STC
            {
                if (Chip.conditionalBits["CarryBit"] == false)
                    Chip.conditionalBits["CarryBit"] = true;

                Chip.programCounter++;
                return "Success";
            }
            else if (opBinaryString.StartsWith("00") && (opBinaryString.EndsWith("100") || opBinaryString.EndsWith("101")))
            {
                string regCode = opBinaryString.Substring(2, 3);
                string reg = Chip.RegCode.FirstOrDefault(x => x.Value == regCode).Key;
                Chip.programCounter++;

                if (opBinaryString.EndsWith("100"))
                    return Instructions.INR_Instr(reg);
                else if (opBinaryString.EndsWith("101"))
                    return Instructions.DCR_Instr(reg);
            }
            else if (opBinaryString == "00101111") // CMA
            {
                BitArray regA = Instructions.ConvertIntTo8BitArray(Chip.registers["A"]);
                BitArray regAOnesComplement = Instructions.ConvertBitArrayToOnesComplement(regA);
                Chip.registers["A"] = Instructions.ConvertBitArrayToInt(regAOnesComplement);
                Chip.programCounter++;
                return "Success";
            }
            else if (opBinaryString == "00100111")
            {
                Chip.programCounter++;
                return Instructions.DAA_Instr();
            }
            else if (opBinaryString == "00000000") // NOP
            {
                Chip.programCounter++;
                return "Success";
            }
            else if (opBinaryString.StartsWith("01"))
            {
                string operand1Code = opBinaryString.Substring(2, 3);
                string operand2Code = opBinaryString.Substring(5, 3);
                string operand1 = Chip.RegCode.FirstOrDefault(x => x.Value == operand1Code).Key;
                string operand2 = Chip.RegCode.FirstOrDefault(x => x.Value == operand2Code).Key;

                Chip.programCounter++;
                return Instructions.MOV_Instr(operand1, operand2);
            }
            else if (opBinaryString.StartsWith("000") && opBinaryString.EndsWith("111"))
            {
                string opLab = opBinaryString.Substring(3, 2);
                Chip.programCounter++;

                if (opLab == "00")
                    return Instructions.RLC_Instr();
                else if (opLab == "01")
                    return Instructions.RRC_Instr();
                else if (opLab == "10")
                    return Instructions.RAL_Instr();
                else if (opLab == "11")
                    return Instructions.RAR_Instr();
            }
            else if (opBinaryString.StartsWith("000") &&
                    (opBinaryString.EndsWith("0010") || opBinaryString.EndsWith("1010")))
            {
                string reg = "B";
                Chip.programCounter++;

                if (opBinaryString[3] == '1')
                    reg = "D";

                if (opBinaryString.EndsWith("0010"))
                    return Instructions.STAX_Instr(reg);
                else if (opBinaryString.EndsWith("1010"))
                    return Instructions.LDAX_Instr(reg);
            }
            else if (opBinaryString.StartsWith("10"))
            {
                string opLab = opBinaryString.Substring(2, 3);
                string regCode = opBinaryString.Substring(5, 3);
                string reg = Chip.RegCode.FirstOrDefault(x => x.Value == regCode).Key;
                int value = Chip.registers[reg];
                Chip.programCounter++;

                if (opLab == "000") // ADD
                    return Instructions.ADI_Instr(value);
                else if (opLab == "010") // SUB
                    return Instructions.SUI_Instr(value);
                else if (opLab == "111") // CMP
                    return Instructions.CPI_Instr(value);
                else if (opLab == "001" || opLab == "011")
                {
                    if (Chip.conditionalBits["CarryBit"])
                        value++;

                    if (!Instructions.IsValueInOneByteRangeTwosComplement(value))
                        value = Instructions.NormalizeOneByteValueTwosComplement(value);

                    if (opLab == "001") // ADC
                        return Instructions.ADD_Instr(reg);
                    else if (opLab == "011") // SBB
                        return Instructions.SUI_Instr(value);
                }
                else if (opLab == "100" || opLab == "101" || opLab == "110")
                {
                    if (Chip.registers.ContainsKey(reg))
                        value = Chip.registers[reg];
                    else
                    {
                        int mAddress = Instructions.GetM();
                        value = Chip.memory[mAddress];
                    }

                    if (opLab == "100") // ANA
                        return Instructions.ANI_Instr(value);
                    else if (opLab == "101") // XRA
                        return Instructions.XRI_Instr(value);
                    else if (opLab == "110") // ORA
                        return Instructions.ORI_Instr(value);
                }
            }
            else if (opBinaryString.StartsWith("11") &&
                    (opBinaryString.EndsWith("0101") || opBinaryString.EndsWith("0001")))
            {
                string regPairCode = opBinaryString.Substring(2, 2);
                string regPair = string.Empty;
                Chip.programCounter++;

                if (regPairCode == "00")
                    regPair = "B";
                else if (regPairCode == "01")
                    regPair = "D";
                else if (regPairCode == "10")
                    regPair = "H";
                else
                    regPair = "PSW";

                if (opBinaryString.EndsWith("0101")) // PUSH
                    return Instructions.PUSH_Instr(regPair);
                else if (opBinaryString.EndsWith("0001")) // POP
                    return Instructions.POP_Instr(regPair);
            }
            else if (opBinaryString.StartsWith("00") &&
                    (opBinaryString.EndsWith("0011") || opBinaryString.EndsWith("1011") ||
                     opBinaryString.EndsWith("1001") || opBinaryString.EndsWith("0001")))
            {
                string regPairCode = opBinaryString.Substring(2, 2);
                string regPair = string.Empty;
                Chip.programCounter++;

                if (regPairCode == "00")
                    regPair = "B";
                else if (regPairCode == "01")
                    regPair = "D";
                else if (regPairCode == "10")
                    regPair = "H";
                else
                    regPair = "SP";

                if (opBinaryString.EndsWith("0011")) // INX
                    return Instructions.INX_Instr(regPair);
                else if (opBinaryString.EndsWith("1011")) // DCX
                    return Instructions.DCX_Instr(regPair);
                else if (opBinaryString.EndsWith("1001")) // DAD
                    return Instructions.DAD_Instr(regPair);
                else if (opBinaryString.EndsWith("0001")) // LXI
                {
                    int lowByteValue = Chip.memory[++Chip.programCounter];
                    int highByteValue = Chip.memory[++Chip.programCounter];
                    return Instructions.LXI_Instr(regPair, highByteValue, lowByteValue);
                }
            }
            else if (opBinaryString == "11101011") // XCHG
            {
                int temp = Chip.registers["H"];
                Chip.registers["H"] = Chip.registers["D"];
                Chip.registers["D"] = temp;

                temp = Chip.registers["L"];
                Chip.registers["L"] = Chip.registers["E"];
                Chip.registers["E"] = temp;

                Chip.programCounter++;
                return "Success";
            }
            else if (opBinaryString == "11100011") // XTHL
            {
                if (Chip.stackPointer == 65535)
                    return "ERROR: StackPointer address is too high for XTHL operation";

                int temp = Chip.registers["H"];
                Chip.registers["H"] = Chip.memory[Chip.stackPointer + 1];
                Chip.memory[Chip.stackPointer + 1] = temp;

                temp = Chip.registers["L"];
                Chip.registers["L"] = Chip.memory[Chip.stackPointer];
                Chip.memory[Chip.stackPointer] = temp;

                Chip.programCounter++;
                return "Success";
            }
            else if (opBinaryString == "11111001") // STHL
            {
                Chip.stackPointer = Instructions.GetM();
                Chip.programCounter++;
                return "Success";
            }
            else if (opBinaryString.StartsWith("00") && opBinaryString.EndsWith("0001")) // LXI
            {
                string regPairCode = opBinaryString.Substring(2, 2);
                string regPair = string.Empty;

                if (regPairCode == "00")
                    regPair = "B";
                else if (regPairCode == "01")
                    regPair = "D";
                else if (regPairCode == "10")
                    regPair = "H";
                else
                    regPair = "SP";

                int highByteValue = Chip.memory[++Chip.programCounter];
                int lowByteValue = Chip.memory[++Chip.programCounter];

                Chip.programCounter++;
                return Instructions.LXI_Instr(regPair, highByteValue, lowByteValue);
            }
            else if (opBinaryString.StartsWith("00") && opBinaryString.EndsWith("110")) // MVI
            {
                string regCode = opBinaryString.Substring(2, 3);
                string reg = Chip.RegCode.FirstOrDefault(x => x.Value == regCode).Key;
                int value = Chip.memory[++Chip.programCounter];

                Chip.programCounter++;
                return Instructions.MVI_Instr(reg, value);
            }
            else if (opBinaryString.StartsWith("11") && opBinaryString.EndsWith("110"))
            {
                string opCode = opBinaryString.Substring(2, 3);
                int value = Chip.memory[++Chip.programCounter];
                Chip.programCounter++;

                if (opCode == "000") // ADI
                    return Instructions.ADI_Instr(value);
                else if (opCode == "001" || opCode == "011")
                {
                    if (Chip.conditionalBits["CarryBit"])
                        value++;

                    if (!Instructions.IsValueInOneByteRangeTwosComplement(value))
                        value = Instructions.NormalizeOneByteValueTwosComplement(value);

                    if (opCode == "001") // ACI
                        return Instructions.ADI_Instr(value);
                    else if (opCode == "011") // SBI
                        return Instructions.SUI_Instr(value);
                }
                else if (opCode == "010") // SUI
                    return Instructions.SUI_Instr(value);
                else if (opCode == "100") // ANI
                    return Instructions.ANI_Instr(value);
                else if (opCode == "101") // XRI
                    return Instructions.XRI_Instr(value);
                else if (opCode == "110") // ORI
                    return Instructions.ORI_Instr(value);
                else if (opCode == "111") // CPI
                    return Instructions.CPI_Instr(value);
            }
            else if (opBinaryString.StartsWith("001") && opBinaryString.EndsWith("010"))
            {
                string opCode = opBinaryString.Substring(3, 2);
                int lowByteValue = Chip.memory[++Chip.programCounter];
                int highByteValue = Chip.memory[++Chip.programCounter];
                int address = Instructions.Concat8BitIntValues(highByteValue, lowByteValue);
                Chip.programCounter++;

                if (!Instructions.IsValueInTwoBytesRange(address))
                    return "ERROR: Invalid address";

                if (opCode == "10") // STA
                    return Instructions.STA_Instr(address);
                else if (opCode == "11") // LDA
                    return Instructions.LDA_Instr(address);
                else if (opCode == "00") // SHLD
                    return Instructions.SHLD_Instr(address);
                else if (opCode == "01") // LHLD
                    return Instructions.LHLD_Instr(address);
            }
            else if (opBinaryString == "11101001") // PCHL
            {
                Chip.programCounter = Instructions.GetM();
                Chip.programCounter++;
                return "Success";
            }
            else if (opBinaryString.StartsWith("11") &&
                    (opBinaryString.EndsWith("010") || opBinaryString.EndsWith("011")))
            {
                string opCode = opBinaryString.Substring(2, 3);
                int lowByteValue = Chip.memory[++Chip.programCounter];
                int highByteValue = Chip.memory[++Chip.programCounter];
                int address = Instructions.Concat8BitIntValues(highByteValue, lowByteValue);

                if (!Instructions.IsValueInTwoBytesRange(address))
                    return $"ERROR: Invalid address";

                if (opCode == "000" && opBinaryString.EndsWith("1")) // JMP
                {
                    Chip.programCounter = address;
                    return "Success";
                }
                else if (opCode == "011") // JC
                {
                    if (Chip.conditionalBits["CarryBit"])
                        Chip.programCounter = address;

                    return "Success";
                }
                else if (opCode == "010") // JNC
                {
                    if (!Chip.conditionalBits["CarryBit"])
                        Chip.programCounter = address;

                    return "Success";
                }
                else if (opCode == "001") // JZ
                {
                    if (Chip.conditionalBits["ZeroBit"])
                        Chip.programCounter = address;

                    return "Success";
                }
                else if (opCode == "000" && opBinaryString.EndsWith("0")) // JNZ
                {
                    if (!Chip.conditionalBits["ZeroBit"])
                        Chip.programCounter = address;

                    return "Success";
                }
                else if (opCode == "111") // JM
                {
                    if (Chip.conditionalBits["SignBit"])
                        Chip.programCounter = address;

                    return "Success";
                }
                else if (opCode == "110") // JP
                {
                    if (!Chip.conditionalBits["SignBit"])
                        Chip.programCounter = address;

                    return "Success";
                }
                else if (opCode == "101") // JPE
                {
                    if (Chip.conditionalBits["ParityBit"])
                        Chip.programCounter = address;

                    return "Success";
                }
                else if (opCode == "100") // JPO
                {
                    if (!Chip.conditionalBits["ParityBit"])
                        Chip.programCounter = address;

                    return "Success";
                }
            }
            else if (opBinaryString.StartsWith("11") &&
                    (opBinaryString.EndsWith("100") || opBinaryString.EndsWith("101")))
            {
                string opCode = opBinaryString.Substring(2, 3);
                int lowByteValue = Chip.memory[++Chip.programCounter];
                int highByteValue = Chip.memory[++Chip.programCounter];
                int address = Instructions.Concat8BitIntValues(highByteValue, lowByteValue);
                Chip.programCounter++;

                if (Chip.programCounter > 65535)
                    Chip.programCounter = 0;

                int[] nextAddressHighLowByteValues = Instructions.Extract8BitHighAndLowValues(Chip.programCounter);
                int nextAddressHighByteValue = nextAddressHighLowByteValues[0];
                int nextAddressLowByteValue = nextAddressHighLowByteValues[1];

                if (!Instructions.IsValueInTwoBytesRange(address))
                    return "ERROR: Invalid address";

                if (Chip.stackPointer < 1)
                    return "ERROR: StackPointer value is too low";

                if (opCode == "001" && opBinaryString.EndsWith("1")) // CALL
                {
                    Chip.memory[--Chip.stackPointer] = nextAddressHighByteValue;
                    Chip.memory[--Chip.stackPointer] = nextAddressLowByteValue;
                    Chip.programCounter = address;
                    return "Success";
                }
                else if (opCode == "011") // CC
                {
                    if (Chip.conditionalBits["CarryBit"])
                    {
                        Chip.memory[--Chip.stackPointer] = nextAddressHighByteValue;
                        Chip.memory[--Chip.stackPointer] = nextAddressLowByteValue;
                        Chip.programCounter = address;
                    }

                    return "Success";
                }
                else if (opCode == "010") // CNC
                {
                    if (!Chip.conditionalBits["CarryBit"])
                    {
                        Chip.memory[--Chip.stackPointer] = nextAddressHighByteValue;
                        Chip.memory[--Chip.stackPointer] = nextAddressLowByteValue;
                        Chip.programCounter = address;
                    }

                    return "Success";
                }
                else if (opCode == "001" && opBinaryString.EndsWith("0")) // CZ
                {
                    if (Chip.conditionalBits["ZeroBit"])
                    {
                        Chip.memory[--Chip.stackPointer] = nextAddressHighByteValue;
                        Chip.memory[--Chip.stackPointer] = nextAddressLowByteValue;
                        Chip.programCounter = address;
                    }

                    return "Success";
                }
                else if (opCode == "000") // CNZ
                {
                    if (!Chip.conditionalBits["ZeroBit"])
                    {
                        Chip.memory[--Chip.stackPointer] = nextAddressHighByteValue;
                        Chip.memory[--Chip.stackPointer] = nextAddressLowByteValue;
                        Chip.programCounter = address;
                    }

                    return "Success";
                }
                else if (opCode == "111") // CM
                {
                    if (Chip.conditionalBits["SignBit"])
                    {
                        Chip.memory[--Chip.stackPointer] = nextAddressHighByteValue;
                        Chip.memory[--Chip.stackPointer] = nextAddressLowByteValue;
                        Chip.programCounter = address;
                    }

                    return "Success";
                }
                else if (opCode == "110") // CP
                {
                    if (!Chip.conditionalBits["SignBit"])
                    {
                        Chip.memory[--Chip.stackPointer] = nextAddressHighByteValue;
                        Chip.memory[--Chip.stackPointer] = nextAddressLowByteValue;
                        Chip.programCounter = address;
                    }

                    return "Success";
                }
                else if (opCode == "101") // CPE
                {
                    if (Chip.conditionalBits["ParityBit"])
                    {
                        Chip.memory[--Chip.stackPointer] = nextAddressHighByteValue;
                        Chip.memory[--Chip.stackPointer] = nextAddressLowByteValue;
                        Chip.programCounter = address;
                    }

                    return "Success";
                }
                else if (opCode == "100") // CPO
                {
                    if (!Chip.conditionalBits["ParityBit"])
                    {
                        Chip.memory[--Chip.stackPointer] = nextAddressHighByteValue;
                        Chip.memory[--Chip.stackPointer] = nextAddressLowByteValue;
                        Chip.programCounter = address;
                    }

                    return "Success";
                }
            }
            else if (opBinaryString.StartsWith("11") &&
                    (opBinaryString.EndsWith("000") || opBinaryString.EndsWith("001")))
            {
                string opCode = opBinaryString.Substring(2, 3);
                int nextAddressHighByteValue = -1;
                int nextAddressLowByteValue = -1;

                if (Chip.stackPointer == 65535)
                    return "ERROR: Stack is empty";

                if (opCode == "001" && opBinaryString.EndsWith("1")) // RET
                {
                    nextAddressLowByteValue = Chip.memory[Chip.stackPointer++];
                    nextAddressHighByteValue = Chip.memory[Chip.stackPointer];
                    Chip.programCounter = Instructions.Concat8BitIntValues(nextAddressHighByteValue, nextAddressLowByteValue);
                    return "Success";
                }
                else if (opCode == "011") // RC
                {
                    if (Chip.conditionalBits["CarryBit"])
                    {
                        nextAddressLowByteValue = Chip.memory[Chip.stackPointer++];
                        nextAddressHighByteValue = Chip.memory[Chip.stackPointer];
                        Chip.programCounter = Instructions.Concat8BitIntValues(nextAddressHighByteValue, nextAddressLowByteValue);
                    }

                    return "Success";
                }
                else if (opCode == "010") // RNC
                {
                    if (!Chip.conditionalBits["CarryBit"])
                    {
                        nextAddressLowByteValue = Chip.memory[Chip.stackPointer++];
                        nextAddressHighByteValue = Chip.memory[Chip.stackPointer];
                        Chip.programCounter = Instructions.Concat8BitIntValues(nextAddressHighByteValue, nextAddressLowByteValue);
                    }

                    return "Success";
                }
                else if (opCode == "001" && opBinaryString.EndsWith("0")) // RZ
                {
                    if (Chip.conditionalBits["ZeroBit"])
                    {
                        nextAddressLowByteValue = Chip.memory[Chip.stackPointer++];
                        nextAddressHighByteValue = Chip.memory[Chip.stackPointer];
                        Chip.programCounter = Instructions.Concat8BitIntValues(nextAddressHighByteValue, nextAddressLowByteValue);
                    }

                    return "Success";
                }
                else if (opCode == "000") // RNZ
                {
                    if (!Chip.conditionalBits["ZeroBit"])
                    {
                        nextAddressLowByteValue = Chip.memory[Chip.stackPointer++];
                        nextAddressHighByteValue = Chip.memory[Chip.stackPointer];
                        Chip.programCounter = Instructions.Concat8BitIntValues(nextAddressHighByteValue, nextAddressLowByteValue);
                    }

                    return "Success";
                }
                else if (opCode == "111") // RM
                {
                    if (Chip.conditionalBits["SignBit"])
                    {
                        nextAddressLowByteValue = Chip.memory[Chip.stackPointer++];
                        nextAddressHighByteValue = Chip.memory[Chip.stackPointer];
                        Chip.programCounter = Instructions.Concat8BitIntValues(nextAddressHighByteValue, nextAddressLowByteValue);
                    }

                    return "Success";
                }
                else if (opCode == "110") // RP
                {
                    if (!Chip.conditionalBits["SignBit"])
                    {
                        nextAddressLowByteValue = Chip.memory[Chip.stackPointer++];
                        nextAddressHighByteValue = Chip.memory[Chip.stackPointer];
                        Chip.programCounter = Instructions.Concat8BitIntValues(nextAddressHighByteValue, nextAddressLowByteValue);
                    }

                    return "Success";
                }
                else if (opCode == "101") // RPE
                {
                    if (Chip.conditionalBits["ParityBit"])
                    {
                        nextAddressLowByteValue = Chip.memory[Chip.stackPointer++];
                        nextAddressHighByteValue = Chip.memory[Chip.stackPointer];
                        Chip.programCounter = Instructions.Concat8BitIntValues(nextAddressHighByteValue, nextAddressLowByteValue);
                    }

                    return "Success";
                }
                else if (opCode == "100") // RPO
                {
                    if (!Chip.conditionalBits["ParityBit"])
                    {
                        nextAddressLowByteValue = Chip.memory[Chip.stackPointer++];
                        nextAddressHighByteValue = Chip.memory[Chip.stackPointer];
                        Chip.programCounter = Instructions.Concat8BitIntValues(nextAddressHighByteValue, nextAddressLowByteValue);
                    }

                    return "Success";
                }
            }

            return "";
        }

        private static int ConvertBinaryStringToInt(string binaryString)
        {
            return Convert.ToInt32(binaryString, 2);
        }
    }
}
