using System;
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

        public static string CheckCodeForErrorsAndExecute(string code)
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

                    string executionMessage = ExecuteFromMemoryOnCounter();

                    if (executionMessage != "Success")
                        return executionMessage;

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

                if (!Instructions.IsValueInOneByteRangeTwosComplement(value))
                    return "ERROR: Invalid ADI value";

                return Instructions.ADI_Instr(value);
            }
            else
                return "ERROR: Instruction not found";
        }

        private static string LoadInstructionToMemory(string instr, string text)
        {
            string errorMessage = string.Empty;

            if (Instructions.IsOpLabNoOperand(instr))
            {
                if (text != string.Empty)
                    return $"ERROR: Invalid {instr} operand";

                string instructionCode = Instructions.opLabPartOrFullCode[instr];
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (Instructions.IsOpLabSingleReg(instr))
            {
                if (!Chip.RegCode.ContainsKey(text))
                    return $"ERROR: Invalid {instr} operand";

                string regCode = Chip.RegCode[text];
                string instructionCode = "00" + regCode + Instructions.opLabPartOrFullCode[instr];
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
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

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (Instructions.IsOpLabDataTransfer(instr))
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

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (Instructions.IsOpLabRegOrMemToAcc(instr))
            {
                if (!Chip.RegCode.ContainsKey(text))
                    return $"ERROR: Invalid {instr} operand";

                string regCode = Chip.RegCode[text];
                string instructionCode = "10" + Instructions.opLabPartOrFullCode[instr] + regCode;
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (Instructions.IsOpLabRotateAcc(instr))
            {
                if (text != string.Empty)
                    return $"ERROR: Invalid {instr} operand";

                string instructionCode = "000" + Instructions.opLabPartOrFullCode[instr] + "111";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "PUSH")
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

                string instructionCode = "11" + regPairCode + "0101";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "POP")
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

                string instructionCode = "11" + regPairCode + "0001";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "DAD")
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

                string instructionCode = "00" + regPairCode + "1001";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "INX")
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

                string instructionCode = "00" + regPairCode + "0011";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "DCX")
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

                string instructionCode = "00" + regPairCode + "1011";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "XCHG")
            {
                string instructionCode = "11101011";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "XTHL")
            {
                string instructionCode = "11100011";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }
            else if (instr == "SPHL")
            {
                string instructionCode = "11111001";
                int instructionCodeInt = ConvertBinaryStringToInt(instructionCode);

                Chip.memory[Chip.programCounter] = instructionCodeInt;
                return "Success";
            }

            return "ERROR: Invalid instruction";
        }

        private static string ExecuteFromMemoryOnCounter()
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
            else if (opBinaryString.StartsWith("00") && opBinaryString.EndsWith("100"))
            {
                string regCode = opBinaryString.Substring(2, 3);
                string reg = Chip.RegCode.FirstOrDefault(x => x.Value == regCode).Key;

                Chip.programCounter++;
                return Instructions.INR_Instr(reg);
            }
            else if (opBinaryString.StartsWith("00") && opBinaryString.EndsWith("101"))
            {
                string regCode = opBinaryString.Substring(2, 3);
                string reg = Chip.RegCode.FirstOrDefault(x => x.Value == regCode).Key;

                Chip.programCounter++;
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
            else if (opBinaryString.StartsWith("000") && opBinaryString.EndsWith("0010"))
            {
                string reg = "B";

                if (opBinaryString[3] == '1')
                    reg = "D";

                Chip.programCounter++;
                return Instructions.STAX_Instr(reg);
            }
            else if (opBinaryString.StartsWith("000") && opBinaryString.EndsWith("1010"))
            {
                string reg = "B";

                if (opBinaryString[3] == '1')
                    reg = "D";

                Chip.programCounter++;
                return Instructions.LDAX_Instr(reg);
            }
            else if (opBinaryString.StartsWith("10")) 
            {
                string opLab = opBinaryString.Substring(2, 3);
                string regCode = opBinaryString.Substring(5, 3);
                string reg = Chip.RegCode.FirstOrDefault(x => x.Value == regCode).Key;

                Chip.programCounter++;

                if (opLab == "000")
                    return Instructions.ADD_Instr(reg);
                else if (opLab == "001") // ADC
                {
                    if (Chip.conditionalBits["CarryBit"])
                        Chip.registers["A"]++;

                    if (!Instructions.IsValueInOneByteRange(Chip.registers["A"]))
                        Chip.registers["A"] = Instructions.NormalizeOneByteValue(Chip.registers["A"]);

                    return Instructions.ADD_Instr(reg);
                }
                else if (opLab == "010")
                    return Instructions.SUB_Instr(reg);
                else if (opLab == "011") // SBB | REPLACE WITH SUBI
                {
                    if (Chip.conditionalBits["CarryBit"])
                        Instructions.DCR_Instr("A");

                    if (!Instructions.IsValueInOneByteRange(Chip.registers["A"]))
                        Chip.registers["A"] = Instructions.NormalizeOneByteValue(Chip.registers["A"]);

                    return Instructions.SUB_Instr(reg);
                }
                else if (opLab == "100")
                    return Instructions.ANA_Instr(reg);
                else if (opLab == "101")
                    return Instructions.XRA_Instr(reg);
                else if (opLab == "110")
                    return Instructions.ORA_Instr(reg);
                else if (opLab == "111")
                    return Instructions.CMP_Instr(reg);
            }
            else if (opBinaryString.StartsWith("11") && opBinaryString.EndsWith("0101")) // PUSH
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
                    regPair = "PSW";

                Chip.programCounter++;
                return Instructions.PUSH_Instr(regPair);
            }
            else if (opBinaryString.StartsWith("11") && opBinaryString.EndsWith("0001")) // POP
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
                    regPair = "PSW";

                Chip.programCounter++;
                return Instructions.POP_Instr(regPair);
            }
            else if (opBinaryString.StartsWith("00") && opBinaryString.EndsWith("1001")) // DAD
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

                Chip.programCounter++;
                return Instructions.DAD_Instr(regPair);
            }
            else if (opBinaryString.StartsWith("00") && opBinaryString.EndsWith("0011")) // INX
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

                Chip.programCounter++;
                return Instructions.INX_Instr(regPair);
            }
            else if (opBinaryString.StartsWith("00") && opBinaryString.EndsWith("1011")) // DCX
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

                Chip.programCounter++;
                return Instructions.DCX_Instr(regPair);
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

            return "";
        }

        private static int ConvertBinaryStringToInt(string binaryString)
        {
            return Convert.ToInt32(binaryString, 2);
        }
    }
}
