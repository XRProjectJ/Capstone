using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallel : ComponentClass
{
    public List<ComponentClass> innerStart = new List<ComponentClass>();
    public List<ComponentClass> innerEnd = new List<ComponentClass>();

    private double calcSerialR(ComponentClass start)
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
        SetR(1/result);
    }
    private double calcSerialV(ComponentClass start)
    {
        double result = 0;
        ComponentClass cur = start;
        while (true)
        {
            result += cur.GetV();
            if (innerEnd.Contains(cur))
            {
                break;
            }
            cur = cur.plus.links[0].GetComponent();
        }
        return result;
    }
    public void calcV()
    {
        double result = 0;
        double max = 0;
        for (int i = 0; i < innerStart.Count; i++)
        {
            double tmp = calcSerialV(innerStart[i]);
            if (max < tmp)
            {
                max = tmp;
            }
        }
        SetV(result);
    }
    private void setSerialRoot(ComponentClass start)
    {
        ComponentClass cur = start;
        while (true)
        {
            cur.SetRootParallel(this);
            if (innerEnd.Contains(cur))
            {
                break;
            }
            cur = cur.plus.links[0].GetComponent();
        }
    }
    public void SetRoot()
    {
        for (int i = 0; i < innerStart.Count; i++)
        {
            setSerialRoot(innerStart[i]);
        }
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
    public override bool IsParallel()
    {
        return true;
    }
}
