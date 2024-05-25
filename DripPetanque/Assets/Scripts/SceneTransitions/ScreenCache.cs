using UnityEngine;

public abstract class ScreenCache : MonoBehaviour
{
    public abstract void Show(bool show);
    public abstract void UpdateCache(float progress);
}
