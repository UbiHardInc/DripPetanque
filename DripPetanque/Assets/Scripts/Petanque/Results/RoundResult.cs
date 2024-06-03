public struct RoundResult
{
    public readonly bool WonRound => Score > 0;

    public int Score;

    public RoundResult(int score)
    {
        Score = score;
    }
}
