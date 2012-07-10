using System;
using System.Reflection;

namespace OpenUO.Core.Reflection
{
    public class MethodInvoker<TargetObjectType>
    {
        protected FastInvokeHandler methodHandler;

        public MethodInvoker(string Method)
        {
            methodHandler = BaseMethodInvoker.GetMethodInvoker(typeof(TargetObjectType).GetMethod(Method));
        }

        public MethodInvoker(MethodInfo Method)
        {
            methodHandler = BaseMethodInvoker.GetMethodInvoker(Method);
        }

        public object Invoke(TargetObjectType TargetObject, params object[] Paramters)
        {
            return methodHandler.Invoke(TargetObject, Paramters);
        }

        public IAsyncResult BeginInvoke(TargetObjectType TargetObject, object[] Paramters, AsyncCallback Callback, object @Object)
        {
            return methodHandler.BeginInvoke(TargetObject, Paramters, Callback, @Object);
        }

        public override string ToString()
        {
            return "Method Invoker : " + methodHandler.Method.Name;
        }
    }
}
