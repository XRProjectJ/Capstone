using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 블록끼리의 연결을 수행하는 스크립트 : 충돌시 붙음 -> 블록 오브젝트에 있어야함
public class GAttach : MonoBehaviour
{
    public List<GEdgeClass> current; // 새로 연결한 실린더
    public GCubeClass block;

    private GSetFirstMove setFirstMove;
    private GSetPosition setPosition;

    // Start is called before the first frame update
    void Start()
    {
        block = GetComponent<GCubeClass>();
        setFirstMove = GetComponent<GSetFirstMove>();
        setPosition = GetComponent<GSetPosition>();
    }

    private void Attach()
    {
        Debug.Log("Attach()");
        // 실린더 부딪힌 실린더 개수에 따라 위치를 다르게 줘야함 -> 평균내기
        Vector3 goal = new Vector3(0.0f, 0.0f, 0.0f); // 블록이 이동되어야할 위치
        for (int i = 0; i < current.Count; i++)
        {
            goal += current[i].pos.transform.position;
        }
        goal /= current.Count;

        //Empty 로 인해 변경된 부분 시작

        // 붙으면서 이동이 되므로 이 블록과 연결된 블록들도 다 같이 이동해야함
        // -> 이 블록의 firstMove 비활성화 및 다른 블록의 firstMove 비활성화
        // -> 안그러면 Update 에 있는 것에서도 setPosition.bfs() 를 호출
        // firstMove 를 활성화하고 Update 에 있는 setPosition.bfs() 를 사용하면 제대로된 위치로 이동하지를 않음 (이유는 모름)
        block.firstMove = false;
        setFirstMove.bfs(block, null);
        Vector3 prevPos = block.transform.position;
        // 이동량 측정 후 이 블록 + 연결된 블록들을 그만큼 이동시킴 
        //Vector3 offset = goal - block.transform.position;
        block.mBlock.transform.position = goal;
        Vector3 offset = block.transform.position - prevPos;
        setPosition.bfs(block, offset);


        // block.firstMove = true;

        for (int i = 0; i < current.Count; i++)
        {
            // 그래프끼리 연결하는 과정
            current[i].link = block;
            block.inner.Add(current[i]);
        }
        // 연결이 됐으면 연결된 것들 중 하나를 제외하고는 전부 firstMove 를 비활성화
        setFirstMove.bfs(block, null);
        // current 비우기
        current.Clear();

    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<GEdgeClass>())
        {
            if (!other.gameObject.GetComponent<GEdgeClass>().hasLink)
            {
                // 연결이 중복되지 않도록 하는 부분
                other.gameObject.GetComponent<GEdgeClass>().hasLink = true;
                current.Add(other.gameObject.GetComponent<GEdgeClass>());
            }

        }
    }
    // Update is called once per frame
    void Update()
    {
        if (current.Count != 0)
        {
            Attach();
        }
    }
}
