using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attachment : MonoBehaviour
{
    public List<Attachment> links = new List<Attachment>();
    [SerializeField] private Attachment pair;
    [SerializeField] private ComponentClass component;
    
    private int linkSize = 0;
    private bool isParallel = false;
    
    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<Attachment>() == null)
        {
            return;
        }
        links.Add(obj.GetComponent<Attachment>());
        linkSize++;
        if(linkSize > 1)
        {
            isParallel = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<Attachment>() == null)
        {
            return;
        }
        links.Remove(obj.GetComponent<Attachment>());
        linkSize--;
        if (linkSize <= 1)
        {
            isParallel = false;
        }
    }
    public bool GetIsParallel()
    {
        return isParallel;
    }
    public Attachment GetPair()
    {
        return pair;
    }
}
