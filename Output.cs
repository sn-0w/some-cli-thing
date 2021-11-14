using Colorful;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace Obfuscator
{
    public class Output
    {
        public static void WriteCentered(string strin)
        {
            Console.SetCursorPosition((Console.WindowWidth - strin.Length) / 2, Console.CursorTop);
            Console.Write(strin);
        }

        public static void WriteCentered(string strin, Color color)
        {
            Console.SetCursorPosition((Console.WindowWidth - strin.Length) / 2, Console.CursorTop);
            Console.Write(strin, color);
        }

        public static void WriteCenteredTypeWriter(string strin)
        {
            for (int i = 0; i < strin.Length; i++)
            {
                Console.SetCursorPosition(((Console.WindowWidth - strin.Length) / 2) + i, Console.CursorTop);
                Console.Write(strin[i].ToString());
                Thread.Sleep(35);
            }
        }

        public static void WriteCenteredTypeWriter(string strin, Color color)
        {
            for (int i = 0; i < strin.Length; i++)
            {
                Console.SetCursorPosition(((Console.WindowWidth - strin.Length) / 2) + i, Console.CursorTop);
                Console.Write(strin[i].ToString(), color);
                Thread.Sleep(35);
            }
        }

        public static void spacer(int sleep = 0)
        {
            Console.WriteLine();
            Thread.Sleep(sleep);

        }

        public static void TypeWriterEffect(string strin, Color color)
        {




            for (int i = 0; i < strin.Length; i++)
            {
                Console.Write(strin[i].ToString(), color);
                Thread.Sleep(35);
            }

        }

        public static void TypeWriterEffect(string strin)
        {


            for (int i = 0; i < strin.Length; i++)
            {
                Console.Write(strin[i].ToString());
                Thread.Sleep(35);
            }
        }

        public static DateTime GetLinkerTime(Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        public static void ShowBanner(Color color)
        {
            Console.Clear();
            Output.spacer(100);
            Output.spacer();
            Output.spacer();
            ColorAlternatorFactory alternatorFactory = new ColorAlternatorFactory();
            ColorAlternator alternator = alternatorFactory.GetAlternator(new[] { ".*" }, color);


            Console.WriteAsciiAlternating("     Snow's App", alternator);

            Output.spacer();
            Output.spacer();
            Output.spacer(100);
        }

    }
}
