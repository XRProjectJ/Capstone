using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

// ��� ��ǰ Ŭ������ �ֻ��� Ŭ����
public class ComponentClass : MonoBehaviour
{
    // ��� ��ǰ�� + �ذ� - �ؿ� �ٸ� ��ǰ�� ����� 
    public PlusAttachment plus;
    public MinusAttachment minus;
    // ���� ������ ��ǰ�� ��� rootParallel �� null �� �ƴ϶� ��ü�� Parallel ��ǰ (�� ������Ʈ)��
    public ComponentClass rootParallel = null;
    // ���� �����ε� �������� ��� ������ ������ ������ ����
    private ComponentClass pairOfStart = null;
    // ���� �����ε� ������ ��� ������ �������� ����
    private ComponentClass pairOfEnd = null;
    // ����, ����, ����
    /*[SerializeField] protected double R = 0;
    [SerializeField] protected double V = 0;
    [SerializeField] protected double I = 0;*/
    public double R = 0;
    public double V = 0;
    public double I = 0;
    public double showR = 0;
    public double showV = 0;
    public double showI = 0;
    // ��ȸ �� �湮�� �ߴ��� Ȯ���ϱ� ���� ����
    public bool visit = false;
    public bool findVisit = false;

    // ��ǰ�� ������ ������ �Լ�
    public virtual void Do()
    {

    }
    // ���� ��ǰ(�� ������Ʈ)���� �ƴ����� �Ǻ��ϴ� �Լ�
    public virtual bool IsParallel()
    {
        return false;
    }
    // Getter, Setter
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
    public double GetShowR()
    {
        return showR;
    }
    public double GetShowV()
    {
        return showV;
    }
    public double GetShowI()
    {
        return showI;
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
}
