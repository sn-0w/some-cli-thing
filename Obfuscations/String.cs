using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Obfuscations
{
    class String
    {
        public static void Execute(ModuleDefMD module)
        {
            int stringcount = 0;
            foreach (TypeDef type in module.GetTypes())
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody || method.Body == null) continue;

                    IList<Instruction> instr = method.Body.Instructions;

                    for (int i = 0; i < instr.Count; i++)
                    {
                        try
                        {
                            if (!(method.Body.Instructions[i].OpCode == OpCodes.Ldstr)) continue;

                            //locals
                            List<Instruction> instrreal = new List<Instruction>();
                            int count = 0;

                            //add variable
                            Random dg = new Random();
                            var local = new Local(method.Module.Import(typeof(string[])).ToTypeSig(), Utils.RandomString(64));
                            method.Body.Variables.Add(local);



                            //add array constructor
                            instrreal.Add(new Instruction(OpCodes.Ldc_I4, 128));
                            instrreal.Add(new Instruction(OpCodes.Newarr, method.Module.Import(typeof(string))));
                            instrreal.Add(new Instruction(OpCodes.Stloc_S, local));

                            //add array items
                            foreach (char boi in instr[i].Operand.ToString())
                            {
                                instrreal.Add(new Instruction(OpCodes.Ldloc, local));
                                instrreal.Add(new Instruction(OpCodes.Ldc_I4, count));
                                instrreal.Add(new Instruction(OpCodes.Ldc_I4, (int)boi));
                                instrreal.Add(Instruction.Create(OpCodes.Call, module.Import(typeof(System.Convert).GetMethod("ToChar", new Type[] { typeof(int) }))));
                                instrreal.Add(Instruction.Create(OpCodes.Call, module.Import(typeof(System.Convert).GetMethod("ToString", new Type[] { typeof(char) }))));
                                instrreal.Add(new Instruction(OpCodes.Stelem_Ref));
                                count++;
                            }

                            //actually add stuff
                            int num4 = 0;
                            foreach (Instruction item in instrreal)
                            {
                                method.Body.Instructions.Insert(num4, item);
                                num4++;
                            }

                            //replace original reference
                            instr.Insert(i + num4, new Instruction(OpCodes.Ldloc, local));
                            instr[i + num4 + 1].OpCode = OpCodes.Call;
                            instr[i + num4 + 1].Operand = module.Import(typeof(System.String).GetMethod("Concat", new Type[] { typeof(string[]) }));
                            stringcount++;


                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }

            Output.TypeWriterEffect(">> Protected " + stringcount + " Strings \n", Color.LightGreen);

        }
    }
}
