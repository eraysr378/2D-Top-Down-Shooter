using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunUpgradeScreen : MonoBehaviour
{
    public PlayerController player;
    public Bullet bullet;
    public Button upgradeDamageButton;
    public Button upgradeFireRateButton;
    public Button upgradeMagCapacityButton;
    public float damageUpgradeAmount;
    public float maxDamage;
    public float fireRateUpgradeAmount; // increasing fire rate  =  decreasing bullet spawn time
    public float maxFireRate;
    public float magCapacityUpgradeAmount;
    public float maxMagCapacity;
    public WeaponTypes weaponType;
    public GameObject upgradeArea;
    public Button buyButton;
    public Button selectButton;

    private void Update()
    {
        if (weaponType == WeaponTypes.Shotgun && player.weapon.isShotgunUnlocked ||
             weaponType == WeaponTypes.Rifle && player.weapon.isRifleUnlocked ||
             weaponType == WeaponTypes.Pistol && player.weapon.isPistolUnlocked)
        {
            upgradeArea.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);
            if (player.gunUpgradePoints > 0)
            {
                if (IsDamageUpgradeable())
                {
                    upgradeDamageButton.gameObject.SetActive(true);
                }
                else
                {
                    upgradeDamageButton.gameObject.SetActive(false);
                }
                if (IsFireRateUpgradeable())
                {
                    upgradeFireRateButton.gameObject.SetActive(true);

                }
                else
                {
                    upgradeFireRateButton.gameObject.SetActive(false);

                }
                if (IsMagCapacityUpgradeable())
                {
                    upgradeMagCapacityButton.gameObject.SetActive(true);

                }
                else
                {
                    upgradeMagCapacityButton.gameObject.SetActive(false);

                }
            }
            else
            {
                upgradeDamageButton.gameObject.SetActive(false);
                upgradeFireRateButton.gameObject.SetActive(false);
                upgradeMagCapacityButton.gameObject.SetActive(false);
            }
        }
        else
        {
            upgradeArea.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(true);
        }


    }
    public void UpgradeDamage()
    {

        if (IsDamageUpgradeable())
        {
            bullet.damage += damageUpgradeAmount;
        }
    }
    public bool IsDamageUpgradeable()
    {
        return bullet.damage < maxDamage;
    }
    public void UpgradeFireRate()
    {

        if (IsFireRateUpgradeable())
        {
            bullet.spawnTime -= fireRateUpgradeAmount;
        }
    }
    public bool IsFireRateUpgradeable()
    {
        return bullet.spawnTime > maxFireRate;
    }
    public void UpgradeMagCapacity()
    {

        if (IsMagCapacityUpgradeable())
        {
            bullet.maxMagazine += magCapacityUpgradeAmount;
        }
    }
    public bool IsMagCapacityUpgradeable()
    {
        return bullet.maxMagazine < maxMagCapacity;
    }

    public void DecreaseGunUpgradePoints()
    {
        player.gunUpgradePoints--;
    }
    public void BuyRifle()
    {
        if (player.money >= 10)
        {
            player.weapon.UnlockRifle();
            selectButton.gameObject.SetActive(true);
        }

    }
    public void BuyShotgun()
    {
        if (player.money >= 10)
        {
            player.weapon.UnlockShotgun();
            selectButton.gameObject.SetActive(true);
        }
    }
    public void BuyGrenade()
    {
        if (player.money >= 1)
        {
            player.weapon.BuyGrenade();
        }
    }
}
