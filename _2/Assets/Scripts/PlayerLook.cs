using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Input (New Input System)")]
    [SerializeField] private InputActionReference lookAction; // Vector2(Mouse)

    [Header("Settings")]
    [SerializeField] private Transform cameraRoot; // 카메라 회전 기준
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float verticalClamp = 80f;

    private Vector2 lookInput;
    private float camPitch;

    private void OnEnable()
    {
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
    }

    private void Update()
    {
        lookInput = lookAction.action.ReadValue<Vector2>() * sensitivity;

        // 마우스 좌우 → 플레이어 회전 (Y축)
        transform.Rotate(Vector3.up * lookInput.x);

        // 마우스 상하 → 카메라 피치 회전
        camPitch -= lookInput.y;
        camPitch = Mathf.Clamp(camPitch, -verticalClamp, verticalClamp);
        cameraRoot.localRotation = Quaternion.Euler(camPitch, 0f, 0f);
    }
}
