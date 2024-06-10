using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public Animator Animator;
    public int maxHP = 100;
    private int currentHP;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
    }

    public bool TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
            return true;
        }

        return false;
    }

    void Die()
    {
        Debug.Log("GAME OVER");
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Animator.SetTrigger("Die");
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<PlayerCombat>().enabled = false;
        enabled = false;
    }

    public int GetPlayerCurrentHP()
    {
        return currentHP;
    }
}
