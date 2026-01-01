using System;
using Script.Core.Background;
using Script.Core.Config;
using Script.Core.GameSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Core
{
    public class GlobalGameManager : MonoBehaviour
    {
        private enum GameDifficultyState
        {
            Normal,
            Hard,
            Extreme,
            Ultimate
        }
        
        public UnityEvent onGameInitialized;
        
        public bool isGameInitialized { get; private set; } = false;
        public GameConfiguration GameConfiguration { get; private set; }
        public static GlobalGameManager Instance { get; private set; }
        public bool isBoardReady => _boardManager.isBoardReady;

        public UnityEvent<int> onPlayerMarkChanged;
        
        public int playerMark { get; private set; } = 0;
        
        [Header("Game Setting")]
        [SerializeField] private float fragmentRespawnInterval = 4f;
        [SerializeField] private int initialFragmentsCount = 4;
        [SerializeField] private float gameDuration = 30f;
        [SerializeField] private int targetPlayerMark = 100;
        
        [Header("Threshold Setting")]
        [SerializeField] private float hardModeThreshold = 0.2f;
        [SerializeField] private float extremeModeThreshold = 0.5f;
        [SerializeField] private float ultimateModeThreshold = 0.9f;
        
        [SerializeField] private TMP_Text gameOverText;
        [SerializeField] private GameObject loadingUIRoot;
        [SerializeField] private GameObject _endGameUIRoot;
        [SerializeField] private SimpleUIController _setupUITitle;
        [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
        [SerializeField] private BoardManager _boardManager;
        [SerializeField] private Counter _fragmentCounter;
        [SerializeField] private Counter _countdownCounter;
        private bool _isGeneratingNewFragment;
        private GameDifficultyState _currentGameDifficultyState = GameDifficultyState.Normal;

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
        
        private async void Start()
        {
            GameConfiguration = await ConfigLoader.LoadConfigFromStreamingAssets();
            
            // Set the game setting from the configuration
            fragmentRespawnInterval = GameConfiguration.fragmentRespawnInterval;
            initialFragmentsCount = GameConfiguration.initialFragmentsCount;
            gameDuration = GameConfiguration.duration;
            targetPlayerMark = GameConfiguration.targetPlayerMark;
            
            _setupUITitle.SetupUITitle(targetPlayerMark, gameDuration);
            
            GSResult loadImageResult = await GlobalGameResourcesManager.Instance.LoadImage(GameConfiguration);
            
            if (!loadImageResult.isSuccess)
            {
                Debug.LogWarning($"Failed to load game configuration images, Reason: {loadImageResult.err}");
            }
            else
            {
                LevelBuilder.Instance.CreateFragmentsByList(GlobalGameResourcesManager.Instance.fragments);
            }

            backgroundSpriteRenderer.sprite = GlobalGameResourcesManager.Instance.backgroundImage;
            backgroundSpriteRenderer.gameObject.GetComponent<BackgroundScaler>().AutoFit();
            
            isGameInitialized = true;
            onGameInitialized.Invoke();
            loadingUIRoot.SetActive(false);
        }

        public void ResetGame()
        {
            // Hard reset the board
            _boardManager.HardResetBoard();
            
            // Reset the game setting from the configuration
            fragmentRespawnInterval = GameConfiguration.fragmentRespawnInterval;
            initialFragmentsCount = GameConfiguration.initialFragmentsCount;
            gameDuration = GameConfiguration.duration;
            targetPlayerMark = GameConfiguration.targetPlayerMark;
            _currentGameDifficultyState = GameDifficultyState.Normal;
            
            // Reset player marks
            playerMark = 0;
            UpdatePlayerMark(0);

            StartGame();
        }

        public void StartGame()
        {
            _boardManager.GenerateCell(initialFragmentsCount);
            _fragmentCounter.StartCounter(fragmentRespawnInterval, true);
            _countdownCounter.StartCounter(gameDuration, false);
        }
        
        public void GenerateNewFragment()
        {
            if (_isGeneratingNewFragment) return;
            _isGeneratingNewFragment = true;
            IncreaseDifficultyIfNeeded();
            _boardManager.GenerateCell(initialFragmentsCount);
            _fragmentCounter.ResetCounter();
            _isGeneratingNewFragment = false;
        }

        private void IncreaseDifficultyIfNeeded()
        {
            switch (_currentGameDifficultyState)
            {
                case GameDifficultyState.Normal:
                    if (playerMark >= targetPlayerMark * hardModeThreshold)
                    {
                        fragmentRespawnInterval *= 0.8f;
                        initialFragmentsCount += 1;
                        _currentGameDifficultyState = GameDifficultyState.Hard;
                        _fragmentCounter.UpdateDuration(fragmentRespawnInterval);
                    }
                    break;
                case GameDifficultyState.Hard:
                    if (playerMark >= targetPlayerMark * extremeModeThreshold)
                    {
                        fragmentRespawnInterval *= 0.7f;
                        initialFragmentsCount += 2;
                        _currentGameDifficultyState = GameDifficultyState.Extreme;
                        _fragmentCounter.UpdateDuration(fragmentRespawnInterval);

                    }
                    break;
                case GameDifficultyState.Extreme:
                    if (playerMark >= targetPlayerMark * ultimateModeThreshold)
                    {
                        fragmentRespawnInterval *= 0.6f;
                        initialFragmentsCount += 2;
                        _currentGameDifficultyState = GameDifficultyState.Ultimate;
                        _fragmentCounter.UpdateDuration(fragmentRespawnInterval);

                    }
                    break;
                case GameDifficultyState.Ultimate:
                    break;
                default: break;
            }
        }

        private void UpdatePlayerMark(float mark)
        {
            playerMark += Math.Max((int) mark, 0);
            onPlayerMarkChanged.Invoke(playerMark);
            GenerateNewFragment();
        }
        
        public void UpdatePlayerMark(bool isGetMark)
        {
            UpdatePlayerMark(isGetMark ? GameConfiguration.rewardPoints : GameConfiguration.penaltyPoints);
        }
        
        public void OnTimerEnd()
        {
            _endGameUIRoot.SetActive(true);
            
            // Stop all timer
            _fragmentCounter.StopCounter();
            _countdownCounter.StopCounter();

            gameOverText.SetText(playerMark >= targetPlayerMark
                ? GameConfiguration.endGameMessagesWin
                : GameConfiguration.endGameMessagesLose);
        }
    }
}