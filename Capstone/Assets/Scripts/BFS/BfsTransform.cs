using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS �� ���� ����ϴٺ��� template method �� �������
// BFS �� �ʿ��� ��� �� ��ũ��Ʈ�� ����� ���� bfsMainFunction �� ���ϴ� ����� ������ ��
// �� ��ũ��Ʈ�� Transform root �Ű������� �޴� BFS �뵵
public abstract class BfsTransform : BfsTemplate<Transform>
{
    override public abstract void bfsMainFunction(GCubeClass obj, Transform root);
}
