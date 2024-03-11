using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 실린더에 들어가는 스크립트 (그래프의 간선을 의미)
public class GEdgeClass : MonoBehaviour
{
    public GCubeClass link; // 이 간선에 연결된 다른 블록
    public GameObject cylinder; // 실린더
    public GameObject pos; // link 가 붙을 때 이동되어야하는 위치 
    public GCubeClass block; // 이 실린더를 갖는 블록의 Attach
    public bool hasLink = false; // 이미 연결된 블록이 있는지 없는지를 나타내는 변수
}
