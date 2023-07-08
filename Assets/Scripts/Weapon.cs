using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponTypes
{
    Rifle,
    Shotgun,
    Grenade,
    Pistol
}

public class Weapon : MonoBehaviour
{
    private ObjectPooler objectPooler;
    public CameraController cameraController;
    public SpriteRenderer weaponSprite;
    public PlayerController player;
    public Rigidbody2D playerRb;
    public SpriteRenderer fireEffectSprite;
    public GameObject weaponSelectArea;
    //public Button reloadButton;
    public PistolBullet pistolBullet;
    public GrenadeBullet grenadeBullet;
    public RifleBullet rifleBullet;
    public ShotgunBullet shotgunBullet;
    public WeaponTypes weaponType;
    public Transform rifleFirePoint;
    public Transform shotgunFirePoint;
    public Transform PistolFirePoint;
    public Transform spentBulletSpawnPosition;
    private AudioManager audioManager;
    public float spentBulletSpeed;
    public float spentBulletLifeTime;
    public bool isReloading;
    public Bullet currentBullet;
    public TMPro.TextMeshProUGUI magazineText;
    public LineRenderer lineRenderer;
    public Vector2 weaponDirection;
    public bool isLaserActive = false;

    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
        audioManager = FindObjectOfType<AudioManager>();


    }
    private void Update()
    {

        weaponDirection = rifleFirePoint.position - transform.position;
        UpdateLaser();
        GetCurrentBullet();
        magazineText.text = currentBullet.currentMagazine.ToString() + "/" + currentBullet.maxMagazine.ToString();

    }
    public void Fire(Vector2 aimDirection)
    {
        switch (weaponType)
        {
            case WeaponTypes.Rifle:
                StartCoroutine(RifleFire(aimDirection));
                break;
            case WeaponTypes.Shotgun:
                StartCoroutine(ShotgunFire(aimDirection));
                break;
            case WeaponTypes.Grenade:
                StartCoroutine(GrenadeFire(aimDirection));
                break;
            case WeaponTypes.Pistol:
                StartCoroutine(PistolFire(aimDirection));
                break;
            default:
                break;
        }


    }
    public IEnumerator RifleFire(Vector2 aimDirection)
    {
        if (rifleBullet.currentMagazine <= 0)
        {
            StartCoroutine(PlayReloadAnimation(rifleBullet));
            yield break;
        }
        StartCoroutine(DisplayGunFireEffect());
        player.animator.SetTrigger("Shoot");
        audioManager.Play("RifleShoot");
        float random = Random.Range(-1 * rifleBullet.recoil, rifleBullet.recoil);
        GameObject projectileRifle = objectPooler.SpawnFromPool("RifleBullet", rifleFirePoint.position, player.transform.rotation);
        projectileRifle.transform.Rotate(new Vector3(0, 0, random));
        projectileRifle.GetComponent<Rigidbody2D>().AddForce(projectileRifle.transform.up * rifleBullet.speed, ForceMode2D.Impulse);
        cameraController.StartExplosion(rifleBullet.shakingTime, rifleBullet.shakingForce);

        GameObject spentBullet = objectPooler.SpawnFromPool("SpentRifleBullet", spentBulletSpawnPosition.position, player.transform.rotation);
        // add randomness to the force whic is applied to spent bullet
        Vector3 randVec = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        spentBullet.GetComponent<Rigidbody2D>().AddForce
            ((spentBullet.transform.position - player.transform.position + randVec).normalized * spentBulletSpeed, ForceMode2D.Impulse);
        // to make the spent bullet more realistic, add random rotation
        random = Random.Range(-75, 75);
        spentBullet.transform.Rotate(new Vector3(0, 0, random));
        StartCoroutine(Deactivate(spentBullet, spentBulletLifeTime));
        // Decrease the bullet amount in the rifle magazine
        rifleBullet.currentMagazine--;
        // prevent shooting without stopping
        PlayerController.isRifleShooting = true;
        yield return new WaitForSeconds(rifleBullet.spawnTime);
        PlayerController.isRifleShooting = false;
    }
    public IEnumerator ShotgunFire(Vector2 aimDirection)
    {
        if (shotgunBullet.currentMagazine <= 0)
        {
            StartCoroutine(PlayReloadAnimation(shotgunBullet));
            yield break;
        }
        StartCoroutine(DisplayGunFireEffect());
        player.animator.SetTrigger("Shoot");
        audioManager.Play("ShotgunShoot");
        StartCoroutine(ShotgunPushBack(aimDirection));
        float recoil = -1 * shotgunBullet.recoil;
        for (int i = 0; i < shotgunBullet.shotgunBulletAmount; i++)
        {
            GameObject projectileShotgun = objectPooler.SpawnFromPool("ShotgunBullet", shotgunFirePoint.position, player.transform.rotation);
            projectileShotgun.transform.Rotate(new Vector3(0, 0, recoil));
            projectileShotgun.GetComponent<Rigidbody2D>().AddForce(projectileShotgun.transform.up * shotgunBullet.speed, ForceMode2D.Impulse);
            recoil += 2 * shotgunBullet.recoil / shotgunBullet.shotgunBulletAmount;
        }

        cameraController.StartExplosion(shotgunBullet.shakingTime, shotgunBullet.shakingForce);
        GameObject spentBullet = objectPooler.SpawnFromPool("SpentShotgunBullet", spentBulletSpawnPosition.position, player.transform.rotation);
        spentBullet.GetComponent<Rigidbody2D>().AddForce
              ((spentBullet.transform.position - player.transform.position).normalized * spentBulletSpeed, ForceMode2D.Impulse);
        // to make the spent bullet more realistic, add random rotation
        float random = Random.Range(-30, 30);
        spentBullet.transform.Rotate(new Vector3(0, 0, random));
        StartCoroutine(Deactivate(spentBullet, spentBulletLifeTime));
        // Decrease the bullet amount in the shotgun magazine
        shotgunBullet.currentMagazine--;
        // prevent shooting without stopping
        PlayerController.isShotgunShooting = true;
        yield return new WaitForSeconds(shotgunBullet.spawnTime);
        PlayerController.isShotgunShooting = false;
    }
    public IEnumerator GrenadeFire(Vector2 aimDirection)
    {
        player.animator.SetTrigger("Shoot");
        audioManager.Stop("GrenadeShoot");
        audioManager.Play("GrenadeShoot");
        GameObject projectileGrenade = objectPooler.SpawnFromPool("GrenadeBullet", rifleFirePoint.position, player.transform.rotation);
        //GameObject projectileGrenade = Instantiate(grenadeBullet.gameObject, firePoint.position, player.transform.rotation);
        //projectileGrenade.SetActive(true);
        projectileGrenade.GetComponent<Rigidbody2D>().AddForce(aimDirection.normalized * grenadeBullet.speed, ForceMode2D.Impulse);
        // prevent shooting without stopping
        PlayerController.isGrenadeShooting = true;
        yield return new WaitForSeconds(grenadeBullet.spawnTime);
        PlayerController.isGrenadeShooting = false;
    }
    public IEnumerator PistolFire(Vector2 aimDirection)
    {
        if (pistolBullet.currentMagazine <= 0)
        {
            StartCoroutine(PlayReloadAnimation(pistolBullet));
            yield break;
        }
        StartCoroutine(DisplayGunFireEffect());
        player.animator.SetTrigger("Shoot");
        audioManager.Play("RifleShoot");/***/
        float random = Random.Range(-1 * pistolBullet.recoil, pistolBullet.recoil);
        GameObject projectileRifle = objectPooler.SpawnFromPool("PistolBullet", PistolFirePoint.position, player.transform.rotation);
        projectileRifle.transform.Rotate(new Vector3(0, 0, random));
        projectileRifle.GetComponent<Rigidbody2D>().AddForce(projectileRifle.transform.up * pistolBullet.speed, ForceMode2D.Impulse);
        cameraController.StartExplosion(pistolBullet.shakingTime, pistolBullet.shakingForce);

        /**/
        GameObject spentBullet = objectPooler.SpawnFromPool("SpentPistolBullet", spentBulletSpawnPosition.position, player.transform.rotation);
        // add randomness to the force whic is applied to spent bullet
        Vector3 randVec = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        spentBullet.GetComponent<Rigidbody2D>().AddForce
            ((spentBullet.transform.position - player.transform.position + randVec).normalized * spentBulletSpeed, ForceMode2D.Impulse);
        // to make the spent bullet more realistic, add random rotation
        random = Random.Range(-75, 75);
        spentBullet.transform.Rotate(new Vector3(0, 0, random));
        StartCoroutine(Deactivate(spentBullet, spentBulletLifeTime));
        // Decrease the bullet amount in the rifle magazine
        pistolBullet.currentMagazine--;
        // prevent shooting without stopping
        PlayerController.isPistolShooting = true;
        yield return new WaitForSeconds(pistolBullet.spawnTime);
        PlayerController.isPistolShooting = false;
    }

    public void StartWaitNDestroy(GameObject obj, float secs)
    {
        StartCoroutine(WaitNDestroy(obj, secs));
    }
    public void StartWaitNDeactivate(GameObject obj, float secs)
    {
        StartCoroutine(WaitNDeactivate(obj, secs));
    }
    private IEnumerator WaitNDestroy(GameObject obj, float secs)
    {
        yield return new WaitForSeconds(secs);
        Destroy(obj);
    }
    private IEnumerator WaitNDeactivate(GameObject obj, float secs)
    {
        yield return new WaitForSeconds(secs);
        obj.SetActive(false);
    }
    public void SelectRifle()
    {
        weaponType = WeaponTypes.Rifle;
        fireEffectSprite.transform.position = rifleFirePoint.position;
        fireEffectSprite.transform.localScale = new Vector3(1, 1, 1);
    }
    public void SelectShotgun()
    {
        weaponType = WeaponTypes.Shotgun;
        fireEffectSprite.transform.position = shotgunFirePoint.position;
        fireEffectSprite.transform.localScale = new Vector3(2, 2, 2);
    }
    public void SelectGrenade()
    {
        weaponType = WeaponTypes.Grenade;
    }
    public void SelectPistol()
    {
        weaponType = WeaponTypes.Pistol;
        fireEffectSprite.transform.position = PistolFirePoint.position;
        fireEffectSprite.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
    public IEnumerator ShotgunPushBack(Vector2 aimDirection)
    {
        shotgunBullet.isShotgunPushing = true;
        playerRb.velocity = Vector3.zero;
        playerRb.AddForce(-aimDirection.normalized * shotgunBullet.shotgunPushForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(shotgunBullet.shotgunPushTime);
        playerRb.velocity = Vector2.zero;
        shotgunBullet.isShotgunPushing = false;
    }

    public IEnumerator DisplayGunFireEffect()
    {
        fireEffectSprite.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        fireEffectSprite.gameObject.SetActive(false);
    }
    // wait for given seconds then deactivate the given object
    public IEnumerator Deactivate(GameObject obj, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        obj.SetActive(false);
    }
    public IEnumerator PlayReloadAnimation(Bullet bullet)
    {
        player.animator.SetBool("Reload", true);
        isReloading = true;
        weaponSelectArea.SetActive(false);
        if (bullet == shotgunBullet)
        {
            float remainingBullet = bullet.maxMagazine - bullet.currentMagazine;
            for (int i = 0; i < remainingBullet; i++)
            {
                audioManager.Play("ShotgunReload");
                yield return new WaitForSeconds(bullet.reloadTime);
                bullet.currentMagazine++;
                if (player.shootingJoystick.shoot)
                {
                    break;
                }
            }

        }
        else if (bullet == pistolBullet)
        {
            yield return new WaitForSeconds(bullet.reloadTime / 2);
            audioManager.Play("PistolReload");
            yield return new WaitForSeconds(bullet.reloadTime / 2);
            bullet.currentMagazine = bullet.maxMagazine;
        }
        else if (bullet == rifleBullet)
        {
            audioManager.Play("RifleReload");
            yield return new WaitForSeconds(bullet.reloadTime);
            bullet.currentMagazine = bullet.maxMagazine;
        }
        else
        {
            yield return new WaitForSeconds(bullet.reloadTime);
            bullet.currentMagazine = bullet.maxMagazine;
        }
        isReloading = false;
        weaponSelectArea.SetActive(true);
        player.animator.SetBool("Reload", false);
    }
    // if there is at least 1 missing bullet, then reload can be made
    public void Reload()
    {
        if (currentBullet.currentMagazine < currentBullet.maxMagazine && !isReloading)
        {
            StartCoroutine(PlayReloadAnimation(currentBullet));
        }
    }
    // get the current weapons bullet to use when needed
    public void GetCurrentBullet()
    {
        if (weaponType == WeaponTypes.Rifle)
        {
            currentBullet = rifleBullet;
        }
        else if (weaponType == WeaponTypes.Shotgun)
        {
            currentBullet = shotgunBullet;
        }
        else if (weaponType == WeaponTypes.Grenade)
        {
            currentBullet = grenadeBullet;
        }
        else if (weaponType == WeaponTypes.Pistol)
        {
            currentBullet = pistolBullet;
        }
    }
    private void UpdateLaser()
    {
        if (!isLaserActive)
        {
            lineRenderer.gameObject.SetActive(false);
            return;
        }
        if (weaponType == WeaponTypes.Grenade || weaponType == WeaponTypes.Shotgun)
        {
            isLaserActive = false;
            lineRenderer.gameObject.SetActive(false);
            return;
        }
        if (player.abilitySystem.isAnimationPlaying)
        {
            lineRenderer.gameObject.SetActive(false);
            return;
        }
        lineRenderer.gameObject.SetActive(true);
        RaycastHit2D[] hits = Physics2D.RaycastAll(rifleFirePoint.transform.position, weaponDirection);
        for (int i = 0; i < hits.Length; ++i)
        {
            if (hits[i].collider.gameObject.layer != LayerMask.NameToLayer("Player") &&
                hits[i].collider.gameObject.layer != LayerMask.NameToLayer("Bullet"))
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, transform.position);

                lineRenderer.SetPosition(1, hits[i].point);
                break;
            }
        }
    }
    public void ActivateLaser()
    {

        if (!isLaserActive)
        {
            isLaserActive = true;
        }
        else
        {
            isLaserActive = false;
        }
    }
}