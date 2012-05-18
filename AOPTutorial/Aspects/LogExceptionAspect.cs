using System;
using PostSharp.Aspects;

namespace AOPTutorial.Aspects
{
    [Serializable]
    public class LogExceptionAspect : OnExceptionAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            Console.WriteColorLine(string.Format("Exception! {0}", args.Exception.Message), ConsoleColor.Red);
            //have our method return null
            args.ReturnValue = null;
            //make our method return as if nothing happened
            args.FlowBehavior = FlowBehavior.Return;
        }

        //return specific type of exception we're listening for
        public override Type GetExceptionType(System.Reflection.MethodBase targetMethod)
        {
            return typeof(ArgumentException);
        }
    }
}