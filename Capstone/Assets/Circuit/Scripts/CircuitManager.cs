using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ��ȸ�� ����ϴ� ��ũ��Ʈ
public class CircuitManager : MonoBehaviour
{
    // ���İ� ���� �κ��� ����� �� ����,����,������ �����ϰ� �̸� �� ���ؼ� ȸ���� ��ü ����, ����, ������ ���Ϸ�����
    // (24.04.01 ���� ������)
    private List<double> R = new List<double>();
    private List<double> V = new List<double>();
    private List<double> I = new List<double>();

    // ȸ���� ��ü ������ ���ϴ� �Լ� (��ȯ ȣ��� ��ȸ)
    public double calcR(ComponentClass root, ComponentClass node, List<double> nextOfParallelR, ref bool success)
    {
        // ȸ���� ���� �����ߴµ� null -> ȸ�ο� ������ ����
        if (node == null)
        {
            success = false;
            return 0;
        }
        // �湮�� �ߴ��� Ȯ���ϱ� ���� �� : true, false �� �����Ǿ� �ִٸ� ��ȸ���� �Ź� �ʱ�ȭ ���������
        bool visit = !root.GetVisit();
        // ��ȸ�� ��� ���������� ������ ��
        if (node == root && node.GetVisit() == visit) {
            success = true;
            return 0;
        }
        // ���� ��ǰ�� ������ ���� ��
        if (node.plus.GetIsEndOfParallel() && node.GetVisit() != visit)
        {
            // ���� ������ ���� ���� -> ���� �����ϴ� ������ ������ ���װ��� ���� ���� ���� ���̸� �ȵǱ� ���� 
            nextOfParallelR.Add(calcR(root, node.plus.GetEndOfParallelLink().GetComponent(), nextOfParallelR, ref success));
            node.SetVisit(visit);
            // �ڽ��� ���� ����
            return node.GetR();
        }
        // ���� ��ǰ�� ������ ������ ��
        if (node.plus.GetIsStartOfParallel() && node.GetVisit() != visit)
        {
            // ��� ���� ������ ������ ����
            double sumOfParallelR = 0;
            // ���� ���� ������ ���� ���� ������ ���� ���� ����
            double totalR = 0;
            for (int i = 0; i < node.plus.links.Count; i++)
            {
                sumOfParallelR += 1 / calcR(root, node.plus.links[i].GetComponent(), nextOfParallelR, ref success);
            }
            // ���� ���� ������ ���� ���� ����
            for (int i = 0; i < nextOfParallelR.Count; i++)
            {
                totalR += nextOfParallelR[i];
            }
            nextOfParallelR.Clear();
            node.SetVisit(visit);
            return node.GetR() + 1 / sumOfParallelR+ totalR;
        }
        // ���� ��ǰ�� ���� �����̸鼭 �湮���� ���� ��ǰ�� �� (���� ���δ� ���İ� ����)
        else if(node.GetVisit() != visit)
        {
            node.SetVisit(visit);
            return node.GetR() + calcR(root, node.plus.links[0].GetComponent(), nextOfParallelR, ref success);
        }
        // ���� ��ǰ�� �̹� �湮�� ��ǰ�� ��
        else 
        {
            return 0;
        }

    }
}
