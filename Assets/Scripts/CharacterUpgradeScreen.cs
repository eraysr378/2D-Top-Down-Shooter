using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUpgradeScreen : MonoBehaviour
{
    public PlayerController player;
    public Button increaseMoveSpeedButton;
    public Button increaseMaxHealth;
    public float healthUpgradeAmount;
    public float maxUpgradeableHealth;
    public float moveSpeedUpgradeAmount;
    public float maxMoveSpeed;


    private void Update()
    {
        if(player.characterUpgradePoints > 0)
        {
            if (IsMoveSpeedUpgradeable())
            {
                increaseMoveSpeedButton.gameObject.SetActive(true);
            }
            else
            {
                increaseMoveSpeedButton.gameObject.SetActive(false);
            }
            if (IsMaxHealthUpgradeable())
            {
                increaseMaxHealth.gameObject.SetActive(true);
            }
            else
            {
                increaseMaxHealth.gameObject.SetActive(false);
            }
        }
        else
        {
            increaseMoveSpeedButton.gameObject.SetActive(false);
            increaseMaxHealth.gameObject.SetActive(false);
        }
    }
    public void UpgradeMoveSpeed()
    {
        if (IsMoveSpeedUpgradeable())
        {
            player.moveSpeed += moveSpeedUpgradeAmount;
        }
    }
    public void UpgradeMaxHealth()
    {
        if (IsMaxHealthUpgradeable())
        {
            player.maxHealth += healthUpgradeAmount;
        }
    }
    public bool IsMoveSpeedUpgradeable()
    {
        return player.moveSpeed < maxMoveSpeed;
    }
    public bool IsMaxHealthUpgradeable()
    {
        return player.maxHealth < maxUpgradeableHealth;
    }
    public void DecreaseSkillPoint()
    {
        player.characterUpgradePoints--;
    }


}
