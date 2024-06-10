using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bat : MonoBehaviour
{
    public Animator animator;
    
    public int maxHealth = 100;
    private int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Dead");
        var rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 1;
        gameObject.GetComponentInParent<AIDestinationSetter>().enabled = false;
        gameObject.GetComponentInParent<AIPath>().enabled = false;
        enabled = false;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
