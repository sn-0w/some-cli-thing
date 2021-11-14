using Authentication;
using Colorful;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Console = Colorful.Console;
namespace Obfuscator
{
    class Program
    {
        public static api SussyAuth = new api("Obfuscation", "R6PJnnkoOg", "b0186bbb560f65bb37246a7a0c5d48f4bcd5998caccd07cb48c72300274e7281", "1.0");



        static void Main(string[] args)
        {
            Thread thread = new Thread(namechanger);
            thread.Start();
            SussyAuth.init();

        start:
            Output.ShowBanner(Color.SteelBlue);
            Output.TypeWriterEffect("Welcome to Snows Application.\n", Color.DeepSkyBlue);
            Output.TypeWriterEffect("Please Select your Option: \n", Color.DeepSkyBlue);
            Output.spacer(100);

            string[] options = new string[] { "Login", "Register", "Info", "Exit" };
            int selectedoption = selectmenu(Color.DeepSkyBlue, Color.SteelBlue, options);

            Output.spacer();
            Output.spacer();
            Output.spacer();
            switch(selectedoption)
            {
                case 0:
                    Output.TypeWriterEffect("Username: \n",Color.SteelBlue);
                    Console.ForegroundColor = Color.DeepSkyBlue;
                    string username = Console.ReadLine();
                    Output.TypeWriterEffect("Password: \n", Color.SteelBlue);
                    Console.ForegroundColor = Color.DeepSkyBlue;
                    string password = Console.ReadLine();
                    if(username == "" || password == "")
                    {
                        Console.WriteLine("please dont leave anything empty");
                        Thread.Sleep(1000);
                        goto start;
                    }

                    if (SussyAuth.login(username, password))
                    {
                        Console.Clear();
                        Output.spacer();
                        Output.TypeWriterEffect("Login Successful", Color.LightGreen);
                        Thread.Sleep(100);
                        Obfuscator.Main.Mainmenu();
                    }
                    else
                    {
                        Console.Clear();
                        Output.spacer();
                        Output.TypeWriterEffect("Wrong Username or Password", Color.IndianRed);
                        Thread.Sleep(1000);
                        goto start;
                    }


                        break;
                case 1:
                    Output.TypeWriterEffect("Username: \n", Color.SteelBlue);
                    Console.ForegroundColor = Color.DeepSkyBlue;
                    string username1 = Console.ReadLine();
                    Output.TypeWriterEffect("Password: \n", Color.SteelBlue);
                    Console.ForegroundColor = Color.DeepSkyBlue;
                    string password1 = Console.ReadLine();
                    Output.TypeWriterEffect("License: \n", Color.SteelBlue);
                    Console.ForegroundColor = Color.DeepSkyBlue;
                    string license = Console.ReadLine();
                    if (username1 == "" || password1 == "" || license == "")
                    {
                        Console.Clear();
                        Output.spacer();
                        Output.TypeWriterEffect("please dont leave anything empty",Color.IndianRed);
                        Thread.Sleep(1000);
                        goto start;
                    }
                    else
                    {
                        if (SussyAuth.register(username1, password1, license))
                        {
                            Console.Clear();
                            Output.spacer();
                            Output.TypeWriterEffect("Successfully Registered!", Color.LightGreen);
                            Thread.Sleep(1000);
                            goto start;
                        }
                        else
                        {
                            Console.Clear();
                            Output.spacer();
                            Output.TypeWriterEffect("Error Registering", Color.IndianRed);
                            Thread.Sleep(1000);
                            goto start;
                        }
                    }
                    break;
                case 2:
                    Console.Clear();
                    ColorAlternatorFactory alternatorFactory = new ColorAlternatorFactory();
                    ColorAlternator alternator = alternatorFactory.GetAlternator(new[] { ".*" }, Color.SteelBlue);
                    Console.WriteAsciiAlternating("     Snow's App", alternator);
                    Output.WriteCenteredTypeWriter("Made by Snow#0013\n", Color.DeepSkyBlue);
                    var linkTimeLocal = Output.GetLinkerTime(Assembly.GetExecutingAssembly());
                    Output.WriteCenteredTypeWriter("Compiled "+linkTimeLocal.ToString()+ "\n", Color.DeepSkyBlue);
                    Output.WriteCenteredTypeWriter("Press any Button to return...\n", Color.DeepSkyBlue);
                    Console.Read();
                    goto start;

                    break;
                case 3:
                    Console.Clear();
                    Output.spacer();
                    Output.WriteCentered("Thanks for using Snow's App\n", Color.IndianRed);
                    Output.WriteCentered("Shutting down...\n", Color.IndianRed);
                    Output.spacer(1000);
                    Output.WriteCentered("3",Color.IndianRed);
                    Thread.Sleep(1000);
                    Output.WriteCentered("2", Color.IndianRed);
                    Thread.Sleep(1000);
                    Output.WriteCentered("1", Color.IndianRed);
                    Thread.Sleep(1000);
                    Output.WriteCentered("0", Color.IndianRed);
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                    break;




            }

            Console.ReadLine();


        }

        public static int selectmenu(Color selectedcolor,Color unselectedcolor,string[] options)
        {
            const int startX = 3;
            int startY = Console.CursorTop;
            const int optionsPerLine = 1;
            const int spacingPerLine = 14;

            int currentSelection = 0;

            ConsoleKey key;

            Console.CursorVisible = false;
            bool firsttime = true;
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
                            Output.TypeWriterEffect(">"+options[i]);
                        }
                        else
                        {

                            Output.TypeWriterEffect(" "+options[i], unselectedcolor);
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
                            Console.Write(">"+options[i]);

                        }
                        else
                        {

                            Console.Write(" " + options[i], unselectedcolor);
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
                }
            } while (key != ConsoleKey.Enter);

            Console.CursorVisible = true;

            return currentSelection;
        }

        static void namechanger()
        {
            while (true)
            {
                Console.Title = "[ " + RandomString(16) + " ]";
                Thread.Sleep(50);
            }
        }

        static private readonly Random _random = new Random();

        static public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                if (_random.Next(0, 2) == 0)
                {
                    @char = @char.ToString().ToLower().ToCharArray()[0];
                }
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
 
    }
}
