using System;
using System.Collections;

namespace CsvTools
{
    public static class CsvTools
    {
        /* Takes the given file path and creates a 2D string array
         * If string[x][y] the x array represents the rows, the y array represents the columns
         * path is the file path to the CSV and err will return -1 on error and 0 on success
         */
        public static string[][] CsvFileToStringArray(string path, out int err)
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

            return CsvStringToStringArray(fileText, out err);
            
        }

        /* Takes a string[] where each index represents a newline and returns a 2D array
         * If string[x][y] the x array represents the rows, the y array represents the columns
         * data is the CSV data and err will return -1 on error and 0 on success
         */
        public static string[][] CsvStringToStringArray(string[] data, out int err)
        {
            string[][] result = new string[data.Length][];

            // reading the file line by line
            for (int i = 0; i < data.Length; i++)
            {
                // used to hold the current row, each comma seprated value is it's own List item
                List<string> line = new List<string>();
                line.Add("");

                // moving through line
                for (int j = 0; j < data[i].Length; j++)
                {
                    // a "
                    if (data[i][j] == '\u0022')
                    {
                        int endPos;
                        string literal = ReadStringLiteral(data[i].Substring(j), out endPos);
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
                            j += data[i].Substring(j).IndexOf('\u002C');
                        }
                    }
                    // a ,
                    else if (data[i][j] == '\u002C')
                        // the end of the column was found, start a new column
                        line.Add("");
                    else
                        line[line.Count - 1] += data[i][j];
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

        // Overloads function that has a leadingString argument, assumes that leadingString is empty
         
        public static string StringArrayToCsvString(string[][] data)
        {
            return StringArrayToCsvString(data, "");
        }

        /* Takes a 2d array and creates a single string where each cell is delimited by a comma
         * If string[x][y] the x array represents the rows, the y array represents the columns
         * leadingString is used if something is wanted to be added to the front of every cell (like spaces)
         */
        public static string StringArrayToCsvString(string[][] data, string leadingString)
        {
            string result = "";

            foreach (string[] line in data)
                for (int i = 0; i < line.Length; i++)
                {
                    if (i > 0)
                        result += ",";

                    // a "
                    if (line[i].Contains('\u0022'))
                    {
                        // add double quote at the begining and end and escape all double quotes
                        result += '\u0022';
                        line[i] = line[i].Replace("\u0022", "\u0022\u0022");
                        line[i] += '\u0022';
                    }
                    // a ,
                    else if (line[i].Contains('\u002C'))
                    {
                        // add double quote at the begining and end
                        result += '\u0022';
                        line[i] += '\u0022';
                    }

                    result = result + leadingString + line[i];
                }
            return result;
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            int err;
            string[][] file = CsvTools.CsvFileToStringArray("test.csv", out err);
            if (err == -1)
            {
                Console.WriteLine("had an error");
                Environment.Exit(-1);
            }

            Console.WriteLine(CsvTools.StringArrayToCsvString(file));

        }
    }
}