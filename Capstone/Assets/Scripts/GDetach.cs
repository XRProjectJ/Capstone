using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����� ���� ��ũ��Ʈ : 1�� ���ȿ� ���������� Ŭ���� �� ���� ����� ������ ������
// -> A ��� ������ 1�� �ȿ� B ��� ������ A,B ����� ������ ��ü��

// ��Ͽ� ���� ���� ���� �ƴ϶� ���ӻ� �ϳ��� ������� -> ����� ī�޶� ������
public class GDetach : MonoBehaviour
{
    // ������ �ε���
    private int curIndex = 0;
    private float  timer = 0;
    // �ش� ����� ���� �پ�����, �Ʒ��� �پ�����, ���� �ʾҴ��� �� ���� ���¸� ��Ÿ��
    public enum STATUS {INNER_LINK, OUTER_LINK, NO_LINK};
    // �ѹ� �� �� ����� �Ǵ� ����� 2����
    public GCubeClass[] removeCandidate = new GCubeClass[2];
    private GSetPosition setPosition;

    private void Start()
    {
        setPosition = GetComponent<GSetPosition>();
    }
    // removeCandidate �� �� �� �������� Ȯ���ϴ� �Լ�
    public bool isFull()
    {
        if(removeCandidate[0] != null && removeCandidate[1] != null)
        {
            return true;
        }
        return false;
    }
    // removeCandidate �� ����� �߰��ϴ� �Լ�
    public void add(GCubeClass item)
    {
        removeCandidate[curIndex] = item;
        curIndex = (curIndex + 1) % 2;
    }
    // ����� ���� �Լ� : �Ű����� up �� ���� �ִ� ���, down �� �Ʒ��� �ִ� ���
    public bool detach(GCubeClass up, GCubeClass down)
    {
        bool isLink = false;
        // ����Ǿ����� ��Ÿ���� List �� ������ �ϴ� ����� �ε����� ���� 
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
    // removeCandidate �� �ʱ�ȭ�ϴ� �Լ� (�����ð����� ȣ���� ����)
    private void candidateReset()
    {
        removeCandidate[0] = null;
        removeCandidate[1] = null;
    }
    // ���õ� �� ����� ��� ���� �Ǿ����� Ȯ��
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
