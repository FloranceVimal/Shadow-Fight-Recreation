using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    [Header("Targeting")]
    public Transform playerToChase; // The brain needs to know who to look at!

    [Header("Zombie Stats")]
    public float moveSpeed = 3f;
    public float attackRange = 1.5f; // How close before it starts swinging?
    public float timeBetweenAttacks = 1f; // Cooldown so it doesn't punch 60 times a second

    private Rigidbody2D rb;
    private Animator anim;
    private Health myHealth;
    private float attackTimer;

    void Start()
    {

        if (PlayerPrefs.GetInt("IsSinglePlayer", 0) == 0)
        {
            this.enabled = false; // Shut down the Zombie script
            return; // Stop reading the rest of this Start function
        }

        // If we made it here, it means we are in Single Player mode!
        // Cut the strings! Turn off the human keyboard controls.
        if (GetComponent<Movement>() != null)
        {
            GetComponent<Movement>().enabled = false;
        }


        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        myHealth = GetComponent<Health>();

        // Cut the strings! Turn off the human keyboard controls.
        if (GetComponent<Movement>() != null)
        {
            GetComponent<Movement>().enabled = false;
        }
    }

    void Update()
    {
        // If the zombie is dead, stop thinking!
        if (myHealth.currentHealth <= 0 || playerToChase == null) return;

        // How far away is the player?
        float distanceToPlayer = Vector2.Distance(transform.position, playerToChase.position);

        // Tick down the attack cooldown timer like a stopwatch
        attackTimer -= Time.deltaTime;

        if (distanceToPlayer > attackRange)
        {
            Chase();
        }
        else if (attackTimer <= 0)
        {
            Attack();
        }
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    private void Chase()
    {
        // 1. Figure out which direction the player is (Left is -1, Right is 1)
        float direction = playerToChase.position.x < transform.position.x ? -1f : 1f;

        // 2. Walk in that direction
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // 3. Flip the zombie's body to face the player while KEEPING their original size!
        Vector3 currentScale = transform.localScale;
        
        // Mathf.Abs grabs the true size without negative signs, then we multiply by direction
        currentScale.x = Mathf.Abs(currentScale.x) * direction;
        
        transform.localScale = currentScale;
    }
    private void Attack()
    {
        // 1. Stop walking to brace for the punch
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // 2. Trigger the punch animation! 
        // IMPORTANT: Change "Punch" to match whatever your attack trigger is named in the Animator!
        anim.SetTrigger("Attack");

        // 3. Reset the cooldown timer
        attackTimer = timeBetweenAttacks;
    }
}