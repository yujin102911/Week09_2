using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Phone : MonoBehaviour, IInteractable
{
    [Header("Phone Screen")]
    [SerializeField] private GameObject phoneUI;
    [SerializeField] private Image screenImage;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    [Header("Camera Focus")]
    [SerializeField] private Transform focusPoint;   // PhoneFocusPoint 지정
    [SerializeField] private float focusMoveTime = 1.0f; // 카메라 이동 시간
    [SerializeField] private float focusHoldTime = 4.0f; // 머무는 시간

    [Header("Auto Turn Off")]
    [SerializeField] private float autoOffTime = 6.0f; // 켜진 뒤 자동으로 꺼지는 시간(초)

    private bool isOn = false;
    private bool isFocusing = false;
    private Coroutine autoOffCoroutine;

    public void Interact(GameObject player)
    {
        if (isFocusing) return; // 중복 방지

        isOn = !isOn;
        SetScreen(isOn);
        Debug.Log($"📱 Phone {(isOn ? "ON" : "OFF")}");

        // 자동 꺼짐 타이머 관리
        if (autoOffCoroutine != null)
            StopCoroutine(autoOffCoroutine);

        if (isOn)
        {
            autoOffCoroutine = StartCoroutine(AutoTurnOff());

            Camera mainCam = Camera.main;
            if (mainCam != null && focusPoint != null)
                StartCoroutine(FocusSequence(mainCam, player));
        }
    }

    private void SetScreen(bool state)
    {
        if (phoneUI != null) phoneUI.SetActive(true);
        if (screenImage != null) screenImage.sprite = state ? onSprite : offSprite;
    }

    private IEnumerator AutoTurnOff()
    {
        yield return new WaitForSeconds(autoOffTime);
        if (isOn)
        {
            isOn = false;
            SetScreen(false);
            Debug.Log("📴 Phone automatically turned OFF (timeout)");
        }
    }

    private IEnumerator FocusSequence(Camera cam, GameObject player)
    {
        isFocusing = true;

        // ✅ PlayerLook 잠깐 꺼주기
        PlayerLook look = player.GetComponent<PlayerLook>();
        if (look != null) look.enabled = false;

        // 현재 카메라 위치/회전 저장
        Vector3 startPos = cam.transform.position;
        Quaternion startRot = cam.transform.rotation;

        // 목표 위치/회전 (FocusPoint)
        Vector3 targetPos = focusPoint.position;
        Quaternion targetRot = focusPoint.rotation;

        float t = 0f;

        // ---- 1. 폰 쪽으로 스윽 이동 ----
        while (t < 1f)
        {
            t += Time.deltaTime / focusMoveTime;
            cam.transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            cam.transform.rotation = Quaternion.Slerp(startRot, targetRot, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        // ---- 2. 잠시 머무르기 ----
        yield return new WaitForSeconds(focusHoldTime);

        // ---- 3. 원래 위치로 복귀 ----
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / focusMoveTime;
            cam.transform.position = Vector3.Lerp(targetPos, startPos, Mathf.SmoothStep(0f, 1f, t));
            cam.transform.rotation = Quaternion.Slerp(targetRot, startRot, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        // ✅ PlayerLook 다시 켜주기
        if (look != null) look.enabled = true;

        isFocusing = false;
    }

    public string GetInteractionText()
    {
        return isOn ? "폰 화면 보기 (E)" : "폰 켜기 (E)";
    }
}
