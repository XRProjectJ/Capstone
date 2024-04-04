using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 순회를 담당하는 스크립트
// (24.04.04 기준 : 전지가 병렬 연결일 때 제외하고 저항 구해짐, 테스트를 더 해봐야 알겠지만 전체 전압은 잘 구해지는듯)

public class CircuitManager : MonoBehaviour
{
    // 병렬과 직렬 부분을 나누어서 각 저항,전압,전류를 저장하고 이를 다 더해서 회로의 전체 저항, 전압, 전류를 구하려고함
    // (24.04.01 기준 사용안함)
    private List<double> R = new List<double>();
    private List<double> V = new List<double>();
    private List<double> I = new List<double>();
    double timer = 0;
    bool first = false;

    [SerializeField] private ComponentClass root;

    // 회로의 전체 저항을 구하는 함수 (순환 호출로 순회)
    public double calcEntireR(ComponentClass root, ComponentClass node, List<double> nextOfParallelR, ref bool success, bool visit)
    {
        Debug.Log(node.transform.name);
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
            return node.GetR();
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
            // 병렬 연결 이후의 값을 전부 더함
            for (int i = 0; i < nextOfParallelR.Count; i++)
            {
                totalR += nextOfParallelR[i];
            }
            nextOfParallelR.Clear();
            
            return node.GetR() + 1 / sumOfParallelR+ totalR;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            //Debug.Log("직렬");
            node.SetVisit(visit);
            //Debug.Log("로그: "+node.plus.links[0]);
            return node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), nextOfParallelR, ref success, visit);
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
            // 병렬 이후의 값을 구함 -> 따로 저장하는 이유는 병렬의 저항값에 직렬 저항 값이 섞이면 안되기 때문 
            nextOfParallelV.Add(calcEntireV(root, node.plus.GetEndOfParallelLink().GetComponent(), nextOfParallelV, ref success, visit));
            node.SetVisit(visit);
            // 자신의 값을 리턴
            return node.GetV();
        }
        // 지금 부품이 병렬의 시작일 때
        if (node.plus.GetIsStartOfParallel())
        {
            //Debug.Log("병렬 시작");
            // 병렬 연결 이후의 값을 전부 저장
            double totalV = 0;

            // 병렬 연결 이후의 값을 전부 더함
            for (int i = 0; i < nextOfParallelV.Count; i++)
            {
                totalV += nextOfParallelV[i];
            }
            nextOfParallelV.Clear();
            node.SetVisit(visit);
            return node.GetV()+totalV;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            //Debug.Log("직렬");
            node.SetVisit(visit);

            //Debug.Log("로그: "+node.plus.links[0]);
            return node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), nextOfParallelV, ref success, visit);
        }


    }
    public bool circuit(ComponentClass root)
    {
        bool success = true;
        List<double> listR = new List<double>();
        List<double> listV = new List<double>();
        // 방문을 했는지 확인하기 위한 값 : true, false 로 고정되어 있다면 순회마다 매번 초기화 시켜줘야함
        bool visit = !root.GetVisit();
        double r = calcEntireR(root, root,listR, ref success, visit);
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
        double v = calcEntireV(root, root, listV, ref success, visit);
        if (success)
        {
            Debug.Log("전체 전압 : " + v);
        }
        else
        {
            Debug.Log("CircuitManager Error : 전압 구하기 실패");
            return false;
        }
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
}
