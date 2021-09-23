using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Runner : MonoBehaviour
{
    [Header(" Effects ")]
    [SerializeField] private ParticleSystem explosionPrefab;

    GameManager gameManager;
    PoolSystem poolSystem;
    public bool isTargeted;
    public GameObject target;

    void Awake() 
    {
        gameManager = FindObjectOfType<GameManager>();
        if(gameManager.readyforMove && !GetComponent<Animator>().GetBool("Run")){ GetComponent<Animator>().SetBool("Run",true); }
    }

    private void Update() {
        if(isTargeted && !target){isTargeted = false;}
    }
    
    public void Kill()
    {
        Instantiate(explosionPrefab,new Vector3(transform.position.x,transform.position.y + 0.1f,transform.position.z),Quaternion.identity);
        transform.parent.GetComponent<SquadFormation>().RemoveRunner(this);
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Outside")
        {
            SquadFormation squadFormation = FindObjectOfType<SquadFormation>();
            squadFormation.setFormation = false;
            
            transform.parent = null;
            Destroy(this.gameObject , 1f);
        }
        if(other.tag == "Stair")
        {
            GetComponent<Animator>().SetBool("Run",false);
            transform.parent = null;
        }
    }

    public void setTarget(GameObject _target)
    {
        isTargeted = true;
        target = _target;
    }
}