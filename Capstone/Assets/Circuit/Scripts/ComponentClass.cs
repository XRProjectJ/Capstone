using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using Oculus.Interaction;
using System;

// ???? ???? ???????? ?????? ??????
public class ComponentClass : MonoBehaviour
{
    // ???? ?????? + ???? - ???? ???? ?????? ?????? 
    public PlusAttachment plus;
    public MinusAttachment minus;
    // ???? ?????? ?????? ???? rootParallel ?? null ?? ?????? ?????? Parallel ???? (?? ????????)??
    private ComponentClass rootParallel = null;
    // ???? ???????? ???????? ???? ?????? ?????? ?????? ????
    private ComponentClass pairOfStart = null;
    // ???? ???????? ?????? ???? ?????? ???????? ????
    private ComponentClass pairOfEnd = null;
    // ????, ????, ????
    /*[SerializeField] protected double R = 0;
    [SerializeField] protected double V = 0;
    [SerializeField] protected double I = 0;*/
    public double R = 0;
    public double V = 0;
    public double I = 0;
    public double showR = 0;
    public double showV = 0;
    public double showI = 0;

    private bool grab = false;
    // ???? ?? ?????? ?????? ???????? ???? ????
    private bool visit = false;
    private bool findVisit = false;

    

    // ?????? ?????? ?????? ????
    public virtual void Do(double totalCurrent)
    {

    }
    // ???? ????(?? ????????)???? ???????? ???????? ????
    public virtual bool IsParallel()
    {
        return false;
    }
    public virtual bool IsLine()
    {
        return false;
    }
    // Getter, Setter
    public void SetGrab(bool value)
    {
        grab = value;
    }
    public void doGrab()
    {
        Debug.Log("doGrab");
        grab = true;
    }
    public void doNotGrab()
    {
        grab = false;
    }
    public void SetR(double R)
    {
        this.R = R;
    }
    public void SetPairOfStart(ComponentClass item)
    {
        this.pairOfStart = item;
    }
    public void SetPairOfEnd(ComponentClass item)
    {
        this.pairOfEnd = item;
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
    public void SetShowR(double showR)
    {
        this.showR = showR;
    }
    public void SetShowV(double showV)
    {
        this.showV = showV;
    }
    public void SetShowI(double showI) 
    { 
        this.showI = showI;
    }
    public void SetFindVisit(bool findVisit)
    {
        this.findVisit = findVisit;
    }
    public void SetRootParallel(ComponentClass root)
    {
        this.rootParallel = root;
    }
    public double GetR()
    {
        return Math.Round(this.R, 2);
    }
    public double GetV()
    {
        return Math.Round(this.V, 2);
    }
    public double GetI() 
    { 
        return Math.Round(this.I, 2);
    }
    public bool GetVisit()
    {
        return visit;
    }
    public double GetShowR()
    {
        return Math.Round(showR, 2);
    }
    public double GetShowV()
    {
        return Math.Round(showV, 2);
    }
    public double GetShowI()
    {
        return Math.Round(showI, 2);
    }
    public bool GetFindVisit()
    {
        return findVisit;
    }
    public ComponentClass GetRootParallel()
    {
        return rootParallel;
    }
    public ComponentClass GetPairOfStart()
    {
        return pairOfStart;
    }
    public ComponentClass GetPairOfEnd()
    {
        return pairOfEnd;
    }
    public bool GetGrab()
    {
        return grab;
    }
}
