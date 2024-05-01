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
    // �������� �ƴ����� �Ǻ��ϴ� �Լ� (24.04.01 ���� ������)
    public virtual bool IsLine()
    {
        return false;
    }
    // Getter, Setter
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
    
}
