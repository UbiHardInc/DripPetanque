public static class MathUtils
{
    public static float Smoothstep(float x)
    {
        return 3 * x * x - 2 * x * x * x;
    }
}
