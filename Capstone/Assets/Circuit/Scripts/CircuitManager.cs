using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

// ��ȸ�� ����ϴ� ��ũ��Ʈ

public class CircuitManager : MonoBehaviour
{
    struct ParallelR
    {
        public double R;
        public ComponentClass start;
    };
    // �� ��ǰ�� ����, ������ ���� �� ���
    private List<ParallelR> parallelRs = new List<ParallelR>();
    double timer = 0;
    bool first = false;
    [SerializeField] private ComponentClass root;
    [SerializeField] private GameObject startComponent;


    // ȸ���� ��ü ������ ���ϴ� �Լ� (��ȯ ȣ��� ��ȸ)
    // ù��° �Ű����� : ��ȸ�� ������ ��ǰ
    // �ι�° �Ű����� : ���� ��ȸ���� ��ǰ
    // ����° �Ű����� : ���� ���� ���� �������� ��ȸ�ؾ��ϴ� ��ǰ
    // �׹�° �Ű����� : ��ȸ�� ���� ����
    // �ټ���° �Ű����� : ���� ��ǰ�� �湮������ �� �Ű������� ���� visit ���� ������ �־���� (�湮 ���ΰ� �ƴ�)
    public double calcEntireR(ComponentClass root, ComponentClass node, ref ComponentClass nextOfParallel, ref bool success, bool visit)
    {
        if (success == false)
        {
            return 0;
        }
        // ȸ�ΰ� �߰��� �����ִ� ���
        if (node.plus.links.Count <= 0)
        {
            Debug.Log(node.transform.parent.transform.name);
            success = false;
            return 0;
        }

        // ��ȸ�� ��� ���������� ������ ��
        if (node == root && node.GetVisit() == visit)
        {
            return 0;
        }
        double result = 0;
        // ���� ��ǰ�� �̹� �湮�� ��ǰ�� ��
        if (node.GetVisit() == visit)
        {
            return 0;
        }
        // ���� ��ǰ�� ������ ���� ��
        else if (node.plus.GetIsEndOfParallel())
        {
            node.SetVisit(visit);

            nextOfParallel = node.plus.links[0].GetComponent();

            // �ڽ��� ���� ����
            result = node.GetR();
            return result;
        }
        // ���� ��ǰ�� ������ ������ ��
        if (node.plus.GetIsStartOfParallel())
        {
            node.SetVisit(visit);
            // ���� ������ ������ ���� ��
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

            // ���� ���θ� ���� ��ȸ ������ ���� �ܺ��� ��ȸ�� �ٽ� ����
            totalR = calcEntireR(root, nextOfParallel, ref nextOfParallel, ref success, visit);

            result = node.GetR() + 1 / sumOfParallelR + totalR;
            return result;
        }
        // ���� ��ǰ�� ���� �����̸鼭 �湮���� ���� ��ǰ�� �� (���� ���δ� ���İ� ����)
        else
        {
            node.SetVisit(visit);

            result = node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), ref nextOfParallel, ref success, visit);
            return result;
        }


    }
    //ȸ���� ��ü ���� ���ϴ� �Լ�
    // ù��° �Ű����� : ��ȸ�� ������ ��ǰ
    // �ι�° �Ű����� : ���� ��ȸ���� ��ǰ
    // ����° �Ű����� : ���� ������ ���а���
    // �׹�° �Ű����� : ��ȸ�� ���� ����
    // �ټ���° �Ű����� : ���� ��ǰ�� �湮������ �� �Ű������� ���� visit ���� ������ �־���� (�湮 ���ΰ� �ƴ�)
    public double calcEntireV(ComponentClass root, ComponentClass node, ref ComponentClass nextOfParallel, ref bool success, bool visit)
    {
        if (success == false)
        {
            return 0;
        }
        // ȸ�ΰ� �߰��� �����ִ� ���
        if (node.plus.links.Count <= 0)
        {
            Debug.Log(node.transform.parent.transform.name);
            success = false;
            return 0;
        }

        // ��ȸ�� ��� ���������� ������ ��
        if (node == root && node.GetVisit() == visit)
        {
            return 0;
        }
        double result = 0;
        // ���� ��ǰ�� �̹� �湮�� ��ǰ�� ��
        if (node.GetVisit() == visit)
        {
            return 0;
        }
        // ���� ��ǰ�� ������ ���� ��
        else if (node.plus.GetIsEndOfParallel())
        {
            node.SetVisit(visit);

            nextOfParallel = node.plus.links[0].GetComponent();

            // �ڽ��� ���� ����
            result = node.GetV();
            return result;
        }
        // ���� ��ǰ�� ������ ������ ��
        if (node.plus.GetIsStartOfParallel())
        {
            node.SetVisit(visit);
            // ���� ������ ������ ���� ��
            double maxV = 0;
            // ���� ���� ������ ���� ���� ����
            double totalV = 0;
            for (int i = 0; i < node.plus.links.Count; i++)
            {

                double tmpV = calcEntireV(root, node.plus.links[i].GetComponent(), ref nextOfParallel, ref success, visit);
                if(tmpV > maxV)
                {
                    maxV = tmpV;
                }
            }

            // ���� ���θ� ���� ��ȸ ������ ���� �ܺ��� ��ȸ�� �ٽ� ����
            totalV = calcEntireV(root, nextOfParallel, ref nextOfParallel, ref success, visit);

            result = node.GetV() + maxV + totalV;
            return result;
        }
        // ���� ��ǰ�� ���� �����̸鼭 �湮���� ���� ��ǰ�� �� (���� ���δ� ���İ� ����)
        else
        {
            node.SetVisit(visit);

            result = node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), ref nextOfParallel, ref success, visit);
            return result;
        }


    }

    // ���� ������ ����(����)�� ������ ���� ��� (��ȯȣ��)
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
            return;
        }
        // ���� ��ǰ�� �̹� �湮�� ��ǰ�� ��
        if (node.GetVisit() == visit)
        {
            return;
        }
        // ���� ��ǰ�� ������ ���� ��
        else if (node.plus.GetIsEndOfParallel())
        {
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
            node.SetVisit(visit);

            // ���� ���� ������ ��ü ������ ã��
            for(int i=0; i < parallelRs.Count; i++)
            {
                if (parallelRs[i].start == node)
                {
                    //sumOfParallelR �� 0 �� �ƴ϶�� ���� �������� ��Ÿ�� -> ���� ��ǰ�� ����
                    sumOfParallelR = parallelRs[i].R;
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
            node.SetVisit(visit);
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
        next = null;
        v = calcEntireV(root, root, ref next, ref success, visit);
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

    // ��ȸ �� �������� ������� (ȸ�ο��� ������ ������ �ڵ�� �������� ����)
    ComponentClass createStartingComponent(ComponentClass parallel, ComponentClass next)
    {
        Debug.Log("createStartingComponent ����");
        ComponentClass start = Instantiate(startComponent).GetComponent<ComponentClass>();
        start.transform.name = "start";
        ComponentClass receive = Instantiate(startComponent).GetComponent<ComponentClass>();
        receive.transform.name = "receive";
        ComponentClass give = Instantiate(startComponent).GetComponent<ComponentClass>();
        give.transform.name = "give";

        start.minus.links.Add(receive.plus);
        start.plus.links.Add(give.minus);

        receive.minus.links.Add(parallel.plus);
        give.plus.links.Add(next.minus);

        Debug.Log("parallel : "+parallel.transform.parent.transform.name);
        Debug.Log("next : " + next.transform.parent.transform.name);

        return start;

    }
    // ��ȸ�� ������ ������ �ڵ����� ã���� (�����Ѱ� ������ ��ü������ ����)
    ComponentClass findStartingPoint(ComponentClass root)
    {
        ComponentClass result = root;
        Debug.Log("findStartingPoint ����");
        ComponentClass parallel = root.plus.links[0].GetComponent();
        Stack<PlusAttachment> parallelComponent = new Stack<PlusAttachment>();
        bool stackUse = false;
        while (parallel != root && parallel != null)
        {
            
            if (parallel.plus.GetIsEndOfParallel())
            {
                if(parallelComponent.Count > 0)
                {
                    parallelComponent.Pop();
                    stackUse = true;
                }
            }
            else if (parallel.plus.GetIsStartOfParallel())
            {
                parallelComponent.Push(parallel.plus);
                stackUse = true;
            }
            if(stackUse && parallelComponent.Count <= 0) {
                result = createStartingComponent(parallel, parallel.plus.links[0].GetComponent());
                break;
            }
            parallel = parallel.plus.links[0].GetComponent();
            
        }
        
        return result;
    }
    void calc()
    {
        Debug.Log("calc ����");
        ComponentClass start = findStartingPoint(root);
        circuit(start);
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
            calc();
            //circuit(root);
        }
        
    }
    //ȸ���� ��ü ���� ���ϴ� �Լ� (24.04.29 ���� �Ⱦ�)
    // ù��° �Ű����� : ��ȸ�� ������ ��ǰ
    // �ι�° �Ű����� : ���� ��ȸ���� ��ǰ
    // ����° �Ű����� : ���� ������ ���а���
    // �׹�° �Ű����� : ��ȸ�� ���� ����
    // �ټ���° �Ű����� : ���� ��ǰ�� �湮������ �� �Ű������� ���� visit ���� ������ �־���� (�湮 ���ΰ� �ƴ�)
    /* public double calcEntireV(ComponentClass root, ComponentClass node, List<double> nextOfParallelV, ref bool success, bool visit)
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
             return 0;
         }
         double result = 0;
         // ���� ��ǰ�� �̹� �湮�� ��ǰ�� ��
         if (node.GetVisit() == visit) 
         { 
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
             return result;
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
             return result;
         }
         // ���� ��ǰ�� ���� �����̸鼭 �湮���� ���� ��ǰ�� �� (���� ���δ� ���İ� ����)
         else
         {         
             node.SetVisit(visit);

             result = node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), nextOfParallelV, ref success, visit);
             return result;
         }


     }*/
}
