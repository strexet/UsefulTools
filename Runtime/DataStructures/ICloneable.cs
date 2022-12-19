namespace UsefulTools.Runtime.DataStructures
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}