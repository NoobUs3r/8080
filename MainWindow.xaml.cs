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

            int cols = 17;
            int rows = 16;
            string[] columnHeaders = new string[] {"\\", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                                   "A", "B", "C", "D", "E", "F"};
            string[] rowHeaders = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                                "A", "B", "C", "D", "E", "F"};

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
                        dr[m] = rowHeaders[j];
                    else
                        dr[m] = " ";
                }

                dt.Rows.Add(dr);
            }

            memoryTable.ItemsSource = dt.DefaultView;
            UpdateMemoryWindow();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            CodeParser.Clear();
            CodeParser.CheckCodeForErrors(CodeBox.Text);

            if (CodeParser.errorMessage != string.Empty)
                MessageBox.Show(CodeParser.errorMessage);

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
            int m = 0;

            for (int i = 0; i < 16; i++)
            {
                for (int j = 1; j < 17; j++)
                {
                    /*if (Chip.memory[m].Length == 1)
                        dt.Rows[i][j] = "0" + Chip.memory[m];
                    else*/
                    dt.Rows[i][j] = Chip.memory[m];

                    m++;
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

        /*private void PopulateHexTable()
        {
            int cols = 16;
            int rows = 17;
            string[] columnHeaders = new string[] {" ", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                                   "A", "B", "C", "D", "E", "F"};
            string[] rowHeaders = new string[] {" ", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                                "A", "B", "C", "D", "E", "F"};

            for (int c = 0; c < cols; c++)
                hexTable.Columns.Add(new DataGridTextColumn());

            for (int r = 0; r < rows; r++)
            {
                DataGridRow tr = new DataGridRow();

                for (int c = 0; c < cols; c++)
                {
                    if (c == 0)
                        tr.Cells.Add(new TableCell(new Paragraph(new Run(rowHeaders[r]))));
                    else
                    {
                        if (r == 0)
                            tr.Cells.Add(new TableCell(new Paragraph(new Run(columnHeaders[c]))));
                        else
                            tr.Cells.Add(new TableCell(new Paragraph(new Run("00"))));
                    }
                }

                TableRowGroup trg = new TableRowGroup();
                trg.Rows.Add(tr);
                hexTable.RowGroups.Add(trg);
            }
        }*/
    }
}
