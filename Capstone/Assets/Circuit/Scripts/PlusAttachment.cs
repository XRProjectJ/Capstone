using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusAttachment : Attachment
{
    public List<MinusAttachment> links = new List<MinusAttachment>();
    [SerializeField] protected MinusAttachment pair;
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
            isParallel = true;
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
            isParallel = false;
        }
    }
    public MinusAttachment GetPair()
    {
        return pair;
    }
}
