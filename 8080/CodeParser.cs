using System;
using System.Collections;

namespace _8080
{
    public static class CodeParser
    {
        public static string instruction = string.Empty;
        public static string operands = string.Empty;
        public static int programCounterForCurrentStepRow = 0;

        /*public static string GetAllLabels(string code)
        {
            string codeUppercase = code.ToUpper();
            string[] lines = codeUppercase.Split(new string[] { "\r\n", "\r", "\n" },
                                                 StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                line = RemoveCommentFromLine(line);
                //line = RemoveSpacesFromEnd(line);
                line = line.Trim();
                string[] words = line.Split(" ");

                if (line == string.Empty)
                    continue;

                string firstWord = words[0];

                // Check if LABEL
                if (firstWord.Contains(":"))
                {
                    if (!IsLabelValid(firstWord))
                        return "ERROR: Invalid label";

                    Instructions.SetLabelMemoryAddress(firstWord, -1);
                }
            }

            return "Success";
        }*/

        public static string CheckCodeForErrorsAndWriteToMemory(string code, int currentStepRow = -1)
        {
            string codeUppercase = code.ToUpper();
            string[] lines = codeUppercase.Split(new string[] { "\r\n", "\r", "\n" },
                                                 StringSplitOptions.None);

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
                    //lineRemainingPart = RemoveSpacesFromBeginning(lineRemainingPart);
                    lineRemainingPart = lineRemainingPart.Trim();

                    // Check if LABEL
                    if (word.Contains(":") && j == 0)
                    {
                        if (!IsLabelValid(word))
                            return "ERROR: Invalid label";

                        //word = word.Replace(":", "");
                        //string settingMessage = Instructions.SetLabelMemoryAddress(word, Chip.ProgramCounter);

                        //if (settingMessage != "Success")
                        //    return settingMessage;

                        continue;
                    }
                    // Check if instruction
                    else if (!Instructions.DoesInstructionExist(word))
                        return "ERROR: Invalid instruction";

                    // Execute
                    instruction = word;
                    operands = lineRemainingPart;
                    string instructionLoadMessage = LoadInstructionToMemory(instruction, operands);

                    if (instructionLoadMessage != "Success")
                        return instructionLoadMessage;

                    break;
                }

                if (i == currentStepRow)
                    programCounterForCurrentStepRow = Chip.ProgramCounter;
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
            //label = RemoveSpacesFromBeginning(label);
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

        /*public static string RemoveSpacesFromBeginning(string line)
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
        }*/

        private static int totalInstructionsLoaded = 0;

        public static int TotalInstructionsLoaded
        {
            get
            {
                return totalInstructionsLoaded;
            }
            set
            {
                totalInstructionsLoaded = value;
            }
        }

        private static string LoadInstructionToMemory(string instr, string text)
        {
            string errorMessage = string.Empty;
            totalInstructionsLoaded++;

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

                string opLabCode = Chip.GetRegCode(instr);
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

                string opLabCode = Chip.GetRegCode(text);
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
                if (!Instructions.IsValueOperandFormatValid(text))
                    return $"ERROR: Invalid {instr} operand format";

                int text_Int = Instructions.ConvertValueOperandToDecimal(text, ref errorMessage);

                if (errorMessage != string.Empty)
                    return errorMessage;

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
                if (!Instructions.IsValueOperandFormatValid(text))
                    return $"ERROR: Invalid {instr} operand format";

                int text_Int = Instructions.ConvertValueOperandToDecimal(text, ref errorMessage);

                if (errorMessage != string.Empty)
                    return errorMessage;

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
                    string operand = operands[i];

                    while (operand.StartsWith(" "))
                        operand = operand[1..];

                    while (operand.EndsWith(" "))
                        operand = operand[0..^1];

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
                    string operand = operands[i];

                    while (operand.StartsWith(" "))
                        operand = operand[1..];

                    while (operand.EndsWith(" "))
                        operand = operand[0..^1];

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

        public static string ExecuteFromMemoryOnCounter()
        {
            int op = Chip.GetMemory(Chip.ProgramCounter);
            string opBinaryString = Instructions.ConvertIntTo8BinaryString(op);

            if (opBinaryString == "00111111") // CMC
            {
                if (Chip.GetConditionalBit("CarryBit") == false)
                    Chip.SetConditionalBit("CarryBit", true);
                else
                    Chip.SetConditionalBit("CarryBit", false);

                Chip.ProgramCounter++;
                return "Success";
            }
            else if (opBinaryString == "00110111") // STC
            {
                if (Chip.GetConditionalBit("CarryBit") == false)
                    Chip.SetConditionalBit("CarryBit", true);

                Chip.ProgramCounter++;
                return "Success";
            }
            else if (opBinaryString.StartsWith("00") && (opBinaryString.EndsWith("100") || opBinaryString.EndsWith("101")))
            {
                string regCode = opBinaryString.Substring(2, 3);
                string reg = Chip.GetRegByCode(regCode);
                Chip.ProgramCounter++;

                if (opBinaryString.EndsWith("100"))
                    return Instructions.INR_Instr(reg);
                else if (opBinaryString.EndsWith("101"))
                    return Instructions.DCR_Instr(reg);
            }
            else if (opBinaryString == "00101111") // CMA
            {
                BitArray regA = Instructions.ConvertIntTo8BitArray(Chip.GetRegister("A"));
                BitArray regAOnesComplement = Instructions.ConvertBitArrayToOnesComplement(regA);
                Chip.SetRegister("A", Instructions.ConvertBitArrayToInt(regAOnesComplement));
                Chip.ProgramCounter++;
                return "Success";
            }
            else if (opBinaryString == "00100111")
            {
                Chip.ProgramCounter++;
                return Instructions.DAA_Instr();
            }
            else if (opBinaryString == "00000000") // NOP
            {
                Chip.ProgramCounter++;
                return "Success";
            }
            else if (opBinaryString.StartsWith("01"))
            {
                string operand1Code = opBinaryString.Substring(2, 3);
                string operand2Code = opBinaryString.Substring(5, 3);
                string operand1 = Chip.GetRegByCode(operand1Code);
                string operand2 = Chip.GetRegByCode(operand2Code);

                Chip.ProgramCounter++;
                return Instructions.MOV_Instr(operand1, operand2);
            }
            else if (opBinaryString.StartsWith("000") && opBinaryString.EndsWith("111"))
            {
                string opLab = opBinaryString.Substring(3, 2);
                Chip.ProgramCounter++;

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
                Chip.ProgramCounter++;

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
                string reg = Chip.GetRegByCode(regCode);
                int value = Chip.GetRegister(reg);
                Chip.ProgramCounter++;

                if (opLab == "000") // ADD
                    return Instructions.ADI_Instr(value);
                else if (opLab == "010") // SUB
                    return Instructions.SUI_Instr(value);
                else if (opLab == "111") // CMP
                    return Instructions.CPI_Instr(value);
                else if (opLab == "001" || opLab == "011")
                {
                    if (Chip.GetConditionalBit("CarryBit"))
                        value++;

                    if (!Instructions.IsValueInOneByteRange(value))
                        value = Instructions.NormalizeToOneByteValue(value);

                    //if (!Instructions.IsValueInOneByteRangeTwosComplement(value))
                    //value = Instructions.NormalizeOneByteValueTwosComplement(value);

                    if (opLab == "001") // ADC
                        return Instructions.ADI_Instr(value);
                    else if (opLab == "011") // SBB
                        return Instructions.SUI_Instr(value);
                }
                else if (opLab == "100" || opLab == "101" || opLab == "110")
                {
                    if (Chip.DoesRegisterExist(reg))
                        value = Chip.GetRegister(reg);
                    else
                    {
                        int mAddress = Instructions.GetM();
                        value = Chip.GetMemory(mAddress);
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
                Chip.ProgramCounter++;

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
                Chip.ProgramCounter++;

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
                    int lowByteValue = Chip.GetMemory(++Chip.ProgramCounter);
                    int highByteValue = Chip.GetMemory(++Chip.ProgramCounter);
                    return Instructions.LXI_Instr(regPair, highByteValue, lowByteValue);
                }
            }
            else if (opBinaryString == "11101011") // XCHG
            {
                int temp = Chip.GetRegister("H");
                Chip.SetRegister("H", Chip.GetRegister("D"));
                Chip.SetRegister("D", temp);

                temp = Chip.GetRegister("L");
                Chip.SetRegister("L", Chip.GetRegister("E"));
                Chip.SetRegister("E", temp);

                Chip.ProgramCounter++;
                return "Success";
            }
            else if (opBinaryString == "11100011") // XTHL
            {
                if (Chip.StackPointer == 65535)
                    return "ERROR: StackPointer address is too high for XTHL operation";

                int temp = Chip.GetRegister("H");
                Chip.SetRegister("H", Chip.GetMemory(Chip.StackPointer + 1));
                Chip.SetMemory(Chip.StackPointer + 1, temp);

                temp = Chip.GetRegister("L");
                Chip.SetRegister("L", Chip.GetMemory(Chip.StackPointer));
                Chip.SetMemory(Chip.StackPointer, temp);

                Chip.ProgramCounter++;
                return "Success";
            }
            else if (opBinaryString == "11111001") // STHL
            {
                Chip.StackPointer = Instructions.GetM();
                Chip.ProgramCounter++;
                return "Success";
            }
            else if (opBinaryString.StartsWith("00") && opBinaryString.EndsWith("110")) // MVI
            {
                string regCode = opBinaryString.Substring(2, 3);
                string reg = Chip.GetRegByCode(regCode);
                int value = Chip.GetMemory(++Chip.ProgramCounter);

                Chip.ProgramCounter++;
                return Instructions.MVI_Instr(reg, value);
            }
            else if (opBinaryString.StartsWith("11") && opBinaryString.EndsWith("110"))
            {
                string opCode = opBinaryString.Substring(2, 3);
                int value = Chip.GetMemory(++Chip.ProgramCounter);
                Chip.ProgramCounter++;

                if (opCode == "000") // ADI
                    return Instructions.ADI_Instr(value);
                else if (opCode == "001" || opCode == "011")
                {
                    if (Chip.GetConditionalBit("CarryBit"))
                        value++;

                    if (!Instructions.IsValueInOneByteRange(value))
                        value = Instructions.NormalizeToOneByteValue(value);

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
                int lowByteValue = Chip.GetMemory(++Chip.ProgramCounter);
                int highByteValue = Chip.GetMemory(++Chip.ProgramCounter);
                int address = Instructions.Concat8BitIntValues(highByteValue, lowByteValue);
                Chip.ProgramCounter++;

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
                Chip.ProgramCounter = Instructions.GetM();
                Chip.ProgramCounter++;
                return "Success";
            }
            else if (opBinaryString.StartsWith("11") &&
                    (opBinaryString.EndsWith("010") || opBinaryString.EndsWith("011")))
            {
                string opCode = opBinaryString.Substring(2, 3);
                int lowByteValue = Chip.GetMemory(++Chip.ProgramCounter);
                int highByteValue = Chip.GetMemory(++Chip.ProgramCounter);
                int address = Instructions.Concat8BitIntValues(highByteValue, lowByteValue);

                if (!Instructions.IsValueInTwoBytesRange(address))
                    return $"ERROR: Invalid address";

                if (opCode == "000" && opBinaryString.EndsWith("1")) // JMP
                {
                    Chip.ProgramCounter = address;
                    return "Success";
                }
                else if (opCode == "011") // JC
                {
                    if (Chip.GetConditionalBit("CarryBit"))
                        Chip.ProgramCounter = address;

                    return "Success";
                }
                else if (opCode == "010") // JNC
                {
                    if (!Chip.GetConditionalBit("CarryBit"))
                        Chip.ProgramCounter = address;

                    return "Success";
                }
                else if (opCode == "001") // JZ
                {
                    if (Chip.GetConditionalBit("ZeroBit"))
                        Chip.ProgramCounter = address;

                    return "Success";
                }
                else if (opCode == "000" && opBinaryString.EndsWith("0")) // JNZ
                {
                    if (!Chip.GetConditionalBit("ZeroBit"))
                        Chip.ProgramCounter = address;

                    return "Success";
                }
                else if (opCode == "111") // JM
                {
                    if (Chip.GetConditionalBit("SignBit"))
                        Chip.ProgramCounter = address;

                    return "Success";
                }
                else if (opCode == "110") // JP
                {
                    if (!Chip.GetConditionalBit("SignBit"))
                        Chip.ProgramCounter = address;

                    return "Success";
                }
                else if (opCode == "101") // JPE
                {
                    if (Chip.GetConditionalBit("ParityBit"))
                        Chip.ProgramCounter = address;

                    return "Success";
                }
                else if (opCode == "100") // JPO
                {
                    if (!Chip.GetConditionalBit("ParityBit"))
                        Chip.ProgramCounter = address;

                    return "Success";
                }
            }
            else if (opBinaryString.StartsWith("11") &&
                    (opBinaryString.EndsWith("100") || opBinaryString.EndsWith("101")))
            {
                string opCode = opBinaryString.Substring(2, 3);
                int lowByteValue = Chip.GetMemory(++Chip.ProgramCounter);
                int highByteValue = Chip.GetMemory(++Chip.ProgramCounter);
                int address = Instructions.Concat8BitIntValues(highByteValue, lowByteValue);

                if (Chip.ProgramCounter == 65535)
                    Chip.ProgramCounter = 0;
                else 
                    Chip.ProgramCounter++;

                if (!Instructions.IsValueInTwoBytesRange(address))
                    return "ERROR: Invalid address";

                if (Chip.StackPointer < 1)
                    return "ERROR: StackPointer value is too low";

                if (opCode == "001" && opBinaryString.EndsWith("1")) // CALL
                {
                    Instructions.CALL_Instr(Chip.ProgramCounter, address);
                    return "Success";
                }
                else if (opCode == "011") // CC
                {
                    if (Chip.GetConditionalBit("CarryBit"))
                        Instructions.CALL_Instr(Chip.ProgramCounter, address);

                    return "Success";
                }
                else if (opCode == "010") // CNC
                {
                    if (!Chip.GetConditionalBit("CarryBit"))
                        Instructions.CALL_Instr(Chip.ProgramCounter, address);

                    return "Success";
                }
                else if (opCode == "001" && opBinaryString.EndsWith("0")) // CZ
                {
                    if (Chip.GetConditionalBit("ZeroBit"))
                        Instructions.CALL_Instr(Chip.ProgramCounter, address);

                    return "Success";
                }
                else if (opCode == "000") // CNZ
                {
                    if (!Chip.GetConditionalBit("ZeroBit"))
                        Instructions.CALL_Instr(Chip.ProgramCounter, address);

                    return "Success";
                }
                else if (opCode == "111") // CM
                {
                    if (Chip.GetConditionalBit("SignBit"))
                        Instructions.CALL_Instr(Chip.ProgramCounter, address);

                    return "Success";
                }
                else if (opCode == "110") // CP
                {
                    if (!Chip.GetConditionalBit("SignBit"))
                        Instructions.CALL_Instr(Chip.ProgramCounter, address);

                    return "Success";
                }
                else if (opCode == "101") // CPE
                {
                    if (Chip.GetConditionalBit("ParityBit"))
                        Instructions.CALL_Instr(Chip.ProgramCounter, address);

                    return "Success";
                }
                else if (opCode == "100") // CPO
                {
                    if (!Chip.GetConditionalBit("ParityBit"))
                        Instructions.CALL_Instr(Chip.ProgramCounter, address);

                    return "Success";
                }
            }
            else if (opBinaryString.StartsWith("11") &&
                    (opBinaryString.EndsWith("000") || opBinaryString.EndsWith("001")))
            {
                string opCode = opBinaryString.Substring(2, 3);

                if (Chip.StackPointer > 65533)
                    return "ERROR: StackPointer value is too high";

                if (opCode == "001" && opBinaryString.EndsWith("1")) // RET
                {
                    Instructions.RET_Instr();
                    return "Success";
                }
                else if (opCode == "011") // RC
                {
                    if (Chip.GetConditionalBit("CarryBit"))
                        Instructions.RET_Instr();

                    return "Success";
                }
                else if (opCode == "010") // RNC
                {
                    if (!Chip.GetConditionalBit("CarryBit"))
                        Instructions.RET_Instr();

                    return "Success";
                }
                else if (opCode == "001" && opBinaryString.EndsWith("0")) // RZ
                {
                    if (Chip.GetConditionalBit("ZeroBit"))
                        Instructions.RET_Instr();

                    return "Success";
                }
                else if (opCode == "000") // RNZ
                {
                    if (!Chip.GetConditionalBit("ZeroBit"))
                        Instructions.RET_Instr();

                    return "Success";
                }
                else if (opCode == "111") // RM
                {
                    if (Chip.GetConditionalBit("SignBit"))
                        Instructions.RET_Instr();

                    return "Success";
                }
                else if (opCode == "110") // RP
                {
                    if (!Chip.GetConditionalBit("SignBit"))
                        Instructions.RET_Instr();

                    return "Success";
                }
                else if (opCode == "101") // RPE
                {
                    if (Chip.GetConditionalBit("ParityBit"))
                        Instructions.RET_Instr();

                    return "Success";
                }
                else if (opCode == "100") // RPO
                {
                    if (!Chip.GetConditionalBit("ParityBit"))
                        Instructions.RET_Instr();

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
