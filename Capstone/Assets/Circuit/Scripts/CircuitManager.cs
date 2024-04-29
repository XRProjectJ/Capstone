using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

// 순회를 담당하는 스크립트

public class CircuitManager : MonoBehaviour
{
    struct ParallelR
    {
        public double R;
        public ComponentClass start;
    };
    // 각 부품의 전압, 전류를 구할 때 사용
    private List<ParallelR> parallelRs = new List<ParallelR>();
    double timer = 0;
    bool first = false;
    [SerializeField] private ComponentClass root;
    [SerializeField] private GameObject startComponent;


    // 회로의 전체 저항을 구하는 함수 (순환 호출로 순회)
    // 첫번째 매개변수 : 순회를 시작한 부품
    // 두번째 매개변수 : 현재 순회중인 부품
    // 세번째 매개변수 : 병렬 연결 이후 다음으로 순회해야하는 부품
    // 네번째 매개변수 : 순회의 성공 여부
    // 다섯번째 매개변수 : 현재 부품이 방문했으면 이 매개변수의 값을 visit 으로 가지고 있어야함 (방문 여부가 아님)
    public double calcEntireR(ComponentClass root, ComponentClass node, ref ComponentClass nextOfParallel, ref bool success, bool visit)
    {
        if (success == false)
        {
            return 0;
        }
        // 회로가 중간에 끊겨있는 경우
        if (node.plus.links.Count <= 0)
        {
            Debug.Log(node.transform.parent.transform.name);
            success = false;
            return 0;
        }

        // 순회를 모두 성공적으로 마쳤을 때
        if (node == root && node.GetVisit() == visit)
        {
            return 0;
        }
        double result = 0;
        // 지금 부품이 이미 방문한 부품일 때
        if (node.GetVisit() == visit)
        {
            return 0;
        }
        // 지금 부품이 병렬의 끝일 때
        else if (node.plus.GetIsEndOfParallel())
        {
            node.SetVisit(visit);

            nextOfParallel = node.plus.links[0].GetComponent();

            // 자신의 값을 리턴
            result = node.GetR();
            return result;
        }
        // 지금 부품이 병렬의 시작일 때
        if (node.plus.GetIsStartOfParallel())
        {
            node.SetVisit(visit);
            // 병렬 연결의 내부의 저항 값
            double sumOfParallelR = 0;
            // 병렬 연결 이후의 값을 전부 저장
            double totalR = 0;
            for (int i = 0; i < node.plus.links.Count; i++)
            {
                sumOfParallelR += 1 / calcEntireR(root, node.plus.links[i].GetComponent(), ref nextOfParallel, ref success, visit);
            }
            // 각 부품의 전압, 전류를 구할 때 사용
            ParallelR tmp = new ParallelR();
            tmp.R = 1 / sumOfParallelR;
            tmp.start = node;
            parallelRs.Add(tmp);

            // 병렬 내부를 전부 순회 했으면 병렬 외부의 순회를 다시 진행
            totalR = calcEntireR(root, nextOfParallel, ref nextOfParallel, ref success, visit);

            result = node.GetR() + 1 / sumOfParallelR + totalR;
            return result;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            node.SetVisit(visit);

            result = node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), ref nextOfParallel, ref success, visit);
            return result;
        }


    }
    //회로의 전체 전압 구하는 함수
    // 첫번째 매개변수 : 순회를 시작한 부품
    // 두번째 매개변수 : 현재 순회중인 부품
    // 세번째 매개변수 : 병렬 이후의 전압값들
    // 네번째 매개변수 : 순회의 성공 여부
    // 다섯번째 매개변수 : 현재 부품이 방문했으면 이 매개변수의 값을 visit 으로 가지고 있어야함 (방문 여부가 아님)
    public double calcEntireV(ComponentClass root, ComponentClass node, ref ComponentClass nextOfParallel, ref bool success, bool visit)
    {
        if (success == false)
        {
            return 0;
        }
        // 회로가 중간에 끊겨있는 경우
        if (node.plus.links.Count <= 0)
        {
            Debug.Log(node.transform.parent.transform.name);
            success = false;
            return 0;
        }

        // 순회를 모두 성공적으로 마쳤을 때
        if (node == root && node.GetVisit() == visit)
        {
            return 0;
        }
        double result = 0;
        // 지금 부품이 이미 방문한 부품일 때
        if (node.GetVisit() == visit)
        {
            return 0;
        }
        // 지금 부품이 병렬의 끝일 때
        else if (node.plus.GetIsEndOfParallel())
        {
            node.SetVisit(visit);

            nextOfParallel = node.plus.links[0].GetComponent();

            // 자신의 값을 리턴
            result = node.GetV();
            return result;
        }
        // 지금 부품이 병렬의 시작일 때
        if (node.plus.GetIsStartOfParallel())
        {
            node.SetVisit(visit);
            // 병렬 연결의 내부의 전압 값
            double maxV = 0;
            // 병렬 연결 이후의 값을 전부 저장
            double totalV = 0;
            for (int i = 0; i < node.plus.links.Count; i++)
            {

                double tmpV = calcEntireV(root, node.plus.links[i].GetComponent(), ref nextOfParallel, ref success, visit);
                if(tmpV > maxV)
                {
                    maxV = tmpV;
                }
            }

            // 병렬 내부를 전부 순회 했으면 병렬 외부의 순회를 다시 진행
            totalV = calcEntireV(root, nextOfParallel, ref nextOfParallel, ref success, visit);

            result = node.GetV() + maxV + totalV;
            return result;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            node.SetVisit(visit);

            result = node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), ref nextOfParallel, ref success, visit);
            return result;
        }


    }

    // 병렬 내부의 직렬(가지)의 저항의 합을 계산 (순환호출)
    private double calcRSerialInParallel(ComponentClass node)
    {
        if (node.plus.GetIsEndOfParallel())
        {
            return node.GetR();
        }
        return node.GetR() + calcRSerialInParallel(node.plus.links[0].GetComponent());
    }
    // 회로의 각 부품의 전압, 전류를 구하는 함수 (순환 호출로 순회)
    // sumOfParallelR 이 0이 아니면 병렬 내부를 방문중임을 암시, sumOfSerialR 이 0이 아니면 병렬의 가지 중 시작점이 아닌 곳을 방문중임을 암시
    public void calcComponent(ComponentClass root, ComponentClass node, double entireR,  double entireV, ref bool success, bool visit, double sumOfParallelR=0, double sumOfSerialR = 0)
    {
        //Debug.Log(node.transform.name);
        if (success == false)
        {
            return;
        }
        // 회로가 중간에 끊겨있는 경우
        if (node.plus.links.Count <= 0)
        {
            success = false;
            return;
        }

        // 순회를 모두 성공적으로 마쳤을 때
        if (node == root && node.GetVisit() == visit)
        {
            return;
        }
        // 지금 부품이 이미 방문한 부품일 때
        if (node.GetVisit() == visit)
        {
            return;
        }
        // 지금 부품이 병렬의 끝일 때
        else if (node.plus.GetIsEndOfParallel())
        {
            node.SetVisit(visit);
            double I = entireV / entireR;
            node.SetV(I * node.GetR());
            node.SetI(node.GetV() / node.GetR());
            calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit);

            return;
        }
        // 지금 부품이 병렬의 시작일 때
        if (node.plus.GetIsStartOfParallel())
        {
            node.SetVisit(visit);

            // 현재 병렬 연결의 전체 저항을 찾기
            for(int i=0; i < parallelRs.Count; i++)
            {
                if (parallelRs[i].start == node)
                {
                    //sumOfParallelR 이 0 이 아니라면 병렬 내부임을 나타냄 -> 다음 부품에 적용
                    sumOfParallelR = parallelRs[i].R;
                }
            }
            double I = entireV / sumOfParallelR;

            for (int i = 0; i < node.plus.links.Count; i++)
            {
                node.SetV(I * node.GetR());
                node.SetI(node.GetV() / node.GetR());
                calcComponent(root, node.plus.links[i].GetComponent(), entireR, entireV, ref success, visit, sumOfParallelR, sumOfSerialR);
            }
            return;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            node.SetVisit(visit);
            double I = 0;

            // 병렬 내부의 직렬 중 (가지 중) 시작점인 부품
            if (sumOfParallelR != 0 && sumOfSerialR == 0)
            {
                double tmpV = 0;
                double tmpI = 0;
                double tmpR = 0;

                I = entireV / entireR;
                
                tmpV = I * sumOfParallelR;
                tmpR = calcRSerialInParallel(node);
                //sumOfSerialR 이 0이 아니라면 병렬의 직렬 (가지) 중 첫번째 부품이 아니란 것을 의미 -> 다음 부품에 적용
                sumOfSerialR = tmpR;
                tmpI = tmpV / tmpR;
                node.SetI(tmpI);
                node.SetV(tmpI * node.GetR());
                calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit, sumOfParallelR, sumOfSerialR);
            }
            // 병렬 내부의 직렬 중 시작 점이 아닌 부품
            else if (sumOfParallelR != 0 && sumOfSerialR != 0)
            {
                double tmpV = 0;
                double tmpI = 0;
                double tmpR = 0;

                I = entireV / entireR;

                tmpV = I * sumOfParallelR;
                tmpR = sumOfSerialR;
                tmpI = tmpV / tmpR;
                node.SetI(tmpI);
                node.SetV(tmpI * node.GetR());
                calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit, sumOfParallelR, sumOfSerialR);
            }
            // 일반적인 직렬
            else
            {
                I = entireV / entireR;
                node.SetV(I * node.GetR());
                node.SetI(node.GetV() / node.GetR());
                calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit, sumOfParallelR, sumOfSerialR);
            }
            
            return;
        }
    }
    public bool circuit(ComponentClass root)
    {
        bool success = true;
        List<double> listR = new List<double>();
        ComponentClass next = null;
        List<double> listV = new List<double>();
        double r = 0;
        double v = 0;
        double i = 0;
        // 방문을 했는지 확인하기 위한 값 : true, false 로 고정되어 있다면 순회마다 매번 초기화 시켜줘야함
        bool visit = !root.GetVisit();
        r = calcEntireR(root, root, ref next, ref success, visit);
        
        if (success)
        {
            Debug.Log("전체 저항 : " + r);
        }
        else
        {
            Debug.Log("CircuitManager Error : 저항 구하기 실패");
            return false;
        }
        
        visit = !root.GetVisit();
        success = true;
        next = null;
        v = calcEntireV(root, root, ref next, ref success, visit);
        if (success)
        {
            Debug.Log("전체 전압 : " + v);
        }
        else
        {
            Debug.Log("CircuitManager Error : 전압 구하기 실패");
            return false;
        }
        i = v / r;
        Debug.Log("전체 전류 : " + i);

        visit = !root.GetVisit();
        calcComponent(root, root, r, v, ref success, visit);
        parallelRs.Clear();
        return success;

    }

    // 순회 할 시작점을 만들어줌 (회로에는 보이지 않으나 코드상에 논리적으로 존재)
    ComponentClass createStartingComponent(ComponentClass parallel, ComponentClass next)
    {
        Debug.Log("createStartingComponent 시작");
        ComponentClass start = Instantiate(startComponent).GetComponent<ComponentClass>();
        start.transform.name = "start";
        ComponentClass receive = Instantiate(startComponent).GetComponent<ComponentClass>();
        receive.transform.name = "receive";
        ComponentClass give = Instantiate(startComponent).GetComponent<ComponentClass>();
        give.transform.name = "give";

        start.minus.links.Add(receive.plus);
        start.plus.links.Add(give.minus);

        receive.minus.links.Add(parallel.plus);
        give.plus.links.Add(next.minus);

        Debug.Log("parallel : "+parallel.transform.parent.transform.name);
        Debug.Log("next : " + next.transform.parent.transform.name);

        return start;

    }
    // 순회를 시작할 지점을 자동으로 찾아줌 (마땅한게 없으면 자체적으로 생성)
    ComponentClass findStartingPoint(ComponentClass root)
    {
        ComponentClass result = root;
        Debug.Log("findStartingPoint 시작");
        ComponentClass parallel = root.plus.links[0].GetComponent();
        Stack<PlusAttachment> parallelComponent = new Stack<PlusAttachment>();
        bool stackUse = false;
        while (parallel != root && parallel != null)
        {
            
            if (parallel.plus.GetIsEndOfParallel())
            {
                if(parallelComponent.Count > 0)
                {
                    parallelComponent.Pop();
                    stackUse = true;
                }
            }
            else if (parallel.plus.GetIsStartOfParallel())
            {
                parallelComponent.Push(parallel.plus);
                stackUse = true;
            }
            if(stackUse && parallelComponent.Count <= 0) {
                result = createStartingComponent(parallel, parallel.plus.links[0].GetComponent());
                break;
            }
            parallel = parallel.plus.links[0].GetComponent();
            
        }
        
        return result;
    }
    void calc()
    {
        Debug.Log("calc 시작");
        ComponentClass start = findStartingPoint(root);
        circuit(start);
    }
    void Start()
    {
        //circuit(root);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 1.0 && first == false)
        {
            first = true;
            calc();
            //circuit(root);
        }
        
    }
    //회로의 전체 전압 구하는 함수 (24.04.29 기준 안씀)
    // 첫번째 매개변수 : 순회를 시작한 부품
    // 두번째 매개변수 : 현재 순회중인 부품
    // 세번째 매개변수 : 병렬 이후의 전압값들
    // 네번째 매개변수 : 순회의 성공 여부
    // 다섯번째 매개변수 : 현재 부품이 방문했으면 이 매개변수의 값을 visit 으로 가지고 있어야함 (방문 여부가 아님)
    /* public double calcEntireV(ComponentClass root, ComponentClass node, List<double> nextOfParallelV, ref bool success, bool visit)
     {
         //Debug.Log(node.transform.name);
         if (success == false)
         {
             return 0;
         }
         // 회로가 중간에 끊겨있는 경우
         if (node.plus.links.Count <= 0)
         {
             success = false;
             return 0;
         }

         // 순회를 모두 성공적으로 마쳤을 때
         if (node == root && node.GetVisit() == visit)
         {
             return 0;
         }
         double result = 0;
         // 지금 부품이 이미 방문한 부품일 때
         if (node.GetVisit() == visit) 
         { 
             return 0;
         }
         // 지금 부품이 병렬의 끝일 때
         else if (node.plus.GetIsEndOfParallel())
         {  
             // 병렬 이후의 값을 구함 -> 따로 저장하는 이유는 병렬의 전압값에 직렬 전압 값이 섞이면 안되기 때문 
             nextOfParallelV.Add(calcEntireV(root, node.plus.GetEndOfParallelLink().GetComponent(), nextOfParallelV, ref success, visit));
             node.SetVisit(visit);
             // 자신의 값을 리턴
             result = node.GetV();
             return result;
         }
         // 지금 부품이 병렬의 시작일 때
         if (node.plus.GetIsStartOfParallel())
         {
             // 모든 병렬 연결의 가장 큰 전압을 저장
             double maxV = 0;

             for (int i = 0; i < node.plus.links.Count; i++)
             {
                 double tmp = calcEntireV(root, node.plus.links[i].GetComponent(), nextOfParallelV, ref success, visit);
                 if(tmp > maxV)
                 {
                     maxV = tmp;
                 }
             }
             // 병렬 연결 이후의 값을 전부 저장
             double totalV = 0;

             // 병렬 연결 이후의 값을 전부 더함
             for (int i = 0; i < nextOfParallelV.Count; i++)
             {
                 totalV += nextOfParallelV[i];
             }
             nextOfParallelV.Clear();
             node.SetVisit(visit);
             result = node.GetV() + totalV+maxV;
             return result;
         }
         // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
         else
         {         
             node.SetVisit(visit);

             result = node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), nextOfParallelV, ref success, visit);
             return result;
         }


     }*/
}
