namespace EchoGrid.Matching
{
    public enum MatchResultType
    {
        Match,
        Mismatch,
        Echo
    }

    public readonly struct MatchResult
    {
        public MatchResultType Type
        {
            get;
        }

        public MatchResult(
            MatchResultType type)
        {
            Type =
                type;
        }
    }
}