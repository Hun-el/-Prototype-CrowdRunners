using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SquadFormation : MonoBehaviour
{
    [Header(" Components ")]
    [SerializeField] private TextMesh squadAmountText;

    [Header(" Formation Settings ")]
    [Range(0f, 1f)][SerializeField] private float radiusFactor;
    [Range(0f, 1f)][SerializeField] private float angleFactor;

    [Header(" Settings ")]
    [SerializeField] private Runner runnerPrefab;

    public bool setFormation = false;

    private PoolSystem pool;

    private void Start() {
        pool = new PoolSystem( runnerPrefab );
        pool.InstantiateRunnerPool( 100 );
        AddRunners(1,true);
    }

    void Update()
    {
        if(setFormation){ FermatSpiralPlacement(0.3f); }
        squadAmountText.text = (transform.childCount).ToString();
    }

    private void FermatSpiralPlacement(float speed = 1)
    {
        float goldenAngle = 137.5f * angleFactor;  

        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.tag == "Stickman")
            { 
                float x = radiusFactor * Mathf.Sqrt(i+1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (i+1));
                float z = radiusFactor * Mathf.Sqrt(i+1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (i+1));

                Vector3 runnerLocalPosition = new Vector3(x, transform.GetChild(i).transform.position.y, z);
                transform.GetChild(i).localPosition = Vector3.MoveTowards(transform.GetChild(i).localPosition, runnerLocalPosition, speed * Time.deltaTime);
            }
        }
    }

    public void Turn(int right)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.tag == "Stickman")
            { 
                transform.GetChild(i).DOLocalRotate(new Vector3(0,45 * right,0) , 1);
            }
        }
    }

    public float GetSquadRadius()
    {
        return radiusFactor * Mathf.Sqrt(transform.childCount);
    }

    public void AddRunners(int amount,bool starter)
    {
        setFormation = true;
        Invoke("StopFormation",2f);

        for (int i = 0; i < amount; i++)
        {
            Runner runnerInstance = pool.GetRunnerPool();
            runnerInstance.transform.localPosition = new Vector3(transform.position.x,transform.position.y + .1f,transform.position.z);
            runnerInstance.transform.SetParent(transform);
            runnerInstance.gameObject.SetActive(true);
            runnerInstance.transform.DOScale(new Vector3(0.75f,0.75f,0.75f) , 0.5f);
            runnerInstance.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.white;
            runnerInstance.transform.GetChild(1).GetComponent<Renderer>().material.DOColor(new Color(0/255f, 108f/255f, 255f/255f, 255/255f),0.6f).SetDelay(0.6f);
            runnerInstance.gameObject.GetComponent<Animator>().SetBool("Run",!starter);
            runnerInstance.gameObject.GetComponent<Animator>().speed = Random.Range(0.85f,1f);
        }
    }

    public void RemoveRunner(Runner runner)
    {
        pool.AddPool(runner);
    }

    void StopFormation()
    {
        setFormation = false;
    }
}

