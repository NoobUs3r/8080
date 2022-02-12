using _8080;
using NUnit.Framework;

namespace _8080Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        private void ClearChip()
        {
            for (int i = 0; i < Chip.memory.Length; i++)
            {
                Chip.memory[i] = 0;
            }

            Chip.programCounter = 0;
            Chip.stackPointer = 65535;

            Chip.registers["A"] = 0;
            Chip.registers["B"] = 0;
            Chip.registers["C"] = 0;
            Chip.registers["D"] = 0;
            Chip.registers["E"] = 0;
            Chip.registers["H"] = 0;
            Chip.registers["L"] = 0;

            Chip.conditionalBits["CarryBit"] = false;
            Chip.conditionalBits["AuxiliaryCarryBit"] = false;
            Chip.conditionalBits["SignBit"] = false;
            Chip.conditionalBits["ZeroBit"] = false;
            Chip.conditionalBits["ParityBit"] = false;
        }

        [Test]
        public void CMC_Good()
        {
            ClearChip();
            bool ogCarry = Chip.conditionalBits["CarryBit"];

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMC");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreNotEqual(ogCarry, Chip.conditionalBits["CarryBit"], "CMC CarryBit value is not changed");
        }

        [Test]
        public void CMC_Bad ()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMC 1");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void STC_Good1()
        {
            ClearChip();
            Chip.conditionalBits["CarryBit"] = false;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STC");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(true, Chip.conditionalBits["CarryBit"], "STC CarryBit value is not set to true");
        }

        [Test]
        public void STC_Good2()
        {
            ClearChip();
            Chip.conditionalBits["CarryBit"] = true;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STC");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(true, Chip.conditionalBits["CarryBit"], "STC CarryBit value is not set to true");
        }

        [Test]
        public void STC_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STC 1");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void INR_Good1()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INR A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.registers["A"], "INR reg A value is not incremented by one");
        }

        [Test]
        public void INR_Good2()
        {
            ClearChip();
            Chip.registers["A"] = 255;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INR A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(0, Chip.registers["A"], "INR reg A value is not incremented by one");
        }

        [Test]
        public void INR_Good3()
        {
            ClearChip();
            Chip.registers["L"] = 11;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INR M");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.memory[11], "INR M address is not incremented by one");
        }

        [Test]
        public void INR_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INR 1");
            Assert.AreNotEqual(parserMessage, "Success");


            Chip.programCounter = 0;
            CodeParser.CheckCodeForErrorsAndWriteToMemory("INR A");
            Chip.programCounter = 0;
            CodeParser.ExecuteFromMemoryOnCounter();

            Assert.AreNotEqual(0, Chip.registers["A"], "INR reg A value is not incremented by one");
        }

        [Test]
        public void DCR_Good1()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCR A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(255, Chip.registers["A"], "DCR reg A value is not decremented by one");
        }

        [Test]
        public void DCR_Good2()
        {
            ClearChip();
            Chip.registers["A"] = 255;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCR A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(254, Chip.registers["A"], "DCR reg A value is not decremented by one");
        }

        [Test]
        public void DCR_Good3()
        {
            ClearChip();
            Chip.registers["L"] = 11;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCR M");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(255, Chip.memory[11], "DCR M address is not decremented by one");
        }

        [Test]
        public void DCR_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCR 1");
            Assert.AreNotEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            CodeParser.CheckCodeForErrorsAndWriteToMemory("DCR A");
            Chip.programCounter = 0;
            CodeParser.ExecuteFromMemoryOnCounter();

            Assert.AreNotEqual(0, Chip.registers["A"], "DCR reg A value is not decremented by one");
        }

        [Test]
        public void CMA_Good()
        {
            ClearChip();
            Chip.registers["A"] = 81;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMA");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(174, Chip.registers["A"], "CMA reg A value is not complemented");
        }

        [Test]
        public void CMA_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMA A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void DAA_Good()
        {
            ClearChip();
            Chip.registers["A"] = 155;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DAA");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.registers["A"], "DAA reg A value is incorrect");
        }

        [Test]
        public void DAA_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DAA A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void MOV_Good1()
        {
            ClearChip();
            Chip.registers["E"] = 3;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("MOV A, E");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(3, Chip.registers["A"], "MOV reg A value is incorrect");
        }

        [Test]
        public void MOV_Good2()
        {
            ClearChip();
            Chip.registers["L"] = 11;
            Chip.registers["E"] = 3;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("MOV M, E");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(3, Chip.memory[11], "MOV M address value is incorrect");
        }

        [Test]
        public void MOV_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("MOV A, 5");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void STAX_Good()
        {
            ClearChip();
            Chip.registers["C"] = 11;
            Chip.registers["A"] = 3;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STAX B");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(3, Chip.memory[11], "STAX memory address value is incorrect");
        }

        [Test]
        public void STAX_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STAX C");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void LDAX_Good()
        {
            ClearChip();
            Chip.registers["E"] = 11;
            Chip.memory[11] = 3;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LDAX D");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(3, Chip.memory[11], "LDAX reg A value is incorrect");
        }

        [Test]
        public void LDAX_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LDAX E");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ADD_Good1()
        {
            ClearChip();
            Chip.registers["A"] = 108;
            Chip.registers["D"] = 46;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADD D");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(154, Chip.registers["A"], "ADD reg A value is incorrect");
        }

        [Test]
        public void ADD_Good2()
        {
            ClearChip();
            Chip.registers["A"] = 10;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADD A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(20, Chip.registers["A"], "ADD reg A value is incorrect");
        }

        [Test]
        public void ADD_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADD 2");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ADC_Good1()
        {
            ClearChip();
            Chip.registers["A"] = 66;
            Chip.registers["C"] = 61;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADC C");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(127, Chip.registers["A"], "ADC reg A value is incorrect");
        }

        [Test]
        public void ADC_Good2()
        {
            ClearChip();
            Chip.registers["A"] = 66;
            Chip.registers["C"] = 61;
            Chip.conditionalBits["CarryBit"] = true;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADC C");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(128, Chip.registers["A"], "ADC reg A value is incorrect");
        }

        [Test]
        public void ADC_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADC 2");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void SUB_Good1()
        {
            ClearChip();
            Chip.registers["A"] = 62;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SUB A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(0, Chip.registers["A"], "SUB reg A value is incorrect");
        }

        [Test]
        public void SUB_Good2()
        {
            ClearChip();
            Chip.registers["A"] = 100;
            Chip.registers["H"] = 50;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SUB H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(50, Chip.registers["A"], "SUB reg A value is incorrect");
        }

        [Test]
        public void SUB_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SUB 2");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void SBB_Good1()
        {
            ClearChip();
            Chip.registers["A"] = 4;
            Chip.registers["L"] = 2;
            Chip.conditionalBits["CarryBit"] = true;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SBB L");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.registers["A"], "SBB reg A value is incorrect");
        }

        [Test]
        public void SBB_Good2()
        {
            ClearChip();
            Chip.registers["A"] = 4;
            Chip.registers["L"] = 2;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SBB L");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(2, Chip.registers["A"], "SBB reg A value is incorrect");
        }

        [Test]
        public void SBB_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SBB 2");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ANA_Good()
        {
            ClearChip();
            Chip.registers["A"] = 252;
            Chip.registers["C"] = 15;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ANA C");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(12, Chip.registers["A"], "ANA reg A value is incorrect");
        }

        [Test]
        public void ANA_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ANA");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void XRA_Good1()
        {
            ClearChip();
            Chip.registers["A"] = 252;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XRA A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(0, Chip.registers["A"], "XRA reg A value is incorrect");
        }

        [Test]
        public void XRA_Good2()
        {
            ClearChip();
            Chip.registers["A"] = 255;
            Chip.registers["B"] = 254;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XRA B");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.registers["A"], "XRA reg A value is incorrect");
        }

        [Test]
        public void XRA_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XRA");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ORA_Good()
        {
            ClearChip();
            Chip.registers["A"] = 51;
            Chip.registers["C"] = 15;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ORA C");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(63, Chip.registers["A"], "ORA reg A value is incorrect");
        }

        [Test]
        public void ORA_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ORA");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CMP_Good1()
        {
            ClearChip();
            Chip.registers["A"] = 10;
            Chip.registers["E"] = 5;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMP E");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(false, Chip.conditionalBits["CarryBit"], "CMP CarryBit value is incorrect");
            Assert.AreEqual(false, Chip.conditionalBits["ZeroBit"], "CMP ZeroBit value is incorrect");
        }

        [Test]
        public void CMP_Good2()
        {
            ClearChip();
            Chip.registers["A"] = 2;
            Chip.registers["E"] = -5;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMP E");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(true, Chip.conditionalBits["CarryBit"], "CMP CarryBit value is incorrect");
            Assert.AreEqual(false, Chip.conditionalBits["ZeroBit"], "CMP ZeroBit value is incorrect");
        }

        [Test]
        public void CMP_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMP");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RLC_Good()
        {
            ClearChip();
            Chip.registers["A"] = 242;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RLC");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(229, Chip.registers["A"], "RLC reg A value is incorrect");
            Assert.AreEqual(true, Chip.conditionalBits["CarryBit"], "RLC CarryBit value is incorrect");
        }

        [Test]
        public void RLC_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RLC A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RRC_Good()
        {
            ClearChip();
            Chip.registers["A"] = 242;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RRC");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(121, Chip.registers["A"], "RRC reg A value is incorrect");
            Assert.AreEqual(false, Chip.conditionalBits["CarryBit"], "RRC CarryBit value is incorrect");
        }

        [Test]
        public void RRC_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RRC A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RAL_Good()
        {
            ClearChip();
            Chip.registers["A"] = 181;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RAL");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(106, Chip.registers["A"], "RAL reg A value is incorrect");
            Assert.AreEqual(true, Chip.conditionalBits["CarryBit"], "RAL CarryBit value is incorrect");
        }

        [Test]
        public void RAL_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RAL A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RAR_Good()
        {
            ClearChip();
            Chip.registers["A"] = 106;
            Chip.conditionalBits["CarryBit"] = true;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RAR");
            Assert.AreEqual(parserMessage, "Success");

            Chip.programCounter = 0;
            string executionMessage = CodeParser.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(181, Chip.registers["A"], "RAR reg A value is incorrect");
            Assert.AreEqual(false, Chip.conditionalBits["CarryBit"], "RAR CarryBit value is incorrect");
        }

        [Test]
        public void RAR_Bad()
        {
            ClearChip();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RAR A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        /*
        [Test]
        public void MVI_TestGood()
        {
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("MVI", "A, 9");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int regValue = Chip.registers["A"];
            Assert.AreEqual(regValue, 9, "ERROR: reg doesn't equal MVI instruction operand");
        }

        [Test]
        public void MVI_TestBad()
        {
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("MVI", "A, 9, 8");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            _8080.CodeParser.ExecuteInstructionMethod("MVI", "A, 9");
            int regValue = Chip.registers["A"];
            Assert.AreNotEqual(regValue, 8, "ERROR: reg incorrectly equals MVI instruction operand");
        }

        [Test]
        public void MOV_TestGood()
        {
            Chip.registers["A"] = 9;
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("MOV", "B, A");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int regValue = Chip.registers["B"];
            Assert.AreEqual(regValue, 9, "ERROR: reg doesn't equal MOV instruction operand");
        }

        [Test]
        public void MOV_TestBad()
        {
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("MOV", "A, 9");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            Chip.registers["A"] = 9;
            _8080.CodeParser.ExecuteInstructionMethod("MOV", "B, A");
            int regValue = Chip.registers["B"];
            Assert.AreNotEqual(regValue, 8, "ERROR: reg incorrectly equals MOV instruction operand");
        }

        [Test]
        public void LXI_TestGood()
        {
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("LXI", "B, 253");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int regValue1 = Chip.registers["B"];
            Assert.AreEqual(regValue1, 15, "ERROR: reg1 doesn't equal LXI instruction operand");
            int regValue2 = Chip.registers["C"];
            Assert.AreEqual(regValue2, 13, "ERROR: reg2 doesn't equal LXI instruction operand");
        }

        [Test]
        public void LXI_TestBad()
        {
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("LXI", "B, A");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            _8080.CodeParser.ExecuteInstructionMethod("LXI", "B, 253");
            int regValue1 = Chip.registers["B"];
            Assert.AreNotEqual(regValue1, 16, "ERROR: reg1 incorrectly equals LXI instruction operand");
            int regValue2 = Chip.registers["C"];
            Assert.AreNotEqual(regValue2, 14, "ERROR: reg2 incorrectly equals LXI instruction operand");
        }

        [Test]
        public void LDA_TestGood()
        {
            Chip.memory[13] = 1;
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("LDA", "13");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int regValue = Chip.registers["A"];
            Assert.AreEqual(regValue, 1, "ERROR: reg doesn't equal LDA instruction operand");
        }

        [Test]
        public void LDA_TestBad()
        {
            Chip.memory[13] = 1;
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("LDA", "300");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            _8080.CodeParser.ExecuteInstructionMethod("LDA", "13");
            int regValue = Chip.registers["A"];
            Assert.AreNotEqual(regValue, 2, "ERROR: reg incorrectly equals LDA instruction operand");
        }

        [Test]
        public void STA_TestGood()
        {
            Chip.registers["A"] = 13;
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("STA", "255");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int addressValue = Chip.memory[255];
            Assert.AreEqual(addressValue, 13, "ERROR: reg doesn't equal STA instruction address value");
        }

        [Test]
        public void STA_TestBad()
        {
            Chip.registers["A"] = 13;
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("STA", "A");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            _8080.CodeParser.ExecuteInstructionMethod("STA", "255");
            int addressValue = Chip.memory[255];
            Assert.AreNotEqual(addressValue, 15, "ERROR: reg incorrectly equals STA instruction address value");
        }

        [Test]
        public void LHLD_TestGood()
        {
            Chip.memory[0] = 13;
            Chip.memory[1] = 15;
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("LHLD", "0");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int lValue = Chip.registers["L"];
            int hValue = Chip.registers["H"];
            Assert.AreEqual(lValue, 13, "ERROR: L reg doesn't equal LHLD instruction address value");
            Assert.AreEqual(hValue, 15, "ERROR: H reg doesn't equal LHLD instruction address value");
        }

        [Test]
        public void LHLD_TestBad()
        {
            Chip.memory[0] = 13;
            Chip.memory[1] = 15;
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("LHLD", "A");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            int lValue = Chip.registers["L"];
            int hValue = Chip.registers["H"];
            Assert.AreNotEqual(lValue, 14, "ERROR: reg incorrectly equals LHLD instruction address value");
            Assert.AreNotEqual(hValue, 16, "ERROR: reg incorrectly equals LHLD instruction address value");
        }

        [Test]
        public void SHLD_TestGood()
        {
            Chip.registers["L"] = 13;
            Chip.registers["H"] = 15;
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("SHLD", "0");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int lAddress = Chip.memory[0];
            int hAddress = Chip.memory[1];
            Assert.AreEqual(lAddress, 13, "ERROR: L reg doesn't equal SHLD instruction address value");
            Assert.AreEqual(hAddress, 15, "ERROR: H reg doesn't equal SHLD instruction address value");
        }

        [Test]
        public void SHLD_TestBad()
        {
            Chip.registers["L"] = 13;
            Chip.registers["H"] = 15;
            string instructionMessage = _8080.CodeParser.ExecuteInstructionMethod("SHLD", "A");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            int lAddress = Chip.memory[0];
            int hAddress = Chip.memory[1];
            Assert.AreNotEqual(lAddress, 14, "ERROR: L reg incorrectly equals LHLD instruction address value");
            Assert.AreNotEqual(hAddress, 16, "ERROR: H reg incorrectly equals LHLD instruction address value");
        }*/
    }
}