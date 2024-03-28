using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Attachment : MonoBehaviour
{
    
    [SerializeField] protected ComponentClass component;

    protected int linkSize = 0;
    protected bool isParallel = false;

    public bool GetIsParallel()
    {
        return isParallel;
    }
    public ComponentClass GetComponent()
    {
        return component;
    }
    
}
