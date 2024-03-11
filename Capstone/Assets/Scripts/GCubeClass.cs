using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach 블록에 들어가는 스크립트 (그래프의 노드를 의미)
public class GCubeClass : MonoBehaviour
{
    public List<GEdgeClass> outer; // 블록이 다른 블록과 연결 할 수 있도록하는 실린더
    public List<GEdgeClass> inner; // 블록과 연결된 다른 블록의 실린더
    public GameObject aBlock; // Attach 블록 (실린더랑 연결되는 블록의 아래부분)
    public GameObject mBlock; // MainBlock - 실린더+큐브+AttachBlock 을 합친것
    public List<GEdgeClass> current; // 새로 연결한 실린더

    // 처음으로 움직이는 블록인지 -> firstMove 가 true 인 것만 연결된 블록들을 따라움직이게 함
    // 그렇지않으면 A-B 블록이 연결된 상태에서 A 가 움직여서 B 도 이동시키면 B 가 움직였으니 다시 A 가 움직이는 일을 반복하여
    // 이상한 곳으로 날아가버림
    public bool firstMove = false;
    // 블록 덩어리에서 선택된 블록과 그렇지 않은 블록들의 거리를 저장
    public Vector3 distance = new Vector3(0.0f, 0.0f, 0.0f);
    // BFS 에서 해당 블록을 이미 한번 방문했었는지 체크하는 변수
    public bool visit = false;

    private Vector3 prevPos = new Vector3(0.0f, 0.0f, 0.0f); // 이동량을 계산하기 위해 이전의 위치를 저장
    private Vector3 prevRot = new Vector3(0.0f, 0.0f, 0.0f); // 회전량을 계산하기 위해 이전의 회전각을 저장
    private Quaternion prevRotQua = Quaternion.identity;

    private GSetPosition setPosition; // BFS 템플릿을 사용하기 위한 변수
    private GRotationProcess rotationProcess; // BFS 템플릿을 사용하기 위한 변수
    private GSetChangeTransform setChangeTransform; // BFS 템플릿을 사용하기 위한 변수
    private GSetRotation setRotation;

    private void Start()
    {
        // 게임이 시작될때  BFS 템플릿을 사용하기 위한 변수를 초기화
        setPosition = GetComponent<GSetPosition>();
        rotationProcess = GetComponent<GRotationProcess>();
        setChangeTransform = GetComponent<GSetChangeTransform>();
        setRotation = GetComponent<GSetRotation>();
        // 그냥 시작하면 hasChanged 가 true 로 바뀜 -> (0,0,0) 을 벗어나서 그런 것 같음 (정확한 이유 X)
        this.mBlock.transform.hasChanged = false;
    }
    public Vector3 getPrevRot()
    {
        return prevRot;
    }
    public Vector3 getPrevPos()
    {
        return prevPos;
    }
    public Quaternion getPrevRotQua()
    {
        return prevRotQua;
    }
    private void Update()
    {
        // 블록이 이동을 했으면서 firstMove 가 활성화 되어 있는 경우에 연결된 블록들을 그만큼 똑같이 이동시킴
        if (this.mBlock.transform.hasChanged && this.firstMove == true)
        {
            //Debug.Log(mBlock.transform.name);
            // 이동량 구하고 연결된 블록을 그만큼 이동시키기
            Vector3 moveOffset = this.mBlock.transform.position - prevPos;
            setPosition.bfs(this, moveOffset);

            // 회전량 구하고 연결된 블록을 그만큼 회전시키기
            /*Vector3 currentRot = Quaternion.ToEulerAngles(this.mBlock.transform.rotation);
            //Vector3 currentRot = Quaternion.ToEulerAngles(this.mBlock.transform.localRotation);
            Vector3 rotateOffset = currentRot - prevRot;
            if (rotateOffset != new Vector3(0.0f, 0.0f, 0.0f))
            {
                rotationProcess.bfs(this, rotateOffset);
            }*/
            Vector3 currentRot = Quaternion.ToEulerAngles(this.mBlock.transform.rotation);
            //Vector3 currentRot = Quaternion.ToEulerAngles(this.mBlock.transform.localRotation);
            Vector3 rotateOffset = currentRot - prevRot;
            if (rotateOffset != new Vector3(0.0f, 0.0f, 0.0f))
            {
                setRotation.bfs(this, this);
            }

            // 이 블록 + 연결된 블록의 hasChanged 비활성화
            this.mBlock.transform.hasChanged = false;
            setChangeTransform.bfs(this, null);
        }
        prevPos = this.mBlock.transform.position;
        prevRot = Quaternion.ToEulerAngles(this.mBlock.transform.rotation);
        prevRotQua = this.mBlock.transform.rotation;
        /*prevPos = this.mBlock.transform.localPosition;
        prevRot = Quaternion.ToEulerAngles(this.mBlock.transform.localRotation);*/
    }

}