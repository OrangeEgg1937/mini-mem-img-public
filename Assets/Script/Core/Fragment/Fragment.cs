using Script.Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fragment : MonoBehaviour, IPointerClickHandler
{
    private static readonly Vector3 DefaultPos = new Vector3(0f, 0f, -1f);
    private static readonly Vector2 DefaultSize = new Vector2(1.5f, 1.5f);

    private FragmentData _fragmentData;
    private SpriteRenderer spriteRenderer => _spriteRenderer ??= gameObject.GetComponent<SpriteRenderer>();
    private SpriteRenderer _spriteRenderer;

    public void Setup(FragmentData fragmentData, Vector3 position, Vector2 size)
    {
        _fragmentData = fragmentData;
        spriteRenderer.sprite = fragmentData.sprite;
        spriteRenderer.size = size;
        transform.Translate(position);
        if (!gameObject.GetComponent<BoxCollider2D>())
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    public void Setup(FragmentData fragmentData)
    {
        Setup(fragmentData, DefaultPos, DefaultSize);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GlobalGameManager.Instance.isBoardReady) return;
        
        GlobalGameManager.Instance.UpdatePlayerMark(_fragmentData.isMeme);
    }
}