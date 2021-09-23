using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowControl : MonoBehaviour
{
    public Window[] window;

    public void deleteWindows()
    {
        for(int i = 0; i < window.Length; i++)
        {
            Destroy(window[i]);
        }
    }
}
