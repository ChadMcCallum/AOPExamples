using System;
using System.Collections.Generic;
using PostSharp.Aspects;

namespace AOPTutorial.Aspects
{
    [Serializable]
    public class CacheAspect : MethodInterceptionAspect
    {
        private Dictionary<CacheKey, object> cache = new Dictionary<CacheKey, object>();

        //check in-memory dictionary for cached value, if not found execute method and add to dictionary
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            var key = new CacheKey(args);
            if (cache.ContainsKey(key))
            {
                args.ReturnValue = cache[key];
            }
            else
            {
                var result = args.Invoke(args.Arguments);
                cache.Add(key, result);
                args.ReturnValue = result;
            }
        }

        private class CacheKey
        {
            private string _argument;
            private string _method;
            private DateTime _expires;

            public CacheKey(MethodInterceptionArgs args)
            {
                //note this is a terrible example, we should be creating an array
                //of arguments we can compare against, but it's a demo
                _argument = args.Arguments[0].ToString();
                _method = args.Method.Name;

                //expires in one minute
                _expires = DateTime.Now.AddMinutes(1);
            }

            public override bool Equals(object obj)
            {
                //never match if our key is expired
                if (DateTime.Now > _expires)
                    return false;

                var otherKey = obj as CacheKey;
                if (otherKey == null)
                    return false;

                return (_argument == otherKey._argument && _method == otherKey._method);
            }

            public override int GetHashCode()
            {
                return _argument.GetHashCode() & _method.GetHashCode();
            }
        }
    }
}