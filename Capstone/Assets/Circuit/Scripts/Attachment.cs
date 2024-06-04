using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Attachment : MonoBehaviour
{
    [SerializeField] protected ComponentClass component;
    private Vector3 offset = Vector3.zero;
    private void Start()
    {
/*        GameObject model = component.gameObject.transform.parent.gameObject;
        Debug.Log("model : " + model);
        GameObject compo = component.gameObject;
        Debug.Log("compo : " + component);
        Vector3 modelPos = model.transform.localPosition;
        offset = modelPos - this.transform.localPosition;
        offset.x = offset.x * model.transform.localScale.x * compo.transform.localScale.x;
        offset.y = offset.y * model.transform.localScale.y * compo.transform.localScale.y;
        offset.z = offset.z * model.transform.localScale.z * compo.transform.localScale.z;
        Debug.Log("offset : " + offset);*/
    }
    public ComponentClass GetComponent()
    {
        return component;
    }
    public Vector3 GetOffset()
    {
        return offset;
    }
    
}
