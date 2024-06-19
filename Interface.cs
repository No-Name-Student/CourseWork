using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Authentication.ExtendedProtection;
using static System.Windows.Forms.LinkLabel;
using System.Windows.Forms.DataVisualization.Charting;

namespace Обратная_задача
{
    public partial class Interface : Form
    {
        // Инициализация массивов для хранения даннх
        string ogFileName;
        Equation currentEqu = new Equation();

        // Инициализация формы
        public Interface()
        {
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"data"));
            InitializeComponent();

        }

        // Проверка корректности введенных данных
        private void CheckData(object sender, EventArgs e)
        {
            try { Double.Parse(((TextBox)sender).Text.Replace('.', ',')); }
            catch (FormatException) { ((TextBox)sender).Text = "0"; }
        }

        // Функция рисования графиков
        private void drawButton_Click(object sender, EventArgs e)
        {
            if (currentEqu.a.Count == 0) { MessageBox.Show("Функция не расчитана", "Ошибка расчета значений", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (Double.Parse(yStartBox.Text.Replace('.', ',')) >=Double.Parse(yEndBox.Text.Replace('.', ',')) ||Double.Parse(xStartBox.Text.Replace('.', ','))>= Double.Parse(xEndBox.Text.Replace('.', ','))|| xInterval.Text=="0"||yInterval.Text=="0")
            { { MessageBox.Show("Введены некорректные значения границ и интервалов", "Ошибка построения графика", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; } }
            chart.ChartAreas[0].AxisY.Minimum= Double.Parse(((TextBox)yStartBox).Text.Replace('.', ','));
            chart.ChartAreas[0].AxisY.Maximum = Double.Parse(((TextBox)yEndBox).Text.Replace('.', ','));
            chart.ChartAreas[0].AxisY.Interval = Double.Parse(((TextBox)yInterval).Text.Replace('.', ','));
            chart.ChartAreas[0].AxisX.Interval = Double.Parse(((TextBox)xInterval).Text.Replace('.', ','));
            for (double i = Double.Parse(((TextBox)xStartBox).Text.Replace('.', ',')); i <= Double.Parse(((TextBox)xEndBox).Text.Replace('.', ',')); i += Double.Parse(((TextBox)xInterval).Text.Replace('.', ',')))
            {
                double fValue = 0, gValue = 0;
                for (int j = 0; j<currentEqu.a.Count;j++)
                {
                    fValue+= Math.Pow(-1, j + 1) * Math.Sin(i + 1 * i) * currentEqu.b[j];
                    gValue += Math.Pow(-1, j + 1) * Math.Sin(i + 1 * i) / (j + 1) * currentEqu.a[j];
                }
                chart.Series[1].Points.AddXY(i, fValue);
                chart.Series[0].Points.AddXY(i, gValue);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (fileName.Text != "Файл не найден")
            {
                currentEqu.SaveFile(ogFileName.Replace(".txt", ""));
                MessageBox.Show($"Сохранено как {ogFileName.Replace(".txt","")}_coefficients.txt в {AppDomain.CurrentDomain.BaseDirectory}", "Файл сохранен", MessageBoxButtons.OK, MessageBoxIcon.Information) ;
            }
            else
            {
                MessageBox.Show("Выберите файл с данными", "Ошибка чтения файла", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Функция стирания нарисованных графиков
        private void eraseButton_Click(object sender, EventArgs e)
        {
            chart.Series[0].Points.Clear();
            chart.Series[1].Points.Clear();
        }

        // Расчет значений вычисленных уравнений
        private void calculateBox_Click(object sender, EventArgs e)
        {
            if ( currentEqu.a.Count==0)
            {
                MessageBox.Show("Функция не расчитана", "Ошибка расчета значений", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else 
            {
                List<double> answers = currentEqu.CalculateValue(Double.Parse(numBox.Text.Replace('.', ',')));
                gxAns.Text = Convert.ToString(answers[1]);
                fxAns.Text = Convert.ToString(answers[0]);
            }
        }

        // Выбор файла с исходными данными и его копирование в папку программы
        private void pickFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Title = "Выберите файл";
            FileDialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"data");
            FileDialog.Filter = "Text file (*.txt)|*.txt";
            FileDialog.FilterIndex = 1;
            FileDialog.ShowDialog();
            if (FileDialog.FileName!="")
            {
                fileName.Text = FileDialog.FileName;
                ogFileName = Path.GetFileName(fileName.Text);
                if (Path.GetDirectoryName(fileName.Text) != Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data"))
                {
                    File.Copy(fileName.Text, $@"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/")}{ogFileName}");
                    MessageBox.Show(ogFileName, ogFileName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                currentEqu.ReadFile(fileName.Text);
            }
            else { fileName.Text = "Файл не найден"; }
        }

        // Функция расчета уравнений
        private void calculateButton_Click(object sender, EventArgs e)
        {
            if (currentEqu.a.Count > 0)
            {
                MessageBox.Show("Коэффиценты уже расчитаны", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (currentEqu.x.Count > 0)
            {
                currentEqu.CalculateСoefficients();
            }
            
            else
            {
                MessageBox.Show("Выберите файл с данными или убедитесь что файл не пуст", "Ошибка чтения файла", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
