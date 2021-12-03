using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _8080
{
    static class Chip
    {
        public static Dictionary<string, string> registers = new Dictionary<string, string>
        {
            { "A", "00" },
            { "B", "00" },
            { "C", "00" },
            { "D", "00" },
            { "E", "00" },
            { "H", "00" },
            { "L", "00" }
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

        public static string[] memory = Enumerable.Repeat("00", 256).ToArray();
    }
}
