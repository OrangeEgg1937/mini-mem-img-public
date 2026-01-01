using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Script.Core.Config
{
    public static class ConfigLoader
    {
        private const string CONFIG_FILENAME = "config.json";

        public static async UniTask<GameConfiguration> LoadConfigFromStreamingAssets()
        {
            UnityWebRequest www = await UnityWebRequest.Get(
                Path.Combine(Application.streamingAssetsPath, CONFIG_FILENAME)).SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error loading JSON: {www.error}");
                throw new FileLoadException(www.error);
            }

            return JsonUtility.FromJson<GameConfiguration>(www.downloadHandler.text);
        }
    }
}