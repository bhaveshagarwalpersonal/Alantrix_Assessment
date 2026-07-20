using System;
using UnityEngine;
using UnityEngine.UI;
using EchoGrid.Domain;

namespace EchoGrid.Presentation
{
    public sealed class CardView :
        MonoBehaviour
    {
        [SerializeField]
        private GameObject front;

        [SerializeField]
        private GameObject back;

        [SerializeField]
        private Image symbol;

        [SerializeField]
        private CardAnimator animator;

        private CardRuntimeState state;

        private Action<CardView>
            clicked;

        public CardRuntimeState State
        {
            get
            {
                return state;
            }
        }

        public void Bind(
            CardRuntimeState runtimeState,
            Action<CardView>
                clickCallback)
        {
            state =
                runtimeState;

            clicked =
                clickCallback;

            ShowBack();
        }

        public void OnClicked()
        {
            clicked?.Invoke(
                this);
        }

        public void Reveal(
            Sprite sprite,
            Action completed)
        {
            animator.Flip(
                true,
                () =>
                {
                    symbol.sprite =
                        sprite;

                    ShowFront();
                },
                completed);
        }

        public void Hide(
            Action completed)
        {
            animator.Flip(
                false,
                ShowBack,
                completed);
        }

        public void ShowFront()
        {
            front.SetActive(
                true);

            back.SetActive(
                false);
        }

        public void ShowBack()
        {
            front.SetActive(
                false);

            back.SetActive(
                true);
        }

        public void ShowMatched()
        {
            ShowFront();

            transform.localScale =
                Vector3.one *
                0.96f;
        }
    }
}