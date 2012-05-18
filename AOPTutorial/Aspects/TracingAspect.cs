using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Aspects;

namespace AOPTutorial.Aspects
{
    [Serializable]
    public class TracingAspect : OnMethodBoundaryAspect
    {
        private DateTime _start;

        public override void OnEntry(MethodExecutionArgs args)
        {
            _start = DateTime.Now;
            //note: this is a terrible example, aspects should be "method agnostic"
            //but I'm doing it to prove a point
            Console.WriteColorLine(string.Format("Starting search for {0}", args.Arguments[0]), ConsoleColor.Blue);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            //note: this is a terrible example, aspects should be "method agnostic"
            //but I'm doing it to prove a point
            Console.WriteColorLine(string.Format("Finished search for {0}", args.Arguments[0]), ConsoleColor.Blue);

            Console.WriteColorLine(string.Format("Search took {0}", DateTime.Now.Subtract(_start)), ConsoleColor.Green);
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            //note: this is a terrible example, aspects should be "method agnostic"
            //but I'm doing it to prove a point :P
            var results = args.ReturnValue as SearchResults;
            Console.WriteColorLine(string.Format("Successfully searched twitter, got {0} results", results.Results.Length), ConsoleColor.Yellow);
        }

        //commenting this out so we can use LogException aspect
        //public override void OnException(MethodExecutionArgs args)
        //{
        //    Console.WriteColorLine(string.Format("Exception! {0}", args.Exception.Message), ConsoleColor.Red);
        //}
    }
}
