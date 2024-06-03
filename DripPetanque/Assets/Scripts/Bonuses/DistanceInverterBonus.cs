public class DistanceInverterBonus : BonusBase
{
    public override void OnBonusAttached()
    {
        base.OnBonusAttached();
        GameManager.Instance.PetanqueSubGameManager.InvertDistances();
    }
}
