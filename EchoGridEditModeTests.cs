// Place under Assets/Tests/EditMode/ with an EditMode Assembly Definition
// referencing the EchoGrid Domain/Matching/Scoring assemblies (or the
// relevant scripts) plus UnityEngine.TestRunner / nunit.framework.
using NUnit.Framework;
using EchoGrid.Domain;
using EchoGrid.Matching;
using EchoGrid.Scoring;

namespace EchoGrid.Tests.EditMode
{
    public class ScoreServiceTests
    {
        [Test]
        public void RegisterMatch_IncrementsComboAndMatches()
        {
            var score = new ScoreService();

            score.RegisterMatch();

            Assert.AreEqual(1, score.Combo);
            Assert.AreEqual(1, score.Matches);
            Assert.AreEqual(ScoreService.BaseMatchScore, score.Score);
        }

        [Test]
        public void RegisterMatch_ScalesScoreWithComboStreak()
        {
            var score = new ScoreService();

            score.RegisterMatch(); // combo 1 -> +100
            score.RegisterMatch(); // combo 2 -> +200
            score.RegisterMatch(); // combo 3 -> +300

            Assert.AreEqual(3, score.Combo);
            Assert.AreEqual(3, score.Matches);
            Assert.AreEqual(600, score.Score);
        }

        [Test]
        public void RegisterMismatch_ResetsComboButKeepsScoreAndMatches()
        {
            var score = new ScoreService();

            score.RegisterMatch();
            score.RegisterMatch();
            score.RegisterMismatch();

            Assert.AreEqual(0, score.Combo);
            Assert.AreEqual(2, score.Matches);
            Assert.AreEqual(300, score.Score); // unaffected by the mismatch
        }

        [Test]
        public void Restore_SetsFieldsExactlyAsGiven()
        {
            var score = new ScoreService();

            score.Restore(score: 450, combo: 3, matches: 4);

            Assert.AreEqual(450, score.Score);
            Assert.AreEqual(3, score.Combo);
            Assert.AreEqual(4, score.Matches);
        }

        [Test]
        public void Reset_ClearsEverything()
        {
            var score = new ScoreService();

            score.RegisterMatch();
            score.Reset();

            Assert.AreEqual(0, score.Score);
            Assert.AreEqual(0, score.Combo);
            Assert.AreEqual(0, score.Matches);
        }
    }

    public class MatchEvaluatorTests
    {
        [Test]
        public void Evaluate_SamePairId_ReturnsMatch()
        {
            var first = new CardRuntimeState(new CardDefinition(0, pairId: 5, isEcho: false));
            var second = new CardRuntimeState(new CardDefinition(1, pairId: 5, isEcho: false));
            var operation = new RevealOperation(first, second);

            MatchResult result = MatchEvaluator.Evaluate(operation);

            Assert.AreEqual(MatchResultType.Match, result.Type);
        }

        [Test]
        public void Evaluate_DifferentPairId_ReturnsMismatch()
        {
            var first = new CardRuntimeState(new CardDefinition(0, pairId: 5, isEcho: false));
            var second = new CardRuntimeState(new CardDefinition(1, pairId: 6, isEcho: false));
            var operation = new RevealOperation(first, second);

            MatchResult result = MatchEvaluator.Evaluate(operation);

            Assert.AreEqual(MatchResultType.Mismatch, result.Type);
        }

        [Test]
        public void Evaluate_EitherCardIsEcho_ReturnsEcho_EvenWithMatchingPairId()
        {
            var first = new CardRuntimeState(new CardDefinition(0, pairId: -1, isEcho: true));
            var second = new CardRuntimeState(new CardDefinition(1, pairId: -1, isEcho: true));
            var operation = new RevealOperation(first, second);

            MatchResult result = MatchEvaluator.Evaluate(operation);

            Assert.AreEqual(MatchResultType.Echo, result.Type);
        }

        [Test]
        public void Evaluate_OneEchoOneNormal_ReturnsEcho_NotMismatch()
        {
            var first = new CardRuntimeState(new CardDefinition(0, pairId: 3, isEcho: false));
            var second = new CardRuntimeState(new CardDefinition(1, pairId: -1, isEcho: true));
            var operation = new RevealOperation(first, second);

            MatchResult result = MatchEvaluator.Evaluate(operation);

            Assert.AreEqual(MatchResultType.Echo, result.Type);
        }
    }

    public class CardRuntimeStateTests
    {
        [Test]
        public void TryReserve_FromFaceDown_SucceedsAndMovesToRevealing()
        {
            var card = new CardRuntimeState(new CardDefinition(0, 0, false));

            bool reserved = card.TryReserve();

            Assert.IsTrue(reserved);
            Assert.AreEqual(CardState.Revealing, card.State);
            Assert.IsTrue(card.IsReserved);
        }

        [Test]
        public void TryReserve_WhenAlreadyReserved_Fails()
        {
            var card = new CardRuntimeState(new CardDefinition(0, 0, false));
            card.TryReserve();

            bool secondReserve = card.TryReserve();

            Assert.IsFalse(secondReserve);
        }

        [Test]
        public void MarkMatched_ClearsReservationAndSetsMatchedState()
        {
            var card = new CardRuntimeState(new CardDefinition(0, 0, false));
            card.TryReserve();
            card.MarkFaceUp();
            card.MarkResolving();

            card.MarkMatched();

            Assert.AreEqual(CardState.Matched, card.State);
            Assert.IsFalse(card.IsReserved);
        }

        [Test]
        public void ReturnFaceDown_AllowsCardToBeSelectedAgain()
        {
            var card = new CardRuntimeState(new CardDefinition(0, 0, false));
            card.TryReserve();
            card.MarkFaceUp();
            card.MarkResolving();
            card.MarkReturning();

            card.ReturnFaceDown();

            Assert.AreEqual(CardState.FaceDown, card.State);
            Assert.IsTrue(card.CanBeSelected());
        }
    }

    public class BoardGeneratorTests
    {
        [Test]
        public void Generate_SameSeed_ProducesIdenticalCardOrder()
        {
            var generator = new BoardGenerator();

            BoardDefinition first = generator.Generate(4, 4, seed: 12345);
            BoardDefinition second = generator.Generate(4, 4, seed: 12345);

            Assert.AreEqual(first.Cards.Count, second.Cards.Count);
            for (int i = 0; i < first.Cards.Count; i++)
            {
                Assert.AreEqual(first.Cards[i].CardId, second.Cards[i].CardId);
                Assert.AreEqual(first.Cards[i].PairId, second.Cards[i].PairId);
                Assert.AreEqual(first.Cards[i].IsEcho, second.Cards[i].IsEcho);
            }
        }

        [Test]
        public void Generate_DifferentSeed_TypicallyProducesDifferentOrder()
        {
            var generator = new BoardGenerator();

            BoardDefinition first = generator.Generate(4, 4, seed: 111);
            BoardDefinition second = generator.Generate(4, 4, seed: 222);

            bool anyDifferent = false;
            for (int i = 0; i < first.Cards.Count; i++)
            {
                if (first.Cards[i].CardId != second.Cards[i].CardId)
                {
                    anyDifferent = true;
                    break;
                }
            }

            Assert.IsTrue(anyDifferent, "Different seeds should not reliably produce the identical order.");
        }

        [Test]
        public void Generate_EveryPairIdAppearsExactlyTwice()
        {
            var generator = new BoardGenerator();
            BoardDefinition board = generator.Generate(4, 4, seed: 999);

            var pairCounts = new System.Collections.Generic.Dictionary<int, int>();
            foreach (CardDefinition card in board.Cards)
            {
                if (card.IsEcho) continue;

                if (!pairCounts.ContainsKey(card.PairId))
                {
                    pairCounts[card.PairId] = 0;
                }
                pairCounts[card.PairId]++;
            }

            foreach (var count in pairCounts.Values)
            {
                Assert.AreEqual(2, count);
            }
        }

        [Test]
        public void Generate_OddCellCount_AddsExactlyOneEchoCard()
        {
            var generator = new BoardGenerator();

            // 3x3 = 9 cells -> odd -> one echo card expected.
            BoardDefinition board = generator.Generate(3, 3, seed: 42);

            int echoCount = 0;
            foreach (CardDefinition card in board.Cards)
            {
                if (card.IsEcho) echoCount++;
            }

            Assert.AreEqual(1, echoCount);
        }
    }
}
