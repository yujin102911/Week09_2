using UnityEngine;
using UnityEngine.UI;

public class UIAimController : MonoBehaviour
{
    [Header("Aim Settings")]
    [SerializeField] private Image aimImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private float colorLerpSpeed = 10f;

    private Color currentTargetColor;

    private void Awake()
    {
        if (aimImage == null)
            aimImage = GetComponent<Image>();

        currentTargetColor = normalColor;
        aimImage.color = normalColor;
    }

    private void Update()
    {
        if (aimImage == null) return;

        // 부드럽게 색 전환
        aimImage.color = Color.Lerp(aimImage.color, currentTargetColor, Time.deltaTime * colorLerpSpeed);
    }

    /// <summary>
    /// 상호작용 가능한 오브젝트 위로 마우스가 향할 때 호출
    /// </summary>
    public void SetHoverState(bool isHovering)
    {
        currentTargetColor = isHovering ? hoverColor : normalColor;
    }
}
