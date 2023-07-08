using Pathfinding;
using Pathfinding.Util;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum EnemyType
{
    Normal,
    Fast,
    Boss
}

public class EnemyController : MonoBehaviour, IPooledObject
{
    public HealthBar healthBar;
    public GameManager gameManager;
    public PlayerController player;
    public AIPath aiPath;
    public Rigidbody2D rb;
    public GameObject dieEffect;
    public SpriteRenderer enemySprite;
    public AudioManager audioManager;
    public Animator animator;
    public float speed;
    public float attackRange;
    public float attackTime;
    public bool isAttacking;
    private float health;
    public float maxHealth;
    public float damageGiven;
    public EnemyType enemyType;
    public float pushBackForce;
    private float healthPercentage;
    private Vector2 direction;
    public float bulletHitAngle;
    private ObjectPooler objectPooler;
    void IPooledObject.OnObjectSpawn()
    {
        objectPooler = ObjectPooler.Instance;
        aiPath.maxSpeed = speed;
        StartCoroutine(EnemyColorChange());
        aiPath.canMove = true;
        isAttacking = false;
        health = maxHealth;
        healthPercentage = health / maxHealth;
        healthBar.SetHealth(healthPercentage);
    }
    // Start is called before the first frame update
   /* void Start()
    {
        aiPath.maxSpeed = speed;
        health = maxHealth;
        healthPercentage = health / maxHealth;
        healthBar.SetHealth(healthPercentage);
    }*/

    // Update is called once per frame
    void Update()
    {
        if(aiPath.velocity != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
        IsDied();
        direction = (player.transform.position - transform.position);
        direction.Normalize();
      
    }
    private void FixedUpdate()
    {
        // if player is close enough, attack
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var collider in colliders)
        {
            if (isAttacking == false && collider.gameObject.layer == player.gameObject.layer)
            {
                StartCoroutine(Attack());
                break;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // calculate the bullet hit angle because when displaying dying effect,
        // particles will go to the same direction as the bullet
        if (collision.GetComponent<Rigidbody2D>())
        {
            Vector2 dir = collision.GetComponent<Rigidbody2D>().velocity.normalized;
            bulletHitAngle = -(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }
        if (collision.gameObject.tag == "RifleBullet")
        {
            GetDamage(player.weapon.rifleBullet.damage);
        }
        else if (collision.gameObject.tag == "ShotgunBullet")
        {
            GetDamage(player.weapon.shotgunBullet.damage);
        }
        else if(collision.gameObject.tag == "PistolBullet")
        {
            GetDamage(player.weapon.pistolBullet.damage);
        }
        else if (collision.gameObject.tag == "GrenadeBullet")
        {
            var hitColliders = Physics2D.OverlapCircleAll(collision.transform.position, player.weapon.grenadeBullet.grenadeSplashRange);
            foreach(var hitCollider in hitColliders)
            {
                var enemy = hitCollider.GetComponent<EnemyController>();
                if (enemy)
                {
                    enemy.GetDamage(player.weapon.grenadeBullet.damage);
                    enemy.bulletHitAngle = bulletHitAngle;
                }
            }        
        }
    }
    public void GetDamage(float damage)
    {
        audioManager.Play("EnemyGetHit");
        StartCoroutine(EnemyColorChange());
        if(pushBackForce != 0)
        {
            StartCoroutine(PushBack());
        }
        health -= damage;
        healthPercentage = health / maxHealth;
        healthBar.SetHealth(healthPercentage);
    }
    void IsDied()
    {
        if(health <= 0)
        {
            GameObject projectile = Instantiate(dieEffect, transform.position, Quaternion.identity);
            projectile.transform.rotation = Quaternion.Euler(bulletHitAngle, 90, 0);
            player.EnemyKilled(enemyType);
            gameObject.SetActive(false);           
            gameManager.CreateEnemy();
        }
    }
    // when get hit, push a little back to make player feel hit effect
    IEnumerator PushBack()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(-direction * pushBackForce, ForceMode2D.Impulse);
        aiPath.canMove = false;
        yield return new WaitForSeconds(0.1f);
        aiPath.canMove = true;
        rb.velocity = Vector2.zero;
    }
    // when enemy get hit, display this by increasing transparency
    IEnumerator EnemyColorChange()
    {
        enemySprite.color = new Color(enemySprite.color.r, enemySprite.color.g, enemySprite.color.b,0.4f);
        yield return new WaitForSeconds(0.15f);
        enemySprite.color = new Color(enemySprite.color.r, enemySprite.color.g, enemySprite.color.b, 1f);
    }
    IEnumerator Attack()
    {
        animator.SetTrigger("Attack");
        isAttacking = true;
        player.GetDamage(damageGiven);
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;

    }


    
}
