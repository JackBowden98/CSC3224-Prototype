using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] public float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Transform m_FrontCheck;							// A position marking where to check for walls

	const float k_GroundedRadius = .2f;		// Radius of the overlap circle to determine if grounded
	private bool m_Grounded;				// Whether or not the player is grounded.
	const float k_CeilingRadius = .2f;      // Radius of the overlap circle to determine if the player can stand up

	bool isTouchingFront;
	bool m_WallSliding;
	const float k_WallRadius = .2f;      // Radius of the overlap circle to determine if next to wall
	public float wallSlidingSpeed;

	// if flipping is allowed
	public bool canFlip = true;

	private bool falling;
	private bool m_AirControl = false;

	private bool wallJump;
	public float xWallForce;
	public float yWallForce;
	public float wallJumpForceDuration;

	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;
	private int facingDirection = 1;
	private Vector3 m_Velocity = Vector3.zero;

	private bool knockback;
    private float knockbackStartTime;
	[SerializeField] private float knockbackDuration;
	[SerializeField] private Vector2 knockbackSpeed;

	private bool recoil;
	private float recoilStartTime;
	[SerializeField] private float recoilDuration;
	[SerializeField] private float recoilSpeed;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public UnityEvent OnSlideEvent;


	private void Awake()
	{
		m_AirControl = true;
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnSlideEvent == null)
			OnSlideEvent = new UnityEvent();

	}

    private void Update()
    {
		CheckKnockback();
	}

    private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] groundColliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < groundColliders.Length; i++)
		{
			if (groundColliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

		bool wasTouchingWall = isTouchingFront;
		isTouchingFront = false;
		m_WallSliding = false;
		falling = false;

		Collider2D[] wallColliders = Physics2D.OverlapCircleAll(m_FrontCheck.position, k_WallRadius, m_WhatIsGround);
		for (int i = 0; i < wallColliders.Length; i++)
		{
			if (wallColliders[i].gameObject != gameObject)
			{
				isTouchingFront = true;
				if (!m_Grounded && isTouchingFront && Input.GetAxis("Horizontal") != 0)
                {
					if (!wasTouchingWall)
						OnSlideEvent.Invoke();
				}
			}
		}
	}


	public void Move(float move, bool jump)
	{
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl && !knockback)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		if (!m_Grounded && isTouchingFront)
		{
			m_WallSliding = true;
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, Mathf.Clamp(m_Rigidbody2D.velocity.y, -wallSlidingSpeed, float.MaxValue));
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
		if (m_WallSliding && jump)
		{
			m_WallSliding = false;
			wallJump = true;
			m_AirControl = false;
			Flip();
			Invoke("SetWallJumpFalse", wallJumpForceDuration);
		}
		if (wallJump)
		{
			m_Rigidbody2D.velocity = new Vector2(facingDirection * xWallForce, yWallForce);
		}
		if (!m_Grounded && !m_WallSliding && !jump)
        {
			falling = true;
        }
	}

	public void KnockBack(int direction)
    {
		knockback = true;
		knockbackStartTime = Time.time;
		m_Rigidbody2D.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }

	private void CheckKnockback()
    {
		if (Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
			knockback = false;
			m_Rigidbody2D.velocity = new Vector2(0.0f, m_Rigidbody2D.velocity.y);

		}
    }

	public void Recoil(int direction)
	{
		recoil = true;
		recoilStartTime = Time.time;
		m_Rigidbody2D.velocity = new Vector2(recoilSpeed * direction, m_Rigidbody2D.velocity.y);
	}

	private void CheckRecoil()
	{
		if (Time.time >= recoilStartTime + recoilDuration && recoil)
		{
			recoil = false;
			m_Rigidbody2D.velocity = new Vector2(0.0f, m_Rigidbody2D.velocity.y);

		}
	}

	private void SetWallJumpFalse()
    {
		wallJump = false;
		m_AirControl = true;
	}

	public bool Falling()
    {
		return falling;
	}

	public int GetFacingDirection()
    {
		return facingDirection;
    }

	private void Flip()
	{
		if (canFlip && !knockback)
        {
			// Switch the way the player is labelled as facing.
			m_FacingRight = !m_FacingRight;
			facingDirection *= -1;

			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
        }
	}

	public void DisableFlip()
    {
		canFlip = false;
    }

	public void EnableFlip()
	{
		canFlip = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Soul"))
		{
			Destroy(collision.gameObject);

		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(m_FrontCheck.position, k_WallRadius);
	}
}
