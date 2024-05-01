using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
    //[SerializeField] private ComponentClass root;
    [SerializeField] private GameObject startComponent;
    [SerializeField] private Camera cam;


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
            //Debug.Log("����");
            return 0;
        }
        // ȸ�ΰ� �߰��� �����ִ� ���
        if (node.plus.links.Count <= 0)
        {
            //Debug.Log("�߰��� ����");
            Debug.Log(node.transform.parent.transform.name);
            success = false;
            return 0;
        }

        // ��ȸ�� ��� ���������� ������ ��
        if (node == root && node.GetVisit() == visit)
        {
            //Debug.Log("��ȸ ����");
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
            //Debug.Log("������ ��");
            node.SetVisit(visit);

            nextOfParallel = node.plus.links[0].GetComponent();

            // �ڽ��� ���� ����
            result = node.GetR();
            return result;
        }
        // ���� ��ǰ�� ������ ������ ��
        if (node.plus.GetIsStartOfParallel())
        {
            //Debug.Log("������ ����");
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
            //Debug.Log("����");
            //Debug.Log("�̸� : " + node.transform.name);
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
    public void calcComponent(ComponentClass root, ComponentClass node, double entireR, double entireV, ref bool success, bool visit, double sumOfParallelR = 0, double sumOfSerialR = 0)
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
        // �������� ����, ����, ������ ������
        if (node.transform.GetComponent<Power>() != null)
        {
            node.SetVisit(visit);
            calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit);
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
            for (int i = 0; i < parallelRs.Count; i++)
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
    // ȸ���� �� ��ǰ�� ����, ����, ������ �ʱ�ȭ �ϴ� �Լ� (BFS)
    // �������� ������ �����ϰ� ���� 0���� �ʱ�ȭ, �׿��� ��ǰ�� ������ �����ϰ� ���� 0���� �ʱ�ȭ
    public void clearComponent(ComponentClass root, bool visit)
    {
        Debug.Log("clearComponent ����");
        Queue<ComponentClass> q = new Queue<ComponentClass>();
        root.SetVisit(visit);
        q.Enqueue(root);
        while (q.Count > 0)
        {
            ComponentClass cur = q.Dequeue();
            
            double prevR = cur.GetR();
            double prevV = cur.GetV();
            double prevI = cur.GetI();
            cur.SetShowR(prevR);
            cur.SetShowI(prevI);
            cur.SetShowV(prevV);
            for (int i = 0; i < cur.plus.links.Count; i++)
            {
                if (cur.plus.links[i].GetComponent().GetVisit() != visit)
                {
                    cur.plus.links[i].GetComponent().SetVisit(visit);
                    q.Enqueue(cur.plus.links[i].GetComponent());
                }
            }
            
            if (cur.transform.GetComponent<Power>() != null)
            {
                cur.SetR(0);
                cur.SetI(0);
            }
            else
            {
                cur.SetV(0);
                cur.SetI(0);
            }
        }

    }

    // ���� ���õ� �Լ��� ��� ȣ���ϴ� �Լ�
    public bool circuit(ComponentClass root, ref bool success)
    {
        ComponentClass next = null;
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
        visit = !root.GetVisit();
        clearComponent(root, visit);
        return success;

    }

    // ��ȸ �� �������� ������� (ȸ�ο��� ������ ������ �ڵ�� �������� ����)
    // ������ ������ ������ ���� �� ���
    ComponentClass createStartingComponent(ComponentClass parallel, List<ComponentClass> next, bool visit)
    {
        Debug.Log("createStartingComponent ����");
        Debug.Log("parallel : " + parallel.transform.parent.transform.name);
        ComponentClass start = Instantiate(startComponent).GetComponent<ComponentClass>();
        start.transform.name = "start";
        ComponentClass receive = Instantiate(startComponent).GetComponent<ComponentClass>();
        receive.transform.name = "receive";
        ComponentClass give = Instantiate(startComponent).GetComponent<ComponentClass>();
        give.transform.name = "give";
        ComponentClass outReceive = Instantiate(startComponent).GetComponent<ComponentClass>();
        outReceive.transform.name = "outReceive";
        ComponentClass outGive = Instantiate(startComponent).GetComponent<ComponentClass>();
        outGive.transform.name = "outGive";

        start.SetVisit(visit);
        receive.SetVisit(visit);
        give.SetVisit(visit);
        outReceive.SetVisit(visit);
        outGive.SetVisit(visit);

        start.minus.links.Add(receive.plus);
        give.minus.links.Add(start.plus);
        start.plus.links.Add(give.minus);
        receive.plus.links.Add(start.minus);

        receive.minus.links.Add(outReceive.plus);
        outReceive.plus.links.Add(receive.minus);

        give.plus.links.Add(outGive.minus);
        outGive.minus.links.Add(give.plus);

        outReceive.minus.links.Add(parallel.plus);
        for(int i=0; i <  next.Count; i++)
        {
            outGive.plus.links.Add(next[i].minus);
        }


        return start;

    }
    // ��ȸ �� �������� ������� (ȸ�ο��� ������ ������ �ڵ�� �������� ����)
    // ������ �������� ���� �� ���
    ComponentClass createStartingComponent2(ComponentClass parallel, List<ComponentClass> next, bool visit)
    {
        Debug.Log("createStartingComponent2 ����");
        Debug.Log("parallel : " + parallel.transform.parent.transform.name);
        ComponentClass start = Instantiate(startComponent).GetComponent<ComponentClass>();
        start.transform.name = "start";
        ComponentClass receive = Instantiate(startComponent).GetComponent<ComponentClass>();
        receive.transform.name = "receive";
        ComponentClass give = Instantiate(startComponent).GetComponent<ComponentClass>();
        give.transform.name = "give";
        ComponentClass outReceive = Instantiate(startComponent).GetComponent<ComponentClass>();
        outReceive.transform.name = "outReceive";
        ComponentClass outGive = Instantiate(startComponent).GetComponent<ComponentClass>();
        outGive.transform.name = "outGive";

        start.SetVisit(visit);
        receive.SetVisit(visit);
        give.SetVisit(visit);
        outReceive.SetVisit(visit);
        outGive.SetVisit(visit);

        start.minus.links.Add(receive.plus);
        give.minus.links.Add(start.plus);
        start.plus.links.Add(give.minus);
        receive.plus.links.Add(start.minus);

        receive.minus.links.Add(outReceive.plus);
        outReceive.plus.links.Add(receive.minus);

        give.plus.links.Add(outGive.minus);
        outGive.minus.links.Add(give.plus);

        outGive.plus.links.Add(parallel.plus.links[0].GetComponent().minus);
        for (int i = 0; i < next.Count; i++)
        {
            outReceive.minus.links.Add(next[i].plus);
        }


        return start;

    }
    
    // ��ȸ�� ������ ������ �ڵ����� ã���� (�����Ѱ� ������ ��ü������ ����)
    ComponentClass findStartingPoint(ComponentClass root, ref bool success, bool visit)
    {
        // FindVisit �� �ٽ� false �� ����� ���� FindVisit �� true �� �ٲ� ��ǰ�� ������ ����Ʈ
        List<ComponentClass> clearList = new List<ComponentClass>();
        ComponentClass result = root;
        // toggle �� false �� ��� :
        // 1. ���ĸ� �ִ� ���
        // 2. �Ʒ��� ���� �ݺ������� ������ ���� ���ۺ��� ���� ���� ��ȸ�� ���������� �ǵ��ƿ�������
        // createStartingComponent() �� ȣ�� ���� ���
        bool toggle = false;
        Debug.Log("findStartingPoint ����");
        if (root.plus.links.Count <= 0)
        {
            success = false;
            toggle = false;
            return result;
        }
        ComponentClass parallel = root.plus.links[0].GetComponent();
        // ��ȣ ����ó�� ������ ������ push�ϰ� ������ ���� pop�Ѵ�
        Stack<PlusAttachment> parallelComponent = new Stack<PlusAttachment>();
        // ������ ����� �Ƚ���� : �Ƚ����� ���ĸ� �ִ� �����
        bool stackUse = false;
        while (parallel != root && parallel != null && parallel.GetFindVisit() != true)
        {
            parallel.SetFindVisit(true);
            clearList.Add(parallel);
            //Debug.Log("���� ��ġ : " + parallel.transform.parent.transform.name);
            // ������ ���� ���� ���
            if (parallel.plus.GetIsEndOfParallel())
            {
                // ��ȸ ������ ������ ���� ���� �� �� ���� => ȸ�ΰ� �߸��� ���� �ƴ�
                if (parallelComponent.Count > 0)
                {
                    parallelComponent.Pop();
                    stackUse = true;
                }
            }
            // ������ �������� ���� ���
            else if (parallel.plus.GetIsStartOfParallel())
            {
                parallelComponent.Push(parallel.plus);
                stackUse = true;
            }
            // ������ ����� Ȯ���ϰ� (���� ������ �Ƚ����� ���� ������ �������� ������ �ȳ��� ���� �� ����)
            // ������ ��µ��� ������ ��� ������ ������ ���� �ٱ��� �ִٰ� ����
            if (stackUse && parallelComponent.Count <= 0)
            {
                List<ComponentClass> next = new List<ComponentClass>();
                // ������ �������� ��ȸ�������� ����� ���
                if (parallel.plus.GetIsStartOfParallel())
                {
                    for (int i = 0; i < parallel.plus.links.Count; i++)
                    {
                        next.Add(parallel.plus.links[i].GetComponent());
                    }
                    result = createStartingComponent(parallel, next, visit);
                }
                // ������ ������ ��ȸ�������� ����� ���
                else if (parallel.plus.GetIsEndOfParallel())
                {
                    ComponentClass tmp = parallel.plus.links[0].GetComponent();
                    for (int i = 0; i < tmp.minus.links.Count; i++)
                    {
                        next.Add(tmp.minus.links[i].GetComponent());
                    }
                    result = createStartingComponent2(parallel, next, visit);
                }
                success = true;
                toggle = true;
                break;
            }
            // �߰��� ���� �ִ� ȸ��
            if (parallel.plus.links.Count <= 0)
            {
                success = false;
                toggle = false;
                return result;
            }
            // �̹� �湮�� �ߴ� ��ǰ�̸� �ٽ� �湮���� ���� 
            for (int i = 0; i < parallel.plus.links.Count; i++)
            {
                if (parallel.plus.links[i].GetComponent().GetFindVisit() == true)
                {
                    continue;
                }
                parallel = parallel.plus.links[i].GetComponent();
                break;
            }


        }
        // ���ĸ� �ְų� ���� �ݺ������� ������ �������� ��ã�� ���
        if (!toggle)
        {
            // ������ ������ �ݺ������� ������ �������� ��ã�� ���
            if (stackUse)
            {
                // ���� ��ó���� ������ ������ ������ ������ ����
                while (!parallel.minus.GetIsEndOfParallel())
                {
                    parallel = parallel.plus.links[0].GetComponent();
                }
            }
            List<ComponentClass> next = new List<ComponentClass>();
            next.Add(parallel.plus.links[0].GetComponent());
            result = createStartingComponent(parallel, next, visit);
        }
        // FindVisit �� �ʱ�ȭ �������ν� �ٽ� ����� �� �ְ� ��
        for (int i = 0; i < clearList.Count; i++)
        {
            clearList[i].SetFindVisit(false);
        }
        return result;
    }
    public void calc(ComponentClass root)
    {
        Debug.Log("calc ����");
        bool success = true;
        bool visit = root.GetVisit();
        ComponentClass start = findStartingPoint(root, ref success, visit);
        circuit(start, ref success);
        //success �� false �� ȸ�ΰ� �߰��� ���� �ִٴ� ���̰� �������� ��������� �ʾ���
        if (success)
        {
            // ��� ��ȸ�� ������ ������� ������ ����
            Destroy(start.plus.links[0].GetComponent().transform.gameObject);
            Destroy(start.minus.links[0].GetComponent().transform.gameObject);
            Destroy(start.plus.links[0].GetComponent().plus.links[0].GetComponent().transform.gameObject);
            Destroy(start.minus.links[0].GetComponent().minus.links[0].GetComponent().transform.gameObject);
            Destroy(start.transform.gameObject);
        }
        

    }
    // ȸ�� ��ǰ�� Ŭ���Ǹ� ��ȸ����
    private void click()
    {
        if (Input.GetMouseButtonDown(0))
        {
           
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                
                GameObject obj = hit.transform.gameObject;
                Debug.Log("obj.name : " + obj.transform.name);
                ComponentClass root = obj.GetComponentInChildren<ComponentClass>();
                if (root)
                {
                    Debug.Log("click");
                    calc(root);
                }
            }
        }
        

    }
    void Start()
    {

    }

    void Update()
    {
        click();

    }
}
