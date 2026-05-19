using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Image healthBar;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackTime = 0.2f;

    [Header("Visual Effects")]
    public SpriteRenderer spriteRenderer;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    private Rigidbody2D rb;
    private Movement movementScript;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<Movement>();
        UpdateHealthBar();
        // --- SAFETY NET ---
        if (rb == null) Debug.LogError("WARNING: " + gameObject.name + " is missing a Rigidbody2D!");
        if (movementScript == null) Debug.LogError("WARNING: " + gameObject.name + " is missing the Movement script!");
    }

    public void TakeDamage(int damageAmount,Transform attacker)
    {
        currentHealth -= damageAmount;
        
        Debug.Log(gameObject.name + " took " + damageAmount + " damage! Health left: " + currentHealth);
        UpdateHealthBar();
        float knockbackDirection = attacker.position.x < transform.position.x ? 1f : -1f;
        StartCoroutine(KnockbackRoutine(knockbackDirection));
        if (spriteRenderer != null) 
        {
            StartCoroutine(FlashRoutine());
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private IEnumerator KnockbackRoutine(float direction)
    {
        // 1. Turn OFF the player's ability to move (Hit Stun!)
        movementScript.enabled = false;

        // 2. Shove them backward and slightly up into the air
        rb.linearVelocity = new Vector2(direction * knockbackForce, knockbackForce / 2f);

        // 3. Wait for the knockback time to finish (e.g., 0.2 seconds)
        yield return new WaitForSeconds(knockbackTime);

        // 4. Turn the player's movement back ON so they can fight back
        movementScript.enabled = true;
    }

    private IEnumerator FlashRoutine()
    {
        // 1. Remember the original color (usually pure white)
        Color originalColor = spriteRenderer.color;

        // 2. Change the sprite to the damage color (Red)
        spriteRenderer.color = damageColor;

        // 3. Wait for a tiny fraction of a second
        yield return new WaitForSeconds(flashDuration);

        // 4. Change it back to normal
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        // Find the GameManager in the scene and tell it we just got knocked out!
        FindObjectOfType<GameManager>().Knockout(this.gameObject);
    }
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            // Fill Amount requires a decimal between 0 and 1 (like 0.9 for 90%)
            // We use (float) to force Unity to do decimal math instead of whole numbers!
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}