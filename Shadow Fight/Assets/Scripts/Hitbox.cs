using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage = 10;
    public string targetTag; 

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag(targetTag))
        {
            
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            
            if (enemyHealth != null)
            {
                
                enemyHealth.TakeDamage(damage,transform.parent);
            }
        }
    }
}