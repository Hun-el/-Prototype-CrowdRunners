using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;

    int amount;
    int min,max;

    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        min = gameManager.minEnemy + (((PlayerPrefs.GetInt("level") / gameManager.increaseEnemyPerLevel) - 1)*gameManager.increaseEnemy);
        max  = gameManager.maxEnemy + (((PlayerPrefs.GetInt("level") / gameManager.increaseEnemyPerLevel) - 1)*gameManager.increaseEnemy);
        if(max > 50){max = 50;}
        if(10 > max - min){ min = max - (10);}

        amount = Random.Range(min,max+1);

        for (int i = 0; i < amount; i++)
        {
            Instantiate(objectToSpawn, transform);
        } 
    }

    void Update()
    {
        if (transform.childCount <= 1)
        {
            Destroy(gameObject);
        }
    }
}
