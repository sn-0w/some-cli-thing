using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Obfuscations
{

    class Renamer
    {
        public static void Execute(ModuleDefMD module)
        {
            int typecounter = 0;
            int methodcounter = 0;
            foreach(var type in module.GetTypes())
            {
                if (!type.IsRuntimeSpecialName && !type.IsSpecialName)
                {
                    type.Name = "<AmongᅠUsᅠObfuscator>ᅠ" + Utils.RandomString(16);
                    typecounter++;
                }

                foreach (var method in type.Methods)
                {
                    if (method.IsRuntimeSpecialName || method.IsSpecialName || method.Name.Contains("<")) continue;

                    method.Name = "<AmongᅠUsᅠObfuscator>ᅠ" + Utils.RandomString(16);
                    methodcounter++;
                }
            }
            Output.TypeWriterEffect(">> Renamed "+typecounter+" Types \n", Color.LightGreen);
            Output.TypeWriterEffect(">> Renamed " + methodcounter + " Methods \n", Color.LightGreen);
        }


    }
}
