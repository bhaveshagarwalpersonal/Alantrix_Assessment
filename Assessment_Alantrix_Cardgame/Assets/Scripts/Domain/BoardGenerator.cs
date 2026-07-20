using System;
using System.Collections.Generic;

namespace EchoGrid.Domain
{
    public sealed class BoardGenerator
    {
        public BoardDefinition Generate(
            int rows,
            int columns,
            int seed)
        {
            if (
                rows <= 0 ||
                columns <= 0)
            {
                throw new ArgumentException(
                    "Invalid board dimensions.");
            }

            int totalCards =
                rows *
                columns;

            int pairCount =
                totalCards /
                2;

            bool hasEcho =
                totalCards %
                2 !=
                0;

            var cards =
                new List<CardDefinition>(
                    totalCards);

            int cardId =
                0;

            for (
                int pairId = 0;
                pairId < pairCount;
                pairId++)
            {
                cards.Add(
                    new CardDefinition(
                        cardId++,
                        pairId,
                        false));

                cards.Add(
                    new CardDefinition(
                        cardId++,
                        pairId,
                        false));
            }

            if (
                hasEcho)
            {
                cards.Add(
                    new CardDefinition(
                        cardId,
                        -1,
                        true));
            }

            Shuffle(
                cards,
                seed);

            return new BoardDefinition(
                rows,
                columns,
                seed,
                pairCount,
                cards);
        }

        private void Shuffle(
            List<CardDefinition> cards,
            int seed)
        {
            var random =
                new Random(
                    seed);

            for (
                int index =
                    cards.Count - 1;
                index > 0;
                index--)
            {
                int swapIndex =
                    random.Next(
                        index + 1);

                CardDefinition temporary =
                    cards[index];

                cards[index] =
                    cards[swapIndex];

                cards[swapIndex] =
                    temporary;
            }
        }
    }
}