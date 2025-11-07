using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject answerPanel;
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button openAnswerButton;
    [SerializeField] private TMP_Text resultText;

    private AnswerManager answerManager;

    private void Awake()
    {
        answerManager = FindObjectOfType<AnswerManager>();

        if (answerPanel != null)
            answerPanel.SetActive(false);

        submitButton.onClick.AddListener(OnSubmitAnswer);
        openAnswerButton.onClick.AddListener(OpenAnswerPanel);
    }

    private void OpenAnswerPanel()
    {
        if (answerPanel != null)
            answerPanel.SetActive(true);

        if (resultText != null)
            resultText.text = "";

        answerInput.text = "";
        answerInput.ActivateInputField(); // 바로 입력 가능 상태로
    }

    private void OnSubmitAnswer()
    {
        string playerInput = answerInput.text.Trim();

        if (string.IsNullOrEmpty(playerInput))
        {
            resultText.text = "⚠️ 정답을 입력하세요.";
            return;
        }

        bool isCorrect = answerManager.CheckAnswer(playerInput);
        resultText.text = isCorrect ? "✅ 정답입니다!" : "❌ 오답입니다.";

        if (isCorrect)
        {
            // 정답 시 UI 닫기 (선택)
            StartCoroutine(CloseAfterDelay(1.5f));
        }
    }

    private System.Collections.IEnumerator CloseAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        if (answerPanel != null)
            answerPanel.SetActive(false);
    }
}
