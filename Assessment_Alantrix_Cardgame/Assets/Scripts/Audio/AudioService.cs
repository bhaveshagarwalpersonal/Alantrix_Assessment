using UnityEngine;

namespace EchoGrid.Audio
{
    public sealed class AudioService :
        MonoBehaviour
    {
        [SerializeField]
        private AudioSource source;

        [SerializeField]
        private AudioClip flipClip;

        [SerializeField]
        private AudioClip matchClip;

        [SerializeField]
        private AudioClip mismatchClip;

        [SerializeField]
        private AudioClip winClip;

        public void PlayFlip()
        {
            Play(
                flipClip);
        }

        public void PlayMatch()
        {
            Play(
                matchClip);
        }

        public void PlayMismatch()
        {
            Play(
                mismatchClip);
        }

        public void PlayWin()
        {
            Play(
                winClip);
        }

        private void Play(
            AudioClip clip)
        {
            if (
                clip != null)
            {
                source.PlayOneShot(
                    clip);
            }
        }
    }
}