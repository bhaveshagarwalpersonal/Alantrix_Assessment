using EchoGrid.Domain;

namespace EchoGrid.Matching
{
    public sealed class RevealOperation
    {
        public CardRuntimeState First
        {
            get;
        }

        public CardRuntimeState Second
        {
            get;
        }

        public bool IsResolving
        {
            get;
            private set;
        }

        public bool IsResolved
        {
            get;
            private set;
        }

        public RevealOperation(
            CardRuntimeState first,
            CardRuntimeState second)
        {
            First =
                first;

            Second =
                second;
        }

        public bool TryBeginResolution()
        {
            if (
                IsResolving ||
                IsResolved)
            {
                return false;
            }

            IsResolving =
                true;

            return true;
        }

        public void Complete()
        {
            IsResolving =
                false;

            IsResolved =
                true;
        }
    }
}