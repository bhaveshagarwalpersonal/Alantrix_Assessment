using System.Collections.Generic;

namespace EchoGrid.Domain
{
    public sealed class BoardDefinition
    {
        public int Rows
        {
            get;
        }

        public int Columns
        {
            get;
        }

        public int Seed
        {
            get;
        }

        public int PairCount
        {
            get;
        }

        public IReadOnlyList<CardDefinition>
            Cards
        {
            get;
        }

        public BoardDefinition(
            int rows,
            int columns,
            int seed,
            int pairCount,
            List<CardDefinition> cards)
        {
            Rows =
                rows;

            Columns =
                columns;

            Seed =
                seed;

            PairCount =
                pairCount;

            Cards =
                cards;
        }
    }
}