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
        Movement moveScript = GetComponent<Movement>();
        if (moveScript != null && moveScript.isBlocking)
        {
            Debug.Log(gameObject.name + " BLOCKED the attack!");
            
            // Still push them backward a tiny bit for impact, but don't deal damage!
            float guardKnockbackDir = attacker.position.x < transform.position.x ? 1f : -1f;
            StartCoroutine(KnockbackRoutine(guardKnockbackDir * 0.5f)); 
            
            return; // This completely stops the rest of the damage code from running!
        }
        
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

    private IEnumerator KnockbackRoutine(float direction)
    {
        ZombieAI aiScript = GetComponent<ZombieAI>();
        bool wasHuman = movementScript.enabled;
        bool wasZombie = (aiScript != null && aiScript.enabled);

        movementScript.enabled = false;
        if (aiScript != null) aiScript.enabled = false;

        rb.linearVelocity = new Vector2(direction * knockbackForce, knockbackForce / 2f);

        // Wait for the knockback time to finish
        yield return new WaitForSeconds(knockbackTime);

        // --- NEW: THE DEATH CHECK ---
        // If this hit killed us, stop the code right here! Do not revive!
        if (currentHealth <= 0) yield break;

        // Give control back to the CORRECT brain!
        if (wasHuman) movementScript.enabled = true;
        if (wasZombie) aiScript.enabled = true;
    }
}