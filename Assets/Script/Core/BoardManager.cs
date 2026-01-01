using System;
using System.Collections;
using System.Collections.Generic;
using Akua.Tool.ShuffleUtil;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    private static Vector2Int _boardSize = new Vector2Int(3, 3);

    public UnityEvent onBoardResetComplete;

    public bool isBoardReady { get; private set; } = true;
    
    [Header("Settings")] 
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] [Range(0f, 0.5f)] private float randomnessStrength = 0.5f;

    [Header("Reference")] [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform boardTransform;
    [SerializeField] private GameObject fragmentPrefab;
    [SerializeField] private ParticleSystem _refreshEffect;

    private List<int> _shuffleTilePosition1D = new List<int>();
    private List<int> _shuffleTileCached = new List<int>();
    private List<CellData> _cellPool = new List<CellData>();
    private List<Vector2Int> _LazyPosition2DMap = new List<Vector2Int>()
    {
        new Vector2Int(-2, 2), new Vector2Int(0, 2), new Vector2Int(2, 2),
        new Vector2Int(-2, 0), new Vector2Int(0, 0), new Vector2Int(2, 0),
        new Vector2Int(-2, -2), new Vector2Int(0, -2), new Vector2Int(2, -2),
    };
    private int _currentCellIndex = 0;
    private Coroutine _refreshCoroutine;

    private void Awake()
    {
        for (int i = 0; i < _boardSize.x * _boardSize.y; i++)
        {
            _shuffleTilePosition1D.Add(i);
        }
    }
    
    public void HardResetBoard()
    {
        SoftResetBoard();
        
        foreach (var cell in _cellPool)
        {
            Destroy(cell.CellGameObject);
        }
        _cellPool.Clear();
    }

    public void SoftResetBoard()
    {
        _refreshEffect.Play();

        if (_refreshCoroutine != null)
        {
            StopCoroutine(_refreshCoroutine);
        }
        _refreshCoroutine = StartCoroutine(WaitBoardReset());
        
        foreach (var cell in _cellPool)
        {
            cell.CellGameObject.SetActive(false);
        }

        _shuffleTileCached = _shuffleTilePosition1D.ShuffleUnChanged();

        _currentCellIndex = 0;
    }

    private IEnumerator WaitBoardReset()
    {
        isBoardReady = false;

        yield return new WaitForSeconds(0.3f);
        
        isBoardReady = true;
    }

    public void GenerateCell(int number)
    {
        SoftResetBoard();

        // Create enough cell for pool regenerate
        if (_cellPool.Count < number)
        {
            int diff = number - _cellPool.Count;
            for (int i = 0; i < diff; i++)
            {
                _cellPool.Add(new CellData()
                {
                    CellGameObject = Instantiate(fragmentPrefab, boardTransform)
                });
            }
        }
        
        // Spawn target first
        Vector3 targetSpawnPos = GetFreeTileWorldPos();
        var targetFd = LevelBuilder.Instance.targetFragment;
        _cellPool[0].CellGameObject.GetComponent<Fragment>().Setup(targetFd);
        _cellPool[0].CellGameObject.transform.position = new Vector3(targetSpawnPos.x, targetSpawnPos.y, -3f);
        _cellPool[0].CellGameObject.SetActive(true);

        for (int x = 1; x < number; x++)
        {
            Vector3 spawnPos = GetFreeTileWorldPos();
            var fd = LevelBuilder.Instance.RequestFragmentData();
            _cellPool[x].CellGameObject.GetComponent<Fragment>().Setup(fd);
            _cellPool[x].CellGameObject.transform.position = new Vector3(spawnPos.x, spawnPos.y, -3f);
            _cellPool[x].CellGameObject.SetActive(true);
        }
    }

    public Vector3 GetFreeTileWorldPos()
    {
        Vector2Int gridPos = GetRandomUniquePosition();
        
        Vector2 offset = Random.insideUnitCircle * randomnessStrength * new Vector2(1.5f, 1.5f);
        Vector3 resultPos = gridPos + offset;
        
        return resultPos;
    }

    private Vector2Int GetRandomUniquePosition()
    {
        Vector2Int pos = _LazyPosition2DMap[_shuffleTileCached[_currentCellIndex]];
        _currentCellIndex++;
        return pos;
    }

    public class CellData
    {
        public GameObject CellGameObject = null;
    }
}