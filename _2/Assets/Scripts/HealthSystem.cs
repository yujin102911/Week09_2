using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    public int MaxHealth => maxHealth;

    public int CurrentHealth { get; private set; }

    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged; // (current, max)
    public UnityEvent onDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public bool CanPay(int cost) => CurrentHealth >= cost;

    public void Pay(int cost, string reason = "")
    {
        if (cost <= 0) return;
        CurrentHealth = Mathf.Max(0, CurrentHealth - cost);
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);
        Debug.Log($"[HP] -{cost} ({reason}) => {CurrentHealth}/{maxHealth}");

        if (CurrentHealth <= 0)
            onDeath?.Invoke();
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

}
