using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public static PlayerHealth instance;

    
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    
    public bool IsAtMax() => currentHealth == maxHealth;
    
    public void AddHealth(int healAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
    }
}
