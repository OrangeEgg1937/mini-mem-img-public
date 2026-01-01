using System.Collections.Generic;
using System.Linq;
using Akua.Tool.ShuffleUtil;
using Script.Core;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder Instance { get; private set; }
    
    // Private member
    private LevelBuilder _levelBuilder;
    private List<FragmentData> _fragmentsPool = new List<FragmentData>();
    private Queue<FragmentData> _fragmentsQueue = new Queue<FragmentData>();
    public FragmentData targetFragment { get; private set; }

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

    public void CreateFragmentsByList(List<FragmentData> fragments)
    {
        // Reserve fragments pool
        // float totalWeight = fragments.Sum(fragment => fragment.rawImageResData.appearRate);

        foreach (var fg in fragments)
        {
            if (fg.isMeme)
            {
                targetFragment = fg;
            }
            else
            {
                _fragmentsPool.Add(fg);
            }
        }

        FillFragmentsQueueIfNeeded();
    }
    
    private void FillFragmentsQueueIfNeeded()
    {
        if (_fragmentsQueue.Count == 0)
        {
            var shuffledFragments = ShuffleUtil.ShuffleUnChanged(_fragmentsPool);
            foreach (var fg in shuffledFragments)
            {
                _fragmentsQueue.Enqueue(fg);
            }
        }
    }
    
    public FragmentData RequestFragmentData()
    {
        FillFragmentsQueueIfNeeded();
        return _fragmentsQueue.Dequeue();
    }
}
