using System;
using System.Collections.Generic;
using System.Linq;

namespace _8080
{
    public static class Chip
    {
        private static Dictionary<string, int> registers = new Dictionary<string, int>
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
            else if (value < 0 || value > 255)
                throw new Exception("Value is invalid");

            registers[key] = value;
        }

        public static int GetRegister(string key)
        {
            if (!registers.ContainsKey(key))
                throw new Exception("Register does not exist");

            return registers[key];
        }

        public static bool DoesRegisterExist(string reg)
        {
            return registers.ContainsKey(reg);
        }

        public static string GetNextRegister(string reg)
        {
            if (!registers.ContainsKey(reg))
                throw new Exception("Register does not exist");

            for (int i = 0; i < registers.Count; i++)
            {
                if (registers.ElementAt(i).Key == reg && i != registers.Count - 1)
                    return registers.ElementAt(++i).Key;
            }

            return registers.ElementAt(0).Key;
        }

        private static Dictionary<string, bool> conditionalBits = new Dictionary<string, bool>
        {
            { "CarryBit", false },
            { "AuxiliaryCarryBit", false },
            { "SignBit", false },
            { "ZeroBit", false },
            { "ParityBit", false }
        };

        public static void SetConditionalBit(string key, bool value)
        {
            if (!conditionalBits.ContainsKey(key))
                throw new Exception("ConditionalBit does not exist");

            conditionalBits[key] = value;
        }

        public static bool GetConditionalBit(string key)
        {
            if (!conditionalBits.ContainsKey(key))
                throw new Exception("ConditionalBit does not exist");

            return conditionalBits[key];
        }

        private static int[] memory = Enumerable.Repeat(0, 65536).ToArray();

        public static void SetMemory(int index, int value)
        {
            if (index < 0 || index > 65535)
                throw new Exception("Memory index out of range");
            else if (value < 0 || value > 255)
                throw new Exception("Value is invalid");

            memory[index] = value;
        }

        public static int GetMemory(int index)
        {
            if (index < 0 || index > 65535)
                throw new Exception("Memory index out of range");

            return memory[index];
        }

        private static List<int> defineByteMemoryAddresses = new List<int>();

        public static void SetDefineByteMemoryAddress(int address)
        {
            if (address < 0 || address > 65535)
                throw new Exception("Memory address out of range");

            defineByteMemoryAddresses.Add(address);
        }

        public static bool IsAddressDefineByte(int address)
        {
            if (address < 0 || address > 65535)
                throw new Exception("Memory address out of range");

            if (defineByteMemoryAddresses.Contains(address))
            {
                defineByteMemoryAddresses.Remove(address);
                return true;
            }

            return false;
        }

        private static int programCounter = 0;

        public static int ProgramCounter
        {
            get
            {
                return programCounter;
            }
            set
            {
                if (value < 0 || value > 65535)
                    throw new Exception("Value is invalid");

                programCounter = value;
            }
        }

        private static int stackPointer = 65535;

        public static int StackPointer
        {
            get
            {
                return stackPointer;
            }
            set
            {
                if (value < 0 || value > 65535)
                    throw new Exception("Value is invalid");

                stackPointer = value;
            }
        }

        private static readonly Dictionary<string, string> regCode = new Dictionary<string, string>
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

        public static string GetRegCode(string key)
        {
            if (!regCode.ContainsKey(key))
                throw new Exception("Register does not exist");

            return regCode[key];
        }

        public static string GetRegByCode(string code)
        {
            if (!regCode.ContainsValue(code))
                throw new Exception("Code does not exist");

            return regCode.FirstOrDefault(x => x.Value == code).Key;
        }

        public static bool DoesRegCodePairExist(string reg)
        {
            return regCode.ContainsKey(reg);
        }
    }
}
