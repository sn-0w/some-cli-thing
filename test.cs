using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator
{
    class test
    {


        void testing1()
        {
            test2.sus = Console.WriteLine;
        }

        void testing2()
        {
            test2.sus("poggers");
        }
    }
    
    static class test2
    {
        public delegate void amogus(string s);

        public static amogus sus;


    }
}
