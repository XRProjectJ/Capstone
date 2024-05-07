using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallel : ComponentClass
{
    public List<ComponentClass> innerStart = new List<ComponentClass>();
    public List<ComponentClass> innerEnd = new List<ComponentClass>();
    public struct Branch
    {
        public double resist;
        public List<ComponentClass> components;
    }
    private List<Branch> branches = new List<Branch>();
    private double calcSerialR(ComponentClass start)
    {
        double result = 0;
        ComponentClass cur = start;
        Branch branch = new Branch();
        List<ComponentClass> components = new List<ComponentClass>();
        branch.components = components;
        while (true)
        {
            result += cur.GetR();
            branch.components.Add(cur);
            if (innerEnd.Contains(cur))
            {
                break;
            }
            cur = cur.plus.links[0].GetComponent();
        }
        branch.resist = result;
        branches.Add(branch);
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
        SetV(max);
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
        minus.links[0].links.Remove(this.minus);
        plus.links[0].links.Remove(this.plus);
        for (int i=0; i < innerStart.Count; i++)
        {
            minus.links[0].links.Add(innerStart[i].minus);
        }
        for (int i = 0; i < innerEnd.Count; i++)
        {
            plus.links[0].links.Add(innerEnd[i].plus);
        }
        for(int i=0; i < branches.Count; i++)
        {
            for(int j=0; j < branches[i].components.Count; j++)
            {
                branches[i].components[j].SetRootParallel(null);
            }
        }
        Destroy(this.gameObject);
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
    public List<Branch> GetBranches()
    {
        return branches;
    }
    public override bool IsParallel()
    {
        return true;
    }

}
