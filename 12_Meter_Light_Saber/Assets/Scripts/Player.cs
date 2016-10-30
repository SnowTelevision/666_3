﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    public float jumpHeight;
    public float timeToJumpApex;

    public float accelerationTimeAirorne;
    public float accelerationTimeGrounded;
    public float moveSpeed;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax;
    public float wallStickTime;
    public float timeToWallUnstick;

    public int jumpTimes;
    int jumpsRemaining;
    public float attackPrepTime;
    public float attackCD;
    public float dodgePrepTime;
    public float dodgeCD;

    //float gravity = -80;
    //float jumpVelocity = 20;
    float gravity;
    float jumpVelocity;

    Vector3 velocity;
    public float velocityXSmoothing;
    float targetVelocityX = 0;

    Controller2D controller;

    public bool isDodging;
    public bool isAttacking;
    public bool isAttackPrep;

	// Use this for initialization
	void Start ()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
        targetVelocityX = 0;

        accelerationTimeAirorne = 0;
        accelerationTimeGrounded = 0;

        isAttacking = false;
        isDodging = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        if(isAttacking) //Cannot move while attacking
        {
            input.x = 0;
        }

        if (!isAttacking) //Allow horizontal move while not attacking
        {
            targetVelocityX = input.x * moveSpeed;
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirorne);

        bool wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0 
            && controller.GetComponent<Controller2D>().canWallSlide) //If is wall sliding
        {
            wallSliding = true;
            accelerationTimeAirorne = 0.2f;
            moveSpeed = 12;

            if (velocity.y < -wallSlideSpeedMax && (controller.GetComponent<Controller2D>().autoWallSlide || velocity.x != 0)) //Set the maximum drop speed when sliding on wall
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0) //Give more wall grab time for jump to opposite side
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }

                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }

            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }

        if(controller.collisions.below) //Set physics when not wall sliding
        {
            accelerationTimeAirorne = 0;
            moveSpeed = 6;
        }

        if(controller.collisions.above || controller.collisions.below) //Stop accumulating gravity if is on ground & prevent going up through roof
        {
            velocity.y = 0;
        }

        if (Input.GetButtonDown("Jump") && !isAttacking && !isDodging) //Jump logic
        {
            if (wallSliding && controller.GetComponent<Controller2D>().canWallJump) //Wall jump
            {
                //print("wall jump");
                if(wallDirX == input.x) //Wall Climb
                {
                    //print(wallDirX);
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    //print(velocity.x);
                    velocity.y = wallJumpClimb.y;
                }

                else if (input.x == 0) //Jump off wall
                {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }

                else //Jump to opposite direction
                {
                    //print("else");
                    velocity.x = -wallDirX * wallLeap.x;
                    //print(velocity.x);
                    velocity.y = wallLeap.y;
                }
            }

            if (controller.collisions.below) //Jump from ground
            {
                //print("jump");
                velocity.y = jumpVelocity;
            }

            else if(!controller.collisions.below && jumpsRemaining > 0 && !wallSliding) //Air jump
            {
                velocity.y = jumpVelocity;
                jumpsRemaining--;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if(wallSliding || controller.collisions.below) //Reset jump times
        {
            jumpsRemaining = jumpTimes - 1;
        }

        if(Input.GetButtonDown("Fire1") && !isAttacking && !isDodging) //Attack with light saber
        {
            isAttacking = true;
            StartCoroutine(attackPrep());
        }

        if(Input.GetButtonDown("Fire2") && !isAttacking && !isDodging) //Dodge move
        {
            isDodging = true;
            //dodge;
        }

        if(Input.GetButtonDown("Fire1") && isAttacking && isAttackPrep) //Cancel attack
        {
            isAttacking = false;
            isAttackPrep = false;
        }
	}

    public IEnumerator attackPrep() //Preparation for attacking
    {
        isAttackPrep = true;
        yield return new WaitForSeconds(attackPrepTime);
        isAttackPrep = false;
        //attack
    }
}
