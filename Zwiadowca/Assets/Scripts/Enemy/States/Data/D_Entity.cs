using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Data")]
public class D_Entity : ScriptableObject
{

    public float maxHealth;

    public float damageHopSpeedX;
    public float damageHopSpeedY;

    [SerializeField]
    public Vector2 knockbackSpeed;

    public float knockbackDuration;

    public float wallCheckDistance;
    public float ledgeCheckDistance;

    public float minAgroDistance;
    public float maxAgroDistance;

    public float closeRangeActionDistance;

    public float flashLenght;

    public GameObject hitParticle;
    
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
}
