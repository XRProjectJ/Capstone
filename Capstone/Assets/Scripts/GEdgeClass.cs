using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Ǹ����� ���� ��ũ��Ʈ (�׷����� ������ �ǹ�)
public class GEdgeClass : MonoBehaviour
{
    public GCubeClass link; // �� ������ ����� �ٸ� ���
    public GameObject cylinder; // �Ǹ���
    public GameObject pos; // link �� ���� �� �̵��Ǿ���ϴ� ��ġ 
    public GCubeClass block; // �� �Ǹ����� ���� ����� Attach
    public bool hasLink = false; // �̹� ����� ����� �ִ��� �������� ��Ÿ���� ����
}
