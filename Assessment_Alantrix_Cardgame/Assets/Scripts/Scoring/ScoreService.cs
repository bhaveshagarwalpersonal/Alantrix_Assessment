namespace EchoGrid.Scoring
{
    public sealed class ScoreService
    {
        public const int BaseMatchScore =
            100;

        public int Score
        {
            get;
            private set;
        }

        public int Combo
        {
            get;
            private set;
        }

        public int Matches
        {
            get;
            private set;
        }

        public void RegisterMatch()
        {
            Combo++;

            Matches++;

            Score +=
                BaseMatchScore *
                Combo;
        }

        public void RegisterMismatch()
        {
            Combo =
                0;
        }

        public void Restore(
            int score,
            int combo,
            int matches)
        {
            Score =
                score;

            Combo =
                combo;

            Matches =
                matches;
        }

        public void Reset()
        {
            Score =
                0;

            Combo =
                0;

            Matches =
                0;
        }
    }
}