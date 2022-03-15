namespace _8080
{
    public static class InstrExecuter
    {
        public static string ExecuteFromMemoryOnCounter()
        {
            int op = Chip.GetMemory(Chip.ProgramCounter);
            string opBinaryString = Instructions.ConvertIntTo8BinaryString(op);

            if (opBinaryString == "00111111")
            {
                Chip.ProgramCounter++;
                return Instructions.CMC_Instr();
            }
            else if (opBinaryString == "00110111")
            {
                Chip.ProgramCounter++;
                return Instructions.STC_Instr();
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
            else if (opBinaryString == "00101111")
            {
                Chip.ProgramCounter++;
                return Instructions.CMA_Instr();
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
            else if (opBinaryString == "01110110") // HLT
            {
                Chip.ProgramCounter++;
                Chip.IsHalted = true;
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

                if (opBinaryString.EndsWith("0101"))
                    return Instructions.PUSH_Instr(regPair);
                else if (opBinaryString.EndsWith("0001"))
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

                if (opBinaryString.EndsWith("0011"))
                    return Instructions.INX_Instr(regPair);
                else if (opBinaryString.EndsWith("1011"))
                    return Instructions.DCX_Instr(regPair);
                else if (opBinaryString.EndsWith("1001"))
                    return Instructions.DAD_Instr(regPair);
                else if (opBinaryString.EndsWith("0001"))
                {
                    int lowByteValue = Chip.GetMemory(Chip.ProgramCounter++);
                    int highByteValue = Chip.GetMemory(Chip.ProgramCounter++);
                    return Instructions.LXI_Instr(regPair, highByteValue, lowByteValue);
                }
            }
            else if (opBinaryString == "11101011")
            {
                Chip.ProgramCounter++;
                return Instructions.XCHG_Instr();
            }
            else if (opBinaryString == "11100011")
            {
                if (Chip.StackPointer == 65535)
                    return "ERROR: StackPointer address is too high for XTHL operation";

                Chip.ProgramCounter++;
                return Instructions.XTHL_Instr();
            }
            else if (opBinaryString == "11111001") // STHL
            {
                Chip.StackPointer = Instructions.GetM();
                Chip.ProgramCounter++;
                return "Success";
            }
            else if (opBinaryString.StartsWith("00") && opBinaryString.EndsWith("110"))
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

                if (opCode == "000")
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
                else if (opCode == "010")
                    return Instructions.SUI_Instr(value);
                else if (opCode == "100")
                    return Instructions.ANI_Instr(value);
                else if (opCode == "101")
                    return Instructions.XRI_Instr(value);
                else if (opCode == "110")
                    return Instructions.ORI_Instr(value);
                else if (opCode == "111")
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

                if (opCode == "10")
                    return Instructions.STA_Instr(address);
                else if (opCode == "11")
                    return Instructions.LDA_Instr(address);
                else if (opCode == "00")
                    return Instructions.SHLD_Instr(address);
                else if (opCode == "01")
                    return Instructions.LHLD_Instr(address);
            }
            else if (opBinaryString == "11101001") // PCHL
            {
                Chip.ProgramCounter = Instructions.GetM();
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

                if (Chip.ProgramCounter == 65535)
                    Chip.ProgramCounter = 0;
                else
                    Chip.ProgramCounter++;

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

                if (Chip.ProgramCounter == 65535)
                    Chip.ProgramCounter = 0;
                else
                    Chip.ProgramCounter++;

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

            return "ERROR: Unknown instruction.";
        }
    }
}
