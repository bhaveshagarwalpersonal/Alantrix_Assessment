using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EchoGrid.Domain;
using EchoGrid.Matching;
using EchoGrid.Scoring;
using EchoGrid.Presentation;
using EchoGrid.Persistence;

namespace EchoGrid.Core
{
    public sealed class GameSessionController :
        MonoBehaviour
    {
        [Header("Board")]
        [SerializeField]
        private int rows = 4;

        [SerializeField]
        private int columns = 4;

        [SerializeField]
        private int seed = 12345;

        [Header("References")]
        [SerializeField]
        private BoardView boardView;

        [SerializeField]
        private Audio.AudioService
            audioService;

        [SerializeField]
        private UI.GameHUD
            gameHUD;

        [Header("Symbols")]
        [SerializeField]
        private Sprite[] cardSymbols;

        [SerializeField]
        private Sprite echoSprite;

        private BoardDefinition board;

        private BoardGenerator generator;

        private ScoreService scoreService;

        private readonly Dictionary<
            CardRuntimeState,
            CardView>
            viewByState =
            new Dictionary<
                CardRuntimeState,
                CardView>();

        private readonly List<
            CardRuntimeState>
            pendingSelection =
            new List<
                CardRuntimeState>();

        private readonly List<
            RevealOperation>
            operations =
            new List<
                RevealOperation>();
        private SaveService saveService;
        private void Awake()
        {
            generator =
                new BoardGenerator();

            scoreService =
                new ScoreService();

            saveService =
                new SaveService();
        }

        private void Start()
        {
            StartNewGame();
        }

        public void StartNewGame()
        {
            scoreService.Reset();

            pendingSelection.Clear();

            operations.Clear();

            board =
                generator.Generate(
                    rows,
                    columns,
                    seed);

            BuildBoard();

            gameHUD?.Refresh(
                scoreService);
        }

        private void BuildBoard()
        {
            viewByState.Clear();

            boardView.Build(
                board,
                HandleCardClicked);

            foreach (
                CardView view
                in boardView.Views)
            {
                viewByState.Add(
                    view.State,
                    view);
            }
        }

        private void HandleCardClicked(
            CardView view)
        {
            CardRuntimeState card =
                view.State;

            if (
                !card.TryReserve())
            {
                return;
            }

            audioService?.PlayFlip();

            pendingSelection.Add(
                card);

            Sprite sprite =
                GetSymbol(
                    card.Definition);

            view.Reveal(
                sprite,
                () =>
                {
                    card.MarkFaceUp();

                    TryCreateRevealOperation();
                });
        }

        private void
            TryCreateRevealOperation()
        {
            if (
                pendingSelection.Count <
                2)
            {
                return;
            }

            CardRuntimeState first =
                pendingSelection[0];

            CardRuntimeState second =
                pendingSelection[1];

            pendingSelection.RemoveAt(
                0);

            pendingSelection.RemoveAt(
                0);

            first.MarkResolving();

            second.MarkResolving();

            var operation =
                new RevealOperation(
                    first,
                    second);

            operations.Add(
                operation);

            StartCoroutine(
                ResolveOperation(
                    operation));
        }

        private IEnumerator
            ResolveOperation(
                RevealOperation operation)
        {
            if (
                !operation
                    .TryBeginResolution())
            {
                yield break;
            }

            MatchResult result =
                MatchEvaluator.Evaluate(
                    operation);

            if (
                result.Type ==
                MatchResultType.Match)
            {
                scoreService
                    .RegisterMatch();

                operation.First
                    .MarkMatched();

                operation.Second
                    .MarkMatched();

                GetView(
                    operation.First)
                    .ShowMatched();

                GetView(
                    operation.Second)
                    .ShowMatched();

                audioService
                    ?.PlayMatch();
            }
            else
            {
                if (
                    result.Type ==
                    MatchResultType.Mismatch)
                {
                    scoreService
                        .RegisterMismatch();

                    audioService
                        ?.PlayMismatch();
                }

                yield return
                    new WaitForSeconds(
                        0.35f);

                operation.First
                    .MarkReturning();

                operation.Second
                    .MarkReturning();

                GetView(
                    operation.First)
                    .Hide(
                        () =>
                        {
                            operation.First
                                .ReturnFaceDown();
                        });

                GetView(
                    operation.Second)
                    .Hide(
                        () =>
                        {
                            operation.Second
                                .ReturnFaceDown();
                        });
            }

            operation.Complete();

            operations.Remove(
                operation);

            gameHUD?.Refresh(
                scoreService);

            CheckWin();
        }

        private CardView GetView(
            CardRuntimeState state)
        {
            return viewByState[state];
        }

        private Sprite GetSymbol(
            CardDefinition definition)
        {
            if (
                definition.IsEcho)
            {
                return echoSprite;
            }

            return cardSymbols[
                definition.PairId %
                cardSymbols.Length];
        }

        private void CheckWin()
        {
            if (
                scoreService.Matches >=
                board.PairCount)
            {
                audioService
                    ?.PlayWin();

                gameHUD
                    ?.ShowWin();
            }
        }
        public void SaveGame()
        {
            var data =
                new SaveData();

            data.rows =
                rows;

            data.columns =
                columns;

            data.seed =
                seed;

            data.score =
                scoreService.Score;

            data.combo =
                scoreService.Combo;

            data.matches =
                scoreService.Matches;

            foreach (
                CardView view
                in boardView.Views)
            {
                if (
                    view.State.State ==
                    CardState.Matched)
                {
                    data.matchedCardIds.Add(
                        view.State
                            .Definition
                            .CardId);
                }
            }

            saveService.Save(
                data);
        }

        public void LoadGame()
        {
            SaveData data =
                saveService.Load();

            if (
                data ==
                null)
            {
                return;
            }

            rows =
                data.rows;

            columns =
                data.columns;

            seed =
                data.seed;

            scoreService.Restore(
                data.score,
                data.combo,
                data.matches);

            board =
                generator.Generate(
                    rows,
                    columns,
                    seed);

            BuildBoard();

            foreach (
                CardView view
                in boardView.Views)
            {
                int cardId =
                    view.State
                        .Definition
                        .CardId;

                if (
                    data.matchedCardIds
                        .Contains(
                            cardId))
                {
                    view.State
                        .MarkMatched();

                    view.ShowMatched();
                }
            }

            gameHUD.Refresh(
                scoreService);

            gameHUD.SetSeed(
                seed);
        }
    }
}