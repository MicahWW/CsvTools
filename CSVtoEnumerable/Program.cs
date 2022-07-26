using System;
using System.Collections;
using System.Linq;


namespace CSVtoEnumerable
{
    public static class CsvToEnumerable
    {
        public static string[][] ToStringArr(string path)
        {
            string[] fileText = File.ReadAllLines(path);
            string[][] result = new string[fileText.Length][];

            // reading the file
            for(int i = 0; i < fileText.Length; i++)
            {
                List<string> line = new List<string>();
                line.Add("");

                for (int j = 0; j < fileText[i].Length; j++)
                {
                    if (fileText[i][j] == '\u0022')
                    {
                        int endPos;
                        string literal = ReadStringLiteral(fileText[i].Substring(j), out endPos);
                        if (endPos == -1)
                            // reached the end of the line without a closing double quote
                            Environment.Exit(-1);
                        else
                        {
                            line[line.Count - 1] += literal;
                            line.Add("");
                            j += endPos;
                        }
                    }
                    else if (fileText[i][j] == '\u002C')
                    {
                        line.Add("");
                        j++;
                    }
                    else
                    {
                        line[line.Count - 1] += fileText[i][j];
                    }
                }

                    result[i] = line.ToArray();
            }

            return result;
        }

        /* finds the part of the passed string (str) that is encapsulated by double quotes (\u0022)
         * to escape a double quote it must be preceded by a double quote
         * it returns the encapsulated string
         * it sets the passed int (endOfLiteral) as the position in str immediately after the double quote
         */

        public static string ReadStringLiteral(string str, out int endOfLiteral)
        {
            string result = "";
            int pos = str.IndexOf('\u0022') + 1;

            for (; pos < str.Length; pos++)
            {
                if (str[pos] == '\u0022')
                {
                    if (pos + 1 == str.Length)
                    {
                        endOfLiteral = pos + 1;
                        return result;
                    }
                    else if (str[pos + 1] == '\u0022')
                        pos++;
                    else
                    {
                        // fix endof
                        endOfLiteral = pos + 1;
                        return result;
                    }
                }
                result += str[pos];

            }

            endOfLiteral = -1;
            return "";
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            string[][] file = CsvToEnumerable.ToStringArr("test.csv");

            foreach (string[] line in file)
            {
                foreach (string s in line)
                {
                    Console.Write("|" + s + "|" + "\t ");
                }
                Console.WriteLine();
            }

        }
    }
}