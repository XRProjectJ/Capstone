using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS �� ���� ����ϴٺ��� template method �� �������
// BFS �� �ʿ��� ��� �� ��ũ��Ʈ�� ����� ���� bfsMainFunction �� ���ϴ� ����� ������ ��
// �� ��ũ��Ʈ�� BFS�� �Ѱ��ϴ� ��ũ��Ʈ -> �Ű������� ���� ���·� ���� �� �ֵ��� ������
public abstract class BfsTemplate<T> : MonoBehaviour
{
    public abstract void bfsMainFunction(GCubeClass obj, T parameter);
    public void bfs(GCubeClass start, T parameter)
    {
        Queue<GCubeClass> que = new Queue<GCubeClass>();
        GCubeClass cur;
        start.visit = true;
        que.Enqueue(start);
        while (que.Count != 0)
        {
            cur = que.Dequeue();
            if (cur != start)
            {
                bfsMainFunction(cur, parameter);
            }

            for (int i = 0; i < cur.outer.Count; i++)
            {
                if (cur.outer[i].link)
                {
                    if (cur.outer[i].link.visit == true)
                    {
                        continue;
                    }
                    cur.outer[i].link.visit = true;
                    que.Enqueue(cur.outer[i].link);
                }
            }
            for (int i = 0; i < cur.inner.Count; i++)
            {
                if (cur.inner[i].block)
                {
                    if (cur.inner[i].block.visit == true)
                    {
                        continue;
                    }
                    cur.inner[i].block.visit = true;
                    que.Enqueue(cur.inner[i].block);
                }
            }
        }
        // visit �� �ٽ� false �� �ٲ���� ���� ����
        start.visit = false;
        que.Enqueue(start);
        while (que.Count != 0)
        {
            cur = que.Dequeue();
            for (int i = 0; i < cur.outer.Count; i++)
            {
                if (cur.outer[i].link)
                {
                    if (cur.outer[i].link.visit == false)
                    {
                        continue;
                    }
                    cur.outer[i].link.visit = false;
                    que.Enqueue(cur.outer[i].link);
                }
            }
            for (int i = 0; i < cur.inner.Count; i++)
            {
                if (cur.inner[i].block)
                {
                    if (cur.inner[i].block.visit == false)
                    {
                        continue;
                    }
                    cur.inner[i].block.visit = false;
                    que.Enqueue(cur.inner[i].block);
                }
            }
        }
    }
}
