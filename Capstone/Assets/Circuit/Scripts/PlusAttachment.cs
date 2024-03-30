using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusAttachment : Attachment
{
    public List<MinusAttachment> links = new List<MinusAttachment>();
    [SerializeField] protected MinusAttachment pair;
    protected bool isEndOfParallel = false;
    protected bool isStartOfParallel = false;
    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<MinusAttachment>() == null)
        {
            return;
        }
        links.Add(obj.GetComponent<MinusAttachment>());
        linkSize++;
        if (linkSize > 1)
        {
            isStartOfParallel = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<MinusAttachment>() == null)
        {
            return;
        }
        links.Remove(obj.GetComponent<MinusAttachment>());
        linkSize--;
        if (linkSize <= 1)
        {
            isStartOfParallel = false;
            isEndOfParallel = false;
        }
    }
    public MinusAttachment GetPair()
    {
        return pair;
    }
    public bool GetIsEndOfParallel()
    {
        return isEndOfParallel;
    }
    public void SetIsEndOfParallel(bool value)
    {
        isEndOfParallel = value;
    }
    public bool GetIsStartOfParallel()
    {
        return isStartOfParallel;
    }
    public void SetIsStartOfParallel(bool value)
    {
        isStartOfParallel = value;
    }
}
