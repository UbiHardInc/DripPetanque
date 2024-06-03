using System.Collections.Generic;

public struct GameResultDatas
{
    public BasePetanquePlayer Winner;
    public List<BasePetanquePlayer> AllPlayers;
}

public struct RoundResultDatas
{
    public BasePetanquePlayer Winner;
    public int RoundIndex;
    public List<BasePetanquePlayer> AllPlayers;
}
