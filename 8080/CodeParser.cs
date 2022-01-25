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

                    string instructionMethodMessage = ExecuteInstructionMethod(CodeParser.instruction, CodeParser.operands);

                    if (instructionMethodMessage != "Success")
                        return instructionMethodMessage;

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
                int[] highAndLowValues = Instructions.ExtractHighAndLowValues(operand2_Int);
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
    }
}
