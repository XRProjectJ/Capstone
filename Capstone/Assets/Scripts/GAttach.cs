using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ϳ����� ������ �����ϴ� ��ũ��Ʈ : �浹�� ���� -> ��� ������Ʈ�� �־����
public class GAttach : MonoBehaviour
{
    public List<GEdgeClass> current; // ���� ������ �Ǹ���
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
        // �Ǹ��� �ε��� �Ǹ��� ������ ���� ��ġ�� �ٸ��� ����� -> ��ճ���
        Vector3 goal = new Vector3(0.0f, 0.0f, 0.0f); // ����� �̵��Ǿ���� ��ġ
        for (int i = 0; i < current.Count; i++)
        {
            goal += current[i].pos.transform.position;
        }
        goal /= current.Count;

        //Empty �� ���� ����� �κ� ����

        // �����鼭 �̵��� �ǹǷ� �� ��ϰ� ����� ��ϵ鵵 �� ���� �̵��ؾ���
        // -> �� ����� firstMove ��Ȱ��ȭ �� �ٸ� ����� firstMove ��Ȱ��ȭ
        // -> �ȱ׷��� Update �� �ִ� �Ϳ����� setPosition.bfs() �� ȣ��
        // firstMove �� Ȱ��ȭ�ϰ� Update �� �ִ� setPosition.bfs() �� ����ϸ� ����ε� ��ġ�� �̵������� ���� (������ ��)
        block.firstMove = false;
        setFirstMove.bfs(block, null);
        Vector3 prevPos = block.transform.position;
        // �̵��� ���� �� �� ��� + ����� ��ϵ��� �׸�ŭ �̵���Ŵ 
        //Vector3 offset = goal - block.transform.position;
        block.mBlock.transform.position = goal;
        Vector3 offset = block.transform.position - prevPos;
        setPosition.bfs(block, offset);


        // block.firstMove = true;

        for (int i = 0; i < current.Count; i++)
        {
            // �׷������� �����ϴ� ����
            current[i].link = block;
            block.inner.Add(current[i]);
        }
        // ������ ������ ����� �͵� �� �ϳ��� �����ϰ�� ���� firstMove �� ��Ȱ��ȭ
        setFirstMove.bfs(block, null);
        // current ����
        current.Clear();

    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<GEdgeClass>())
        {
            if (!other.gameObject.GetComponent<GEdgeClass>().hasLink)
            {
                // ������ �ߺ����� �ʵ��� �ϴ� �κ�
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
