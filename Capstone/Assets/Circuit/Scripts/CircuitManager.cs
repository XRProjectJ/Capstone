using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ��ȸ�� ����ϴ� ��ũ��Ʈ

public class CircuitManager : MonoBehaviour
{
    // ���İ� ���� �κ��� ����� �� ����,����,������ �����ϰ� �̸� �� ���ؼ� ȸ���� ��ü ����, ����, ������ ���Ϸ�����
    // (24.04.01 ���� ������)
    /*    private List<double> R = new List<double>();
        private List<double> V = new List<double>();
        private List<double> I = new List<double>();*/

    struct ParallelR
    {
        public double R;
        public ComponentClass start;
    };
    private List<ParallelR> parallelRs = new List<ParallelR>();
    double timer = 0;
    bool first = false;

    [SerializeField] private ComponentClass root;

    // ȸ���� ��ü ������ ���ϴ� �Լ� (��ȯ ȣ��� ��ȸ)
    public double calcEntireR(ComponentClass root, ComponentClass node, List<double> nextOfParallelR, ref bool success, bool visit)
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
        if (node == root && node.GetVisit() == visit) {
            //success = true;
            return 0;
        }
        double result = 0;
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
            result = node.GetR();
            /*Debug.Log(node.transform.name);
            Debug.Log("�������� ���� ���� : " + result);*/
            return result;
            //return node.GetR();
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
            // �� ��ǰ�� ����, ������ ���� �� ���
            ParallelR tmp = new ParallelR();
            tmp.R = 1/sumOfParallelR;
            tmp.start = node;
            parallelRs.Add(tmp);
/*            Debug.Log("���� ���� ������ �� : " + 1/sumOfParallelR);*/
            // ���� ���� ������ ���� ���� ����
            for (int i = 0; i < nextOfParallelR.Count; i++)
            {
                totalR += nextOfParallelR[i];
            }
            nextOfParallelR.Clear();

            result = node.GetR() + 1 / sumOfParallelR + totalR;
            //Debug.Log(node.transform.name);
            //Debug.Log("�������� ���� ���� : " + result);
            return result;
            //return node.GetR() + 1 / sumOfParallelR+ totalR;
        }
        // ���� ��ǰ�� ���� �����̸鼭 �湮���� ���� ��ǰ�� �� (���� ���δ� ���İ� ����)
        else
        {
            //Debug.Log("����");
            node.SetVisit(visit);
            //Debug.Log("�α�: "+node.plus.links[0]);

            result = node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), nextOfParallelR, ref success, visit);
            //Debug.Log(node.transform.name);
            //Debug.Log("�������� ���� ���� : " + result);
            return result;
            //return node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), nextOfParallelR, ref success, visit);
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
        double result = 0;
        // ���� ��ǰ�� �̹� �湮�� ��ǰ�� ��
        if (node.GetVisit() == visit)
        {
            /*Debug.Log("�̹� �湮");*/
            return 0;
        }
        // ���� ��ǰ�� ������ ���� ��
        else if (node.plus.GetIsEndOfParallel())
        {
            
            // ���� ������ ���� ���� -> ���� �����ϴ� ������ ������ ���а��� ���� ���� ���� ���̸� �ȵǱ� ���� 
            nextOfParallelV.Add(calcEntireV(root, node.plus.GetEndOfParallelLink().GetComponent(), nextOfParallelV, ref success, visit));
            node.SetVisit(visit);
            // �ڽ��� ���� ����
            result = node.GetV();
            /*Debug.Log("���� ��");
            Debug.Log(node.transform.name);
            Debug.Log("�������� ���� ���� : " + result);*/
            return result;
            //return node.GetV();
        }
        // ���� ��ǰ�� ������ ������ ��
        if (node.plus.GetIsStartOfParallel())
        {
            // ��� ���� ������ ���� ū ������ ����
            double maxV = 0;

            for (int i = 0; i < node.plus.links.Count; i++)
            {
                double tmp = calcEntireV(root, node.plus.links[i].GetComponent(), nextOfParallelV, ref success, visit);
                if(tmp > maxV)
                {
                    maxV = tmp;
                }
            }
            // ���� ���� ������ ���� ���� ����
            double totalV = 0;

            // ���� ���� ������ ���� ���� ����
            for (int i = 0; i < nextOfParallelV.Count; i++)
            {
                totalV += nextOfParallelV[i];
            }
            nextOfParallelV.Clear();
            node.SetVisit(visit);
            result = node.GetV() + totalV+maxV;
            /*Debug.Log("���� ����");
            Debug.Log(node.transform.name);
            Debug.Log("�������� ���� ���� : " + result);*/
            return result;
            //return node.GetV() + totalV+maxV;
        }
        // ���� ��ǰ�� ���� �����̸鼭 �湮���� ���� ��ǰ�� �� (���� ���δ� ���İ� ����)
        else
        {
            
            node.SetVisit(visit);

            //Debug.Log("�α�: "+node.plus.links[0]);

            result = node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), nextOfParallelV, ref success, visit);
            /*Debug.Log("����");
            Debug.Log(node.transform.name);
            Debug.Log("�������� ���� ���� : " + result);*/
            return result;
            //return node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), nextOfParallelV, ref success, visit);
        }


    }
    // ���� ������ ����(����)�� ������ ���� ���
    private double calcRSerialInParallel(ComponentClass node)
    {
        if (node.plus.GetIsEndOfParallel())
        {
            return node.GetR();
        }
        return node.GetR() + calcRSerialInParallel(node.plus.links[0].GetComponent());
    }
    // ȸ���� �� ��ǰ�� ����, ������ ���ϴ� �Լ� (��ȯ ȣ��� ��ȸ)
    // sumOfParallelR �� 0�� �ƴϸ� ���� ���θ� �湮������ �Ͻ�, sumOfSerialR �� 0�� �ƴϸ� ������ ���� �� �������� �ƴ� ���� �湮������ �Ͻ�
    public void calcComponent(ComponentClass root, ComponentClass node, double entireR,  double entireV, ref bool success, bool visit, double sumOfParallelR=0, double sumOfSerialR = 0)
    {
        //Debug.Log(node.transform.name);
        if (success == false)
        {
            return;
        }
        // ȸ�ΰ� �߰��� �����ִ� ���
        if (node.plus.links.Count <= 0)
        {
            success = false;
            return;
        }

        // ��ȸ�� ��� ���������� ������ ��
        if (node == root && node.GetVisit() == visit)
        {
            //success = true;
            return;
        }
        double result = 0;
        // ���� ��ǰ�� �̹� �湮�� ��ǰ�� ��
        if (node.GetVisit() == visit)
        {
            //Debug.Log("�̹� �湮");
            return;
        }
        // ���� ��ǰ�� ������ ���� ��
        else if (node.plus.GetIsEndOfParallel())
        {
            //Debug.Log("���� ��");
            node.SetVisit(visit);
            double I = entireV / entireR;
            node.SetV(I * node.GetR());
            node.SetI(node.GetV() / node.GetR());
            calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit);

            return;
        }
        // ���� ��ǰ�� ������ ������ ��
        if (node.plus.GetIsStartOfParallel())
        {
            //Debug.Log("���� ����");
            node.SetVisit(visit);

            // ���� ���� ������ ��ü ������ ã��
            for(int i=0; i < parallelRs.Count; i++)
            {
                if (parallelRs[i].start == node)
                {
                    //sumOfParallelR �� 0 �� �ƴ϶�� ���� �������� ��Ÿ�� -> ���� ��ǰ�� ����
                    sumOfParallelR = parallelRs[i].R;
                    Debug.Log("���� ������ ���� ��: " + sumOfParallelR);
                }
            }
            double I = entireV / sumOfParallelR;

            for (int i = 0; i < node.plus.links.Count; i++)
            {
                node.SetV(I * node.GetR());
                node.SetI(node.GetV() / node.GetR());
                calcComponent(root, node.plus.links[i].GetComponent(), entireR, entireV, ref success, visit, sumOfParallelR, sumOfSerialR);
            }
            


            return;
        }
        // ���� ��ǰ�� ���� �����̸鼭 �湮���� ���� ��ǰ�� �� (���� ���δ� ���İ� ����)
        else
        {
            //Debug.Log("����");
            node.SetVisit(visit);
            //Debug.Log("�α�: "+node.plus.links[0]);
            double I = 0;

            // ���� ������ ���� �� (���� ��) �������� ��ǰ
            if (sumOfParallelR != 0 && sumOfSerialR == 0)
            {
                double tmpV = 0;
                double tmpI = 0;
                double tmpR = 0;

                I = entireV / entireR;
                
                tmpV = I * sumOfParallelR;
                tmpR = calcRSerialInParallel(node);
                //sumOfSerialR �� 0�� �ƴ϶�� ������ ���� (����) �� ù��° ��ǰ�� �ƴ϶� ���� �ǹ� -> ���� ��ǰ�� ����
                sumOfSerialR = tmpR;
                tmpI = tmpV / tmpR;
                node.SetI(tmpI);
                node.SetV(tmpI * node.GetR());
                calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit, sumOfParallelR, sumOfSerialR);
            }
            // ���� ������ ���� �� ���� ���� �ƴ� ��ǰ
            else if (sumOfParallelR != 0 && sumOfSerialR != 0)
            {
                double tmpV = 0;
                double tmpI = 0;
                double tmpR = 0;

                I = entireV / entireR;

                tmpV = I * sumOfParallelR;
                tmpR = sumOfSerialR;
                tmpI = tmpV / tmpR;
                node.SetI(tmpI);
                node.SetV(tmpI * node.GetR());
                calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit, sumOfParallelR, sumOfSerialR);
            }
            // �Ϲ����� ����
            else
            {
                I = entireV / entireR;
                node.SetV(I * node.GetR());
                node.SetI(node.GetV() / node.GetR());
                calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit, sumOfParallelR, sumOfSerialR);
            }
            
            return;
        }
    }
    public bool circuit(ComponentClass root)
    {
        bool success = true;
        List<double> listR = new List<double>();
        ComponentClass next = null;
        List<double> listV = new List<double>();
        double r = 0;
        double v = 0;
        double i = 0;
        // �湮�� �ߴ��� Ȯ���ϱ� ���� �� : true, false �� �����Ǿ� �ִٸ� ��ȸ���� �Ź� �ʱ�ȭ ���������
        bool visit = !root.GetVisit();
        //r = calcEntireR(root, root,listR, ref success, visit);
        r = calcEntireR(root, root, ref next, ref success, visit);
        
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
        success = true;
        v = calcEntireV(root, root, listV, ref success, visit);
        if (success)
        {
            Debug.Log("��ü ���� : " + v);
        }
        else
        {
            Debug.Log("CircuitManager Error : ���� ���ϱ� ����");
            return false;
        }
        i = v / r;
        Debug.Log("��ü ���� : " + i);

        visit = !root.GetVisit();
        calcComponent(root, root, r, v, ref success, visit);
        parallelRs.Clear();
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

    // ȸ���� ��ü ������ ���ϴ� �Լ� (��ȯ ȣ��� ��ȸ)
    public double calcEntireR(ComponentClass root, ComponentClass node, ref ComponentClass nextOfParallel, ref bool success, bool visit)
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
        double result = 0;
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
            node.SetVisit(visit);
            
            nextOfParallel = node.plus.links[0].GetComponent();
            Debug.Log("������ ���� �� ���� ��ǰ : " + nextOfParallel);
            // �ڽ��� ���� ����
            result = node.GetR();
            Debug.Log(node.transform.name);
            //Debug.Log("�������� ���� ���� : " + result);
            return result;
            //return node.GetR();
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
                sumOfParallelR += 1 / calcEntireR(root, node.plus.links[i].GetComponent(), ref nextOfParallel, ref success, visit);
            }
            // �� ��ǰ�� ����, ������ ���� �� ���
            ParallelR tmp = new ParallelR();
            tmp.R = 1 / sumOfParallelR;
            tmp.start = node;
            parallelRs.Add(tmp);
            Debug.Log("���� ������ �� ������ ���� ��ǰ : "+nextOfParallel);
            totalR = calcEntireR(root, nextOfParallel, ref nextOfParallel, ref success, visit);
            result = node.GetR() + 1 / sumOfParallelR + totalR;
            Debug.Log(node.transform.name);
            return result;
            //return node.GetR() + 1 / sumOfParallelR+ totalR;
        }
        // ���� ��ǰ�� ���� �����̸鼭 �湮���� ���� ��ǰ�� �� (���� ���δ� ���İ� ����)
        else
        {
            //Debug.Log("����");
            node.SetVisit(visit);
            //Debug.Log("�α�: "+node.plus.links[0]);

            result = node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), ref nextOfParallel, ref success, visit);
            Debug.Log(node.transform.name);
            //Debug.Log("�������� ���� ���� : " + result);
            return result;
            //return node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), nextOfParallelR, ref success, visit);
        }


    }
}
