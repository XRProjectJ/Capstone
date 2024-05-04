using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallel : ComponentClass
{
    public List<ComponentClass> innerStart = new List<ComponentClass>();
    public List<ComponentClass> innerEnd = new List<ComponentClass>();

    public double calcSerialR(ComponentClass start)
    {
        double result = 0;
        ComponentClass cur = start;
        while (true)
        {
            result += cur.GetR();
            if (innerEnd.Contains(cur))
            {
                break;
            }
            cur = cur.plus.links[0].GetComponent();
        }
        return result;
    }
    public void calcR()
    {
        double result = 0;
        for(int i=0; i < innerStart.Count; i++)
        {
            result += 1 / calcSerialR(innerStart[i]);
        }
        SetR(result);
    }
    public void DeleteParallel()
    {
        for(int i=0; i < innerStart.Count; i++)
        {
            minus.links[0].links.Add(innerStart[i].minus);
        }
        for (int i = 0; i < innerEnd.Count; i++)
        {
            plus.links[0].links.Add(innerEnd[i].plus);
        }
        Destroy(transform.parent);
    }
    public void SetInnerStart(ComponentClass item)
    {
        innerStart.Add(item);
    }
    public void SetInnerEnd(ComponentClass item)
    {
        innerEnd.Add(item);
    }
    public List<ComponentClass> GetInnerStart()
    {
        return innerStart;
    }
    public List<ComponentClass> GetInnerEnd()
    {
        return innerEnd;
    }
}
