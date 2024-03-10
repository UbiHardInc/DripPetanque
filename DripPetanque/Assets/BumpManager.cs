using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Singletons;

public class BumpManager : MonoBehaviourSingleton<BumpManager>
{
    public enum BumpersStrenght
    {
        soft,
        medium,
        hard
    }
    
    public int softBumper = 50;
    public int mediumBumper = 80;
    public int hardBumper = 110;
    
    public int GetBumperStrenght(BumpersStrenght bumperStrenght){
        switch (bumperStrenght)
        {
            case BumpersStrenght.soft:
                return softBumper;
            case BumpersStrenght.medium:
                return mediumBumper;
            case BumpersStrenght.hard:
                return hardBumper;
            default:
                return mediumBumper;
        }
    }
}
