using TMPro;
using UnityEngine;
using EchoGrid.Scoring;

namespace EchoGrid.UI
{
    public sealed class GameHUD :
        MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private TMP_Text comboText;

        [SerializeField]
        private TMP_Text seedText;

        [SerializeField]
        private GameObject winPanel;

        [SerializeField]
        private TMP_Text finalScoreText;

        public void Refresh(
            ScoreService score)
        {
            scoreText.text =
                $"Score: {score.Score}";

            comboText.text =
                score.Combo > 0
                    ? $"Combo x{score.Combo}"
                    : string.Empty;
        }

        public void SetSeed(
            int seed)
        {
            seedText.text =
                $"Seed: {seed}";
        }

        public void ShowWin()
        {
            winPanel.SetActive(
                true);

            finalScoreText.text =
                "Final Score: " +
                scoreText.text;
        }
    }
}