using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    // if combat is enabled
    [SerializeField] private bool combatEnabled;
    // time you can apply an input and attack damage
    [SerializeField] private float inputTimer, attackDamage;

    // fields required for circle cast for damage
    [SerializeField] private Transform m_HitCheck;
    [SerializeField] private float k_HitRadius;
    [SerializeField] private LayerMask m_WhatIsDamagable;

    // if there is an iput and is attacking
    private bool gotInput, isAttacking;

    // time of the last input
    private float lastInputTime = Mathf.NegativeInfinity;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("CanAttack", combatEnabled);
    }

    private void Update()
    {
        CheckCombatInput();
        checkAttacks();
    }

    // checks the input and enables combat
    private void CheckCombatInput()
    {
        if (Input.GetButtonDown("Attack"))
        {
            if (combatEnabled)
            {
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }

    // if there is an input and not already attacking, attack
    private void checkAttacks()
    {
        if (gotInput)
        {
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                anim.SetBool("Attack", true);
                anim.SetBool("IsAttacking", isAttacking);
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
    }

    private void CheckAttackHitbox()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(m_HitCheck.position, k_HitRadius, m_WhatIsDamagable);
        foreach (Collider2D collider in hitColliders)
        {
            Debug.Log("Hit!");
            collider.SendMessage("Damage", attackDamage);
        }
    }

    private void FinishAttack()
    {
        isAttacking = false;
        anim.SetBool("IsAttacking", isAttacking);
        anim.SetBool("Attack", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(m_HitCheck.position, k_HitRadius);
    }
}
