using System.Collections.Generic;

namespace Memorandum.Web.Framework.Utilities
{
    internal class Pipeline : List<IHandler>
    {
        public void Run()
        {
            foreach (var item in this)
                item.Handle();
        }
    }

    internal class Pipeline<T> : List<IHandler<T>>
    {
        public void Run(T input)
        {
            foreach (var item in this)
                item.Handle(input);
        }
    }

    internal class Pipeline<T1, T2> : List<IHandler<T1, T2>>
    {
        public void Run(T1 arg1, T2 arg2)
        {
            foreach (var item in this)
                item.Handle(arg1, arg2);
        }
    }

    internal interface IHandler
    {
        void Handle();
    }

    internal interface IHandler<in T>
    {
        void Handle(T input);
    }

    internal interface IHandler<in T1, in T2>
    {
        void Handle(T1 arg1, T2 arg2);
    }
}