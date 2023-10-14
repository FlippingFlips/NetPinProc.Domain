#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace NetPinProc.Domain
{
    public class Pair<T, U>
    {
        public T First { get; set; }
        public U Second { get; set; }

        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }
    }
}
