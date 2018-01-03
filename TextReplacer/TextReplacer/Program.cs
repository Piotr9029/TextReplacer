using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextReplacer
{
    class Program
    {
        static List<String> LoadFromFile()
        {
            List<String> lines = new List<String>();
            string fileName, filePath, extension;
            string[] extensionList = new string[] { ".csv", ".txt", ".xml", ".html" };

            Console.WriteLine("You are loading from a file({0}). \nPass it as a file name including extension. \nIt should be in the same folder as where you are running this program from.", string.Join(", ", extensionList));
            while (true)
            {
                fileName = Console.ReadLine();
                extension = Path.GetExtension(fileName);
                try
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                    if (extensionList.Contains(extension))
                    {
                        //lines = File.ReadLines(filePath).ToList();
                        string line;
                        using (StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.Open)))
                        {
                            while ((line = sr.ReadLine()) != null)
                            {
                                lines.Add(line);
                            }
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("This program doesn't support this file format. Supported file formats are: ({0});", string.Join(", ", extensionList));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine("Try again to give a file name.");
            }

            return lines;
        }

        static List<String> StandardInput()
        {
            List<String> lines = new List<String>();
            string line;

            Console.WriteLine("You have chosen standard input. \nEnter one or more lines of text. \nPress CTRL+Z to exit.");
            do
            {
                line = Console.ReadLine();
                lines.Add(line);
            } while (line != null);
            lines.Remove(lines.Last());

            return lines;
        }

        static SortedSet<int> RangeInput(List<String> lines)
        {
            SortedSet<int> range = new SortedSet<int>();
            string[] rangeSplit;
            string rangeText;
            int[] rangeNumbers = new int[2];

            Console.WriteLine("Define your range for which input lines to process. \nYou can specify it by a number of a line(e.g.: 5), or range of them(e.g: 3-9).\nIn case range is not specified the whole input will process.\nTo exit leave an empty line and press ENTER.");
            while (true)
            {
                rangeText = Console.ReadLine();

                if (rangeText == "")
                {
                    break;
                }
                else
                {
                    try
                    {
                        rangeSplit = rangeText.Split('-');
                        if (rangeSplit.Length > 2)
                        {
                            Console.WriteLine("Incorrect format. Try a number(e.g.:5), or range(e.g.:5-7) of your input lines.");
                        }
                        else if (rangeSplit.Length == 2)
                        {
                            rangeNumbers[0] = Convert.ToInt16(rangeSplit[0]);
                            rangeNumbers[1] = Convert.ToInt16(rangeSplit[1]);
                            if (rangeNumbers[0] <= lines.Count && rangeNumbers[0] > 0 && rangeNumbers[1] <= lines.Count && rangeNumbers[1] > 0)
                            {
                                if (rangeNumbers[0] <= rangeNumbers[1])
                                {
                                    for (int i = rangeNumbers[0]; i <= rangeNumbers[1]; i++)
                                    {
                                        range.Add(i);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Second value: {1} should be greater than or equal to first: {0}.", rangeNumbers[0], rangeNumbers[1]);
                                }
                            }
                            else
                            {
                                Console.WriteLine("The input doesn't have those lines, your input lines starts from 1 and ends at {0}.", lines.Count);
                            }
                        }
                        else
                        {
                            rangeNumbers[0] = Convert.ToInt16(rangeSplit[0]);
                            if (rangeNumbers[0] <= lines.Count && rangeNumbers[0] > 0)
                            {
                                range.Add(rangeNumbers[0]);
                            }
                            else
                            {
                                Console.WriteLine("The input doesn't have this line, your input lines starts from 1 and ends at: {0}.", lines.Count);
                            }
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Incorrect format. Try a number(e.g.:5), or range(e.g.:5-7) of your input lines.");

                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Largest/Smallest supported value is +/-{0}.", Int16.MaxValue);
                    }
                }
            }

            if (!range.Any())
            {
                for (int i = 1; i <= lines.Count; i++)
                {
                    range.Add(i);
                }
            }

            return range;
        }

        static List<String> ReplaceText(List<String> lines, SortedSet<int> range, string[] args)
        {
            List<String> replacedLines = new List<String>();
            int i, j = 0;

            while (j < range.Count)
            {
                i = range.ElementAt(j);
                for (int k = 0; k < args.Length - 1; k += 2)
                {
                    lines[i - 1] = lines[i - 1].Replace(args[k], args[k + 1]);
                }
                replacedLines.Add(lines[i - 1]);
                j++;
            }

            return replacedLines;
        }

        static void SaveToFile(List<String> lines)
        {
            string fileName, filePath, extension;
            string[] extensionList = new string[] { ".csv", ".txt", ".xml", ".html" };

            Console.WriteLine("You are saving to a file({0}).\nPass it as a file name including extension. \nIt will be saved to the same folder as where you are running this program from.", string.Join(", ", extensionList));
            while (true)
            {
                fileName = Console.ReadLine();
                extension = Path.GetExtension(fileName);
                try
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                    if (extensionList.Contains(extension))
                    {
                        using (StreamWriter sw = new StreamWriter(new FileStream(filePath, FileMode.Create)))
                        {
                            foreach (string line in lines)
                            {
                                sw.WriteLine(line);
                            }
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("This program doesn't support this file format. Supported file formats are: ({0}).", string.Join(", ", extensionList));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Try again to give a file name.");
            }
            Console.WriteLine("Your new text has been saved to: " + filePath);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("This is a program for replacing parts of text in given input. \n" +
                "In order to replace parts of text declare them as command line arguments. \n" +
                "Odd arguments are replaced with even arguments proceeding right after them,\n" +
                "e.g.:\"name.exe dog cat ; ,\"=> dog gets replaced with cat, then ; with ,\n" +
                "In case there's no proceeding argument after odd one it will remain unchanged.\n\n");
            if (args.Length < 2)
            {
                Console.WriteLine("There aren't enough arguments in command line, please add at least two to proceed with this program, e.g.: programname.exe dog cat \nPress key to exit program.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            Console.WriteLine("******************************Program begins here*******************************");

            List<String> lines = new List<String>();
            SortedSet<int> range = new SortedSet<int>();
            int choice;
            string choiceText;

            Console.WriteLine("Would you like to load from a file, or standard input?\nEnter: 1 - File, 2 - Standard");
            while (true)
            {
                choiceText = Console.ReadLine();
                if (choiceText == "1" || choiceText == "2")
                {
                    break;
                }
                Console.WriteLine("Please choose between: 1 - File, 2 - Standard");
            }

            choice = Convert.ToInt16(choiceText);
            if (choice == 1)
            {
                lines = LoadFromFile();
            }
            else
            {
                lines = StandardInput();
            }

            range = RangeInput(lines);
            lines = ReplaceText(lines, range, args);

            Console.WriteLine("Would you like to save it to a file, or standard output?\nEnter: 1 - File, 2 - Standard");
            while (true)
            {
                choiceText = Console.ReadLine();
                if (choiceText == "1" || choiceText == "2")
                {
                    break;
                }
                Console.WriteLine("Please choose between: 1 - File, 2 - Standard");
            }

            choice = Convert.ToInt16(choiceText);
            if (choice == 1)
            {
                SaveToFile(lines);
            }
            else
            {
                Console.WriteLine("Your new text:");
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            Console.WriteLine("Press key to exit program");
            Console.ReadKey();
        }

    }
}