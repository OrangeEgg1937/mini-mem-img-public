using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Script.Core.Config;
using Script.Core.GameSystem;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace Script.Core
{
    public class GlobalGameResourcesManager : MonoBehaviour
    {
        public static GlobalGameResourcesManager Instance { get; private set; }
        public List<FragmentData> fragments = new List<FragmentData>();
        public Sprite backgroundImage;

        // private member variables
        private GlobalGameResourcesManager _gameConfiguration;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public async UniTask<GSResult> LoadImage(GameConfiguration gameConfig)
        {
            GSResult result = new()
            {
                isSuccess = true, err = string.Empty
            };

            try
            {
                foreach (var data in gameConfig.rawImageResData)
                {
                    string url = Path.Combine(Application.streamingAssetsPath, gameConfig.resPath, data.name);

                    UnityWebRequest request = await UnityWebRequestTexture.GetTexture(url).SendWebRequest();

                    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                    {
                        Debug.LogError("Error downloading image: " + request.error);
                    }
                    else
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(request);

                        FragmentData fragmentData = new()
                        {
                            rawImageResData = data,
                            sprite = Sprite.Create(
                                texture,
                                new Rect(0, 0, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f),
                                100.0f,
                                0,
                                SpriteMeshType.FullRect
                            )
                        };
                        if (data.typeID == 1)
                        {
                            fragmentData.isMeme = true;
                        }
                        
                        fragments.Add(fragmentData);
                    }
                }
                
                // Load background image
                string backgroundURL = Path.Combine(Application.streamingAssetsPath, gameConfig.backgroundImageName);

                UnityWebRequest backgroundRequest = await UnityWebRequestTexture.GetTexture(backgroundURL).SendWebRequest();
                
                if (backgroundRequest.result == UnityWebRequest.Result.ConnectionError || backgroundRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error downloading image: " + backgroundRequest.error);
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(backgroundRequest);
                    backgroundImage = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f),
                        100.0f,
                        0,
                        SpriteMeshType.FullRect
                    );
                }
            }
            catch (Exception e)
            {
                result.isSuccess = false;
                result.err = e.Message;
            }

            return result;
        }
    }

    [Serializable]
    public class FragmentData
    {
        public Sprite sprite;
        public bool isMeme = false;
        public RawImageResData rawImageResData;
    }
}