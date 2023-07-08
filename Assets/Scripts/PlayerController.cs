using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Joystick movementJoystick;
    public Joystick shootingJoystick;
    public SpriteRenderer playerSprite;
    public HealthBar healthBar;
    public Camera sceneCamera;
    public Animator animator;
    public Weapon weapon;
    public GameObject dieEffect;
    public TMPro.TextMeshProUGUI upgradePointsText;
    public float moveSpeed;
    public float shootingMoveSpeed;
    public Rigidbody2D rb;
    private float health;
    public bool isMoving;
    public float maxHealth;
    public AbilitySystem abilitySystem;
    public static bool isShooting;
    public static bool isPlayerPushed;
    public static bool isShotgunShooting;
    public static bool isRifleShooting;
    public static bool isPistolShooting;
    public static bool isGrenadeShooting;
    public Vector2 moveDirection;
    public Vector2 aimDirection;
    private float healthPercentage;
    public float killedNormalEnemy;
    public float killedBossEnemy;
    public float killedFastEnemy;
    public bool isRunning;
    public bool isWalking;
    public float characterUpgradePoints;
    public float gunUpgradePoints;
    private Vector2 lastBodyDirection;
    public Vector2 currentBodyDirection;
    private float bodyAngle;


    private void Start()
    {
        
        health = maxHealth;
        healthPercentage = health / maxHealth;
        healthBar.SetHealth(healthPercentage);
    }
    // Update is called once per frame
    void Update()
    {
        upgradePointsText.text = "upgrade points:" + characterUpgradePoints;
        IsDead();
        ProcessInputs();
        SetDirection();
    }
    private void FixedUpdate()
    {
        
        Move();
        Fire();

        SetMoveAnimationParams();
    }
    void ProcessInputs()
    {

        moveDirection = movementJoystick.joystickVec.normalized;
        aimDirection = shootingJoystick.joystickVec.normalized;

        if(aimDirection != Vector2.zero)
        {
            lastBodyDirection = shootingJoystick.joystickVec.normalized;
        }
        else if (moveDirection != Vector2.zero)
        {         
            lastBodyDirection = movementJoystick.joystickVec.normalized;
        }


    }
    void Move()
    {
        if (abilitySystem.isAnimationPlaying)
        {
            return;
        }
        // if player is shooting, move speed should be less than normal move speed
        isShooting = true;
       
        if (isRifleShooting)
        {
            rb.velocity = new Vector2(moveDirection.x * shootingMoveSpeed, moveDirection.y * shootingMoveSpeed);
        }
        else if (isShotgunShooting)
        {
            if (!weapon.shotgunBullet.isShotgunPushing)
            {
                rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
            }

        }
        else if (isGrenadeShooting)
        {
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
        }
        else if (isPistolShooting)
        {
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
        }
        else
        {   
            isShooting = false;
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
        }

        rb.rotation = bodyAngle;

        if(rb.velocity == Vector2.zero)
        {
            isRunning = false;
        }
        else
        {
            isRunning = true;
        }
    }
    public void GetDamage(float damage)
    {   
        health -= damage;
        StartCoroutine(PlayerColorChange());
        healthPercentage = health / maxHealth;
        healthBar.SetHealth(healthPercentage);
    }
    void IsDead()
    {
        if (health <= 0)
        {
            Instantiate(dieEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    void Fire()
    {
        if (abilitySystem.isAnimationPlaying)
        {
            return;
        }
        aimDirection = shootingJoystick.joystickVec.normalized;
        if (!weapon.isReloading && aimDirection!=Vector2.zero && !isShooting && shootingJoystick.shoot)
        {
            weapon.Fire(aimDirection);
        }
    }
    // when player get hit, display this by increasing transparency
    IEnumerator PlayerColorChange()
    {
        playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 0.4f);
        yield return new WaitForSeconds(0.1f);
        playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 1f);
    }
    public void EnemyKilled(EnemyType enemyType)
    {
        if (enemyType == EnemyType.Normal)
        {
            killedNormalEnemy++;
        }
        else if (enemyType == EnemyType.Fast)
        {
            killedFastEnemy++;
        }
        else if (enemyType == EnemyType.Boss)
        {
            killedBossEnemy++;
        }
    }
    public Vector2 GetBodyDirection()
    {
        return currentBodyDirection;
    }
    public void SetMoveAnimationParams()
    {
        animator.SetBool("IsRunning", isRunning);
        animator.SetInteger("Weapon", (int)weapon.weaponType);
    }
     void SetDirection()
     {
         if (aimDirection != Vector2.zero)
         {
             currentBodyDirection = aimDirection;
         }
         else if (moveDirection != Vector2.zero)
         {
             currentBodyDirection = moveDirection;
         }
         else
         {
             currentBodyDirection = lastBodyDirection;
         }
         bodyAngle = Mathf.Atan2(currentBodyDirection.y, currentBodyDirection.x) * Mathf.Rad2Deg - 90f;
     }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        abilitySystem.meleeAttackEffect.SetActive(false);
        if (abilitySystem.isAnimationPlaying)
        {
            if(collision.gameObject.layer == 8)
            {
                collision.gameObject.GetComponent<EnemyController>().GetDamage(abilitySystem.meleeDamage);
            }
        }
    }

}
