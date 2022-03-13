using System;
using System.Collections;

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

            for (int m = 0; m < 2; m++)
            {
                Chip.ProgramCounter = 0;
                bool storingLabels = m == 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    line = RemoveCommentFromLine(line);
                    line = line.Trim();
                    string lineRemainingPart = line;
                    string[] words = line.Split(" ");

                    if (line == string.Empty)
                        continue;

                    for (int j = 0; j < words.Length; j++)
                    {
                        string word = words[j];
                        lineRemainingPart = RemoveFirstOccurenceFromString(word, lineRemainingPart);
                        lineRemainingPart = lineRemainingPart.Trim();

                        // Check if LABEL
                        if (word.Contains(":") && j == 0)
                        {
                            if (!IsLabelValid(word))
                                return "ERROR: Invalid label";

                            if (storingLabels)
                            {
                                word = word.Replace(":", "");
                                string settingMessage = Instructions.SetLabelMemoryAddress(word, Chip.ProgramCounter);

                                if (settingMessage != "Success")
                                    return settingMessage;
                            }

                            continue;
                        }
                        // Check if instruction
                        else if (!Instructions.DoesInstructionExist(word))
                            return "ERROR: Invalid instruction";

                        // Execute
                        instruction = word;
                        operands = lineRemainingPart;
                        string instructionLoadMessage = InstrLoader.LoadInstructionToMemory(instruction, operands, storingLabels);

                        if (instructionLoadMessage != "Success")
                            return instructionLoadMessage;

                        break;
                    }
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
            label = label.Trim();
            
            if (semicolonIndex != word.Length)
                afterSemicolon = word[(semicolonIndex + 1)..];

            if (label == string.Empty ||
                label.Contains(" ") || // LABEL can't have spaces inside
                Instructions.DoesInstructionExist(label) ||
                Char.IsDigit(label[0]) ||
                afterSemicolon != string.Empty)
            {
                return false;
            }

            return true;
        }
    }
}
