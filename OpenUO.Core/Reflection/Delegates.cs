
namespace OpenUO.Core.Reflection
{
    public delegate object FastInvokeHandler(object target, params object[] paramters);
    public delegate ReturnType FastInvokeHandler<ReturnType>(object target, params object[] paramters);
    public delegate FieldType FieldFastGetInvokeHandler<TargetType, FieldType>(TargetType obj);
    public delegate void FieldFastSetInvokeHandler<TargetType, FieldType>(TargetType obj, FieldType value);

}
