using System.Reflection;

namespace OpenUO.Core.Reflection
{
    public class FieldAccessor<TargetType, FieldType>
    {
        protected FieldFastGetInvokeHandler<TargetType, FieldType> getMethodHandler;
        protected FieldFastSetInvokeHandler<TargetType, FieldType> setMethodHandler;

        public FieldAccessor(string Field)
        {
            getMethodHandler = BaseFieldAccessor.GetFieldInvoker<TargetType, FieldType>(typeof(TargetType).GetField(Field));
            setMethodHandler = BaseFieldAccessor.SetFieldInvoker<TargetType, FieldType>(typeof(TargetType).GetField(Field));
        }

        public FieldAccessor(FieldInfo Field)
        {
            getMethodHandler = BaseFieldAccessor.GetFieldInvoker<TargetType, FieldType>(Field);
            setMethodHandler = BaseFieldAccessor.SetFieldInvoker<TargetType, FieldType>(Field);
        }
        public FieldType Get(TargetType TargetObject)
        {
            return getMethodHandler(TargetObject);
        }
        public void Set(TargetType TargetObject, FieldType Value)
        {
            setMethodHandler(TargetObject, Value);
        }
        public override string ToString()
        {
            return "Property Invoker : " + getMethodHandler.Method.Name.Substring(4);
        }
    }
}
