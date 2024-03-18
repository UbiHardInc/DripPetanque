public class ComputerShootManager : BaseShootManager<ComputerShootStep, Ball>
{
    protected override PetanqueSubGameManager.PetanquePlayers Owner => PetanqueSubGameManager.PetanquePlayers.Computer;

}
