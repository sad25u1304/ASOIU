using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        public static int Distance(string str1Param, string str2Param)
        {
            if ((str1Param == null) || (str2Param == null)) return -1;

            int str1Len = str1Param.Length;
            int str2Len = str2Param.Length;

            if ((str1Len == 0) && (str2Len == 0)) return 0;
            if (str1Len == 0) return str2Len;
            if (str2Len == 0) return str1Len;

            string str1 = str1Param.ToUpper();
            string str2 = str2Param.ToUpper();

            int[,] matrix = new int[str1Len + 1, str2Len + 1];

            for (int i = 0; i <= str1Len; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= str2Len; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= str1Len; i++)
            {
                for (int j = 1; j <= str2Len; j++)
                {
                    int symbEqual = 0;
                    if (str1[i - 1] != str2[j - 1])
                        symbEqual = 1;

                    int ins = matrix[i, j - 1] + 1;
                    int del = matrix[i - 1, j] + 1;
                    int subst = matrix[i - 1, j - 1] + symbEqual;

                    matrix[i, j] = Math.Min(Math.Min(ins, del), subst);

                    if (i > 1 && j > 1 &&
                        str1[i - 1] == str2[j - 2] &&
                        str1[i - 2] == str2[j - 1])
                    {
                        matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + symbEqual);
                    }
                }
            }
            return matrix[str1Len, str2Len];
        }

        public static void WriteDistance(string str1Param, string str2Param)
        {
            int d = Distance(str1Param, str2Param);
            Console.WriteLine(str1Param + ", " + str2Param + " -> " + d);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Расстояние Дамерау-Левенштейна");
            Console.WriteLine("Для выхода введите exit");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Введите первую строку: ");
                string s1 = Console.ReadLine();

                if (s1 == "exit")
                {
                    Console.WriteLine("Завершение работы");
                    break;
                }

                Console.Write("Введите вторую строку: ");
                string s2 = Console.ReadLine();

                if (s1 == null || s2 == null)
                {
                    Console.WriteLine("Ошибка ввода");
                    continue;
                }

                WriteDistance(s1, s2);
                Console.WriteLine();
            }
        }
    }
}           