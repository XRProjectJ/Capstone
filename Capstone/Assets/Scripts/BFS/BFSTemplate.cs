using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS 를 자주 사용하다보니 template method 로 만들었음
// BFS 가 필요한 경우 이 스크립트를 상속한 다음 bfsMainFunction 에 원하는 기능을 넣으면 됨
// 이 스크립트는 BFS를 총괄하는 스크립트 -> 매개변수를 여러 형태로 받을 수 있도록 지원함
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
        // visit 을 다시 false 로 바꿔줘야 재사용 가능
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
