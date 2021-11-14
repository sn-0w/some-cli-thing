using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using dnlib.PE;

using Console = Colorful.Console;


namespace Obfuscator
{
    class Main
    {
        public static void Mainmenu()
        {
        start:
            Output.ShowBanner(Color.DeepSkyBlue);

            Output.TypeWriterEffect(">> Input Filepath: ", Color.SteelBlue);
            Colorful.Console.ForegroundColor = Color.DeepSkyBlue;
            string path = Console.ReadLine();

            if (!File.Exists(path))
            {
                Output.TypeWriterEffect("Invalid Filepath provided\n", Color.IndianRed);
                Thread.Sleep(1000);
                goto start;
            }
            ModuleDefMD module;
            try
            {
                module = ModuleDefMD.Load(path);
            }
            catch
            {
                Output.TypeWriterEffect("Error Trying to load module\n", Color.IndianRed);
                Thread.Sleep(1000);
                goto start;
            }
            string outputpath = path.Replace(".exe", "-obf.exe");
            outputpath = outputpath.Replace(".dll", "-obf.dll");
            Output.spacer(100);
            Output.TypeWriterEffect(">> Successfully Loaded " + module.Name + " \n", Color.LightGreen);
            Output.TypeWriterEffect(">> Found " + module.GetTypes().Count() + " Types & " + countmethods(module) + " Methods \n", Color.LightGreen);
            Output.spacer();
            string[] options = new string[] { "Renamer", "String-Obfuscation","Variables","Delegates","[ Confirm ]" };
            List<string> checkedstuff = togglemenu(Color.DeepSkyBlue, Color.SteelBlue, options);
            Output.spacer();
            Output.spacer();
            if (checkedstuff.Count == 0)
            {
                Output.TypeWriterEffect(">> No Options Selected... \n", Color.IndianRed);
                Thread.Sleep(1000);
                goto start;
            }

            if (checkedstuff.Contains("Delegates"))
            {
                Output.TypeWriterEffect(">> Executing Delegate Obfuscation... \n", Color.LightGreen);
                Obfuscations.delegates.Execute(module);

            }

            if (checkedstuff.Contains("String-Obfuscation"))
            {
                Output.TypeWriterEffect(">> Executing String Obfuscation... \n", Color.LightGreen);
                Obfuscations.String.Execute(module);

            }

            if (checkedstuff.Contains("Variables"))
            {
                Output.TypeWriterEffect(">> Executing Variable Obfuscation... \n", Color.LightGreen);
                Obfuscations.Variables.Execute(module);

            }


            if (checkedstuff.Contains("Renamer"))
            {
                Output.TypeWriterEffect(">> Executing Renamer... \n", Color.LightGreen);
                Obfuscations.Renamer.Execute(module);

            }




            Output.TypeWriterEffect(">> Saving to "+ outputpath + "\n", Color.LightGreen);
            ModuleWriterOptions opts = new ModuleWriterOptions(module);
            opts.MetadataOptions.Flags = MetadataFlags.KeepOldMaxStack | MetadataFlags.PreserveStandAloneSigRids | MetadataFlags.PreserveExtraSignatureData | MetadataFlags.PreserveAll;
            module.Write(outputpath, opts);
            Output.TypeWriterEffect(">> Sucessfully saved! \n", Color.LightGreen);

            Thread.Sleep(1000);
            goto start;
        }

        public static List<string> togglemenu(Color selectedcolor, Color unselectedcolor, string[] options)
        {
            string checked_ = "[X]";
            string unchecked_ = "[~]";
            string confirm = "[ Confirm ]";
            for (int i =0;i<options.Count();i++)
            {
                if (options[i] == confirm) continue;
                options[i] = unchecked_ + options[i];
            }


            const int startX = 3;
            int startY = Console.CursorTop;
            const int optionsPerLine = 1;
            const int spacingPerLine = 14;

            int currentSelection = 0;
            bool firsttime = true;

            ConsoleKey key;

            Console.CursorVisible = false;
            bool stop = false;
            do
            {
                if (firsttime)
                {
                    for (int i = 0; i < options.Length; i++)
                    {
                        Console.SetCursorPosition(startX + (i % optionsPerLine) * spacingPerLine, startY + i / optionsPerLine);

                        if (i == currentSelection)
                        {


                            Console.ForegroundColor = selectedcolor;

                            Output.TypeWriterEffect(options[i]);


                        }
                        else
                        {

                            Output.TypeWriterEffect(options[i], unselectedcolor);

                        }

                        Console.ResetColor();
                    }
                    firsttime = !firsttime;
                }
                else
                {
                    for (int i = 0; i < options.Length; i++)
                    {
                        Console.SetCursorPosition(startX + (i % optionsPerLine) * spacingPerLine, startY + i / optionsPerLine);

                        if (i == currentSelection)
                        {


                            Console.ForegroundColor = selectedcolor;

                            Console.Write(options[i]);


                        }
                        else
                        {

                            Console.Write(options[i], unselectedcolor);

                        }

                        Console.ResetColor();
                    }
                }

                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            if (currentSelection % optionsPerLine > 0)
                                currentSelection--;
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            if (currentSelection % optionsPerLine < optionsPerLine - 1)
                                currentSelection++;
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            if (currentSelection >= optionsPerLine)
                                currentSelection -= optionsPerLine;
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (currentSelection + optionsPerLine < options.Length)
                                currentSelection += optionsPerLine;
                            break;
                        }
                    case ConsoleKey.Enter:
                        {
                            if(options[currentSelection] == confirm)
                            {
                                stop = !stop;
                            }
                            if (options[currentSelection].StartsWith(unchecked_))
                            {
                                options[currentSelection] = options[currentSelection].Replace(unchecked_, checked_);
                            }
                            else
                            {
                                options[currentSelection] = options[currentSelection].Replace(checked_, unchecked_);
                            }
                            break;
                        }
                }
            } while (!stop);

            Console.CursorVisible = true;

            return checkedoptions(options,checked_);
        }

        public static List<string> checkedoptions(string[] options, string checkedm)
        {
            List<string> checkedthings = new List<string>();
            for (int i = 0; i < options.Length; i++)
            {
                string cur = options[i];
                if (cur.StartsWith(checkedm))
                {
                    checkedthings.Add(cur.Replace(checkedm, string.Empty));
                }
            }

            return checkedthings;
        }


        static int countmethods(ModuleDefMD module)
        {
            int c = 0;
            foreach (var type in module.GetTypes())
            {
                c += type.Methods.Count();
            }
            return c;
        }
    }
}
