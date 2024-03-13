using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerShootManager : BaseShootManager<ComputerShootStep, Ball>
{
    protected override PetanqueSubGameManager.PetanquePlayers Owner => PetanqueSubGameManager.PetanquePlayers.Computer;

}
