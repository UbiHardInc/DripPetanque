using UnityUtility.Singletons;

public class BumpManager : MonoBehaviourSingleton<BumpManager>
{
    public enum BumpersStrenght
    {
        Soft,
        Medium,
        Hard
    }
    
    public int softBumper = 50;
    public int mediumBumper = 80;
    public int hardBumper = 110;
    
    public int GetBumperStrenght(BumpersStrenght bumperStrenght)
    {
        return bumperStrenght switch
        {
            BumpersStrenght.Soft => softBumper,
            BumpersStrenght.Medium => mediumBumper,
            BumpersStrenght.Hard => hardBumper,
            _ => mediumBumper,
        };
    }
}
