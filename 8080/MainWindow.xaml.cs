using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _8080
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateRegWindows();
        }

        DataTable dt;

        private void Window_Loaded(object sender, RoutedEventArgs e)

        {
            dt = new DataTable("memoryTable");

            string[] columnHeaders = new string[] {"\\", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                                   "A", "B", "C", "D", "E", "F"};

            int cols = columnHeaders.Length;
            int rows = 17;

            for (int i = 0; i < cols; i++)
            {
                DataColumn col = new DataColumn(columnHeaders[i], typeof(string));
                dt.Columns.Add(col);
            }
            
            for (int j = 0; j < rows; j++)
            {
                DataRow dr = dt.NewRow();

                for (int m = 0; m < cols; m++)
                {
                    if (m == 0)
                        dr[m] = j.ToString("X");
                }

                dt.Rows.Add(dr);
            }

            memoryTable.ItemsSource = dt.DefaultView;
            UpdateMemoryWindow();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            string parserMessage = CodeParser.CheckCodeForErrorsAndExecute(CodeBox.Text);

            if (parserMessage != "Success")
                MessageBox.Show(parserMessage);

            UpdateRegWindows();
            UpdateMemoryWindow();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearRegValues();
            UpdateRegWindows();
            ClearMemoryValues();
            UpdateMemoryWindow();
        }

        private void UpdateRegWindows()
        {
            regA_Value.Text = Chip.registers["A"].ToString();
            regB_Value.Text = Chip.registers["B"].ToString();
            regC_Value.Text = Chip.registers["C"].ToString();
            regD_Value.Text = Chip.registers["D"].ToString();
            regE_Value.Text = Chip.registers["E"].ToString();
            regH_Value.Text = Chip.registers["H"].ToString();
            regL_Value.Text = Chip.registers["L"].ToString();
        }

        private void UpdateMemoryWindow()
        {
            int address = memoryWindowAddressStart;
            int rowHeader = memoryWindowRowHeaderStart;
            int hex10000 = 65536;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    // Last row must be empty
                    if (address == hex10000)
                    {
                        dt.Rows[i][j] = "-";
                        continue;
                    }

                    if (j == 0)
                    {
                        string hexRowHeaderValue = rowHeader.ToString("X");

                        if (hexRowHeaderValue.Length == 1)
                            hexRowHeaderValue = "00" + hexRowHeaderValue;
                        else if (hexRowHeaderValue.Length == 2)
                            hexRowHeaderValue = "0" + hexRowHeaderValue;

                        dt.Rows[i][j] = hexRowHeaderValue;
                        rowHeader++;
                    }
                    else
                    {
                        dt.Rows[i][j] = Chip.memory[address];
                        address++;
                    }
                }
            }
        }

        private void ClearRegValues()
        {
            Chip.registers["A"] = 0;
            Chip.registers["B"] = 0;
            Chip.registers["C"] = 0;
            Chip.registers["D"] = 0;
            Chip.registers["E"] = 0;
            Chip.registers["H"] = 0;
            Chip.registers["L"] = 0;
        }

        private void ClearMemoryValues()
        {
            for (int i = 0; i < Chip.memory.Length; i++)
            {
                Chip.memory[i] = 0;
            }
        }

        int memoryWindowAddressStart = 0;
        int memoryWindowRowHeaderStart = 0;
        private void UpdateMemoryRowsButton_Click(object sender, RoutedEventArgs e)
        {
            int hexFF0 = 4080;
            string fullAddress = memoryTableStart_Value.Text;
            string rowAddress = fullAddress;

            if (!Int32.TryParse(fullAddress, System.Globalization.NumberStyles.HexNumber, null, out _))
            {
                MessageBox.Show("ERROR: Invalid memory window address format");
                return;
            }

            // Last char is column
            if (rowAddress.Length > 1)
                rowAddress = rowAddress[0..^1];

            int rowAddress_Int = Int32.Parse(rowAddress, System.Globalization.NumberStyles.HexNumber);
            int fullAddress_Int = Int32.Parse(fullAddress, System.Globalization.NumberStyles.HexNumber);

            if (rowAddress_Int > hexFF0)
            {
                rowAddress_Int = hexFF0;
                memoryTableStart_Value.Text = "FF0";
            }
            else if (rowAddress_Int < 0)
            {
                rowAddress_Int = 0;
                memoryTableStart_Value.Text = "0";
            }

            fullAddress = rowAddress_Int.ToString("X");

            if (!fullAddress.StartsWith("0"))
                fullAddress += "0";

            memoryWindowAddressStart = Int32.Parse(fullAddress, System.Globalization.NumberStyles.HexNumber);
            memoryWindowRowHeaderStart = rowAddress_Int;
            UpdateMemoryWindow();
        }
    }
}
