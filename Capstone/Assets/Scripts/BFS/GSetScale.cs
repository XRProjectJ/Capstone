using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ũ�� ��ȯ�� �����ϴ� ��ũ��Ʈ
// -> ����ν�� ũ�Ⱑ ������ �� ����� ��ġ�� �������� �ʾƼ� ����� ������ �޶��� ���̴� ������ ���� 
public class GSetScale : BfsOffset
{
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.transform.parent.localScale = offset;
    }
}
