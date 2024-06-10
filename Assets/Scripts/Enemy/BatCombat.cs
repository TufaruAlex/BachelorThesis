using UnityEngine;
using Pathfinding;

public class BatCombat : MonoBehaviour
{
    public AIPath AIPath;
    public Animator Animator;

    public Transform attackPoint;
    public float attackRange = 0.25f;

    public LayerMask playerLayer;
    public float attackRate = 1f;
    private float nextAttackTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (AIPath.reachedEndOfPath)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        else if (Animator.GetBool("IsAttacking"))
        {
            Animator.SetBool("IsAttacking", false);
        }
    }

    void Attack()
    {
        var hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        if (hitPlayer.Length > 0 && hitPlayer[0].GetComponent<PlayerDamage>().GetPlayerCurrentHP() > 0)
        {
            Animator.SetBool("IsAttacking", true);
            var isDead = hitPlayer[0].GetComponent<PlayerDamage>().TakeDamage(20);
            if (isDead)
            {
                Animator.SetBool("IsAttacking", false);
            }
            Debug.Log("Player got hit! " + hitPlayer[0].name);
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
