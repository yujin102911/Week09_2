using UnityEngine;

public class LightInteract : MonoBehaviour, IInteractable
{
    [Header("Light Settings")]
    [SerializeField] private Light targetLight;              // 켜고 끌 라이트
    [SerializeField] private Renderer emissiveRenderer;      // emission 효과용 (선택)
    [SerializeField] private Color emissionColor = Color.white;
    [SerializeField] private bool isOn = false;

    [Header("Optional Effects")]
    [SerializeField] private AudioSource clickSound;         // 스위치 소리 (선택)
    [SerializeField] private float fadeSpeed = 8f;           // 밝기 전환 속도

    private float currentIntensity = 0f;
    private float targetIntensity = 0f;

    private void Start()
    {
        if (targetLight != null)
        {
            currentIntensity = targetLight.intensity;
            targetIntensity = isOn ? currentIntensity : 0f;
            targetLight.intensity = targetIntensity;
        }

        UpdateEmission(isOn);
    }

    public void Interact(GameObject player)
    {
        isOn = !isOn;
        targetIntensity = isOn ? 1f : 0f;

        if (clickSound != null)
            clickSound.Play();

        UpdateEmission(isOn);
        StopAllCoroutines();
        StartCoroutine(SmoothLightChange());
    }

    private System.Collections.IEnumerator SmoothLightChange()
    {
        if (targetLight == null) yield break;

        float startIntensity = targetLight.intensity;
        float endIntensity = isOn ? 1f : 0f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            targetLight.intensity = Mathf.Lerp(startIntensity, endIntensity, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        targetLight.intensity = endIntensity;
    }

    private void UpdateEmission(bool on)
    {
        if (emissiveRenderer == null) return;

        Material mat = emissiveRenderer.material;
        if (on)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emissionColor * 2f);
        }
        else
        {
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    public string GetInteractionText()
    {
        return isOn ? "불 끄기 (E)" : "불 켜기 (E)";
    }
}
