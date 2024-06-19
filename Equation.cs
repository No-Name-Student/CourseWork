using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Обратная_задача
{
    internal class Equation
    {
        public List<double> x = new List<double>(), y = new List<double>(), a = new List<double>(), b = new List<double>();
        double[] ysin, ycos; double a0;
        int accuracy = 10000;
        public void ReadFile(string name)
        {
            bool answerFlag = false;
            int currentLine;
            x.Clear(); y.Clear(); a.Clear(); b.Clear();
            string[] header = File.ReadLines(name).First().Split(' ');
            if (header[0] == "a" && header[1] == "b") answerFlag = true;
            StreamReader reader = new StreamReader(name);
            if (answerFlag)
            {
                header = reader.ReadLine().Split(' ');
                currentLine = 1;
                while (!reader.EndOfStream)
                {
                    currentLine++;
                    string[] line = reader.ReadLine().Split(' ');
                    try
                    {
                        a.Add(Double.Parse(line[0].Replace('.', ',')));
                        b.Add(Double.Parse(line[1].Replace('.', ',')));
                    }
                    catch (FormatException) { MessageBox.Show($"Убедитесь, что файл не поврежден (строка {currentLine})", "Ошибка чтения файла", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                }
            }
            else
            {
                currentLine = 0;
                while (!reader.EndOfStream)
                {
                    currentLine++;
                    string[] line = reader.ReadLine().Split(' ');
                    try
                    {
                        x.Add(Double.Parse(line[0].Replace('.', ',')));
                        y.Add(Double.Parse(line[1].Replace('.', ',')));
                    }
                    catch (FormatException) { MessageBox.Show($"Убедитесь, что файл не поврежден (строка {currentLine})", "Ошибка чтения файла", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                }
            }
            reader.Close();
        }

        public void CalculateСoefficients()
        {
            a0 = 2.0 / y.Count * y.Sum();
            double l = x.Last();
            double[] fsin = new double[x.Count], fcos = new double[x.Count];
            for (int i = 0; i < accuracy; i++)
            {
                for (int j = 0; j < x.Count; j++)
                {
                    fsin[j] = y[j] * Math.Sin(i + 1 * x[j] * Math.PI / l);
                    fcos[j] = y[j] * Math.Cos(i + 1 * x[j] * Math.PI / l);
                }
                a.Add(2.0 / y.Count * fcos.Sum());
                b.Add(2.0 / y.Count * fsin.Sum());
            }
        }

        public List<double> CalculateValue(double x)
        {
            List<double> rValue = new List<double>();
            rValue.Add(0); rValue.Add(0);
            for (int i = 0; i < a.Count; i++)
            {
                rValue[0] += Math.Pow(-1, i + 1) * Math.Sin(i + 1 * x) * b[i];
                rValue[1] += Math.Pow(-1, i + 1) * Math.Sin(i + 1 * x) / (i + 1) * a[i];
            }
            return rValue;
        }

        public void SaveFile (string name)
        {
            StreamWriter writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/") + name + "_coefficients.txt");
            writer.WriteLine("a b");
            for (int i = 0; i < a.Count; i++)
            {
                writer.WriteLine(a[i]+" " + b[i]);
            }
            writer.Close();
        }
    }
}
