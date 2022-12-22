namespace UsefulTools.Runtime.DataStructures.InterfaceImplementations
{
    public interface IInterfaceImplementation<out T> where T : class
    {
        public T Implementation {get;}
    }
}