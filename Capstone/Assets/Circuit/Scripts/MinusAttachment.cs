using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinusAttachment : Attachment
{
    public List<PlusAttachment> links = new List<PlusAttachment>();
    [SerializeField] protected PlusAttachment pair;
    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<PlusAttachment>() == null)
        {
            return;
        }
        links.Add(obj.GetComponent<PlusAttachment>());
        linkSize++;
        if (linkSize > 1)
        {
            isParallel = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<PlusAttachment>() == null)
        {
            return;
        }
        links.Remove(obj.GetComponent<PlusAttachment>());
        linkSize--;
        if (linkSize <= 1)
        {
            isParallel = false;
        }
    }
    public PlusAttachment GetPair()
    {
        return pair;
    }
}
