using System;
using System.Reflection;
using System.Reflection.Emit;

namespace OpenUO.Core.Reflection
{
    static class BaseFieldAccessor
    {
        public static FieldFastSetInvokeHandler<TargetType, FieldType> SetFieldInvoker<TargetType, FieldType>(string FieldName)
        {
            return SetFieldInvoker<TargetType, FieldType>(typeof(TargetType).GetField(FieldName));
        }

        public static FieldFastGetInvokeHandler<TargetType, FieldType> GetFieldInvoker<TargetType, FieldType>(string FieldName)
        {
            return GetFieldInvoker<TargetType, FieldType>(typeof(TargetType).GetField(FieldName));
        }

        public static FieldFastSetInvokeHandler<TargetType, FieldType> SetFieldInvoker<TargetType, FieldType>(FieldInfo Field)
        {
            Type objectType = typeof(TargetType);

            if (Field != null)
            {
                DynamicMethod dm = new DynamicMethod("Set" + Field.Name, null, new Type[] { objectType, typeof(FieldType) }, objectType);
                ILGenerator il = dm.GetILGenerator();

                // Load the instance of the object (argument 0) onto the stack
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                // Load the value of the object's field (fi) onto the stack
                il.Emit(OpCodes.Stfld, Field);
                // return the value on the top of the stack
                il.Emit(OpCodes.Ret);

                return (FieldFastSetInvokeHandler<TargetType, FieldType>)dm.CreateDelegate(typeof(FieldFastSetInvokeHandler<TargetType, FieldType>));
            }

            throw new Exception(String.Format("Member: '{0}' is not a Field of Type: '{1}'", Field.Name, objectType.Name));
        }

        public static FieldFastGetInvokeHandler<TargetType, FieldType> GetFieldInvoker<TargetType, FieldType>(FieldInfo Field)
        {
            Type objectType = typeof(TargetType);

            if (Field == null)
                throw new Exception(String.Format("Member: '{0}' is not a Field of Type: '{1}'", Field.Name,
                                                  objectType.Name));

            DynamicMethod dm = new DynamicMethod("Get" + Field.Name, typeof(FieldType), new Type[] { objectType },
                                                    objectType);
            ILGenerator il = dm.GetILGenerator();

            // Load the instance of the object (argument 0) onto the stack
            il.Emit(OpCodes.Ldarg_0);
            // Load the value of the object's field (fi) onto the stack
            il.Emit(OpCodes.Ldfld, Field);
            // return the value on the top of the stack
            il.Emit(OpCodes.Ret);

            return
                (FieldFastGetInvokeHandler<TargetType, FieldType>)
                dm.CreateDelegate(typeof(FieldFastGetInvokeHandler<TargetType, FieldType>));
        }
    }
}
