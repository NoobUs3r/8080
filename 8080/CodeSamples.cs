namespace _8080
{
    class CodeSamples
    {
        public static string GetCodeSample(string sampleName)
        {
            string code = "Sample not found";

            if (sampleName == "Code samples")
            {
                code = string.Empty;
            }
            else if (sampleName == "Multiply 5 by 25")
            {
                code = "; Note that values are in HEX\n" +
                       "MVI B, 19H\n" +
                       "MVI C, 4H\n" +
                       "MVI E, 19h\n" + 
                       "FNK:\n" +
                       "     MOV A, B\n" +
                       "     ADD E\n" +
                       "     MOV B, A\n" +
                       "     MOV A, C\n" +
                       "     DCR A\n" +
                       "     MOV C, A\n" +
                       "JNZ FNK\n" +
                       "MOV A, B\n" +
                       "HLT";
            }
            else if (sampleName == "Arithmetic progression A to L")
            {
                code = "; Reg A stores 1, B stores 2 etc.\n" +
                       "MVI A, 2H\n" +
                       "MOV B, A\n" +
                       "INR A\n" +
                       "MOV C, A\n" +
                       "INR A\n" +
                       "MOV D, A\n" +
                       "INR A\n" +
                       "MOV E, A\n" +
                       "INR A\n" +
                       "MOV H, A\n" +
                       "INR A\n" +
                       "MOV L, A\n" +
                       "MVI A, 1H\n" +
                       "HLT";
            }
            else if (sampleName == "Calling a DIV function")
            {
                code = "; DIVC function divides reg A by C\n" +
                       "MVI A, 32H\n" +
                       "MVI C, 5H\n" +
                       "CALL DIVC\n" +
                       "CALL DIVC\n" +
                       "HLT\n" +
                       "DIVC:\n" +
                       "     MVI E, 0H\n" +
                       "     LOOP:\n" +
                       "          INR E\n" +
                       "          SUB C\n" +
                       "          JZ EXIT\n" +
                       "          JP LOOP\n" +
                       "          DCR E\n" +
                       "          EXIT:\n" +
                       "               MOV A, E\n" +
                       "               RET";
            }

            return code;
        }
    }
}
