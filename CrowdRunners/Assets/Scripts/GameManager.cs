using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject platformPrefab;
    [SerializeField] GameObject halfPlatformPrefab;
    [SerializeField] GameObject lastPlatformPrefab;
    [SerializeField] GameObject movingPlatformPrefab;
    [SerializeField] GameObject windowsPrefab;
    [SerializeField] GameObject stairPrefab;
    [SerializeField] GameObject enemySquadPrefab;

    [Header("Canvases Prefabs")]
    [SerializeField] GameObject winPrefab;
    [SerializeField] GameObject losePrefab;

    [Header("Spawn Locations")]
    public Transform platformLoc;
    public Transform windowLoc;
    public Transform enemyLoc;

    [Header("Platform Settings")]
    [Range(6,20)][SerializeField] int maxPlatform;
    [Range(2,10)][SerializeField] int minPlatform;
    [Range(1,10)][SerializeField] int increasePlatformPerLevel;
    [Range(0,10)][SerializeField] int increasePlatform;

    [Header("Normal Platform Settings")]
    [Range(1,100)][SerializeField] int nPlatformChance;

    [Header("Moving Platform Settings")]
    [Range(1,100)][SerializeField] int mPlatformActiveLevel;
    [Range(1,100)][SerializeField] int mPlatformChance;

    [Header("Enemy Settings")]
    [Range(1,50)]public int minEnemy;
    [Range(10,50)]public int maxEnemy;
    [Range(1,10)]public int increaseEnemyPerLevel;
    [Range(0,20)]public int increaseEnemy;

    [Header("Window Settings")]
    [Range(1,60)]public int minValue;
    [Range(10,60)]public int maxValue;
    [Range(1,10)]public int increaseValuePerLevel;
    [Range(0,20)]public int increaseValue;

    [Header("Runner Settings")]
    [Range(0,3)]public float minRunnerSpeed;
    [Range(1,3)]public float maxRunnerSpeed;
    [Range(1,10)]public int increaseSpeedPerLevel;
    [Range(0,2)]public float increaseSpeed;

    [Header("Others")]
    [SerializeField] GameObject mainPlatform;
    public bool readyforMove;
    public bool gameover;

    GameObject platformClone;

    LevelSystem levelSystem;

    void Start()
    {
        levelSystem = FindObjectOfType<LevelSystem>();
        platformClone = mainPlatform;

        if(PlayerPrefs.GetInt("level") < mPlatformActiveLevel){ mPlatformChance = 0; }

        if(minPlatform + ((PlayerPrefs.GetInt("level") / increasePlatformPerLevel) * increasePlatform) <= maxPlatform)
        {
            SetupPlatform(minPlatform + ((PlayerPrefs.GetInt("level") / increasePlatformPerLevel) * increasePlatform));
        }
        else
        {
            SetupPlatform(maxPlatform);
        }
    }

    private void FixedUpdate() {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 3f);
    }

    void SetupPlatform(int count)
    {
        for(int a = 0; a < count; a++)
        {
            GameObject platform = null;
            Collider cloneColl = platformClone.GetComponent<Collider>();
            Vector3 v = new Vector3(cloneColl.bounds.center.x,cloneColl.bounds.center.y,cloneColl.bounds.max.z + (cloneColl.bounds.size.z / 2));

            platform = Choice(new int[]{nPlatformChance, mPlatformChance},new GameObject[]{platformPrefab, movingPlatformPrefab});

            platformClone = Instantiate(platform,v,Quaternion.identity);
            platformClone.transform.SetParent(platformLoc);

            if(platform == platformPrefab){ SpawnWindow(platformClone); SpawnEnemy(platformClone);}

            if(a == count - 1)
            {
                cloneColl = platformClone.GetComponent<Collider>();
                v = new Vector3(cloneColl.bounds.center.x,cloneColl.bounds.center.y,cloneColl.bounds.max.z + (cloneColl.bounds.size.z / 2));
                platformClone = Instantiate(lastPlatformPrefab,v,Quaternion.identity);
                platformClone.transform.SetParent(platformLoc);
            }
        }
    }

    void SpawnWindow(GameObject platformClone)
    {
        Collider col = platformClone.GetComponent<Collider>();
        GameObject windowClone = Instantiate(windowsPrefab,new Vector3(col.bounds.center.x,col.bounds.max.y,col.bounds.min.z + .35f),Quaternion.identity);
        windowClone.transform.SetParent(windowLoc);
    }

    void SpawnEnemy(GameObject platformClone)
    {
        Collider col = platformClone.GetComponent<Collider>();
        GameObject windowClone = Instantiate(enemySquadPrefab,new Vector3(col.bounds.center.x,col.bounds.max.y,col.bounds.max.z - .25f),Quaternion.identity);
        windowClone.transform.SetParent(enemyLoc);
    }

    GameObject Choice(int[] Chancies,GameObject[] prefabs)
    {
        float maxChoice = 0f;
        for(int a = 0; a < Chancies.Length; a++)
        {
            maxChoice += Chancies[a];
        }
        float randChoice = Random.Range(0, maxChoice);
        float sum = 0;
        for (int b = 0; b < Chancies.Length; b++)
        {
            sum += Chancies[b];
            if (randChoice <= sum)
            {
                return prefabs[b];
            }
        }
        return null;
    }

    public void Ready()
    {
        readyforMove = true;
        SquadControl squadControl = FindObjectOfType<SquadControl>();
        squadControl.Move();
    }

    public void Win()
    {
        Instantiate(winPrefab);
        StartCoroutine(LevelUp(true));
        gameover = true;
    }

    public void Lose()
    {
        Instantiate(losePrefab);
        StartCoroutine(LevelUp(false));
        gameover = true;
    }

    IEnumerator LevelUp(bool win)
    {
        if(win)
        {
            levelSystem.LevelUp();
        }
        yield return new WaitForSeconds(1.5f);
        LoadingSystem loadingSystem = GetComponent<LoadingSystem>();
        loadingSystem.LoadLevel("Restart");
    }
}
