using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] LayerMask RunnerLayer;

    bool Detected;
    
    void Update() 
    {
        if(!Detected){ DetectRunner(); }
    }

    void DetectRunner()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, .5f, RunnerLayer);
        if (colliders.Length > 0)
        {
            colliders[0].transform.root.GetComponent<SquadControl>().ReadyAttack(this.transform);

            Attack(colliders[0].transform.root);

            Detected = true;
        }
    }

    void Attack(Transform target)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.tag == "Stickman")
            { 
                transform.GetChild(i).gameObject.GetComponent<Enemy>().readyAttack = true;
            }
        }
    }
}
