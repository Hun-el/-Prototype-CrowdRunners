using System.Collections.Generic;
using UnityEngine;
 
public class PoolSystem
{
    private Runner prefab;
    private Stack<Runner> objectPool = new Stack<Runner>();
 
    public PoolSystem( Runner prefab )
    {
        this.prefab = prefab;
    }
 
    public void InstantiateRunnerPool( int miktar )
    {
        for( int i = 0; i < miktar; i++ )
        {
            Runner _object = Object.Instantiate( prefab );
            AddPool( _object );
        }
    }
 
    public Runner GetRunnerPool()
    {
        if(objectPool.Count > 0 )
        {
            Runner obje = objectPool.Pop();
            return obje;
        }
 
        return Object.Instantiate(prefab);
    }
 
    public void AddPool( Runner _object )
    {
        _object.gameObject.SetActive(false);
        _object.transform.parent = null;
        objectPool.Push( _object );
    }
}
