using Pathfinding.Util;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Bullet : MonoBehaviour, IPooledObject
{
    public Rigidbody2D rb;
    public Weapon weapon;
    public AudioManager audioManager;
    public GameObject wallHitEffect;
    public float reloadTime;
    public float maxMagazine;
    public float currentMagazine;
    public float shakingForce;
    public float shakingTime;
    public float recoil;
    public float spawnTime;
    public float speed;
    protected ObjectPooler objectPooler;
    public CameraController cameraController;
    public float damage;

    public void OnObjectSpawn()
    {
        objectPooler = ObjectPooler.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        switch (other.gameObject.tag)
        {
            case "Wall":
                HitWall();
                break;
            case "Enemy":
                HitEnemy();   
                break;
        }
    }
    public void HitWall()
    {
        if (wallHitEffect != null)
        {
            Instantiate(wallHitEffect, transform.position, Quaternion.identity);
        }
        gameObject.SetActive(false);
    }
    public virtual void HitEnemy()
    {
        gameObject.SetActive(false);
    }

   

}
