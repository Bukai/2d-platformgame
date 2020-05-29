using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField]
    public float maxHealth;

    [SerializeField]
    private GameObject deathChunkParticle, deathBllodParticle;

    public float currentHealth;

    private GameManager gameManager;

    private bool flashActive;
    public float flashLenght;
    private float flashCounter;

    private SpriteRenderer playerSprite;

    private void Start()
    {
        currentHealth = maxHealth;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerSprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        FlashDamageActive();
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        flashActive = true;
        flashCounter = flashLenght;

        if (currentHealth <= 0.0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBllodParticle, transform.position, deathBllodParticle.transform.rotation);
        gameManager.Respawn();
        Destroy(gameObject);
    }

    private void FlashDamageActive()
    {
        if (flashActive)
        {
            if (flashCounter > flashLenght * .66f)
            {
                playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 0.6f);

            }
            else if (flashCounter > flashLenght * .33f)
            {
                playerSprite.color = new Color(1f, 0f, 0f, 1f);
            }
            else if (flashCounter > 0f)
            {
                playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 0.6f);
            }
            else
            {
                playerSprite.color = new Color(1f, 1f, 1f, 1f);
                flashActive = false;
            }

            flashCounter -= Time.deltaTime;
        }
    }
}
