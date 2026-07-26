using UnityEngine;

namespace EchoGrid.Persistence
{
    public sealed class SaveService
    {
        private const string SaveKey =
            "echo_grid_save";

        public void Save(
            SaveData data)
        {
            string json =
                JsonUtility.ToJson(
                    data);

            PlayerPrefs.SetString(
                SaveKey,
                json);

            PlayerPrefs.Save();
        }

        public SaveData Load()
        {
            if (
                !PlayerPrefs.HasKey(
                    SaveKey))
            {
                return null;
            }

            string json =
                PlayerPrefs.GetString(
                    SaveKey);

            return JsonUtility
                .FromJson<SaveData>(
                    json);
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(
                SaveKey);

            PlayerPrefs.Save();
        }
    }
}