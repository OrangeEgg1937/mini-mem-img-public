using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Counter : MonoBehaviour
{
    public UnityEvent onCounterCompleted;
    public UnityEvent<float> onCounterUpdated;

    private bool _isCounterActive = false;
    private bool _isKeepLooping = false;
    private float _lastUpdateTime;
    [SerializeField] private float duration = 2.5f;

    private void Update()
    {
        if (!_isCounterActive) return;

        onCounterUpdated.Invoke(duration - (Time.time - _lastUpdateTime));
        
        if (!(Time.time - _lastUpdateTime >= duration)) return;
        _isCounterActive = _isKeepLooping;
        onCounterCompleted?.Invoke();

        _lastUpdateTime = Time.time;
    }

    public void StartCounter(float countdownTime, bool keepLooping = false)
    {
        duration = countdownTime;
        _isKeepLooping = keepLooping;
        _isCounterActive = true;
        _lastUpdateTime = Time.time;
    }
    
    public void UpdateDuration(float newDuration)
    {
        StartCounter(newDuration, _isKeepLooping);
    }

    public void StopCounter()
    {
        _isCounterActive = false;
    }

    public void ResetCounter()
    {
        StartCounter(duration, _isKeepLooping);
    }
}