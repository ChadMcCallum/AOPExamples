using System;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Aspects;

namespace AOPTutorial.Aspects
{
    [Serializable]
    public class ParallelAspect : MethodInterceptionAspect
    {
        [TracingAspect]
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            var t = new Task<object>(() => args.Invoke(args.Arguments));
            t.Start();
            //because we need to set ReturnValue, block until task completes
            //again, terrible example, but it's a demo :P
            while (!t.IsCompleted)
            {
                Thread.Sleep(10);
            }
            args.ReturnValue = t.Result;
        }
    }

}