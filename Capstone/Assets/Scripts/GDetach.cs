using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 블록을 떼는 스크립트 : 1초 동안에 연속적으로 클릭된 두 개의 블록의 연결을 제거함
// -> A 블록 누르고 1초 안에 B 블록 누르면 A,B 블록의 연결을 해체함

// 블록에 각각 들어가는 것이 아니라 게임상 하나만 있으면됨 -> 현재는 카메라에 들어가있음
public class GDetach : MonoBehaviour
{
    // 현재의 인덱스
    private int curIndex = 0;
    private float  timer = 0;
    // 해당 블록이 위에 붙었는지, 아래에 붙었는지, 붙지 않았는지 에 대한 상태를 나타냄
    public enum STATUS {INNER_LINK, OUTER_LINK, NO_LINK};
    // 한번 뗄 때 대상이 되는 블록은 2개임
    public GCubeClass[] removeCandidate = new GCubeClass[2];
    private GSetPosition setPosition;

    private void Start()
    {
        setPosition = GetComponent<GSetPosition>();
    }
    // removeCandidate 가 꽉 찬 상태인지 확인하는 함수
    public bool isFull()
    {
        if(removeCandidate[0] != null && removeCandidate[1] != null)
        {
            return true;
        }
        return false;
    }
    // removeCandidate 에 블록을 추가하는 함수
    public void add(GCubeClass item)
    {
        removeCandidate[curIndex] = item;
        curIndex = (curIndex + 1) % 2;
    }
    // 블록을 떼는 함수 : 매개변수 up 은 위에 있는 블록, down 은 아래에 있는 블록
    public bool detach(GCubeClass up, GCubeClass down)
    {
        bool isLink = false;
        // 연결되었음을 나타내는 List 중 떼려고 하는 블록의 인덱스를 저장 
        List<int> removeIdx = new List<int>();
        for (int i=0; i < up.inner.Count; i++)
        {
            if(up.inner[i].block == down)
            {
                up.inner[i].link = null;
                up.inner[i].hasLink = false;
                removeIdx.Add(i);
                isLink = true;
            }
        }
        for(int i= 0; i < removeIdx.Count; i++)
        {
            up.inner.RemoveAt(removeIdx[i]-i);
        }
        removeIdx.Clear();

        for (int i = 0; i < down.outer.Count; i++)
        {
            if (down.outer[i].link == up)
            {
                down.outer[i].link = null;
                down.outer[i].hasLink = false;
                removeIdx.Add(i);
                isLink = true;
            }
        }
        for (int i = 0; i < removeIdx.Count; i++)
        {
            down.outer.RemoveAt(removeIdx[i]-i);
        }
        if (isLink)
        {

            up.firstMove = false;
            Vector3 offset = new Vector3(1.0f, 1.0f, 1.0f);
            up.mBlock.transform.position += offset;

            down.firstMove = false;
            down.mBlock.transform.position -= offset;
            setPosition.bfs(up, offset);
            setPosition.bfs(down, -offset);
        }
        return isLink;
    }
    // removeCandidate 를 초기화하는 함수 (일정시간마다 호출할 예정)
    private void candidateReset()
    {
        removeCandidate[0] = null;
        removeCandidate[1] = null;
    }
    // 선택된 두 블록이 어떻게 연결 되었는지 확인
    public STATUS checkIsLinking()
    {
        for (int i=0; i < removeCandidate[0].inner.Count; i++) {
            if(removeCandidate[0].inner[i].block == removeCandidate[1])
            {
                Debug.Log("link status:inner_link");
                return STATUS.INNER_LINK;
            }
        }
        for(int i=0; i < removeCandidate[0].outer.Count; i++)
        {
            if (removeCandidate[0].outer[i].link == removeCandidate[1])
            {
                Debug.Log("link status:outer_link");
                return STATUS.OUTER_LINK;
            }
        }
        Debug.Log("link status:no_link");
        return STATUS.NO_LINK;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > 1.0f)
        {
            timer = 0.0f;
            candidateReset();
        }
        if (isFull())
        {
            STATUS status = checkIsLinking();
            if (status == STATUS.INNER_LINK)
            {
                detach(removeCandidate[0], removeCandidate[1]);
            }
            else if (status == STATUS.OUTER_LINK)
            {
                detach(removeCandidate[1], removeCandidate[0]);
            }
            candidateReset();
        }
    }
}
