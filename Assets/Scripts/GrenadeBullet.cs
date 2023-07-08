using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GrenadeBullet : Bullet
{
    public float grenadeSplashRange;
    public float rotateSpeed;
    private void FixedUpdate()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
    public override void HitEnemy()
    {
        if (gameObject.CompareTag("GrenadeBullet"))
        {
            GameObject projectile = objectPooler.SpawnFromPool("ExplosionEffect", transform.position, Quaternion.identity);
            // bullet is destroyed so that coroutine is started in weapon class
            weapon.StartWaitNDeactivate(projectile, 0.3f);
            audioManager.Play("GrenadeExplosion");
            cameraController.StartExplosion(shakingTime, shakingForce);
        }
        gameObject.SetActive(false);
    }
    /* private void OnDrawGizmos()
     {
         Handles.color = Color.red;
         Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), grenadeSplashRange);
     }*/
}
