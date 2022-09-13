namespace Tartaria.Codebase.DataStructures
{
    public interface IScalable<out T> where T : IScalable<T>
    {
        T Scale(float scale);
    }
}