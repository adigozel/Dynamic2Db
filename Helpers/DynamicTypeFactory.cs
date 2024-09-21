using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.ComponentModel;

namespace Dynamic2Db.Helpers
{
    public static class DynamicTypeFactory
    {
        static AssemblyName aName = new AssemblyName("Dynamic2Db");

        static AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(aName,AssemblyBuilderAccess.Run);

        static ModuleBuilder mb = ab.DefineDynamicModule(aName.Name);
        public static Type CreateType(string name, Dictionary<string,Type> properties)
        {
            TypeBuilder tb = mb.DefineType(name, TypeAttributes.Public);

            foreach (var property in properties)
            {

                FieldBuilder fb = tb.DefineField(
                                     $"_{property.Key.ToLower()}",
                                     property.Value,
                                     FieldAttributes.Private);

                PropertyBuilder pb = tb.DefineProperty(
                    property.Key,
                    PropertyAttributes.HasDefault,
                    property.Value,
                    null);


                MethodAttributes getSetAttr =
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

                MethodBuilder mbGetAccessor = tb.DefineMethod(
                    $"get_{property.Key}",
                    getSetAttr,
                    property.Value,
                    Type.EmptyTypes);

                ILGenerator numberGetIL = mbGetAccessor.GetILGenerator();
                // For an instance property, argument zero is the instance. Load the
                // instance, then load the private field and return, leaving the
                // field value on the stack.
                numberGetIL.Emit(OpCodes.Ldarg_0);
                numberGetIL.Emit(OpCodes.Ldfld, fb);
                numberGetIL.Emit(OpCodes.Ret);

                MethodBuilder mbSetAccessor = tb.DefineMethod(
                          $"set_{property.Key}",
                          getSetAttr,
                          null,
                          new Type[] { property.Value });

                ILGenerator numberSetIL = mbSetAccessor.GetILGenerator();
                // Load the instance and then the numeric argument, then store the
                // argument in the field.
                numberSetIL.Emit(OpCodes.Ldarg_0);
                numberSetIL.Emit(OpCodes.Ldarg_1);
                numberSetIL.Emit(OpCodes.Stfld, fb);
                numberSetIL.Emit(OpCodes.Ret);

                pb.SetGetMethod(mbGetAccessor);
                pb.SetSetMethod(mbSetAccessor);


            }
            

           Type t = tb.CreateType();

           return t;
        }
    
        public static void Refresh()
        {

        }
    }
}
