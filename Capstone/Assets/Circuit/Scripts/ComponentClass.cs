using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

// 모든 부품 클래스의 최상위 클래스
public class ComponentClass : MonoBehaviour
{
    // 모든 부품은 + 극과 - 극에 다른 부품이 연결됨 
    public PlusAttachment plus;
    public MinusAttachment minus;
    // 저항, 전압, 전류
    /*[SerializeField] protected double R = 0;
    [SerializeField] protected double V = 0;
    [SerializeField] protected double I = 0;*/
    public double R = 0;
    public double V = 0;
    public double I = 0;
    public double showR = 0;
    public double showV = 0;
    public double showI = 0;
    // 순회 시 방문을 했는지 확인하기 위한 변수
    public bool visit = false;
    public bool findVisit = false;

    // 부품별 동작을 구현할 함수
    public virtual void Do()
    {

    }
    // 전선인지 아닌지를 판별하는 함수 (24.04.01 기준 사용안함)
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
