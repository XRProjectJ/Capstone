using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Attachment : MonoBehaviour
{
    [SerializeField] protected ComponentClass component;
    public ComponentClass GetComponent()
    {
        return component;
    }
    
}
