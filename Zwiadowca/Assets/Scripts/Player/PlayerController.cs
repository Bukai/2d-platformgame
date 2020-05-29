
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;

    public static PlayerController MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerController>();
            }

            return instance;
        }

    }

    private Rigidbody2D rb;
    private Animator anim;

    public int amountOfJump;
    private int amountOfJumpsLeft;
    private int facingDirection = 1;
    private int lastWallJumpDirection;

    [SerializeField]
    private float knockbackDuration;

    private float movementInputDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;
    private float knockbackStartTime;

    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSlading;
    private bool canNormalJump;
    private bool doubleJump;
    private bool doubleJumpOff = false;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool chackJumpMultiplier;
    private bool canMove;
    private bool canFilp;
    private bool hasWallJumped;
    private bool knockack;

    public float movementSpeed;
    public float jumpForce;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movmentForceInAir;
    public float airDragMultiplier;
    public float varibleJumpHeightMultiplier;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpTimerSet;
    public float turnTimerSet;
    public float wallJumpTimerSet;

    [SerializeField]
    private Vector2 knockbackSpeed;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJump;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    private void FixedUpdate()
    {
        CheckInput();
        CheckJump();
        chackMovmentDirection();
        UpdateAnimation();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckKnockback();
        ApplyMovment();
        CheckSurroundings();
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection && rb.velocity.y < 0)
        {
            isWallSlading = true;
        }
        else
        {
            isWallSlading = false;
        }
    }

    public void Knockback(int direction)
    {
        knockack = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }

    private void CheckKnockback()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration && knockack)
        {
            knockack = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }

    private void chackMovmentDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Filip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Filip();
        }

        if (Mathf.Abs(rb.velocity.x) >= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimation()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSlading", isWallSlading);
        anim.SetBool("DoubleJump", doubleJump);
    }

    public void CheckInput()
    {
        movementInputDirection = Mathf.RoundToInt(SimpleInput.GetAxisRaw("Horizontal"));

        if (movementInputDirection == facingDirection && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFilp = false;

                turnTimer = turnTimerSet;
            }

            amountOfJumpsLeft = amountOfJump + 1;
        }

        if (!canMove)
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                canMove = true;
                canFilp = true;
            }
        }
    }

    public void CheckJump()
    {
        if(jumpTimer > 0)
        {
            if (!isGrounded && isTouchingWall && movementInputDirection != 0.5f && movementInputDirection != facingDirection && amountOfJumpsLeft > 0)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
        }
        
        if(isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer > 0)
        {
            if (hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }

    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            chackJumpMultiplier = true;
        }
    }

    private void WallJump()
    {
        if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            isWallSlading = false;
            amountOfJumpsLeft = amountOfJump;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            chackJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFilp = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }
    }

    public void JumpUp()
    {

        if(isGrounded && amountOfJumpsLeft > 0 && isTouchingWall)
        {
            NormalJump();
        }
        else if (!isGrounded && !isTouchingWall && amountOfJumpsLeft > 0)
        {
            NormalJump();
        }
        else
        {
            jumpTimer = jumpTimerSet;
            isAttemptingToJump = true;
        }
    }

    public void JumpDown()
    {
        if (chackJumpMultiplier)
        {
            chackJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * varibleJumpHeightMultiplier);
        }  
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJump + 1;
        }

        if (isTouchingWall)
        {
            canWallJump = true;
        }
        else
        {
            canWallJump = false;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
            doubleJumpOff = false;
        }

        if (!isTouchingWall && !isGrounded && amountOfJumpsLeft < 1 && !doubleJumpOff)
        {
            int numb = Random.Range(0, 2) + 1;
            
            switch (numb)
            {
                case 1:
                    doubleJumpOff = true;
                    break;
                case 2:
                    doubleJump = true;
                    doubleJumpOff = true;
                    break;
            }

        }
        else if (amountOfJumpsLeft > 0)
        {
            doubleJump = false;
        }
    }

    private void ApplyMovment()
    {
        if (!isGrounded && !isWallSlading && movementInputDirection == 0 && !knockack)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if (canMove && !knockack)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }     
        
        if (isWallSlading)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    public void DisableFilp()
    {
        canFilp = false;
    }

    public void EnableFlip()
    {
        canFilp = true;
    }

    private void Filip()
    {
        if (!isWallSlading && canFilp && !knockack)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
