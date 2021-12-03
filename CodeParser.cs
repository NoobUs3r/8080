using System;
using System.Collections.Generic;
using System.Text;

namespace _8080
{
    static class CodeParser
    {
        public static string errorMessage = string.Empty;

        public static string CheckCodeForErrors(string code)
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

                for (int j = 0; j < words.Length; j++)
                {
                    string word = words[j];
                    lineRemainingPart = RemoveFirstOccurenceFromString(word, lineRemainingPart);
                    lineRemainingPart = RemoveSpacesFromBeginning(lineRemainingPart);

                    // Check if LABEL
                    if (word.Contains(":"))
                    {
                        CheckIfLabelValid(word);

                        if (errorMessage != string.Empty)
                            return errorMessage;
                    }
                    // Check if instruction
                    else if (FindInstructionMatch(word) != "NO MATCH")
                    {
                        InstructionMethod(word, lineRemainingPart);
                        break;
                    }
                }

                line = RemoveSpacesFromBeginning(line);
                line = RemoveSpacesFromEnd(line);

                if (line == string.Empty)
                    continue;

                CheckIfLabelValid(line);

                if (errorMessage != string.Empty)
                    return errorMessage;

                if (!ContainsValidInstruction(line))
                {
                    if (!line.Contains(":")) // Check if it is no a label
                    {
                        // ERROR
                        errorMessage = "ERROR: Invalid instruction";
                        break;
                    }
                }
            }

            return errorMessage;
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
            string fullTextUpdated = (index < 0)
                ? fullText
                : fullText.Remove(index, valueToRemove.Length);

            return fullTextUpdated;
        }

        private static void CheckIfLabelValid(string line)
        {
            if (!line.Contains(":"))
                return;

            bool isErrorRaised = false;
            int semicolonIndex = line.IndexOf(":");
            string label = line.Substring(0, semicolonIndex);

            label = RemoveSpacesFromBeginning(label);

            if (label.EndsWith(" ") || // "LABEL :" is invalid
                label == string.Empty ||
                label.Contains(" ") || // LABEL can't have spaces inside
                FindInstructionMatch(label) != "NO MATCH" ||
                Char.IsDigit(label[0]))
            {
                isErrorRaised = true;
            }

            if (isErrorRaised)
            {
                errorMessage = "ERROR: Invalid label";
            }

            return;
        }

        private static void InstructionMethod(string instr, string text)
        {
            if (instr == "MOV")
                Instructions.MOV_Instr(text, ref errorMessage);
            else if (instr == "MVI")
                Instructions.MVI_Instr(text, ref errorMessage);
            else if (instr == "LXI")
                Instructions.LXI_Instr(text, ref errorMessage);
            else if (instr == "LDA")
                Instructions.LDA_Instr(text, ref errorMessage);
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

        private static bool ContainsValidInstruction(string str)
        {
            bool containsValidInst = false;

            foreach (string instruction in Chip.instructions)
            {
                if (str.Contains(instruction))
                {
                    containsValidInst = true;
                    break;
                }
            }

            return containsValidInst;
        }

        private static string FindInstructionMatch(string str)
        {
            str = str.ToUpper();

            foreach (string instruction in Chip.instructions)
            {
                if (str == instruction)
                    return instruction;
            }

            return "NO MATCH";
        }

        public static void Clear()
        {
            errorMessage = string.Empty;
        }
    }
}
