using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class ComponentClass : MonoBehaviour
{
    public Attachment[] attachment = new Attachment[2];
    [SerializeField] protected double R;
    [SerializeField] protected double V;
    [SerializeField] protected double I;

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
    public virtual bool IsLine()
    {
        return false;
    }
}
