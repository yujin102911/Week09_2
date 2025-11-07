using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Input (New Input System)")]
    [SerializeField] private InputActionReference interactAction; // 예: "E"

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactMask = ~0;
    [SerializeField] private Camera playerCamera;

    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(1f, 0.85f, 0.1f, 0.9f);
    [SerializeField] private bool showHitPoint = true;
    [SerializeField] private float endSphereRadius = 0.12f;

    [SerializeField] private HealthSystem playerHealth;

    [Header("UI Reference")]
    [SerializeField] private UIAimController aimController; // 에임 컨트롤러 연결

    private IInteractable currentTarget;

    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteractPerformed;
        interactAction.action.Disable();
    }

    private void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerHealth == null) playerHealth = GetComponent<HealthSystem>(); // Player에 붙어있다고 가정

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask)) return;

        var interactable = hit.collider.GetComponent<IInteractable>();
        if (interactable == null) return;

        // 비용 컴포넌트 탐색(자식/부모 아무 쪽에 붙어 있어도 인식되게)
        var costComp = hit.collider.GetComponentInParent<InteractCost>();
        int cost = costComp ? costComp.cost : 0;
        string reason = costComp ? costComp.reason : interactable.GetInteractionText();

        // 1) 상호작용 전에 차감 옵션
        if (costComp && costComp.applyBeforeInteraction)
        {
            if (costComp.requireSufficientHealth && !playerHealth.CanPay(cost)) return;
            playerHealth.Pay(cost, reason);
            interactable.Interact(gameObject);
            return;
        }

        // 2) 기본: 먼저 상호작용 수행
        if (costComp && costComp.requireSufficientHealth && !playerHealth.CanPay(cost)) return;
        interactable.Interact(gameObject);

        // 3) 그 다음 비용 차감
        if (cost > 0)
            playerHealth.Pay(cost, reason);
    }

    private void Update()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        // 중앙 레이캐스트
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // 🔸 에임 색 변경
                aimController?.SetHoverState(true);
                currentTarget = interactable;
            }
            else
            {
                aimController?.SetHoverState(false);
                currentTarget = null;
            }
        }
        else
        {
            aimController?.SetHoverState(false);
            currentTarget = null;
        }

        // 상호작용 (E 키)
        if (Keyboard.current.eKey.wasPressedThisFrame && currentTarget != null)
        {
            currentTarget.Interact(gameObject);
        }
    }

    // 씬 뷰에서 보이는 기즈모 (플레이/에디트 둘 다)
    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Camera camRef = playerCamera != null ? playerCamera : Camera.main;
        if (camRef == null) return;

        Vector3 origin = camRef.transform.position;
        Vector3 dir = camRef.transform.forward;
        Vector3 end = origin + dir * interactRange;

        Gizmos.color = gizmoColor;

        // 메인 레이
        Gizmos.DrawLine(origin, end);

        // 끝점 표시
        Gizmos.DrawWireSphere(end, endSphereRadius);

        // 맞은 지점 표시(옵션)
        if (showHitPoint && Physics.Raycast(origin, dir, out RaycastHit hit, interactRange, interactMask))
        {
            Gizmos.DrawWireSphere(hit.point, endSphereRadius * 1.25f);

            // 표면 법선도 살짝
            Gizmos.DrawLine(hit.point, hit.point + hit.normal * 0.25f);
        }
    }
}
