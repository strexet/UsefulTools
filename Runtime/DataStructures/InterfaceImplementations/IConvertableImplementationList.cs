using System.Collections.Generic;

namespace UsefulTools.Runtime.DataStructures.InterfaceImplementations
{
    public interface IConvertableImplementationList<T> : IImplementationList<T> where T : class
    {
        List<T> ToImplementationList();
        List<TParent> ToParentImplementationList<TParent>();
        List<TChild> ToChildImplementationList<TChild>() where TChild : T;
    }
}