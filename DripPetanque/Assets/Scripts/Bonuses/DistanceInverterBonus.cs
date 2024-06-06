using UnityEngine;

public class DistanceInverterBonus : BonusBase
{
    public override void OnBonusAttached(Transform ballTransform)
    {
        base.OnBonusAttached(ballTransform);
        GameManager.Instance.PetanqueSubGameManager.InvertDistances();
    }
}
