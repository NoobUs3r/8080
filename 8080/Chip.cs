﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _8080
{
    public static class Chip
    {
        public static Dictionary<string, int> registers = new Dictionary<string, int>
        {
            { "A", 0 },
            { "B", 0 },
            { "C", 0 },
            { "D", 0 },
            { "E", 0 },
            { "H", 0 },
            { "L", 0 }
        };

        public static void SetRegister(string key, int value)
        {
            if (!registers.ContainsKey(key))
                throw new Exception("Register does not exist");

            if (value < 0)
            {
                value *= -1;
                BitArray valueArray = Instructions.ConvertIntTo8BitArray(value);
                BitArray valueArrayTwosComplement = Instructions.ConvertBitArrayToOnesComplement(valueArray, true);
                int valueTwosComplement = Instructions.ConvertBitArrayToInt(valueArrayTwosComplement);
            }

            registers[key] = value;
        }

        public static int GetRegister(string key)
        {
            if (!registers.ContainsKey(key))
                throw new Exception("Register does not exist");

            return registers[key];
        }

        public static Dictionary<string, bool> conditionalBits = new Dictionary<string, bool>
        {
            { "CarryBit", false },
            { "AuxiliaryCarryBit", false },
            { "SignBit", false },
            { "ZeroBit", false },
            { "ParityBit", false }
        };

        public static string[] instructions =
        {
            "MOV",
            "MVI",
            "LXI",
            "LDA",
            "STA",
            "LHLD",
            "SHLD",
            "LDAX",
            "STAX",
            "XCHG",
            "ADD",
            "ADI",
            "ADC",
            "ACI",
            "SUB",
            "SUI",
            "SBB",
            "SBI",
            "INR",
            "DCR",
            "INX",
            "DCX",
            "DAD",
            "DAA",
            "ANA",
            "ANI",
            "XRA",
            "XRI",
            "ORA",
            "ORI",
            "CMP",
            "CPI",
            "RLC",
            "RRC",
            "RAL",
            "RAR",
            "CMA",
            "CMC",
            "STC",
            "JMP",
            "JC",
            "JNC",
            "JZ",
            "JNZ",
            "JP",
            "JM",
            "JPE",
            "JPO",
            "CALL",
            "CC",
            "CNC",
            "CZ",
            "CNZ",
            "CP",
            "CM",
            "CPE",
            "CPO",
            "RET",
            "RC",
            "RNC",
            "RZ",
            "RNZ",
            "RP",
            "RM",
            "RPE",
            "RPO",
            "RST",
            "PCHL",
            "PUSH",
            "PUSH",
            "POP",
            "POP",
            "XTHL",
            "SPHL",
            "IN",
            "OUT",
            "EI",
            "DI",
            "HLT",
            "NOP"
        };

        public static int[] memory = Enumerable.Repeat(0, 65536).ToArray();
        public static int programCounter = 0;
        public static int stackPointer = 65535;

        public static Dictionary<string, string> RegCode = new Dictionary<string, string>
        {
            { "B", "000" },
            { "C", "001" },
            { "D", "010" },
            { "E", "011" },
            { "H", "100" },
            { "L", "101" },
            { "M", "110" },
            { "A", "111" }
        };
    }
}
