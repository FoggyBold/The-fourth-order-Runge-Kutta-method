using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

//a + (b - a) * i / (size - 1)

namespace _2._1laba
{
    public class Program
    {
        private struct ToSave
        {
            public double[] X1 { get; set; }
            public double[] Y1 { get; set; }
            public double[] X2 { get; set; }
            public double[] Y2 { get; set; }
            public double[] X3 { get; set; }
            public double[] Y3 { get; set; }
        };

        public static void Main()
        {
            try
            {
                double a = 1.6/*0*/, b; //интервал интегрирования, в левой границе которого поставлено начальное условаие задачи
                double y0 = 4.6/*0*/;//значение, определяющее начальное условие задачи
                int n; //начальное количество подотрезков разбиения интервала [a,b]
                double eps;//заданная точность метода

                Console.Write("Введите правую границу отрезка: ");
                b = Double.Parse(Console.ReadLine());
                Console.Write("Введите число подотрезков разбиения интервала [a,b]: ");
                n = int.Parse(Console.ReadLine());
                Console.Write("Введите точность метода: ");
                eps = double.Parse(Console.ReadLine());

                int temp = 0, maxTemp = 100;
                double tempError = 0, temp2Error = 0;
                bool flag = false;

                double[,] firstMatrix = new double[n, n];
                double[,] matrix1, matrix2;
                do
                {
                    temp++;

                    matrix1 = createMatrix(a, b, y0, n);
                    matrix2 = createMatrix(a, b, y0, n * 2);

                    if(temp == 1)
                    {
                        firstMatrix = matrix1;
                    }

                    if(tempError != 0)
                    {
                        temp2Error = tempError;
                    }
                    tempError = countError(matrix1[1, n - 1], matrix2[1, n * 2 - 1]);

                    n *= 2;

                    if(Math.Abs(tempError - temp2Error) < 0.000000001)
                    {
                        flag = true;
                    }
                } while (tempError > eps && temp < maxTemp && !flag);

                if(tempError <= eps)
                {
                    Console.WriteLine("Error = 0");
                }
                else if(temp == maxTemp)
                {
                    Console.WriteLine("Error = 2");
                }
                else if(flag)
                {
                    Console.WriteLine("Error = 1");
                }

                Console.WriteLine($"x = {matrix2[0, n - 1]}; y = {matrix2[1, n - 1]}");
                drawingGraph(firstMatrix, matrix1, matrix2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void drawingGraph(double[,] matrix1, double[,] matrix2, double[,] matrix3)
        {
            try
            {
                ToSave data = new ToSave();
                double[] x = new double[matrix1.GetLength(1)];
                double[] y = new double[matrix1.GetLength(1)];
                for (int i = 0; i < matrix1.GetLength(1); ++i)
                {
                    x[i] = matrix1[0, i];
                    y[i] = matrix1[1, i];
                }
                data.X1 = x;
                data.Y1 = y;
                x = new double[matrix2.GetLength(1)];
                y = new double[matrix2.GetLength(1)];
                for (int i = 0; i < matrix2.GetLength(1); ++i)
                {
                    x[i] = matrix2[0, i];
                    y[i] = matrix2[1, i];
                }
                data.X2 = x;
                data.Y2 = y;
                x = new double[matrix3.GetLength(1)];
                y = new double[matrix3.GetLength(1)];
                for (int i = 0; i < matrix3.GetLength(1); ++i)
                {
                    x[i] = matrix3[0, i];
                    y[i] = matrix3[1, i];
                }
                data.X3 = x;
                data.Y3 = y;

                string json = JsonSerializer.Serialize(data);
                File.WriteAllText(@"D:\лабы\6 семестр\ЧМ\2.1laba\Save\temp.json", json);
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(@"D:\лабы\6 семестр\ЧМ\2.1laba\visualizationGraphs\visualizationGraphs.py")
                {
                    UseShellExecute = true
                };
                p.Start();
                p.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void printMatrix(double[,] matrix)
        {
            for(int i = 0; i < matrix.GetLength(0); i++)
            {
                for(int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.WriteLine(matrix[i, j]);
                }
                Console.WriteLine();
            }
        }

        private static double[,] createMatrix(double a, double b, double y0, int n)
        {
            double h = (b - a) / n;
            double[,] matrix = new double[2, n];
            matrix[0, 0] = a;
            matrix[1, 0] = y0;
            for (int i = 1; i < n - 1; i++)
            {
                matrix[0, i] = a + i * h;
                matrix[1, i] = RungeKutt(matrix[0, i - 1], matrix[1, i - 1], h);
            }
            matrix[0, n - 1] = b;
            matrix[1, n - 1] = RungeKutt(matrix[0, n - 2], matrix[1, n - 2], h);
            return matrix;
        }

        private static double countError(double yn, double y2n)
        {
            return Math.Abs(yn - y2n) / (2 * 2 * 2 * 2 - 1);
        }

        private static double RungeKutt(double x, double y, double h)
        {
            double k1 = K1(x, y, h);
            double k2 = K2(x, y, h, k1);
            double k3 = K3(x, y, h, k2);
            double k4 = K4(x, y, h, k3);
            return y + (k1 + 2 * k2 + 2 * k3 + k4) / 6;
        }

        private static double K1(double x, double y, double h)
        {
            return h * f(x, y);
        }

        private static double K2(double x, double y, double h, double k1)
        {
            return h * f(x + h / 2, y + k1 / 2);
        }

        private static double K3(double x, double y, double h, double k2)
        {
            return h * f(x + h / 2, y + k2 / 2);
        }

        private static double K4(double x, double y, double h, double k3)
        {
            return h * f(x + h, y + k3);
        }

        private static double f(double x, double y)
        {
            return x - y / 10;
        }

        //private static double f(double x, double y)
        //{
        //    return Math.Cos(y) / (1.5 + x) + 0.1 * y * y;
        //}
    }
}
