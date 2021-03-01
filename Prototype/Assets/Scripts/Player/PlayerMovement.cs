using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;

	public CollectableManager cm;


	// Update is called once per frame
	void Update () {

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump"))
		{
			jump = true;
			animator.SetBool("IsJumping", true);
			animator.SetBool("IsSliding", false);
		}

		if (controller.Falling())
        {
			animator.SetBool("IsFalling", true);
			animator.SetBool("IsSliding", false);
		}

		if (Input.GetKeyDown("p"))
		{
			cm.CheatAllSouls();
        }
	}

	public void OnLanding ()
	{
		animator.SetBool("IsJumping", false);
		animator.SetBool("IsSliding", false);
		animator.SetBool("IsFalling", false);
	}

	public void OnSliding()
	{
		animator.SetBool("IsSliding", true);
		animator.SetBool("IsJumping", false);
		animator.SetBool("IsFalling", false);
	}

	void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
		jump = false;
	}
}
