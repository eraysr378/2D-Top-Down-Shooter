using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySystem : MonoBehaviour
{
    public PlayerController player;
    public bool isAnimationPlaying;
    public Image meleeAttackImage;
    public GameObject meleeAttackEffect;
    public float meleeAttackCooldown;
    public float meleeAttackDuration;
    public float meleeDashSpeed;
    public float meleeDamage;
    bool isMeleeAttackCooldown;
    private AudioManager audioManager;

    

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        meleeAttackImage.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMeleeAttackCooldown)
        {
            meleeAttackImage.fillAmount += 1 / meleeAttackCooldown * Time.deltaTime;

        }
        if(meleeAttackImage.fillAmount == 1)
        {
            isMeleeAttackCooldown = false;
        }
    }
    public void Ability1()
    {
        if (!isMeleeAttackCooldown && !player.weapon.isReloading)
        {
            isMeleeAttackCooldown = true;
            meleeAttackImage.fillAmount = 0;
            StartCoroutine(MeleeAttack());
        }
    }
   
    private IEnumerator MeleeAttack()
    {
        player.animator.SetTrigger("MeleeAttack");
        isAnimationPlaying = true;
        player.rb.velocity = player.currentBodyDirection.normalized * meleeDashSpeed;
        meleeAttackEffect.SetActive(true);
        audioManager.Play("MeleeAttack");
        yield return new WaitForSeconds(meleeAttackDuration);
        player.rb.velocity = Vector2.zero;
        isAnimationPlaying = false;
    }
}
