using UnityEngine;
using System.Collections;

public class ClothesInteract : MonoBehaviour, IInteractable
{
    [Header("ID Card Settings")]
    [SerializeField] private Transform idCard;            // 신분증 오브젝트
    [SerializeField] private Transform cardStartPoint;    // 신분증이 들어있는 위치 (옷 안)
    [SerializeField] private Transform cardEndPoint;      // 신분증이 나올 위치 (카메라 앞)
    [SerializeField] private float cardMoveTime = 1.2f;

    [Header("Camera Focus")]
    [SerializeField] private Transform focusPoint;        // 카메라가 이동할 위치
    [SerializeField] private float focusMoveTime = 1.0f;
    [SerializeField] private float focusHoldTime = 2.5f;

    private bool isPlaying = false;

    public void Interact(GameObject player)
    {
        if (isPlaying) return;
        isPlaying = true;

        StartCoroutine(InteractionSequence(player));
    }

    private IEnumerator InteractionSequence(GameObject player)
    {
        Camera cam = Camera.main;
        PlayerLook look = player.GetComponent<PlayerLook>();
        if (look != null) look.enabled = false;

        // --- 1️⃣ 카메라 이동 (옷 쪽으로) ---
        Vector3 camStartPos = cam.transform.position;
        Quaternion camStartRot = cam.transform.rotation;
        Vector3 camTargetPos = focusPoint.position;
        Quaternion camTargetRot = focusPoint.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / focusMoveTime;
            cam.transform.position = Vector3.Lerp(camStartPos, camTargetPos, Mathf.SmoothStep(0, 1, t));
            cam.transform.rotation = Quaternion.Slerp(camStartRot, camTargetRot, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // --- 2️⃣ 신분증 꺼내기 ---
        idCard.position = cardStartPoint.position;
        idCard.rotation = cardStartPoint.rotation;

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / cardMoveTime;
            idCard.position = Vector3.Lerp(cardStartPoint.position, cardEndPoint.position, Mathf.SmoothStep(0, 1, t));
            idCard.rotation = Quaternion.Slerp(cardStartPoint.rotation, cardEndPoint.rotation, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // --- 3️⃣ 신분증 보여주기 대기 ---
        yield return new WaitForSeconds(focusHoldTime);

        // --- 4️⃣ 신분증 다시 넣기 ---
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / cardMoveTime;
            idCard.position = Vector3.Lerp(cardEndPoint.position, cardStartPoint.position, Mathf.SmoothStep(0, 1, t));
            idCard.rotation = Quaternion.Slerp(cardEndPoint.rotation, cardStartPoint.rotation, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // --- 5️⃣ 카메라 복귀 ---
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / focusMoveTime;
            cam.transform.position = Vector3.Lerp(camTargetPos, camStartPos, Mathf.SmoothStep(0, 1, t));
            cam.transform.rotation = Quaternion.Slerp(camTargetRot, camStartRot, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        if (look != null) look.enabled = true;
        isPlaying = false;
    }

    public string GetInteractionText()
    {
        return "옷에서 신분증 꺼내기 (E)";
    }
}
