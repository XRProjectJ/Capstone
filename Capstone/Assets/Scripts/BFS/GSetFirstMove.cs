using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS �� ���鼭 ����� ��ϵ��� firstMove �� false �� �ٲ�
public class GSetFirstMove : BfsOffset
{
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.firstMove = false;
    }
}
