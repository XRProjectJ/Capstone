using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ��ȸ�� ����ϴ� ��ũ��Ʈ
// (24.04.04 ���� : ������ ���� ������ �� �����ϰ� ���� ������, �׽�Ʈ�� �� �غ��� �˰����� ��ü ������ �� �������µ�)

public class CircuitManager : MonoBehaviour
{
    // ���İ� ���� �κ��� ����� �� ����,����,������ �����ϰ� �̸� �� ���ؼ� ȸ���� ��ü ����, ����, ������ ���Ϸ�����
    // (24.04.01 ���� ������)
    private List<double> R = new List<double>();
    private List<double> V = new List<double>();
    private List<double> I = new List<double>();
    double timer = 0;
    bool first = false;

    [SerializeField] private ComponentClass root;

    // ȸ���� ��ü ������ ���ϴ� �Լ� (��ȯ ȣ��� ��ȸ)
    public double calcEntireR(ComponentClass root, ComponentClass node, List<double> nextOfParallelR, ref bool success, bool visit)
    {
        Debug.Log(node.transform.name);
        if (success == false)
        {
            return 0;
        }
        // ȸ�ΰ� �߰��� �����ִ� ���
        if (node.plus.links.Count <= 0)
        {
            success = false;
            return 0;
        }

        // ��ȸ�� ��� ���������� ������ ��
        if (node == root && node.GetVisit() == visit) {
            //success = true;
            return 0;
        }
        
        // ���� ��ǰ�� �̹� �湮�� ��ǰ�� ��
        if(node.GetVisit() == visit)
        {
            //Debug.Log("�̹� �湮");
            return 0;
        }
        // ���� ��ǰ�� ������ ���� ��
        else if (node.plus.GetIsEndOfParallel())
        {
            //Debug.Log("���� ��");
            node.SetVisit(visit);
            // ���� ������ ���� ���� -> ���� �����ϴ� ������ ������ ���װ��� ���� ���� ���� ���̸� �ȵǱ� ���� 
            nextOfParallelR.Add(calcEntireR(root, node.plus.GetEndOfParallelLink().GetComponent(), nextOfParallelR, ref success, visit));
            
            // �ڽ��� ���� ����
            return node.GetR();
        }
        // ���� ��ǰ�� ������ ������ ��
        if (node.plus.GetIsStartOfParallel())
        {
            //Debug.Log("���� ����");
            node.SetVisit(visit);
            // ��� ���� ������ ������ ����
            double sumOfParallelR = 0;
            // ���� ���� ������ ���� ���� ����
            double totalR = 0;
            for (int i = 0; i < node.plus.links.Count; i++)
            {
                sumOfParallelR += 1 / calcEntireR(root, node.plus.links[i].GetComponent(), nextOfParallelR, ref success, visit);
            }
            // ���� ���� ������ ���� ���� ����
            for (int i = 0; i < nextOfParallelR.Count; i++)
            {
                totalR += nextOfParallelR[i];
            }
            nextOfParallelR.Clear();
            
            return node.GetR() + 1 / sumOfParallelR+ totalR;
        }
        // ���� ��ǰ�� ���� �����̸鼭 �湮���� ���� ��ǰ�� �� (���� ���δ� ���İ� ����)
        else
        {
            //Debug.Log("����");
            node.SetVisit(visit);
            //Debug.Log("�α�: "+node.plus.links[0]);
            return node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), nextOfParallelR, ref success, visit);
        }
        

    }
    //ȸ���� ��ü ���� ���ϴ� �Լ�
    public double calcEntireV(ComponentClass root, ComponentClass node, List<double> nextOfParallelV, ref bool success, bool visit)
    {
        //Debug.Log(node.transform.name);
        if (success == false)
        {
            return 0;
        }
        // ȸ�ΰ� �߰��� �����ִ� ���
        if (node.plus.links.Count <= 0)
        {
            success = false;
            return 0;
        }

        // ��ȸ�� ��� ���������� ������ ��
        if (node == root && node.GetVisit() == visit)
        {
            //success = true;
            return 0;
        }

        // ���� ��ǰ�� �̹� �湮�� ��ǰ�� ��
        if (node.GetVisit() == visit)
        {
            //Debug.Log("�̹� �湮");
            return 0;
        }
        // ���� ��ǰ�� ������ ���� ��
        else if (node.plus.GetIsEndOfParallel())
        {
            //Debug.Log("���� ��");
            // ���� ������ ���� ���� -> ���� �����ϴ� ������ ������ ���װ��� ���� ���� ���� ���̸� �ȵǱ� ���� 
            nextOfParallelV.Add(calcEntireV(root, node.plus.GetEndOfParallelLink().GetComponent(), nextOfParallelV, ref success, visit));
            node.SetVisit(visit);
            // �ڽ��� ���� ����
            return node.GetV();
        }
        // ���� ��ǰ�� ������ ������ ��
        if (node.plus.GetIsStartOfParallel())
        {
            //Debug.Log("���� ����");
            // ���� ���� ������ ���� ���� ����
            double totalV = 0;

            // ���� ���� ������ ���� ���� ����
            for (int i = 0; i < nextOfParallelV.Count; i++)
            {
                totalV += nextOfParallelV[i];
            }
            nextOfParallelV.Clear();
            node.SetVisit(visit);
            return node.GetV()+totalV;
        }
        // ���� ��ǰ�� ���� �����̸鼭 �湮���� ���� ��ǰ�� �� (���� ���δ� ���İ� ����)
        else
        {
            //Debug.Log("����");
            node.SetVisit(visit);

            //Debug.Log("�α�: "+node.plus.links[0]);
            return node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), nextOfParallelV, ref success, visit);
        }


    }
    public bool circuit(ComponentClass root)
    {
        bool success = true;
        List<double> listR = new List<double>();
        List<double> listV = new List<double>();
        // �湮�� �ߴ��� Ȯ���ϱ� ���� �� : true, false �� �����Ǿ� �ִٸ� ��ȸ���� �Ź� �ʱ�ȭ ���������
        bool visit = !root.GetVisit();
        double r = calcEntireR(root, root,listR, ref success, visit);
        if (success)
        {
            Debug.Log("��ü ���� : " + r);
        }
        else
        {
            Debug.Log("CircuitManager Error : ���� ���ϱ� ����");
            return false;
        }
        visit = !root.GetVisit();
        double v = calcEntireV(root, root, listV, ref success, visit);
        if (success)
        {
            Debug.Log("��ü ���� : " + v);
        }
        else
        {
            Debug.Log("CircuitManager Error : ���� ���ϱ� ����");
            return false;
        }
        return success;
    }
    void Start()
    {
        //circuit(root);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 1.0 && first == false)
        {
            first = true;
            circuit(root);
            
        }
        
    }
}
