using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS �� ���� ����ϴٺ��� template method �� �������
// BFS �� �ʿ��� ��� �� ��ũ��Ʈ�� ����� ���� coreFunction �� ���ϴ� ����� ������ ��
// �� ��ũ��Ʈ�� Vector3 offset �Ű������� �޴� BFS �뵵
public abstract class BfsOffset : BfsTemplate<Vector3?>
{
    public Vector3 offsetSettingTemplate(Vector3? nullableOffset = null)
    {
        Vector3 offset;
        Vector3 defaultOffset = new Vector3(0.0f, 0.0f, 0.0f);
        offset = nullableOffset ?? defaultOffset;
        return offset;
    }
    public abstract void coreFunction(GCubeClass obj, Vector3 offset);
    override public void bfsMainFunction(GCubeClass obj, Vector3? nullableOffset = null)
    {
        Vector3 offset = offsetSettingTemplate(nullableOffset);
        coreFunction(obj, offset);
    }
}
