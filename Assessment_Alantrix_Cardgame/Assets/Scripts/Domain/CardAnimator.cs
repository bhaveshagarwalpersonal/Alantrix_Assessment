using System;
using System.Collections;
using UnityEngine;

namespace EchoGrid.Presentation
{
    public sealed class CardAnimator :
        MonoBehaviour
    {
        [SerializeField]
        private Transform rotationTarget;

        [SerializeField]
        private float duration =
            0.25f;

        [SerializeField]
        private AnimationCurve curve =
            AnimationCurve.EaseInOut(
                0f,
                0f,
                1f,
                1f);

        private Coroutine currentRoutine;

        public void Flip(
            bool showFront,
            Action midpoint,
            Action completed)
        {
            if (
                currentRoutine !=
                null)
            {
                StopCoroutine(
                    currentRoutine);
            }

            currentRoutine =
                StartCoroutine(
                    FlipRoutine(
                        showFront,
                        midpoint,
                        completed));
        }

        private IEnumerator
            FlipRoutine(
                bool showFront,
                Action midpoint,
                Action completed)
        {
            float elapsed =
                0f;

            bool midpointCalled =
                false;

            Quaternion start =
                rotationTarget
                    .localRotation;

            Quaternion end =
                Quaternion.Euler(
                    0f,
                    showFront
                        ? 180f
                        : 0f,
                    0f);

            while (
                elapsed <
                duration)
            {
                elapsed +=
                    Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        elapsed /
                        duration);

                float curvedT =
                    curve.Evaluate(
                        t);

                rotationTarget
                    .localRotation =
                    Quaternion.Slerp(
                        start,
                        end,
                        curvedT);

                if (
                    !midpointCalled &&
                    t >=
                    0.5f)
                {
                    midpointCalled =
                        true;

                    midpoint?.Invoke();
                }

                yield return null;
            }

            rotationTarget
                .localRotation =
                end;

            completed?.Invoke();

            currentRoutine =
                null;
        }
    }
}