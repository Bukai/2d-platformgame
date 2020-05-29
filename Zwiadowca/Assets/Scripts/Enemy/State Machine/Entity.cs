using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public FiniteStateMachine stateMachine;

    public D_Entity entityData;

    public int facingDirection { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    public GameObject alive { get; private set; }
    public AnimationToStateMachine atsm { get; private set; }

    [SerializeField]
    private Transform wallCheck, ledgeCheck, playerCheck;

    private float currentHealth;

    private int lastDamageDirection;

    private Vector2 velocityWorkspace;

    protected bool isDead;

    private SpriteRenderer enemySprite;
    private bool flashActive;
    private float flashCounter;
    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;


    private bool knockack;
    private float knockbackStartTime;

    public virtual void Start()
    {
        facingDirection = 1;
        currentHealth = entityData.maxHealth;

        alive = transform.Find("Alive").gameObject;
        rb = alive.GetComponent<Rigidbody2D>();
        anim = alive.GetComponent<Animator>();
        atsm = alive.GetComponent<AnimationToStateMachine>();
        enemySprite = alive.GetComponent<SpriteRenderer>();
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");

        stateMachine = new FiniteStateMachine();
    }

    public virtual void Update()
    {
        stateMachine.currentState.LogicUpdate();
        FlashDamageActive();
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
        CheckKnockback();
    }

    public virtual void SetVelocity(float velocity)
    {
        velocityWorkspace.Set(facingDirection * velocity, rb.velocity.y);
        rb.velocity = velocityWorkspace;
    }

    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, alive.transform.right, entityData.wallCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckPlayerInMinAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, alive.transform.right, entityData.minAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, alive.transform.right, entityData.maxAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, alive.transform.right, entityData.closeRangeActionDistance, entityData.whatIsPlayer);
    }

    //public virtual void DamageHop(float velocity)
    //{
    //    velocityWorkspace.Set(rb.velocity.x, velocity);
    //    rb.velocity = velocityWorkspace;
    //}

    public void Knockback(int direction)
    {
        knockack = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(entityData.knockbackSpeed.x * direction, entityData.knockbackSpeed.y);
    }

    private void CheckKnockback()
    {
        if (Time.time >= knockbackStartTime + entityData.knockbackDuration && knockack)
        {
            knockack = false;
            rb.velocity = velocityWorkspace;
        }
    }

    public virtual void Damage(AttackDetails attackDetails)
    {
        currentHealth -= attackDetails.damageamount;
        flashActive = true;
        flashCounter = entityData.flashLenght;

        //DamageHop(entityData.damageHopSpeedY);
        //Knockback(entityData.knockbackSpeed.x, entityData.knockbackSpeed.y);

        Instantiate(entityData.hitParticle, alive.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        if (attackDetails.position.x < alive.transform.position.x)
        {
            lastDamageDirection = 1;
        }
        else
        {
            lastDamageDirection = -1;
        }

        if (lastDamageDirection == 1 && facingDirection == 1)
        {
            Flip();
        }
        else if (lastDamageDirection == -1 && facingDirection == -1)
        {
            Flip();
        }

        if (currentHealth <= 0)
        {
            isDead = true;
        }

        Knockback(lastDamageDirection);
    }

    public virtual void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0f, 180f, 0f);
    }

    private void FlashDamageActive()
    {
        if (flashActive)
        {
            if (flashCounter > entityData.flashLenght * .66f)
            {
                enemySprite.color = new Color(enemySprite.color.r, enemySprite.color.g, enemySprite.color.b, 0.6f);

            }
            else if (flashCounter > entityData.flashLenght * .33f)
            {
                enemySprite.material.shader = shaderGUItext;
                enemySprite.color = Color.white;
            }
            else if (flashCounter > 0f)
            {
                enemySprite.material.shader = shaderSpritesDefault;
                enemySprite.color = new Color(enemySprite.color.r, enemySprite.color.g, enemySprite.color.b, 0.6f);
            }
            else
            {
                enemySprite.color = new Color(1f, 1f, 1f, 1f);
                flashActive = false;
            }

            flashCounter -= Time.deltaTime;
        }
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));

        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.closeRangeActionDistance), 0.1f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.minAgroDistance), 0.1f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.maxAgroDistance), 0.1f);
    }
}
