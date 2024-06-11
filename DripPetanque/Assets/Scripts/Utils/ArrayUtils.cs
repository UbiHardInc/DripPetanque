public static class ArrayUtils
{
    public static int IndexOf<T>(this T[] array, T item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (item.Equals(array[i]))
            {
                return i;
            }
        }
        return -1;
    }
}
