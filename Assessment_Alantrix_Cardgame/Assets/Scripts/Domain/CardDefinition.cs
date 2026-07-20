namespace EchoGrid.Domain
{
    public readonly struct CardDefinition
    {
        public int CardId { get; }

        public int PairId { get; }

        public bool IsEcho { get; }

        public CardDefinition(
            int cardId,
            int pairId,
            bool isEcho)
        {
            CardId = cardId;
            PairId = pairId;
            IsEcho = isEcho;
        }
    }
}