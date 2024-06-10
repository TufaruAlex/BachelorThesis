using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;

    public Transform attackPoint;

    public float attackRange = 0.5f;

    public LayerMask enemyLayer;

    public int attackDamage = 35;

    public float attackRate = 1;
    private float nextAttackTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
                
                /*if (animator.GetBool("IsJumping"))
                {
                    animator.SetBool("IsJumping", false);
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                    animator.SetBool("IsJumping", false);
                }
                else
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }*/
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        var hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (var enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            if (enemy.GetComponent<Bat>().GetCurrentHealth() > 0)
            {
                enemy.GetComponent<Bat>().TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
