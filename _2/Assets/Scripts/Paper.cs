using UnityEngine;
using System.Collections;

public class Paper : MonoBehaviour, IInteractable
{
    [Header("Paper Movement")]
    [SerializeField] private Transform targetPosition;    // 옮겨질 목표 위치
    [SerializeField] private float moveTime = 0.6f;       // 이동 시간
    [SerializeField] private bool isSpecialPaper = false; // 이 종이가 카메라 인터랙트 대상인가?

    [Header("Camera Focus (for special paper only)")]
    [SerializeField] private Transform focusPoint;        // 카메라가 이동할 위치
    [SerializeField] private float focusMoveTime = 0.8f;
    [SerializeField] private float focusHoldTime = 3.0f;

    private bool isMoved = false;
    private bool isMoving = false;

    public void Interact(GameObject player)
    {
        if (isMoving) return;

        if (isSpecialPaper)
        {
            // 특별 종이: 카메라 줌인 연출
            Camera cam = Camera.main;
            if (cam != null && focusPoint != null)
                player.GetComponent<MonoBehaviour>().StartCoroutine(FocusSequence(cam, player));
        }
        else
        {
            // 일반 종이: 지정 위치로 훅 이동
            player.GetComponent<MonoBehaviour>().StartCoroutine(MovePaper());
        }
    }

    private IEnumerator MovePaper()
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 targetPos = targetPosition.position;
        Quaternion targetRot = targetPosition.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            transform.rotation = Quaternion.Slerp(startRot, targetRot, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        isMoved = true;
        isMoving = false;
    }

    private IEnumerator FocusSequence(Camera cam, GameObject player)
    {
        isMoving = true;
        PlayerLook look = player.GetComponent<PlayerLook>();
        if (look != null) look.enabled = false;

        Vector3 startPos = cam.transform.position;
        Quaternion startRot = cam.transform.rotation;

        Vector3 targetPos = focusPoint.position;
        Quaternion targetRot = focusPoint.rotation;

        float t = 0f;

        // 줌인
        while (t < 1f)
        {
            t += Time.deltaTime / focusMoveTime;
            cam.transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            cam.transform.rotation = Quaternion.Slerp(startRot, targetRot, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // 머무르기
        yield return new WaitForSeconds(focusHoldTime);

        // 줌아웃
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / focusMoveTime;
            cam.transform.position = Vector3.Lerp(targetPos, startPos, Mathf.SmoothStep(0, 1, t));
            cam.transform.rotation = Quaternion.Slerp(targetRot, startRot, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        if (look != null) look.enabled = true;
        isMoving = false;
    }

    public string GetInteractionText()
    {
        return isSpecialPaper ? "문서 자세히 보기 (E)" : "종이 옮기기 (E)";
    }
}
