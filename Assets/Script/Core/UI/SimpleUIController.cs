using UnityEngine;
using UnityEngine.UIElements;

public class SimpleUIController : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    
    private Label _timerLabel;
    private Label _playerMarkLabel;
    private Label _titleLabel;
    
    private void Awake()
    {
        var root = _uiDocument.rootVisualElement;
        _timerLabel = root.Q<Label>("timer_label");
        _playerMarkLabel = root.Q<Label>("player_mark_label");
        _titleLabel = _uiDocument.rootVisualElement.Q<Label>("dynamic_title_label");
    }
    
    public void UpdateTimerLabel(float time)
    {
        if (time < 0f)
        {
            _timerLabel.text = "0.00";
            return;
        }
        
        _timerLabel.text = $"{time:F2}";
    }
    
    public void UpdatePlayerMarkLabel(int mark)
    {
        _playerMarkLabel.text = $"{mark}";
    }
    
    public void SetupUITitle(int targetMark, float time)
    {
        _titleLabel.text = $"點擊迷迷，收集到{targetMark}分數！\n{time:0}秒內盡量點擊！";
    }
}
