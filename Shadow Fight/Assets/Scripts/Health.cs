using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackTime = 0.2f;

    private Rigidbody2D rb;
    private Movement movementScript;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<Movement>();

        // --- SAFETY NET ---
        if (rb == null) Debug.LogError("WARNING: " + gameObject.name + " is missing a Rigidbody2D!");
        if (movementScript == null) Debug.LogError("WARNING: " + gameObject.name + " is missing the Movement script!");
    }

    public void TakeDamage(int damageAmount,Transform attacker)
    {
        currentHealth -= damageAmount;
        
        Debug.Log(gameObject.name + " took " + damageAmount + " damage! Health left: " + currentHealth);
        float knockbackDirection = attacker.position.x < transform.position.x ? 1f : -1f;
        StartCoroutine(KnockbackRoutine(knockbackDirection));
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

    private void Die()
    {
        Debug.Log(gameObject.name + " HAS BEEN DEFEATED!");
    }
}