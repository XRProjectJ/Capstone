using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class ComponentClass : MonoBehaviour
{
    public PlusAttachment plus;
    public MinusAttachment minus;
    [SerializeField] protected double R = 0;
    [SerializeField] protected double V = 0;
    [SerializeField] protected double I = 0;
    private bool visit = false;

    public virtual void Do()
    {

    }
    public void SetR(double R)
    {
        this.R = R;
    }
    public void SetV(double V)
    {
        this.V = V;
    }
    public void SetI(double I) 
    {  
        this.I = I;
    }
    public void SetVisit(bool visit)
    {
        this.visit = visit;
    }

    public double GetR()
    {
        return this.R;
    }
    public double GetV()
    {
        return this.V;
    }
    public double GetI() 
    { 
        return this.I;
    }
    public bool GetVisit()
    {
        return visit;
    }
    public virtual bool IsLine()
    {
        return false;
    }
    
}
