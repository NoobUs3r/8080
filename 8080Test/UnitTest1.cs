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

        [Test]
        public void MVI_TestGood()
        {
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("MVI", "A, 9");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int regValue = Chip.registers["A"];
            Assert.AreEqual(regValue, 9, "ERROR: reg doesn't equal MVI instruction operand");
        }

        [Test]
        public void MVI_TestBad()
        {
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("MVI", "A, 9, 8");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            _8080.MainWindow.ExecuteInstructionMethod("MVI", "A, 9");
            int regValue = Chip.registers["A"];
            Assert.AreNotEqual(regValue, 8, "ERROR: reg incorrectly equals MVI instruction operand");
        }

        [Test]
        public void MOV_TestGood()
        {
            Chip.registers["A"] = 9;
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("MOV", "B, A");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int regValue = Chip.registers["B"];
            Assert.AreEqual(regValue, 9, "ERROR: reg doesn't equal MOV instruction operand");
        }

        [Test]
        public void MOV_TestBad()
        {
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("MOV", "A, 9");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            Chip.registers["A"] = 9;
            _8080.MainWindow.ExecuteInstructionMethod("MOV", "B, A");
            int regValue = Chip.registers["B"];
            Assert.AreNotEqual(regValue, 8, "ERROR: reg incorrectly equals MOV instruction operand");
        }

        [Test]
        public void LXI_TestGood()
        {
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("LXI", "B, 253");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int regValue1 = Chip.registers["B"];
            Assert.AreEqual(regValue1, 15, "ERROR: reg1 doesn't equal LXI instruction operand");
            int regValue2 = Chip.registers["C"];
            Assert.AreEqual(regValue2, 13, "ERROR: reg2 doesn't equal LXI instruction operand");
        }

        [Test]
        public void LXI_TestBad()
        {
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("LXI", "B, A");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            _8080.MainWindow.ExecuteInstructionMethod("LXI", "B, 253");
            int regValue1 = Chip.registers["B"];
            Assert.AreNotEqual(regValue1, 16, "ERROR: reg1 incorrectly equals LXI instruction operand");
            int regValue2 = Chip.registers["C"];
            Assert.AreNotEqual(regValue2, 14, "ERROR: reg2 incorrectly equals LXI instruction operand");
        }

        [Test]
        public void LDA_TestGood()
        {
            Chip.memory[13] = 1;
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("LDA", "13");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int regValue = Chip.registers["A"];
            Assert.AreEqual(regValue, 1, "ERROR: reg doesn't equal LDA instruction operand");
        }

        [Test]
        public void LDA_TestBad()
        {
            Chip.memory[13] = 1;
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("LDA", "300");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            _8080.MainWindow.ExecuteInstructionMethod("LDA", "13");
            int regValue = Chip.registers["A"];
            Assert.AreNotEqual(regValue, 2, "ERROR: reg incorrectly equals LDA instruction operand");
        }

        [Test]
        public void STA_TestGood()
        {
            Chip.registers["A"] = 13;
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("STA", "255");
            Assert.AreEqual(instructionMessage, "Success", instructionMessage);
            int addressValue = Chip.memory[255];
            Assert.AreEqual(addressValue, 13, "ERROR: reg doesn't equal STA instruction address value");
        }

        [Test]
        public void STA_TestBad()
        {
            Chip.registers["A"] = 13;
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("STA", "A");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            _8080.MainWindow.ExecuteInstructionMethod("STA", "255");
            int addressValue = Chip.memory[255];
            Assert.AreNotEqual(addressValue, 15, "ERROR: reg incorrectly equals STA instruction address value");
        }

        [Test]
        public void LHLD_TestGood()
        {
            Chip.memory[0] = 13;
            Chip.memory[1] = 15;
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("LHLD", "0");
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
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("LHLD", "A");
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
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("SHLD", "0");
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
            string instructionMessage = _8080.MainWindow.ExecuteInstructionMethod("SHLD", "A");
            Assert.AreNotEqual(instructionMessage, "Success", instructionMessage);
            int lAddress = Chip.memory[0];
            int hAddress = Chip.memory[1];
            Assert.AreNotEqual(lAddress, 14, "ERROR: L reg incorrectly equals LHLD instruction address value");
            Assert.AreNotEqual(hAddress, 16, "ERROR: H reg incorrectly equals LHLD instruction address value");
        }
    }
}