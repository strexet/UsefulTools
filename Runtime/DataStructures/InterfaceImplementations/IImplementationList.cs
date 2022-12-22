namespace UsefulTools.Runtime.DataStructures.InterfaceImplementations
{
    public interface IImplementationList<in T>
    {
        int Count {get;}

        void Add(T item);
        bool Contains(T item);
    }
}