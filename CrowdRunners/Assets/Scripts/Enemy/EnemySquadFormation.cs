using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquadFormation : MonoBehaviour
{
    [Header(" Components ")]
    [SerializeField] TextMesh squadAmountText;

    [Header(" Formation Settings ")]
    [Range(0f, 1f)][SerializeField] private float radiusFactor;
    [Range(0f, 1f)][SerializeField] private float angleFactor;

    EnemyControl enemyControl;

    bool placemented = false;

    void Awake() 
    {
        enemyControl = GetComponent<EnemyControl>();
    }

    private void Start() {
        Invoke("Placement",1f);
    }

    void Update()
    {
        if(!placemented){ FermatSpiralPlacement(); }
        squadAmountText.text = (transform.childCount - 1).ToString();
    }

    void FermatSpiralPlacement(float speed = 10)
    {
        float goldenAngle = 137.5f * angleFactor;  

        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.tag == "Stickman")
            { 
                float x = radiusFactor * Mathf.Sqrt(i+1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (i+1));
                float z = radiusFactor * Mathf.Sqrt(i+1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (i+1));

                Vector3 runnerLocalPosition = new Vector3(x, 0, z);
                transform.GetChild(i).localPosition = Vector3.MoveTowards(transform.GetChild(i).localPosition, runnerLocalPosition, speed * Time.deltaTime);
            }
        }
    }

    void Placement()
    {
        placemented = true;
    }

    public float GetSquadRadius()
    {
        return radiusFactor * Mathf.Sqrt(transform.childCount);
    }
}


