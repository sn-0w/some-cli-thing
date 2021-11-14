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
    class Variables
    {
        public static void Execute(ModuleDefMD module)
        {
            int stringcount = 0;
            int intcount = 0;

            foreach (var type in module.GetTypes())
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody || method.Body == null) continue;
                    Dictionary<string, Local> strings = new Dictionary<string, Local>();
                    Dictionary<int, Local> ints = new Dictionary<int, Local>();
                    int addedstrings = 0;
                    int addedints = 0;

                    for (int i = 0; i < method.Body.Instructions.Count(); i++)
                    {
                        var instr = method.Body.Instructions;
                        // strings
                        if (instr[i].OpCode == OpCodes.Ldstr)
                        {
                            if (!strings.ContainsKey(instr[i].Operand.ToString()))
                            {
                                var local1 = new Local(module.CorLibTypes.Int32, Utils.RandomString(16));
                                method.Body.Variables.Add(local1);
                                instr.Insert(0, Instruction.Create(OpCodes.Ldstr, instr[i].Operand.ToString()));
                                addedstrings++;
                                instr.Insert(1, Instruction.Create(OpCodes.Stloc_S, local1));
                                i += 2;
                                strings.Add(instr[i].Operand.ToString(), local1);
                                stringcount++;
                                
                            }
                        }
                        // ints
                        if (instr[i].IsLdcI4())
                        {
                            if (!ints.ContainsKey(instr[i].GetLdcI4Value()))
                            {
                                var local1 = new Local(module.CorLibTypes.String);
                                method.Body.Variables.Add(local1);
                                instr.Insert(0, Instruction.Create(OpCodes.Ldc_I4, instr[i].GetLdcI4Value()));
                                addedints++;
                                instr.Insert(1, Instruction.Create(OpCodes.Stloc_S, local1));
                                i += 2;

                                ints.Add(instr[i].GetLdcI4Value(), local1);
                                intcount++;
                            }
                        }


                    }
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Nop));
                    int s = 0;
                    int iss = 0;
                    for (int i = 0; i < method.Body.Instructions.Count(); i++)
                    {
                        var instr = method.Body.Instructions;
                        //strings
                        if (instr[i].OpCode == OpCodes.Ldstr)
                        {
                            if (s < addedstrings)
                            {
                                s += 1;
                            }
                            else
                            {
                                instr[i].OpCode = OpCodes.Ldloc_S;
                                instr[i].Operand = strings[instr[i].Operand.ToString()];
                            }
                        }
                        //ints
                        if (instr[i].IsLdcI4())
                        {
                            if (iss < addedints)
                            {
                                iss += 1;
                            }
                            else
                            {
                                int localldc = instr[i].GetLdcI4Value();
                                instr[i].OpCode = OpCodes.Ldloc_S;
                                instr[i].Operand = ints[localldc];
                            }
                        }
                    }

                }
            }

            Output.TypeWriterEffect(">> Placed " + stringcount + " Strings in Variables \n", Color.LightGreen);
            Output.TypeWriterEffect(">> Placed " + intcount + " Integers in Variables \n", Color.LightGreen);

        }
    }
}
