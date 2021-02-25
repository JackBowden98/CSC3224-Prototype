using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float knockbackSpeedY, knockbackSpeedX, knockbackDuration;
    [SerializeField] private float knockbackDeathSpeedY, knockbackDeathSpeedX, deathTorque;

    [SerializeField] private bool applyKnockback;
    private bool knockback;
    private float knockbackStart;

    private float currentHealth;

    private CharacterController2D player;
    private int playerFacingDirection;
    private bool playerOnLeft;

    private Rigidbody2D m_Rigidbody2D;
    private Animator anim;

    private void Start()
    {
        currentHealth = maxHealth;

        player = GameObject.Find("Player").GetComponent<CharacterController2D>();

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        gameObject.SetActive(true);
    }

    private void Update()
    {
        CheckKnockback();
    }

    private void Damage(float hitPoints)
    {
        currentHealth -= hitPoints;
        playerFacingDirection = player.GetFacingDirection();

        if (playerFacingDirection == 1)
        {
            playerOnLeft = true;
        }
        else
        {
            playerOnLeft = false;
        }

        anim.SetBool("PlayerOnLeft", playerOnLeft);
        anim.SetTrigger("Damage");

        if (applyKnockback && currentHealth > 0.0f)
        {
            Knockback();
        }

        if (currentHealth <= 0.0f)
        {
            Die();
        }
    }

    private void Knockback()
    {
        knockback = true;
        knockbackStart = Time.time;
        m_Rigidbody2D.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbackSpeedY);
    }

    private void CheckKnockback()
    {
        if (Time.time >= knockbackStart + knockbackDuration && knockback)
        {
            knockback = false;
            m_Rigidbody2D.velocity = new Vector2(0.0f, m_Rigidbody2D.velocity.y);
        }
    }

    private void Die()
    {
        m_Rigidbody2D.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbackSpeedY);
        m_Rigidbody2D.AddTorque(deathTorque * -playerFacingDirection, ForceMode2D.Impulse);
        gameObject.SetActive(false);
    }
}
