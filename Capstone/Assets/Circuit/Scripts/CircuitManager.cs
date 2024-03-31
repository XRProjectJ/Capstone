using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 순회를 담당하는 스크립트
public class CircuitManager : MonoBehaviour
{
    // 병렬과 직렬 부분을 나누어서 각 저항,전압,전류를 저장하고 이를 다 더해서 회로의 전체 저항, 전압, 전류를 구하려고함
    // (24.04.01 기준 사용안함)
    private List<double> R = new List<double>();
    private List<double> V = new List<double>();
    private List<double> I = new List<double>();

    // 회로의 전체 저항을 구하는 함수 (순환 호출로 순회)
    public double calcR(ComponentClass root, ComponentClass node, List<double> nextOfParallelR, ref bool success)
    {
        // 회로의 끝에 도달했는데 null -> 회로에 문제가 있음
        if (node == null)
        {
            success = false;
            return 0;
        }
        // 방문을 했는지 확인하기 위한 값 : true, false 로 고정되어 있다면 순회마다 매번 초기화 시켜줘야함
        bool visit = !root.GetVisit();
        // 순회를 모두 성공적으로 마쳤을 때
        if (node == root && node.GetVisit() == visit) {
            success = true;
            return 0;
        }
        // 지금 부품이 병렬의 끝일 때
        if (node.plus.GetIsEndOfParallel() && node.GetVisit() != visit)
        {
            // 병렬 이후의 값을 구함 -> 따로 저장하는 이유는 병렬의 저항값에 직렬 저항 값이 섞이면 안되기 때문 
            nextOfParallelR.Add(calcR(root, node.plus.GetEndOfParallelLink().GetComponent(), nextOfParallelR, ref success));
            node.SetVisit(visit);
            // 자신의 값을 리턴
            return node.GetR();
        }
        // 지금 부품이 병렬의 시작일 때
        if (node.plus.GetIsStartOfParallel() && node.GetVisit() != visit)
        {
            // 모든 병렬 연결의 저항을 저장
            double sumOfParallelR = 0;
            // 병렬 연결 이후의 값과 병렬 연결의 값을 전부 저장
            double totalR = 0;
            for (int i = 0; i < node.plus.links.Count; i++)
            {
                sumOfParallelR += 1 / calcR(root, node.plus.links[i].GetComponent(), nextOfParallelR, ref success);
            }
            // 병렬 연결 이후의 값을 전부 더함
            for (int i = 0; i < nextOfParallelR.Count; i++)
            {
                totalR += nextOfParallelR[i];
            }
            nextOfParallelR.Clear();
            node.SetVisit(visit);
            return node.GetR() + 1 / sumOfParallelR+ totalR;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else if(node.GetVisit() != visit)
        {
            node.SetVisit(visit);
            return node.GetR() + calcR(root, node.plus.links[0].GetComponent(), nextOfParallelR, ref success);
        }
        // 지금 부품이 이미 방문한 부품일 때
        else 
        {
            return 0;
        }

    }
}
