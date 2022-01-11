using System;
using System.Collections.Generic;
using System.Text;

namespace _8080
{
    public static class CodeParser
    {
        public static string errorMessage = string.Empty;
        public static string instruction = string.Empty;
        public static string operands = string.Empty;

        public static void CheckCodeForErrors(string code)
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
                        {
                            errorMessage = "ERROR: Invalid label";
                            return;
                        }

                        continue;
                    }
                    // Check if instruction
                    else if (FindInstructionMatch(word) == "NO MATCH")
                    {
                        errorMessage = "ERROR: Invalid instruction";
                        return;
                    }

                    instruction = word;
                    operands = lineRemainingPart;
                    break;
                }
            }

            return;
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
                afterSemicolon = word.Substring(semicolonIndex + 1);

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

        public static void ClearErrorMessage()
        {
            errorMessage = string.Empty;
        }
    }
}
