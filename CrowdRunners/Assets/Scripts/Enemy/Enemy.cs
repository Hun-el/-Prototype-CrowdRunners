using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;

    [SerializeField] LayerMask RunnerLayer;

    Transform target;

    [SerializeField] public bool readyAttack;
    bool targetDestroyed;

    void Update() 
    {
        if(readyAttack && target == null){ FindTargetRunner(); }
        if(readyAttack && target != null && !target.gameObject.activeSelf){ FindTargetRunner(); }
        if(target){ Attack(); }
    }

    void FindTargetRunner()
    {
        Collider[] detectedRunners = Physics.OverlapSphere(transform.position, 10, RunnerLayer);

        if (detectedRunners.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < detectedRunners.Length; i++)
        {
            Runner currentRunner = detectedRunners[i].GetComponent<Runner>();
            if (currentRunner.isTargeted)
            {
                continue;
            }
            currentRunner.setTarget(this.gameObject);
            target = currentRunner.transform;
            break;
        }
    }

    void Attack()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, .5f * Time.deltaTime);
        GetComponent<Animator>().SetBool("Run",true);
    }

    void Kill()
    {
        Instantiate(explosionPrefab,new Vector3(transform.position.x,transform.position.y + 0.1f,transform.position.z),Quaternion.identity);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Runner") && !targetDestroyed)
        {
            targetDestroyed = true;
            other.gameObject.GetComponent<Runner>().Kill();

            Kill();
        }
        if(other.tag == "Outside")
        {
            Kill();
        }
    }
}
