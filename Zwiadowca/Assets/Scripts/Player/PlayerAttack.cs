using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private static PlayerAttack instance;

    public static PlayerAttack MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerAttack>();
            }

            return instance;
        }

    }

    [SerializeField]
    private bool combatEnabled;
    [SerializeField]
    private float inputTimer, attackRadius, attackDamage, cooldown, resetTime;
    [SerializeField]
    private Transform attackHitBoxPos;
    [SerializeField]
    private LayerMask whatIsDamageable;

    private bool gotInput, isAttacking, isFirstAttack, isSecondAttack, isThirdAttack, attack = false;

    public int isComboAttack = 0;

    private float cooldownTimeWeapon;
    private float resetTimeWeapon;
    private float lastInputTime = Mathf.NegativeInfinity;

    private AttackDetails attackDetails;

    private Animator anim;

    private PlayerController player;
    private PlayerStat playerStat;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
        player = GetComponent<PlayerController>();
        playerStat = GetComponent<PlayerStat>();
    }

    void FixedUpdate()
    {
        CheckCombatInput();
        CooldownWeapon();
        ResetWeapon();
        ChceckAttacks();
    }

    public void ButtonClick()
    {   
        if (cooldownTimeWeapon == 0)
        {
            attack = true;
            isComboAttack++;
            resetTimeWeapon = resetTime;
        }
    }

    public void CheckCombatInput()
    {
        
        if (combatEnabled && attack)
        {
            gotInput = true;
            lastInputTime = Time.time;
            attack = false;
        }

        if (resetTimeWeapon == 0 && cooldownTimeWeapon == 0)
        {
            isComboAttack = 0;
            isFirstAttack = false;
        }
    }

    private void ChceckAttacks()
    {
        if (gotInput)
        {
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                anim.SetBool("isAttacking", isAttacking);
                
                if (isComboAttack == 1)
                {
                    isFirstAttack = !isFirstAttack;
                    cooldownTimeWeapon = 0.3f;
                }
                else
                {
                    isFirstAttack = false;
                }
                
                if (isComboAttack == 2)
                {
                    isSecondAttack = !isSecondAttack;
                    cooldownTimeWeapon = 0.3f;
                }
                else
                {
                    isSecondAttack = false;
                }
                
                if (isComboAttack >= 3)
                {
                    isThirdAttack = !isThirdAttack;
                    cooldownTimeWeapon = cooldown;
                    isComboAttack = 0;
                }
                else
                {
                    isThirdAttack = false;
                }

                anim.SetBool("attack1", true);
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("secondAttack", isSecondAttack);
                anim.SetBool("thirdAttack", isThirdAttack);
                
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
    }

    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, attackRadius, whatIsDamageable);

        attackDetails.damageamount = attackDamage;
        attackDetails.position = transform.position;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attackDetails);

        }
    }

    private void FinishAttack()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
    }

    private void Damage(AttackDetails attackDetails)
    {
        int direction;

        playerStat.DecreaseHealth(attackDetails.damageamount);

        if (attackDetails.position.x < transform.position.x)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        player.Knockback(direction);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackHitBoxPos.position, attackRadius);
    }

    private void CooldownWeapon()
    {
        if (cooldownTimeWeapon > 0)
        {
            cooldownTimeWeapon -= Time.deltaTime;
        }

        if (cooldownTimeWeapon < 0)
        {
            cooldownTimeWeapon = 0;
        }
    }

    private void ResetWeapon()
    {
        if (resetTimeWeapon > 0)
        {
            resetTimeWeapon -= Time.deltaTime;
        }

        if (resetTimeWeapon < 0)
        {
            resetTimeWeapon = 0;
        }
    }
}
