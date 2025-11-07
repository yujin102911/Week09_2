using UnityEngine;

public class InteractCost : MonoBehaviour
{
    [Min(0)] public int cost = 1;
    [Tooltip("로그/디버그용 사유")]
    public string reason = "상호작용 비용";

    [Header("동작 옵션")]
    public bool applyBeforeInteraction = false;   // 상호작용 전에 차감
    public bool requireSufficientHealth = false;  // 체력이 부족하면 상호작용 자체를 막음
}
