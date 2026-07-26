using EchoGrid.Domain;

public sealed class PendingCardReveal
{
    public CardRuntimeState Card
    {
        get;
    }

    public bool RevealCompleted
    {
        get;
        private set;
    }

    public PendingCardReveal(
        CardRuntimeState card)
    {
        Card =
            card;
    }

    public void MarkRevealCompleted()
    {
        RevealCompleted =
            true;
    }
}