
namespace CookiesAuthorization.ExtensionMethods
{
    public static class ArrayExtensions
    {
        public static bool Compare<T>(this T[] array, T[] to)
        { 
            for(long i = 0; i < array.Length; i++)
            {
                if (i >= to.Length)
                    return false;

                if (!array[i].Equals(to[i]))
                    return false;
            }

            return true;
        }
    }
}
