using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS �� ���鼭 ����� ��ϵ��� ��ġ�� offset ��ŭ �̵���Ŵ
public class GSetPosition : BfsOffset
{
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.mBlock.transform.position += offset;
    }
}
