using System;
using System.Collections.Generic;
using UnityEngine;
using EchoGrid.Domain;
using EchoGrid.Layout;

namespace EchoGrid.Presentation
{
    public sealed class BoardView :
        MonoBehaviour
    {
        [SerializeField]
        private RectTransform container;

        [SerializeField]
        private CardView cardPrefab;

        [SerializeField]
        private float spacing =
            12f;

        private readonly List<CardView>
            views =
            new List<CardView>();

        private readonly GridLayoutCalculator
            calculator =
            new GridLayoutCalculator();

        public IReadOnlyList<CardView>
            Views =>
            views;

        public void Build(
            BoardDefinition board,
            Action<CardView>
                clicked)
        {
            Clear();

            for (
                int index = 0;
                index <
                board.Cards.Count;
                index++)
            {
                CardDefinition definition =
                    board.Cards[index];

                CardRuntimeState state =
                    new CardRuntimeState(
                        definition);

                CardView view =
                    Instantiate(
                        cardPrefab,
                        container);

                RectTransform rect =
                    view.GetComponent<
                        RectTransform>();

                CardLayout layout =
                    calculator.Calculate(
                        index,
                        board.Rows,
                        board.Columns,
                        container.rect.size,
                        spacing);

                rect.sizeDelta =
                    layout.Size;

                rect.anchoredPosition =
                    layout.Position;

                view.Bind(
                    state,
                    clicked);

                views.Add(
                    view);
            }
        }

        private void Clear()
        {
            foreach (
                CardView view
                in views)
            {
                if (
                    view !=
                    null)
                {
                    Destroy(
                        view.gameObject);
                }
            }

            views.Clear();
        }
    }
}