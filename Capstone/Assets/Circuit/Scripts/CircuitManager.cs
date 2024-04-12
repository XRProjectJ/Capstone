using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 순회를 담당하는 스크립트

public class CircuitManager : MonoBehaviour
{
    // 병렬과 직렬 부분을 나누어서 각 저항,전압,전류를 저장하고 이를 다 더해서 회로의 전체 저항, 전압, 전류를 구하려고함
    // (24.04.01 기준 사용안함)
    /*    private List<double> R = new List<double>();
        private List<double> V = new List<double>();
        private List<double> I = new List<double>();*/

    struct ParallelR
    {
        public double R;
        public ComponentClass start;
    };
    private List<ParallelR> parallelRs = new List<ParallelR>();
    double timer = 0;
    bool first = false;

    [SerializeField] private ComponentClass root;

    // 회로의 전체 저항을 구하는 함수 (순환 호출로 순회)
    public double calcEntireR(ComponentClass root, ComponentClass node, List<double> nextOfParallelR, ref bool success, bool visit)
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
        if (node == root && node.GetVisit() == visit) {
            //success = true;
            return 0;
        }
        double result = 0;
        // 지금 부품이 이미 방문한 부품일 때
        if(node.GetVisit() == visit)
        {
            //Debug.Log("이미 방문");
            return 0;
        }
        // 지금 부품이 병렬의 끝일 때
        else if (node.plus.GetIsEndOfParallel())
        {
            //Debug.Log("병렬 끝");
            node.SetVisit(visit);
            // 병렬 이후의 값을 구함 -> 따로 저장하는 이유는 병렬의 저항값에 직렬 저항 값이 섞이면 안되기 때문 
            nextOfParallelR.Add(calcEntireR(root, node.plus.GetEndOfParallelLink().GetComponent(), nextOfParallelR, ref success, visit));

            // 자신의 값을 리턴
            result = node.GetR();
            /*Debug.Log(node.transform.name);
            Debug.Log("이제까지 구한 저항 : " + result);*/
            return result;
            //return node.GetR();
        }
        // 지금 부품이 병렬의 시작일 때
        if (node.plus.GetIsStartOfParallel())
        {
            //Debug.Log("병렬 시작");
            node.SetVisit(visit);
            // 모든 병렬 연결의 저항을 저장
            double sumOfParallelR = 0;
            // 병렬 연결 이후의 값을 전부 저장
            double totalR = 0;
            for (int i = 0; i < node.plus.links.Count; i++)
            {
                sumOfParallelR += 1 / calcEntireR(root, node.plus.links[i].GetComponent(), nextOfParallelR, ref success, visit);
            }
            // 각 부품의 전압, 전류를 구할 때 사용
            ParallelR tmp = new ParallelR();
            tmp.R = 1/sumOfParallelR;
            tmp.start = node;
            parallelRs.Add(tmp);
/*            Debug.Log("병렬 연결 저항의 합 : " + 1/sumOfParallelR);*/
            // 병렬 연결 이후의 값을 전부 더함
            for (int i = 0; i < nextOfParallelR.Count; i++)
            {
                totalR += nextOfParallelR[i];
            }
            nextOfParallelR.Clear();

            result = node.GetR() + 1 / sumOfParallelR + totalR;
            //Debug.Log(node.transform.name);
            //Debug.Log("이제까지 구한 저항 : " + result);
            return result;
            //return node.GetR() + 1 / sumOfParallelR+ totalR;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            //Debug.Log("직렬");
            node.SetVisit(visit);
            //Debug.Log("로그: "+node.plus.links[0]);

            result = node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), nextOfParallelR, ref success, visit);
            //Debug.Log(node.transform.name);
            //Debug.Log("이제까지 구한 저항 : " + result);
            return result;
            //return node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), nextOfParallelR, ref success, visit);
        }
        

    }

    //회로의 전체 전압 구하는 함수
    public double calcEntireV(ComponentClass root, ComponentClass node, List<double> nextOfParallelV, ref bool success, bool visit)
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
            //success = true;
            return 0;
        }
        double result = 0;
        // 지금 부품이 이미 방문한 부품일 때
        if (node.GetVisit() == visit)
        {
            /*Debug.Log("이미 방문");*/
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
            /*Debug.Log("병렬 끝");
            Debug.Log(node.transform.name);
            Debug.Log("이제까지 구한 전압 : " + result);*/
            return result;
            //return node.GetV();
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
            /*Debug.Log("병렬 시작");
            Debug.Log(node.transform.name);
            Debug.Log("이제까지 구한 전압 : " + result);*/
            return result;
            //return node.GetV() + totalV+maxV;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            
            node.SetVisit(visit);

            //Debug.Log("로그: "+node.plus.links[0]);

            result = node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), nextOfParallelV, ref success, visit);
            /*Debug.Log("직렬");
            Debug.Log(node.transform.name);
            Debug.Log("이제까지 구한 전압 : " + result);*/
            return result;
            //return node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), nextOfParallelV, ref success, visit);
        }


    }
    // 병렬 내부의 직렬(가지)의 저항의 합을 계산
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
            //success = true;
            return;
        }
        double result = 0;
        // 지금 부품이 이미 방문한 부품일 때
        if (node.GetVisit() == visit)
        {
            //Debug.Log("이미 방문");
            return;
        }
        // 지금 부품이 병렬의 끝일 때
        else if (node.plus.GetIsEndOfParallel())
        {
            //Debug.Log("병렬 끝");
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
            //Debug.Log("병렬 시작");
            node.SetVisit(visit);

            // 현재 병렬 연결의 전체 저항을 찾기
            for(int i=0; i < parallelRs.Count; i++)
            {
                if (parallelRs[i].start == node)
                {
                    //sumOfParallelR 이 0 이 아니라면 병렬 내부임을 나타냄 -> 다음 부품에 적용
                    sumOfParallelR = parallelRs[i].R;
                    Debug.Log("병렬 연결의 저항 합: " + sumOfParallelR);
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
            //Debug.Log("직렬");
            node.SetVisit(visit);
            //Debug.Log("로그: "+node.plus.links[0]);
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
        //r = calcEntireR(root, root,listR, ref success, visit);
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
        v = calcEntireV(root, root, listV, ref success, visit);
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
            circuit(root);
            
        }
        
    }

    // 회로의 전체 저항을 구하는 함수 (순환 호출로 순회)
    public double calcEntireR(ComponentClass root, ComponentClass node, ref ComponentClass nextOfParallel, ref bool success, bool visit)
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
            //success = true;
            return 0;
        }
        double result = 0;
        // 지금 부품이 이미 방문한 부품일 때
        if (node.GetVisit() == visit)
        {
            //Debug.Log("이미 방문");
            return 0;
        }
        // 지금 부품이 병렬의 끝일 때
        else if (node.plus.GetIsEndOfParallel())
        {
            //Debug.Log("병렬 끝");
            node.SetVisit(visit);
            
            nextOfParallel = node.plus.links[0].GetComponent();
            Debug.Log("병렬의 끝일 때 다음 부품 : " + nextOfParallel);
            // 자신의 값을 리턴
            result = node.GetR();
            Debug.Log(node.transform.name);
            //Debug.Log("이제까지 구한 저항 : " + result);
            return result;
            //return node.GetR();
        }
        // 지금 부품이 병렬의 시작일 때
        if (node.plus.GetIsStartOfParallel())
        {
            //Debug.Log("병렬 시작");
            node.SetVisit(visit);
            // 모든 병렬 연결의 저항을 저장
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
            Debug.Log("병렬 시작일 때 병렬의 다음 부품 : "+nextOfParallel);
            totalR = calcEntireR(root, nextOfParallel, ref nextOfParallel, ref success, visit);
            result = node.GetR() + 1 / sumOfParallelR + totalR;
            Debug.Log(node.transform.name);
            return result;
            //return node.GetR() + 1 / sumOfParallelR+ totalR;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            //Debug.Log("직렬");
            node.SetVisit(visit);
            //Debug.Log("로그: "+node.plus.links[0]);

            result = node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), ref nextOfParallel, ref success, visit);
            Debug.Log(node.transform.name);
            //Debug.Log("이제까지 구한 저항 : " + result);
            return result;
            //return node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), nextOfParallelR, ref success, visit);
        }


    }
}
