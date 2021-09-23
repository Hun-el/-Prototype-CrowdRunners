using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] TextMesh textMesh;

    int value;
    int min,max;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        setValue();
        SetTextValue();
    }

    void setValue()
    {
        min = gameManager.minValue + (((PlayerPrefs.GetInt("level") / gameManager.increaseValuePerLevel) - 1)*gameManager.increaseValue);
        max  = gameManager.maxValue + (((PlayerPrefs.GetInt("level") / gameManager.increaseValuePerLevel) - 1)*gameManager.increaseValue);
        if(max > 60){max = 60;}
        if(10 > max - min){ min = max - (10);}

        value = Random.Range(min,max+1);
    }

    void SetTextValue()
    {
        textMesh.text = "+"+value.ToString();
    }

    public int Value { get => value; }
}