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
    // 병렬 내부의 부품일 경우 rootParallel 은 null 이 아니라 대체된 Parallel 부품 (빈 오브젝트)임
    public ComponentClass rootParallel = null;
    // 병렬 내부인데 시작점일 경우 병렬의 끝나는 지점을 저장
    private ComponentClass pairOfStart = null;
    // 병렬 내부인데 끝점일 경우 병렬의 시작점을 저장
    private ComponentClass pairOfEnd = null;
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
    // 병렬 부품(빈 오브젝트)인지 아닌지를 판별하는 함수
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
