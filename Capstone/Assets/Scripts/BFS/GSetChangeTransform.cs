using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS �� ���鼭 ����� ��ϵ��� hasChanged �� false �� �ٲ�
// hasChanged �� ����Ƽ���� �⺻������ �����Ǵ� �Ӽ�
public class GSetChangeTransform : BfsOffset
{
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.mBlock.transform.hasChanged =false;
    }
}
