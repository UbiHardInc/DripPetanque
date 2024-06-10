using UnityUtility.Singletons;

public class BumpManager : MonoBehaviourSingleton<BumpManager>
{
    public enum BumpersStrength
    {
        Soft,
        Medium,
        Hard
    }
    
    public int softBumper = 50;
    public int mediumBumper = 80;
    public int hardBumper = 110;
    
    public int GetBumperStrength(BumpersStrength bumperStrength)
    {
        return bumperStrength switch
        {
            BumpersStrength.Soft => softBumper,
            BumpersStrength.Medium => mediumBumper,
            BumpersStrength.Hard => hardBumper,
            _ => mediumBumper,
        };
    }
}
