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

        private void ClearChipAndLabels()
        {
            for (int i = 0; i < 65535; i++)
            {
                Chip.SetMemory(i, 0);
            }

            Chip.ProgramCounter = 0;
            Chip.StackPointer = 65535;

            Chip.SetRegister("A", 0);
            Chip.SetRegister("B", 0);
            Chip.SetRegister("C", 0);
            Chip.SetRegister("D", 0);
            Chip.SetRegister("E", 0);
            Chip.SetRegister("H", 0);
            Chip.SetRegister("L", 0);

            Chip.SetConditionalBit("CarryBit", false);
            Chip.SetConditionalBit("AuxiliaryCarryBit", false);
            Chip.SetConditionalBit("SignBit", false);
            Chip.SetConditionalBit("ZeroBit", false);
            Chip.SetConditionalBit("ParityBit", false);

            Instructions.ClearLabelMemoryAddress();
        }

        [Test]
        public void DB_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DB 'STRING'");
            Assert.AreEqual(parserMessage, "Success");
            Assert.AreEqual(6, Chip.ProgramCounter, "DB ProgramCounter value is incorrent");
        }

        [Test]
        public void DB_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DB 3CH \n JMP 0H");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 2; i++)
            {
                if (Chip.IsAddressDefineByte(Chip.ProgramCounter))
                {
                    Chip.ProgramCounter++;
                    return;
                }

                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "DB reg A value is incorrent");
        }

        [Test]
        public void DB_Good3()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DB 1H, 2H");
            Assert.AreEqual(parserMessage, "Success");
            Assert.AreEqual(2, Chip.ProgramCounter, "DB ProgramCounter value is incorrent");
        }

        [Test]
        public void DB_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DB A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void DW_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DW 3C01H, 3CAEH");
            Assert.AreEqual(parserMessage, "Success");
            Assert.AreEqual(4, Chip.ProgramCounter, "DW ProgramCounter value is incorrent");
            Assert.AreEqual(1, Chip.GetMemory(0), "DW memory value is incorrent");
            Assert.AreEqual(60, Chip.GetMemory(1), "DW memory value is incorrent");
            Assert.AreEqual(174, Chip.GetMemory(2), "DW memory value is incorrent");
            Assert.AreEqual(60, Chip.GetMemory(3), "DW memory value is incorrent");
        }

        [Test]
        public void DW_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DW 3C3CH \n JMP 0H");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                if (Chip.IsAddressDefineByte(Chip.ProgramCounter))
                {
                    Chip.ProgramCounter++;
                    return;
                }

                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "DW reg A value is incorrent");
        }

        [Test]
        public void DW_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DW 3C01AH");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void DS_Good()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DS 9H");
            Assert.AreEqual(parserMessage, "Success");
            Assert.AreEqual(9, Chip.ProgramCounter, "DS ProgramCounter value is incorrent");
        }

        [Test]
        public void DS_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DS A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CMC_Good()
        {
            ClearChipAndLabels();
            bool ogCarry = Chip.GetConditionalBit("CarryBit");

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMC");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(!ogCarry, Chip.GetConditionalBit("CarryBit"), "CMC CarryBit value is not changed");
        }

        [Test]
        public void CMC_Bad ()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMC 1");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void STC_Good1()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("CarryBit", false);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STC");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(true, Chip.GetConditionalBit("CarryBit"), "STC CarryBit value is not set to true");
        }

        [Test]
        public void STC_Good2()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("CarryBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STC");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(true, Chip.GetConditionalBit("CarryBit"), "STC CarryBit value is not set to true");
        }

        [Test]
        public void STC_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STC 1");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void INR_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INR A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.GetRegister("A"), "INR reg A value is not incremented by one");
        }

        [Test]
        public void INR_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 255);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INR A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(0, Chip.GetRegister("A"), "INR reg A value is not incremented by one");
        }

        [Test]
        public void INR_Good3()
        {
            ClearChipAndLabels();
            Chip.SetRegister("L", 11);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INR M");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.GetMemory(11), "INR M address is not incremented by one");
        }

        [Test]
        public void INR_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INR 1");
            Assert.AreNotEqual(parserMessage, "Success");


            Chip.ProgramCounter = 0;
            CodeParser.CheckCodeForErrorsAndWriteToMemory("INR A");
            Chip.ProgramCounter = 0;
            InstrExecuter.ExecuteFromMemoryOnCounter();

            Assert.AreNotEqual(0, Chip.GetRegister("A"), "INR reg A value is not incremented by one");
        }

        [Test]
        public void DCR_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCR A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(255, Chip.GetRegister("A"), "DCR reg A value is not decremented by one");
        }

        [Test]
        public void DCR_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 255);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCR A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(254, Chip.GetRegister("A"), "DCR reg A value is not decremented by one");
        }

        [Test]
        public void DCR_Good3()
        {
            ClearChipAndLabels();
            Chip.SetRegister("L", 11);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCR M");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(255, Chip.GetMemory(11), "DCR M address is not decremented by one");
        }

        [Test]
        public void DCR_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCR 1");
            Assert.AreNotEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            CodeParser.CheckCodeForErrorsAndWriteToMemory("DCR A");
            Chip.ProgramCounter = 0;
            InstrExecuter.ExecuteFromMemoryOnCounter();

            Assert.AreNotEqual(0, Chip.GetRegister("A"), "DCR reg A value is not decremented by one");
        }

        [Test]
        public void CMA_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 81);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMA");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(174, Chip.GetRegister("A"), "CMA reg A value is not complemented");
        }

        [Test]
        public void CMA_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMA A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void DAA_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 155);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DAA");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.GetRegister("A"), "DAA reg A value is incorrect");
        }

        [Test]
        public void DAA_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DAA A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void MOV_Good1()
        {
            ClearChipAndLabels();
            Chip.SetRegister("E", 3);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("MOV A, E");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(3, Chip.GetRegister("A"), "MOV reg A value is incorrect");
        }

        [Test]
        public void MOV_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("L", 11);
            Chip.SetRegister("E", 3);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("MOV M, E");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(3, Chip.GetMemory(11), "MOV M address value is incorrect");
        }

        [Test]
        public void MOV_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("MOV A, 5");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void STAX_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("C", 11);
            Chip.SetRegister("A", 3);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STAX B");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(3, Chip.GetMemory(11), "STAX memory address value is incorrect");
        }

        [Test]
        public void STAX_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STAX C");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void LDAX_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("E", 11);
            Chip.SetMemory(11, 3);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LDAX D");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(3, Chip.GetMemory(11), "LDAX reg A value is incorrect");
        }

        [Test]
        public void LDAX_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LDAX E");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ADD_Good1()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 108);
            Chip.SetRegister("D", 46);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADD D");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(154, Chip.GetRegister("A"), "ADD reg A value is incorrect");
        }

        [Test]
        public void ADD_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 10);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADD A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(20, Chip.GetRegister("A"), "ADD reg A value is incorrect");
        }

        [Test]
        public void ADD_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADD 2");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ADC_Good1()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 66);
            Chip.SetRegister("C", 61);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADC C");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(127, Chip.GetRegister("A"), "ADC reg A value is incorrect");
        }

        [Test]
        public void ADC_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 66);
            Chip.SetRegister("C", 61);
            Chip.SetConditionalBit("CarryBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADC C");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(128, Chip.GetRegister("A"), "ADC reg A value is incorrect");
        }

        [Test]
        public void ADC_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADC 2");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void SUB_Good1()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 62);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SUB A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(0, Chip.GetRegister("A"), "SUB reg A value is incorrect");
        }

        [Test]
        public void SUB_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 100);
            Chip.SetRegister("H", 50);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SUB H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(50, Chip.GetRegister("A"), "SUB reg A value is incorrect");
        }

        [Test]
        public void SUB_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SUB 2");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void SBB_Good1()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 4);
            Chip.SetRegister("L", 2);
            Chip.SetConditionalBit("CarryBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SBB L");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.GetRegister("A"), "SBB reg A value is incorrect");
        }

        [Test]
        public void SBB_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 4);
            Chip.SetRegister("L", 2);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SBB L");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(2, Chip.GetRegister("A"), "SBB reg A value is incorrect");
        }

        [Test]
        public void SBB_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SBB 2");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ANA_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 252);
            Chip.SetRegister("C", 15);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ANA C");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(12, Chip.GetRegister("A"), "ANA reg A value is incorrect");
        }

        [Test]
        public void ANA_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ANA");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void XRA_Good1()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 252);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XRA A");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(0, Chip.GetRegister("A"), "XRA reg A value is incorrect");
        }

        [Test]
        public void XRA_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 255);
            Chip.SetRegister("B", 254);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XRA B");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.GetRegister("A"), "XRA reg A value is incorrect");
        }

        [Test]
        public void XRA_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XRA");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ORA_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 51);
            Chip.SetRegister("C", 15);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ORA C");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(63, Chip.GetRegister("A"), "ORA reg A value is incorrect");
        }

        [Test]
        public void ORA_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ORA");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CMP_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 10);
            Chip.SetRegister("E", 5);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMP E");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(false, Chip.GetConditionalBit("CarryBit"), "CMP CarryBit value is incorrect");
            Assert.AreEqual(false, Chip.GetConditionalBit("ZeroBit"), "CMP ZeroBit value is incorrect");
        }

        [Test]
        public void CMP_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CMP");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RLC_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 242);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RLC");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(229, Chip.GetRegister("A"), "RLC reg A value is incorrect");
            Assert.AreEqual(true, Chip.GetConditionalBit("CarryBit"), "RLC CarryBit value is incorrect");
        }

        [Test]
        public void RLC_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RLC A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RRC_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 242);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RRC");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(121, Chip.GetRegister("A"), "RRC reg A value is incorrect");
            Assert.AreEqual(false, Chip.GetConditionalBit("CarryBit"), "RRC CarryBit value is incorrect");
        }

        [Test]
        public void RRC_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RRC A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RAL_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 181);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RAL");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(106, Chip.GetRegister("A"), "RAL reg A value is incorrect");
            Assert.AreEqual(true, Chip.GetConditionalBit("CarryBit"), "RAL CarryBit value is incorrect");
        }

        [Test]
        public void RAL_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RAL A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RAR_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 106);
            Chip.SetConditionalBit("CarryBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RAR");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(181, Chip.GetRegister("A"), "RAR reg A value is incorrect");
            Assert.AreEqual(false, Chip.GetConditionalBit("CarryBit"), "RAR CarryBit value is incorrect");
        }

        [Test]
        public void RAR_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RAR A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void PUSH_Good1()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 31);
            Chip.StackPointer = 20522;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("PUSH PSW");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(31, Chip.GetMemory(20521), "PUSH reg A value is incorrect");
            Assert.AreEqual(20520, Chip.StackPointer, "PUSH StackPointer value is incorrect");
        }

        [Test]
        public void PUSH_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("D", 143);
            Chip.SetRegister("E", 157);
            Chip.StackPointer = 14892;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("PUSH D");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(143, Chip.GetMemory(14891), "PUSH reg D value is incorrect");
            Assert.AreEqual(157, Chip.GetMemory(14890), "PUSH reg E value is incorrect");
            Assert.AreEqual(14890, Chip.StackPointer, "PUSH StackPointer value is incorrect");
        }

        [Test]
        public void PUSH_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("PUSH 1h");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void POP_Good1()
        {
            ClearChipAndLabels();
            Chip.SetMemory(4665, 61);
            Chip.SetMemory(4666, 147);
            Chip.StackPointer = 4665;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("POP H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(61, Chip.GetRegister("L"), "POP reg L value is incorrect");
            Assert.AreEqual(147, Chip.GetRegister("H"), "POP reg H value is incorrect");
            Assert.AreEqual(4667, Chip.StackPointer, "POP StackPointer value is incorrect");
        }

        [Test]
        public void POP_Good2()
        {
            ClearChipAndLabels();
            Chip.SetMemory(11264, 195);
            Chip.SetMemory(11265, 255);
            Chip.StackPointer = 11264;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("POP PSW");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(255, Chip.GetRegister("A"), "POP reg A value is incorrect");
            Assert.AreEqual(11266, Chip.StackPointer, "POP StackPointer value is incorrect");
        }

        [Test]
        public void POP_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("POP 1h");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void DAD_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("B", 51);
            Chip.SetRegister("C", 159);
            Chip.SetRegister("H", 161);
            Chip.SetRegister("L", 123);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DAD B");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(213, Chip.GetRegister("H"), "DAD reg H value is incorrect");
            Assert.AreEqual(26, Chip.GetRegister("L"), "DAD reg L value is incorrect");
            Assert.AreEqual(false, Chip.GetConditionalBit("CarryBit"), "DAD CarryBit value is incorrect");
        }

        [Test]
        public void DAD_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DAD C");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void INX_Good1()
        {
            ClearChipAndLabels();
            Chip.SetRegister("D", 56);
            Chip.SetRegister("E", 255);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INX D");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(57, Chip.GetRegister("D"), "INX reg D value is incorrect");
            Assert.AreEqual(0, Chip.GetRegister("E"), "INX reg E value is incorrect");
        }

        [Test]
        public void INX_Good2()
        {
            ClearChipAndLabels();
            Chip.StackPointer = 65535;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INX SP");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(0, Chip.StackPointer, "INX StackPointer value is incorrect");
        }

        [Test]
        public void INX_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INX C");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void DCX_Good1()
        {
            ClearChipAndLabels();
            Chip.SetRegister("H", 152);
            Chip.SetRegister("L", 0);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCX H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(151, Chip.GetRegister("H"), "DCX reg H value is incorrect");
            Assert.AreEqual(255, Chip.GetRegister("L"), "DCX reg L value is incorrect");
        }

        [Test]
        public void DCX_Good2()
        {
            ClearChipAndLabels();
            Chip.StackPointer = 0;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCX SP");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(65535, Chip.StackPointer, "DCX StackPointer value is incorrect");
        }

        [Test]
        public void DCX_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("DCX C");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void XCHG_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("H", 0);
            Chip.SetRegister("L", 255);
            Chip.SetRegister("D", 51);
            Chip.SetRegister("E", 85);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XCHG");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(0, Chip.GetRegister("D"), "XCHG reg D value is incorrect");
            Assert.AreEqual(255, Chip.GetRegister("E"), "XCHG reg E value is incorrect");
            Assert.AreEqual(51, Chip.GetRegister("H"), "XCHG reg H value is incorrect");
            Assert.AreEqual(85, Chip.GetRegister("L"), "XCHG reg L value is incorrect");
        }

        [Test]
        public void XCHG_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XCHG A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void XTHL_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("H", 11);
            Chip.SetRegister("L", 60);
            Chip.SetMemory(4269, 16);
            Chip.SetMemory(4270, 12);
            Chip.StackPointer = 4269;

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XTHL");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(60, Chip.GetMemory(4269), "XTHL memory value is incorrect");
            Assert.AreEqual(11, Chip.GetMemory(4270), "XTHL memory value is incorrect");
            Assert.AreEqual(12, Chip.GetRegister("H"), "XTHL reg H value is incorrect");
            Assert.AreEqual(16, Chip.GetRegister("L"), "XTHL reg L value is incorrect");
            Assert.AreEqual(4269, Chip.StackPointer, "XTHL StackPointer value is incorrect");
        }

        [Test]
        public void XTHL_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XTHL A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void SPHL_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("H", 80);
            Chip.SetRegister("L", 108);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SPHL");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(80, Chip.GetRegister("H"), "SPHL reg H value is incorrect");
            Assert.AreEqual(108, Chip.GetRegister("L"), "SPHL reg L value is incorrect");
            Assert.AreEqual(20588, Chip.StackPointer, "SPHL StackPointer value is incorrect");
        }

        [Test]
        public void SPHL_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SPHL A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void LXI_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LXI H, 103H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(1, Chip.GetRegister("H"), "LXI reg H value is incorrect");
            Assert.AreEqual(3, Chip.GetRegister("L"), "LXI reg L value is incorrect");
        }

        [Test]
        public void LXI_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LXI SP, 3ABCH");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(15036, Chip.StackPointer, "LXI StackPointer value is incorrect");
        }

        [Test]
        public void LXI_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LXI A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void MVI_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("MVI H, 3CH");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(60, Chip.GetRegister("H"), "MVI reg H value is incorrect");
        }

        [Test]
        public void MVI_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("H", 60);
            Chip.SetRegister("L", 244);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("MVI M, 0FFH");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(255, Chip.GetMemory(15604), "MVI memory value is incorrect");
        }

        [Test]
        public void MVI_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("MVI A");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ADI_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 20);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADI 42H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(86, Chip.GetRegister("A"), "ADI reg A value is incorrect");
        }

        [Test]
        public void ADI_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ADI SP");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ACI_Good1()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 20);
            Chip.SetConditionalBit("CarryBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ACI 42H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(87, Chip.GetRegister("A"), "ACI reg A value is incorrect");
        }

        [Test]
        public void ACI_Good2()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 20);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ACI 42H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(86, Chip.GetRegister("A"), "ACI reg A value is incorrect");
        }

        [Test]
        public void ACI_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ACI SP");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void SUI_Good()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SUI 1H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(255, Chip.GetRegister("A"), "SUI reg A value is incorrect");
        }

        [Test]
        public void SUI_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SUI SP");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void SBI_Good1()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("CarryBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SBI 1H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(254, Chip.GetRegister("A"), "SBI reg A value is incorrect");
        }

        [Test]
        public void SBI_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SBI 1H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(255, Chip.GetRegister("A"), "SBI reg A value is incorrect");
        }

        [Test]
        public void SBI_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SBI SP");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ANI_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 58);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ANI 0FH");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(10, Chip.GetRegister("A"), "ANI reg A value is incorrect");
        }

        [Test]
        public void ANI_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ANI SP");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void XRI_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 59);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XRI 81H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(186, Chip.GetRegister("A"), "XRI reg A value is incorrect");
        }

        [Test]
        public void XRI_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("XRI SP");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void ORI_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 181);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ORI 0FH");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(191, Chip.GetRegister("A"), "ORI reg A value is incorrect");
        }

        [Test]
        public void ORI_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("ORI SP");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CPI_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 74);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CPI 40H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(74, Chip.GetRegister("A"), "CPI reg A value is incorrect");
            Assert.AreEqual(false, Chip.GetConditionalBit("CarryBit"), "CPI CarryBit value is incorrect");
            Assert.AreEqual(false, Chip.GetConditionalBit("ZeroBit"), "CPI ZeroBit value is incorrect");
        }

        [Test]
        public void CPI_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CPI SP");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void STA_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("A", 74);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STA 5B3H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(74, Chip.GetMemory(1459), "STA memory value is incorrect");
        }

        [Test]
        public void STA_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("STA B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void LDA_Good()
        {
            ClearChipAndLabels();
            Chip.SetMemory(768, 10);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LDA 300H");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(10, Chip.GetRegister("A"), "LDA reg A value is incorrect");
        }

        [Test]
        public void LDA_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LDA B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void SHLD_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("H", 174);
            Chip.SetRegister("L", 41);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SHLD 10AH");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(41, Chip.GetMemory(266), "SHLD memory value is incorrect");
            Assert.AreEqual(174, Chip.GetMemory(267), "SHLD memory value is incorrect");
        }

        [Test]
        public void SHLD_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("SHLD B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void LHLD_Good()
        {
            ClearChipAndLabels();
            Chip.SetMemory(603, 255);
            Chip.SetMemory(604, 3);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LHLD 25BH");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(255, Chip.GetRegister("L"), "LHLD reg L value is incorrect");
            Assert.AreEqual(3, Chip.GetRegister("H"), "LHLD reg H value is incorrect");
        }

        [Test]
        public void LHLD_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("LHLD B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void PCHL_Good()
        {
            ClearChipAndLabels();
            Chip.SetRegister("H", 65);
            Chip.SetRegister("L", 62);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("PCHL");
            Assert.AreEqual(parserMessage, "Success");

            Chip.ProgramCounter = 0;
            string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
            Assert.AreEqual(executionMessage, "Success");

            Assert.AreEqual(16702, Chip.ProgramCounter, "PCHL ProgramCounter value is incorrect");
        }

        [Test]
        public void PCHL_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("PCHL B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void JMP_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JMP 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "JMP reg A value is incorrect");
        }

        [Test]
        public void JMP_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JMP HERE \n INR A \n HERE: INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "JMP reg A value is incorrect");
        }

        [Test]
        public void JMP_Good3()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("INR A \n JMP 0H");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "JMP reg A value is incorrect");
        }

        [Test]
        public void JMP_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JMP B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void JC_Good1()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("CarryBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JC 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "JC reg A value is incorrect");
        }

        [Test]
        public void JC_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JC 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "JC reg A value is incorrect");
        }

        [Test]
        public void JC_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JC B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void JNC_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JNC 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "JNC reg A value is incorrect");
        }

        [Test]
        public void JNC_Good2()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("CarryBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JNC 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "JNC reg A value is incorrect");
        }

        [Test]
        public void JNC_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JNC B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void JZ_Good1()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("ZeroBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JZ 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "JZ reg A value is incorrect");
        }

        [Test]
        public void JZ_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JZ 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "JZ reg A value is incorrect");
        }

        [Test]
        public void JZ_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JZ B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void JNZ_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JNZ 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "JNZ reg A value is incorrect");
        }

        [Test]
        public void JNZ_Good2()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("ZeroBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JNZ 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "JNZ reg A value is incorrect");
        }

        [Test]
        public void JNZ_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JNZ B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void JP_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JP 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "JP reg A value is incorrect");
        }

        [Test]
        public void JP_Good2()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("SignBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JP 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "JP reg A value is incorrect");
        }

        [Test]
        public void JP_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JP B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void JM_Good1()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("SignBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JM 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "JM reg A value is incorrect");
        }

        [Test]
        public void JM_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JM 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "JM reg A value is incorrect");
        }

        [Test]
        public void JM_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JM B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void JPE_Good1()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("ParityBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JPE 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "JPE reg A value is incorrect");
        }

        [Test]
        public void JPE_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JPE 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "JPE reg A value is incorrect");
        }

        [Test]
        public void JPE_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JPE B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void JPO_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JPO 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(1, Chip.GetRegister("A"), "JPO reg A value is incorrect");
        }

        [Test]
        public void JPO_Good2()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("ParityBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JPO 4H \n INR A \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 3; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "JPO reg A value is incorrect");
        }

        [Test]
        public void JPO_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("JPO B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CALL_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL 5H \n HLT \n INR A \n INR A \n INR A \n" +
                                                                                 "RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "CALL reg A value is incorrect");
        }

        [Test]
        public void CALL_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "CALL reg A value is incorrect");
        }

        [Test]
        public void CALL_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CC_Good1()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("CarryBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CC FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "CC reg A value is incorrect");
        }

        [Test]
        public void CC_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CC FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(0, Chip.GetRegister("A"), "CC reg A value is incorrect");
        }

        [Test]
        public void CC_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CC B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CNC_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CNC FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "CNC reg A value is incorrect");
        }

        [Test]
        public void CNC_Good2()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("CarryBit", true);
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CNC FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(0, Chip.GetRegister("A"), "CNC reg A value is incorrect");
        }

        [Test]
        public void CNC_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CNC B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CZ_Good1()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("ZeroBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CZ FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "CZ reg A value is incorrect");
        }

        [Test]
        public void CZ_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CZ FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(0, Chip.GetRegister("A"), "CZ reg A value is incorrect");
        }

        [Test]
        public void CZ_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CZ B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CNZ_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CNZ FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "CNZ reg A value is incorrect");
        }

        [Test]
        public void CNZ_Good2()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("ZeroBit", true);
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CNZ FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(0, Chip.GetRegister("A"), "CNZ reg A value is incorrect");
        }

        [Test]
        public void CNZ_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CNZ B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CM_Good1()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("SignBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CM FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "CM reg A value is incorrect");
        }

        [Test]
        public void CM_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CM FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(0, Chip.GetRegister("A"), "CM reg A value is incorrect");
        }

        [Test]
        public void CM_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CM B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CP_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CP FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "CP reg A value is incorrect");
        }

        [Test]
        public void CP_Good2()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("SignBit", true);
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CP FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(0, Chip.GetRegister("A"), "CP reg A value is incorrect");
        }

        [Test]
        public void CP_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CP B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CPE_Good1()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("ParityBit", true);

            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CPE FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "CPE reg A value is incorrect");
        }

        [Test]
        public void CPE_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CPE FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(0, Chip.GetRegister("A"), "CPE reg A value is incorrect");
        }

        [Test]
        public void CPE_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CPE B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void CPO_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CPO FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "CPO reg A value is incorrect");
        }

        [Test]
        public void CPO_Good2()
        {
            ClearChipAndLabels();
            Chip.SetConditionalBit("ParityBit", true);
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CPO FNK \n HLT \n INR A \n FNK: INR A \n " +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(0, Chip.GetRegister("A"), "CPO reg A value is incorrect");
        }

        [Test]
        public void CPO_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CPO B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RET_Good()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RET \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "RET reg A value is incorrect");
        }

        [Test]
        public void RET_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RET B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RC_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RC \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("CarryBit", true);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "RC reg A value is incorrect");
        }

        [Test]
        public void RC_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RC \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("CarryBit", false);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(3, Chip.GetRegister("A"), "RC reg A value is incorrect");
        }

        [Test]
        public void RC_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RC B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RNC_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RNC \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("CarryBit", false);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "RNC reg A value is incorrect");
        }

        [Test]
        public void RNC_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RNC \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("CarryBit", true);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(3, Chip.GetRegister("A"), "RNC reg A value is incorrect");
        }

        [Test]
        public void RNC_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RNC B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RZ_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RZ \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("ZeroBit", true);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "RZ reg A value is incorrect");
        }

        [Test]
        public void RZ_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RZ \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("ZeroBit", false);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(3, Chip.GetRegister("A"), "RZ reg A value is incorrect");
        }

        [Test]
        public void RZ_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RZ B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RNZ_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RNZ \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("ZeroBit", false);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "RNZ reg A value is incorrect");
        }

        [Test]
        public void RNZ_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RNZ \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("ZeroBit", true);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(3, Chip.GetRegister("A"), "RNZ reg A value is incorrect");
        }

        [Test]
        public void RNZ_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RNZ B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RM_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RM \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("SignBit", true);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "RM reg A value is incorrect");
        }

        [Test]
        public void RM_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RM \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("SignBit", false);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(3, Chip.GetRegister("A"), "RM reg A value is incorrect");
        }

        [Test]
        public void RM_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RM B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RP_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RP \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("SignBit", false);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "RP reg A value is incorrect");
        }

        [Test]
        public void RP_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RP \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("SignBit", true);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(3, Chip.GetRegister("A"), "RP reg A value is incorrect");
        }

        [Test]
        public void RP_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RP B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RPE_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RPE \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("ParityBit", true);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "RPE reg A value is incorrect");
        }

        [Test]
        public void RPE_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RPE \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("ParityBit", false);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(3, Chip.GetRegister("A"), "RPE reg A value is incorrect");
        }

        [Test]
        public void RPE_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RPE B");
            Assert.AreNotEqual(parserMessage, "Success");
        }

        [Test]
        public void RPO_Good1()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RPO \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("ParityBit", false);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(2, Chip.GetRegister("A"), "RPO reg A value is incorrect");
        }

        [Test]
        public void RPO_Good2()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("CALL FNK \n HLT \n INR A \n FNK: INR A \n" +
                                                                                 "INR A \n RPO \n INR A");
            Assert.AreEqual(parserMessage, "Success");
            Chip.ProgramCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                Chip.SetConditionalBit("ParityBit", true);
                string executionMessage = InstrExecuter.ExecuteFromMemoryOnCounter();
                Assert.AreEqual(executionMessage, "Success");

                if (Chip.GetMemory(Chip.ProgramCounter) == 118) // HLT
                {
                    Chip.ProgramCounter++;
                    break;
                }
            }

            Assert.AreEqual(3, Chip.GetRegister("A"), "RPO reg A value is incorrect");
        }

        [Test]
        public void RPO_Bad()
        {
            ClearChipAndLabels();
            string parserMessage = CodeParser.CheckCodeForErrorsAndWriteToMemory("RPO B");
            Assert.AreNotEqual(parserMessage, "Success");
        }
    }
}