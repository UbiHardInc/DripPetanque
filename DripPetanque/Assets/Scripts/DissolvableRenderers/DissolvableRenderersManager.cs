using System.Collections.Generic;

public static class DissolvableRenderersManager
{
    private static readonly List<DissolvableRenderer> s_dissolvableRenderers = new List<DissolvableRenderer>();

    public static void RegisterRenderer(DissolvableRenderer renderer)
    {
        s_dissolvableRenderers.Add(renderer);
    }

    public static void UnregisterRenderer(DissolvableRenderer renderer)
    {
        _ = s_dissolvableRenderers.Remove(renderer);
    }

    public static void SetDissolveAmount(float amount, int dissolvePropertyID)
    {
        s_dissolvableRenderers.ForEach(renderer => renderer.SetDissolveAmount(amount, dissolvePropertyID));
    }
}
