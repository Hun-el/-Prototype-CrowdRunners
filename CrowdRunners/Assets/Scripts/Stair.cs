using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stair : MonoBehaviour
{
    bool triggered;
    
    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Stickman" && !triggered)
        {
            triggered = true;
            gameObject.GetComponent<Renderer>().material.DOColor(Color.white , 0.4f).OnComplete(()=>{ gameObject.GetComponent<Renderer>().material.DOColor(Color.black , 0.4f); });
        }
    }
}
