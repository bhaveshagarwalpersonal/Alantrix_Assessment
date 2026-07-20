using System;
using System.Collections.Generic;

namespace EchoGrid.Persistence
{
    [Serializable]
    public sealed class SaveData
    {
        public int version =
            1;

        public int rows;

        public int columns;

        public int seed;

        public int score;

        public int combo;

        public int matches;

        public List<int>
            matchedCardIds =
            new List<int>();
    }
}