using UnityEngine;

namespace EchoGrid.Layout
{
    public readonly struct CardLayout
    {
        public Vector2 Size
        {
            get;
        }

        public Vector2 Position
        {
            get;
        }

        public CardLayout(
            Vector2 size,
            Vector2 position)
        {
            Size =
                size;

            Position =
                position;
        }
    }

    public sealed class GridLayoutCalculator
    {
        public CardLayout Calculate(
            int index,
            int rows,
            int columns,
            Vector2 containerSize,
            float spacing)
        {
            float cellWidth =
                (
                    containerSize.x
                    -
                    spacing *
                    (columns - 1)
                )
                /
                columns;

            float cellHeight =
                (
                    containerSize.y
                    -
                    spacing *
                    (rows - 1)
                )
                /
                rows;

            float cardSize =
                Mathf.Min(
                    cellWidth,
                    cellHeight);

            float gridWidth =
                columns *
                cardSize
                +
                (columns - 1)
                *
                spacing;

            float gridHeight =
                rows *
                cardSize
                +
                (rows - 1)
                *
                spacing;

            int row =
                index /
                columns;

            int column =
                index %
                columns;

            float startX =
                -gridWidth /
                2f
                +
                cardSize /
                2f;

            float startY =
                gridHeight /
                2f
                -
                cardSize /
                2f;

            float x =
                startX
                +
                column *
                (
                    cardSize
                    +
                    spacing);

            float y =
                startY
                -
                row *
                (
                    cardSize
                    +
                    spacing);

            return new CardLayout(
                new Vector2(
                    cardSize,
                    cardSize),
                new Vector2(
                    x,
                    y));
        }
    }
}