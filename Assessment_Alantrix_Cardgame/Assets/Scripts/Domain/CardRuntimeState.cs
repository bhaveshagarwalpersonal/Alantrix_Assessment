namespace EchoGrid.Domain
{
    public enum CardState
    {
        FaceDown,
        Revealing,
        FaceUp,
        Resolving,
        Returning,
        Matched
    }

    public sealed class CardRuntimeState
    {
        public CardDefinition Definition
        {
            get;
        }

        public CardState State
        {
            get;
            private set;
        }

        public bool IsReserved
        {
            get;
            private set;
        }

        public CardRuntimeState(
            CardDefinition definition)
        {
            Definition =
                definition;

            State =
                CardState.FaceDown;

            IsReserved =
                false;
        }

        public bool CanBeSelected()
        {
            return
                State ==
                CardState.FaceDown
                &&
                !IsReserved;
        }

        public bool TryReserve()
        {
            if (
                !CanBeSelected())
            {
                return false;
            }

            IsReserved =
                true;

            State =
                CardState.Revealing;

            return true;
        }

        public void MarkFaceUp()
        {
            if (
                State ==
                CardState.Revealing)
            {
                State =
                    CardState.FaceUp;
            }
        }

        public void MarkResolving()
        {
            if (
                State ==
                CardState.FaceUp)
            {
                State =
                    CardState.Resolving;
            }
        }

        public void MarkReturning()
        {
            if (
                State ==
                CardState.Resolving)
            {
                State =
                    CardState.Returning;
            }
        }

        public void MarkMatched()
        {
            IsReserved =
                false;

            State =
                CardState.Matched;
        }

        public void ReturnFaceDown()
        {
            IsReserved =
                false;

            State =
                CardState.FaceDown;
        }
    }
}