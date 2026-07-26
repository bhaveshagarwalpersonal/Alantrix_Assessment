namespace EchoGrid.Matching
{
    public static class MatchEvaluator
    {
        public static MatchResult Evaluate(
            RevealOperation operation)
        {
            if (
                operation.First
                    .Definition
                    .IsEcho ||
                operation.Second
                    .Definition
                    .IsEcho)
            {
                return new MatchResult(
                    MatchResultType.Echo);
            }

            bool match =
                operation.First
                    .Definition
                    .PairId
                ==
                operation.Second
                    .Definition
                    .PairId;

            return new MatchResult(
                match
                    ? MatchResultType.Match
                    : MatchResultType.Mismatch);
        }
    }
}