using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ȸ���� �ϴ� ��� 
// 1. ȸ���� �߽��� �Ǵ� � ����� A ��� �ϸ�, A �� ����� ��� ����� A �� ��ġ�� �ű�
// 2. ȸ�� �߽��� �Ǵ� A �� ȸ���� ��ŭ ����� ��� ����� ���ð������� ȸ����Ŵ
// 3. ȸ���� �� ���¿��� 1�� �������� �̵��� ��ŭ �״�� �̵��ϸ� ��
public class GRotationProcess : BfsOffset
{
    public GSetRotation setRotation;
    private void Start()
    {
        setRotation = GetComponent<GSetRotation>();
    }
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.firstMove = false;
        obj.mBlock.transform.Translate(obj.distance);
        Quaternion parentRot = Quaternion.EulerAngles(offset);
        Quaternion myRot = obj.mBlock.transform.rotation;
        obj.mBlock.transform.rotation = parentRot * myRot;
        obj.mBlock.transform.Translate(-obj.distance);

    }
}
