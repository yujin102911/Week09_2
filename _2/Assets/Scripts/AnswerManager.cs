using UnityEngine;

public class AnswerManager : MonoBehaviour
{
    [Header("Answer Settings")]
    [SerializeField] private string correctAnswer = "CAT123"; // 정답 문자열
    [SerializeField] private bool isCaseSensitive = false;

    [Header("References (optional)")]
    [SerializeField] private GameObject clearObject; // 클리어 시 나타날 오브젝트
    [SerializeField] private Animator clearAnimator; // 클리어 애니메이션
    [SerializeField] private AudioSource clearSound; // 효과음

    public bool CheckAnswer(string playerInput)
    {
        bool result;
        if (isCaseSensitive)
            result = playerInput == correctAnswer;
        else
            result = playerInput.ToLower() == correctAnswer.ToLower();

        if (result)
            OnClear();

        return result;
    }

    private void OnClear()
    {
        Debug.Log("🎉 정답! 클리어 처리 실행");

        if (clearObject != null)
            clearObject.SetActive(true);

        if (clearAnimator != null)
            clearAnimator.SetTrigger("Clear");

        if (clearSound != null)
            clearSound.Play();

        // TODO: 나중에 GameManager로 스테이지 클리어 통보 가능
        // GameManager.Instance.StageClear();
    }
}
