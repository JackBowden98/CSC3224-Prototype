using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private enum State
    {
        Running,
        Knockback,
        Dead
    }

    private State currentState;

    [SerializeField] private Transform groundCheck, wallCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance, wallCheckDistance;
    private bool groundDetected, wallDetected;

    private Rigidbody2D rb;

    private Animator anim;

    private int facingDirection;
    private int damageDirection;

    [SerializeField] private float movementSpeed;
    private Vector2 movement;

    [SerializeField] private float maxHealth;
    private float currentHealth;

    [SerializeField] private float knockbackDuration;
    [SerializeField] private Vector2 knockbackSpeed;
    private float knockbackStartTime;

    [SerializeField] private Transform applyDamageCheck;
    [SerializeField] private float lastApplyDamageTime, applyDamageCooldown, applyDamage, applyDamageWidth, applyDamageHeight;
    [SerializeField] private LayerMask whatIsPlayer;
    private Vector2 applyDamageBottomLeft, applyDamageTopRight;
    private float[] attackDeatils = new float[2];
    HitPause hitPause;

    public GameObject theSoulDrop;
    public Transform soulDropPoint;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingDirection = 1;
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        hitPause = GetComponent<HitPause>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Running:
                UpdateRunningState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    // Walking state
    private void EnterRunningState()
    {

    }

    private void UpdateRunningState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

        CheckApplyDamage();

        if (!groundDetected || wallDetected)
        {
            Flip();
        }
        else
        {
            movement.Set(movementSpeed * facingDirection, rb.velocity.y);
            rb.velocity = movement;
        }
    }

    private void ExitRunningState()
    {

    }


    // Knockback state
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        rb.velocity = movement;
        anim.SetBool("Knockback", true);
    }

    private void UpdateKnockbackState()
    {
        // when the knockback has completed its allocated time
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Running);
        }
    }

    private void ExitKnockbackState()
    {
        anim.SetBool("Knockback", false);
    }

    // Knockback state
    private void EnterDeadState()
    {
        GameObject soul = Instantiate(theSoulDrop, soulDropPoint.position, soulDropPoint.rotation);
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }

    // Rest of the code

    private void Flip()
    {
        facingDirection *= -1;
        gameObject.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    // use array to receive multiple parameters from player object
    private void Damage(float[] attackDetails)
    {
        if (gameObject != null)
        {

            // 0 in the array is the amount of damage being recieved
            currentHealth -= attackDetails[0];

            // calculates whether player is on left or right side of the enemy
            if (attackDetails[1] > rb.transform.position.x)
            {
                damageDirection = -1;
            }
            else
            {
                damageDirection = 1;
            }

            // enemy is still alive
            if (currentHealth > 0.0f)
            {
                //hitPause.Pause();
                SwitchState(State.Knockback);
            }
            else if (currentHealth <= 0.0f)
            {
                hitPause.Pause();
                SwitchState(State.Dead);
            }
        }

    }

    private void CheckApplyDamage()
    {
        if (Time.time > lastApplyDamageTime + applyDamageCooldown)
        {
            applyDamageBottomLeft.Set(applyDamageCheck.position.x - (applyDamageWidth / 2), applyDamageCheck.position.y - (applyDamageHeight / 2));
            applyDamageTopRight.Set(applyDamageCheck.position.x + (applyDamageWidth / 2), applyDamageCheck.position.y + (applyDamageHeight / 2));

            Collider2D hit = Physics2D.OverlapArea(applyDamageBottomLeft, applyDamageTopRight, whatIsPlayer);

            if (hit != null)
            {
                lastApplyDamageTime = Time.time;
                attackDeatils[0] = applyDamage;
                attackDeatils[1] = rb.transform.position.x;
                hit.SendMessage("Damage", attackDeatils);
            }
        }
    }



    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Running:
                ExitRunningState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Running:
                EnterRunningState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

        Vector2 botLeft = new Vector2(applyDamageCheck.position.x - (applyDamageWidth / 2), applyDamageCheck.position.y - (applyDamageHeight / 2));
        Vector2 botRight = new Vector2(applyDamageCheck.position.x + (applyDamageWidth / 2), applyDamageCheck.position.y - (applyDamageHeight / 2));
        Vector2 topRight = new Vector2(applyDamageCheck.position.x + (applyDamageWidth / 2), applyDamageCheck.position.y + (applyDamageHeight / 2));
        Vector2 topLeft = new Vector2(applyDamageCheck.position.x - (applyDamageWidth / 2), applyDamageCheck.position.y + (applyDamageHeight / 2));

        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }

}
