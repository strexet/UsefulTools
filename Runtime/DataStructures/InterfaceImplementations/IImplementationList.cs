using System.Collections.Generic;

namespace UsefulTools.Runtime.DataStructures.InterfaceImplementations
{
    public interface IImplementationList<T>
    {
        List<T> ToImplementationList();
        DisposableList<T> ToImplementationDisposableList();
    }
}