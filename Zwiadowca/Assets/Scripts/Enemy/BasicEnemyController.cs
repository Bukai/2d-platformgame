using System.Net.NetworkInformation;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        Konckback,
        Dead
    }

    private State currentState;

    [SerializeField]
    private float groundCheckDistance, wallCheckDistance, movementSpeed, maxHealth, knockbackDuration, 
        lastTouchDamageTime, touchDamageCooldown, touchDamage, touchDamageWidth, touchDamageHight;
    [SerializeField]
    private Transform groundCheck, wallCheck, touchDamageCheck;
    [SerializeField]
    private LayerMask whatIsGround, whatIsPlayer;
    [SerializeField]
    private Vector2 knockbackSpeed;
    [SerializeField]
    private GameObject hitEffect, deathEffect, deathBlood;

    private float[] attackDetails = new float[2];

    private float currentHealth, knockbackStartTime;

    private int facingDirection, damageDirection;

    private Vector2 movement, touchDamageBotLeft, touchDamageTopRight;

    private bool groundDeteced, wallDeteced;

    private GameObject alive;
    private Rigidbody2D aliveRb;
    private Animator aliveAnim;

    void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliveRb = alive.GetComponent<Rigidbody2D>();
        aliveAnim = alive.GetComponent<Animator>();

        currentHealth = maxHealth;
        facingDirection = 1;
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Konckback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    //--WALKING-----------------------------------------------------------------------

    private void EnterMovingState()
    {

    }

    private void UpdateMovingState()
    {
        groundDeteced = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        if (facingDirection == -1)
        {
            wallDeteced = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, whatIsGround);
        }
        else
        {
            wallDeteced = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);
        }

        CheckTouchDamage();

        if (!groundDeteced || wallDeteced)
        {
            Flip();
        }
        else
        {
            movement.Set(movementSpeed * facingDirection, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
    }

    private void ExitMovingState()
    {

    }

    //--KNOCKBACK----------------------------------------------------------------------

    private void EnterKonckbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRb.velocity = movement;
        aliveAnim.SetBool("Knockback", true);
    }

    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitKnocbackState()
    {
        aliveAnim.SetBool("Knockback", false);
    }

    //--DEAD----------------------------------------------------------------------------

    private void EnterDeadState()
    {
        Instantiate(deathEffect, alive.transform.position, deathEffect.transform.rotation);
        Instantiate(deathBlood, alive.transform.position, deathBlood.transform.rotation);
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }

    //--FUNCTIONS-------------------------------------------------------------------------

    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        Instantiate(hitEffect, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

        if (attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        if (currentHealth > 0.0f)
        {
            SwitchState(State.Konckback);
        }
        else if (currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }

    private void CheckTouchDamage()
    {
        if (Time.time >= lastTouchDamageTime + touchDamageCooldown)
        {
            touchDamageBotLeft.Set(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHight / 2));
            touchDamageTopRight.Set(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHight / 2));

            Collider2D hit = Physics2D.OverlapArea(touchDamageBotLeft, touchDamageTopRight, whatIsPlayer);

            if (hit != null)
            {
                lastTouchDamageTime = Time.time;
                attackDetails[0] = touchDamage;
                attackDetails[1] = alive.transform.position.x;
                hit.SendMessage("Damage", attackDetails);
            }
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void SwitchState(State state)
    {

        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.Konckback:
                ExitKnocbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.Konckback:
                EnterKonckbackState();
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
        
        if (facingDirection == -1)
        {
            Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x - wallCheckDistance, wallCheck.position.y));
        }
        else 
        {
            Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        }
        

        Vector2 botLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHight / 2));
        Vector2 botRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHight / 2));
        Vector2 topLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHight / 2));
        Vector2 topRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHight / 2));

        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(botRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, botLeft);
    }
}