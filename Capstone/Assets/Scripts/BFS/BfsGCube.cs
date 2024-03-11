using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS �� ���� ����ϴٺ��� template method �� �������
// BFS �� �ʿ��� ��� �� ��ũ��Ʈ�� ����� ���� coreFunction �� ���ϴ� ����� ������ ��
// �� ��ũ��Ʈ�� GCubeClass �Ű������� �޴� BFS �뵵
public abstract class BfsGCube : BfsTemplate<GCubeClass>
{
    override abstract public void bfsMainFunction(GCubeClass obj, GCubeClass parameter);
}
