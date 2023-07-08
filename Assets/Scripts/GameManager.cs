using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject fastEnemy;
    public GameObject bossEnemy;
    public GameObject normalEnemy;
    public Button characterUpgradesAvailableButton;
    public Button gunUpgradesAvailableButton;
    public float fastEnemySpawnTime;
    public float normalEnemySpawnTime;
    public float bossEnemySpawnTime;
    private float normalEnemySpawnAmount = 1;
    private float fastEnemySpawnAmount = 1;
    private float bossEnemySpawnAmount = 1;
    private bool isFastEnemySpawnable = true;
    private bool isNormalEnemySpawnable = true;
    private bool isBossEnemySpawnable = true;
    private float normalEnemyKillTarget = 5;
    private float fastEnemyKillTarget = 5;
    private float bossEnemyKillTarget = 5;
    public bool isUpgradeable;
    public PlayerController player;
    private ObjectPooler objectPooler;
    private float currentKilledBoss = 0;

    public float startEnemyAmount;
    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        //CreateEnemy();
    }
    /*
    // Update is called once per frame
    void Update()
    {
        //CheckEnemySpawnAmounts();
        CreateEnemy();
    }*/
    private void FixedUpdate()
    {
        CheckUpgradesAvailable();
       /* if(player.killedBossEnemy > currentKilledBoss)
        {
            player.characterUpgradePoints++;
            currentKilledBoss = player.killedBossEnemy;
        }*/
    }
    public void CreateEnemy()
    {
        /************/
        player.characterUpgradePoints++;
        player.gunUpgradePoints++;
        /**************/
        float random = Random.Range(0,30f);
        if(random >= 0 && random <= 10)
        {
            StartCoroutine(CreateNormalEnemy());
        }
        else if(random >10 && random <=20)
        {
            StartCoroutine(CreateFastEnemy());
        }
        else
        {
            StartCoroutine(CreateBossEnemy());
        }
        /*if (isNormalEnemySpawnable)
        {
            StartCoroutine(CreateNormalEnemy());
        }
        if (isFastEnemySpawnable)
        {
            StartCoroutine(CreateFastEnemy());
        }
        if (isBossEnemySpawnable)
        {
            StartCoroutine(CreateBossEnemy());
        }*/
        
    }
    public void CheckEnemySpawnAmounts()
    {
        if(player.killedNormalEnemy >= normalEnemyKillTarget)
        {
            normalEnemySpawnAmount++;
            normalEnemyKillTarget *= 2f;
            normalEnemySpawnTime -= 0.25f;
        }
        if (player.killedFastEnemy >= fastEnemyKillTarget)
        {
            fastEnemySpawnAmount++;
            fastEnemyKillTarget *= 2f;
            fastEnemySpawnTime -= 0.25f;
        }
        if (player.killedBossEnemy >= bossEnemyKillTarget)
        {
            //bossEnemySpawnAmount++;
        }
    }
    public Vector3 RandomPosition()
    {
        float randomx = Random.Range(-8, 8);
        float randomy = Random.Range(-4, 4);
        Vector3 pos = transform.position;
        pos.x = randomx;
        pos.y = randomy;
        return pos;
    }
    public IEnumerator CreateNormalEnemy()
    {
        for(int i = 0; i < normalEnemySpawnAmount; i++)
        {
            Vector3 position = RandomPosition();
            GameObject obj = objectPooler.SpawnFromPool("NormalEnemy", position, Quaternion.identity);
        } 
        isNormalEnemySpawnable = false;
        yield return new WaitForSeconds(normalEnemySpawnTime);
        isNormalEnemySpawnable = true;
    }
    public IEnumerator CreateBossEnemy()
    {
        for(int i = 0; i < bossEnemySpawnAmount; i++)
        {
            Vector3 position = RandomPosition();
            GameObject obj = objectPooler.SpawnFromPool("BossEnemy", position, Quaternion.identity);
        }
        
        isBossEnemySpawnable = false;
        yield return new WaitForSeconds(bossEnemySpawnTime);
        isBossEnemySpawnable = true;
    }
    public IEnumerator CreateFastEnemy()
    {
        for(int i = 0; i < fastEnemySpawnAmount; i++)
        {
            Vector3 position = RandomPosition();
            GameObject obj = objectPooler.SpawnFromPool("FastEnemy", position, Quaternion.identity);
            obj.SetActive(true);
        }
        isFastEnemySpawnable = false;
        yield return new WaitForSeconds(fastEnemySpawnTime);
        isFastEnemySpawnable = true;
    }
    private void CheckUpgradesAvailable()
    {
        if(player.characterUpgradePoints > 0)
        {
            characterUpgradesAvailableButton.gameObject.SetActive(true);
        }
        else
        {
            characterUpgradesAvailableButton.gameObject.SetActive(false);
        }
        if(player.gunUpgradePoints > 0)
        {
            gunUpgradesAvailableButton.gameObject.SetActive(true);
        }
        else
        {
            gunUpgradesAvailableButton.gameObject.SetActive(false);
        }
    }




}
