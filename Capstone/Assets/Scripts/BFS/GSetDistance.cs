using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��� ������� ���õ� ��ϰ� �׷��� ���� ��ϵ��� �Ÿ��� ���� - > ȸ���� �ϱ� ���ؼ� �ʿ�
public class GSetDistance : BfsOffset
{
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.distance = offset - obj.transform.position;
        //obj.distance = offset - obj.transform.localPosition;
    }
}
