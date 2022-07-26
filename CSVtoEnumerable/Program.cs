using System;
using System.Collections;

namespace CSVtoEnumerable
{
    public static class CsvToEnumerable
    {
        // takes the given file path and creates a 2D string array with the CSV divided up
        public static string[][] ToStringArr(string path, out int err)
        {
            string[] fileText;
            try
            {
                fileText = File.ReadAllLines(path);
            }
            catch (Exception e)
            {
                err = -1;
                Console.WriteLine($"{e.GetType().Name}: {e.Message}");
                return new string[0][];
            }

            string[][] result = new string[fileText.Length][];

            // reading the file
            for(int i = 0; i < fileText.Length; i++)
            {
                // used to hold the current row, each comma seprated value is it's own List item
                List<string> line = new List<string>();
                line.Add("");

                // moving through line
                for (int j = 0; j < fileText[i].Length; j++)
                {
                    // a "
                    if (fileText[i][j] == '\u0022')
                    {
                        int endPos;
                        string literal = ReadStringLiteral(fileText[i].Substring(j), out endPos);
                        // reached the end of the line without a closing double quote
                        if (endPos == -1)
                            Environment.Exit(-1);
                        // add the result, move line position to end of the discovered string
                        else
                        {
                            line[line.Count - 1] += literal;
                            line.Add("");
                            j += endPos;
                            // this will ignore any characters inbetween the closing double quote and the next comma
                            j += fileText[i].Substring(j).IndexOf('\u002C');
                        }
                    }
                    // a ,
                    else if (fileText[i][j] == '\u002C')
                        // the end of the column was found, start a new column
                        line.Add("");
                    else
                        line[line.Count - 1] += fileText[i][j];
                }
                result[i] = line.ToArray();
            }
            err = 0;
            return result;
        }

        /* finds the part of the passed string (str) that is encapsulated by double quotes (\u0022)
         * to escape a double quote it must be preceded by a double quote
         * it returns the encapsulated string
         * it sets the passed int (endOfLiteral) as the position in str immediately after the double quote
         */

        private static string ReadStringLiteral(string str, out int endOfLiteral)
        {
            string result = "";
            int pos = str.IndexOf('\u0022') + 1;

            for (; pos < str.Length; pos++)
            {
                if (str[pos] == '\u0022')
                {
                    // Checks if it is at the end of the line
                    if (pos + 1 == str.Length)
                    {
                        endOfLiteral = pos + 1;
                        return result;
                    }
                    // checks if the found double quote was to escape the follow double quote
                    else if (str[pos + 1] == '\u0022')
                        pos++;
                    else
                    {
                        endOfLiteral = pos + 1;
                        return result;
                    }
                }
                result += str[pos];
            }

            // no encapsulated string found
            endOfLiteral = -1;
            return "";
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            int err;
            string[][] file = CsvToEnumerable.ToStringArr("test.csv", out err);

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