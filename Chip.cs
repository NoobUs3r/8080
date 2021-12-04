using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _8080
{
    static class Chip
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

        public static int[] memory = Enumerable.Repeat(0, 256).ToArray();
    }
}
