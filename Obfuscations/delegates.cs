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
    class delegates
    {



        public static void Execute(ModuleDefMD module)
        {

            Dictionary<object,FieldDefUser> fieldoperand = new Dictionary<object, FieldDefUser>();
            Dictionary<object, MethodDefUser> invokeoperand = new Dictionary<object, MethodDefUser>();


            List<object> operands = new List<object>();
            foreach (var type in module.GetTypes())
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody || method.Body == null || method.IsConstructor || method.DeclaringType.IsGlobalModuleType) continue;

                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        var instr = method.Body.Instructions;
                        if (!method.Body.Instructions[i].ToString().Contains("ISupportInitialize") && (method.Body.Instructions[i].OpCode == OpCodes.Call || method.Body.Instructions[i].OpCode == OpCodes.Callvirt || method.Body.Instructions[i].OpCode == OpCodes.Ldloc_S))
                        {
                            if (!method.Body.Instructions[i].ToString().Contains("Object")  && (method.Body.Instructions[i].OpCode == OpCodes.Call || method.Body.Instructions[i].OpCode == OpCodes.Callvirt || method.Body.Instructions[i].OpCode == OpCodes.Ldloc_S))
                            {
                                try
                                {
                                    

                                    if (((MemberRef)instr[i].Operand).HasThis) continue;
                                    if (instr[i].Operand.ToString().Contains("String::")) continue;
                                    if (!operands.Contains(instr[i].Operand)) operands.Add(instr[i].Operand);
                                   
                                }
                                catch { }
                            }
                        }


                    }
                }
            }
            /*var gctor = new MethodDefUser(".cctor",
                    MethodSig.CreateStatic(module.CorLibTypes.Void));
            gctor.Attributes = MethodAttributes.HideBySig | MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.ReuseSlot | MethodAttributes.Static;
            gctor.ImplAttributes = MethodImplAttributes.IL | MethodImplAttributes.Managed;
            module.GlobalType.Methods.Add(gctor);*/

                            var cctorglobal = module.GlobalType.FindOrCreateStaticConstructor();
            cctorglobal.Body = new CilBody();
            cctorglobal.Body.Instructions.Add(new Instruction(OpCodes.Nop));

            foreach (var operand in operands)
            {

                try
                {
                    MemberRef memberRef = (MemberRef)operand;




                    var delegate_ = new TypeDefUser(Utils.RandomString(16), Utils.RandomString(16), module.Import(typeof(System.MulticastDelegate)));
                    delegate_.Attributes = TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Class | TypeAttributes.Sealed;
                    module.Types.Add(delegate_);

                    #region .ctor
                    var ctor = new MethodDefUser(".ctor",
                    MethodSig.CreateInstance(module.CorLibTypes.Void, module.CorLibTypes.Object, module.CorLibTypes.IntPtr));
                    ctor.Attributes = MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.ReuseSlot;
                    ctor.ImplAttributes = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
                    ctor.ParamDefs.Add(new ParamDefUser("object", 1));
                    ctor.ParamDefs.Add(new ParamDefUser("method", 2));

                    delegate_.Methods.Add(ctor);
                    #endregion

                    #region beinginvoke
                    var BeingInvoke = new MethodDefUser("BeginInvoke",
                            MethodSig.CreateInstance(module.ImportAsTypeSig(typeof(System.IAsyncResult)), module.CorLibTypes.String, module.ImportAsTypeSig(typeof(System.AsyncCallback)), module.CorLibTypes.Object));
                    BeingInvoke.Attributes = MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot;
                    BeingInvoke.ImplAttributes = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
                    BeingInvoke.ParamDefs.Add(new ParamDefUser("s", 1));
                    BeingInvoke.ParamDefs.Add(new ParamDefUser("callback", 2));
                    BeingInvoke.ParamDefs.Add(new ParamDefUser("object", 2));
                    delegate_.Methods.Add(BeingInvoke);
                    #endregion

                    #region endinvoke
                    var EndInvoke = new MethodDefUser("EndInvoke",
                            MethodSig.CreateInstance(module.CorLibTypes.Void, module.ImportAsTypeSig(typeof(System.IAsyncResult))));
                    EndInvoke.Attributes = MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot;
                    EndInvoke.ImplAttributes = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
                    EndInvoke.ParamDefs.Add(new ParamDefUser("result", 1));
                    delegate_.Methods.Add(EndInvoke);
                    #endregion

                    #region invoke
                    List<TypeSig> argTypes = new List<TypeSig>();

                    foreach (var param in memberRef.GetParams())
                    {
                        argTypes.Add(param);
                    }

                    var Invoke = new MethodDefUser("Invoke",
                            MethodSig.CreateInstance(module.CorLibTypes.Void, argTypes.ToArray()));
                    Invoke.Attributes = MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot;
                    Invoke.ImplAttributes = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
                    for (int i = 0; i < memberRef.GetParams().Count(); i++)
                    {

                        Invoke.ParamDefs.Add(new ParamDefUser(memberRef.GetParams()[i].ToString(), (ushort)(i + 1)));
                    }
                    delegate_.Methods.Add(Invoke);
                    #endregion

                    var field1 = new FieldDefUser(Utils.RandomString(16),  new FieldSig(delegate_.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                    // Add it to the type we created earlier
                    module.EntryPoint.DeclaringType.Fields.Add(field1);

                    //var local1 = new Local(delegate_.ToTypeSig());
                   // cctorglobal.Body.Variables.Add(local1);
                   // cctorglobal.Body.Instructions.Add(new Instruction(OpCodes.Nop));
                    cctorglobal.Body.Instructions.Add(new Instruction(OpCodes.Ldnull));
                    cctorglobal.Body.Instructions.Add(new Instruction(OpCodes.Ldftn, memberRef));
                    cctorglobal.Body.Instructions.Add(new Instruction(OpCodes.Newobj, ctor));
                    cctorglobal.Body.Instructions.Add(new Instruction(OpCodes.Stsfld, field1));
                    fieldoperand.Add(operand, field1);
                    invokeoperand.Add(operand, Invoke);


                }
                catch { }
            }
            cctorglobal.Body.Instructions.Add(new Instruction(OpCodes.Ret));

            foreach (var type in module.GetTypes())
            {
                for (int ii=0;ii<type.Methods.Count;ii++)
                {
                    var method = type.Methods[ii];
                    if (!method.HasBody || method.Body == null || method.IsConstructor || method.DeclaringType.IsGlobalModuleType) continue;

                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        var instr = method.Body.Instructions;
                        if (!method.Body.Instructions[i].ToString().Contains("ISupportInitialize") && (method.Body.Instructions[i].OpCode == OpCodes.Call || method.Body.Instructions[i].OpCode == OpCodes.Callvirt || method.Body.Instructions[i].OpCode == OpCodes.Ldloc_S))
                        {
                            if (!method.Body.Instructions[i].ToString().Contains("Object") && (method.Body.Instructions[i].OpCode == OpCodes.Call || method.Body.Instructions[i].OpCode == OpCodes.Callvirt || method.Body.Instructions[i].OpCode == OpCodes.Ldloc_S))
                            {
                                try
                                {
                                    var old_operand = instr[i].Operand;
                                    if (fieldoperand.ContainsKey(instr[i].Operand))
                                    {
                                        var methImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
                                        var methFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;
                                        var meth1 = new MethodDefUser(Utils.RandomString(16), 
                                            MethodSig.CreateStatic(invokeoperand[instr[i].Operand].ReturnType, invokeoperand[instr[i].Operand].GetParams().ToArray()), methImplFlags, methFlags);
                                        meth1.Body = new CilBody();

                                        meth1.Body.Instructions.Add(new Instruction(OpCodes.Nop));
                                        meth1.Body.Instructions.Add(new Instruction(OpCodes.Ldsfld, fieldoperand[old_operand]));
                                        foreach(var param in meth1.Parameters)
                                        {
                                            meth1.Body.Instructions.Add(new Instruction(OpCodes.Ldarg_S,param));
                                        }
                                        meth1.Body.Instructions.Add(new Instruction(OpCodes.Callvirt, invokeoperand[instr[i].Operand]));
                                        meth1.Body.Instructions.Add(new Instruction(OpCodes.Ret));



                                        type.Methods.Add(meth1);
                                        ii++;
                                        instr[i].OpCode = OpCodes.Call;
                                         instr[i].Operand = meth1;


                                        //break;
                                    }
                                }
                                catch { }
                            }
                        }


                    }
                }
            }







            Output.TypeWriterEffect(">> Added " + operands.Count() + " Delegates \n", Color.LightGreen);

        }
    }
}
