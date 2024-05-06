using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

// ��ȸ�� ����ϴ� ��ũ��Ʈ

public class Circuit : MonoBehaviour
{
    struct ParallelPair
    {
        public ComponentClass start;
        public ComponentClass end;
    };
    //[SerializeField] private ComponentClass root;
    [SerializeField] private GameObject startComponent;
    [SerializeField] private GameObject parallelComponent;
    [SerializeField] private Camera cam;

    // ������ ���۰� ���� ���ߴ� �Լ�
    public void findPair(ComponentClass start)
    {
        ComponentClass node = start.plus.links[0].GetComponent();
        while (true)
        {
            if(node == null || node == start)
            {
                break;
            }
            if(node.minus.GetIsEndOfParallel() && node.GetPairOfEnd() == null)
            {
                start.SetPairOfStart(node);
                node.SetPairOfEnd(start);
                break;
            }
            node = node.plus.links[0].GetComponent();
        }
    }
    // ȸ���� ���� �κ��� ã�� ������ �κ��� �ϳ��� �������� �ٲٴ� �Լ� (DFS)
    public bool findParallel(ComponentClass root)
    {
        Debug.Log("findParallel ����");
        Stack<ComponentClass> s = new Stack<ComponentClass>();
        Stack<ComponentClass> startParallel = new Stack<ComponentClass>();
        List<ParallelPair> pairs = new List<ParallelPair>();
        bool visit = !root.GetVisit();
        s.Push(root);
        root.SetVisit(visit);
        while (s.Count > 0)
        {
            ComponentClass cur = s.Pop();
            if (cur.plus.GetIsStartOfParallel())
            {
                startParallel.Push(cur);
            }
            for (int i = 0; i < cur.plus.links.Count; i++)
            {
                ComponentClass tmp = cur.plus.links[i].GetComponent();
                if (tmp.GetVisit() != visit)
                {
                    tmp.SetVisit(visit);
                    s.Push(tmp);
                }
            }
        }
        while(startParallel.Count > 0)
        {
            ComponentClass tmp = startParallel.Pop();
            findPair(tmp);
            ParallelPair pair = new ParallelPair();
            pair.start = tmp;
            pair.end = tmp.GetPairOfStart();
            pairs.Add(pair);
        }
        for(int i=0; i < pairs.Count; i++)
        {
            ComponentClass start = pairs[i].start;
            ComponentClass end = pairs[i].end;
            createParallelComponent(start, end);
        }
        return true;
    }
    // ������ �ϳ��� ����(Parallel ��ǰ)���� ����� �Լ�
    public void createParallelComponent(ComponentClass start, ComponentClass end)
    {
        Debug.Log("createParallelComponent ����");
        GameObject parallel = Instantiate(parallelComponent);
        Parallel node = parallel.GetComponent<Parallel>();
        ComponentClass nodeComponent = parallel.GetComponent<ComponentClass>();

        for(int i = 0; i < start.plus.links.Count; i++)
        {
            node.SetInnerStart(start.plus.links[i].GetComponent());
        }
        for (int i = 0; i < end.minus.links.Count; i++)
        {
            node.SetInnerEnd(end.minus.links[i].GetComponent());
        }
        
        node.minus.links.Add(start.plus);
        node.plus.links.Add(end.minus);

        start.plus.links.Clear();
        end.minus.links.Clear();


        start.plus.links.Add(nodeComponent.minus);
        end.minus.links.Add(nodeComponent.plus);

        node.SetRoot();
        node.calcR();
        node.calcV();
        node.SetVisit(start.GetVisit());
    }
    // ȸ�� ��ü ������ ����ϴ� �Լ�
    public double calcEntireR(ComponentClass root , ref bool success)
    {
        ComponentClass cur = root.plus.links[0].GetComponent();
        double result = root.GetR();
        while(cur != root)
        {
            if(cur == null)
            {
                success = false;
                return 0;
            }
            result += cur.GetR();
            cur = cur.plus.links[0].GetComponent();
        }
        return result;
    }
    // ȸ�� ��ü ������ ���ϴ� �Լ�
    public double calcEntireV(ComponentClass root, ref bool success)
    {
        ComponentClass cur = root.plus.links[0].GetComponent();
        double result = root.GetV();
        while (cur != root)
        {
            if (cur == null)
            {
                success = false;
                return 0;
            }
            result += cur.GetV();
            cur = cur.plus.links[0].GetComponent();
        }
        return result;
    }
    // ��ǰ�� ����, ���� ����ϴ� �Լ�
    public void calcComponent(ComponentClass root, double entireR, double entireV)
    {
        Debug.Log("calcComponent ����");
        Queue<ComponentClass> q = new Queue<ComponentClass>();
        bool visit = !root.GetVisit();
        root.SetVisit(visit);
        q.Enqueue(root);
        double calcR = 0;
        double calcV = 0;
        double calcI = 0;
        while (q.Count > 0)
        {
            ComponentClass cur = q.Dequeue();
            // ���Ŀ� ������ �ʴ� ��ǰ�� ��ü ȸ���� ���װ� �������� ���
            if (cur.GetRootParallel() == null)
            {
                calcR = entireR;
                calcV = entireV;
            }
            // ���Ŀ� ���ϴ� ��ǰ�� ���� ������ ���װ� ���� ȸ�ο� �ɸ��� �������� ���
            else
            {
                Parallel parallel = cur.GetRootParallel().GetComponent<Parallel>();
                for(int i=0; i < parallel.GetBranches().Count; i++)
                {
                    if (parallel.GetBranches()[i].components.Contains(cur))
                    {
                        calcR = parallel.GetBranches()[i].resist;
                        break;
                    }
                }
                calcV = cur.GetRootParallel().GetV();
            }
            calcI = calcV / calcR;
            if(cur.gameObject.GetComponent<Power>() == null)
            {
                cur.SetV(calcI * cur.GetR());
                cur.SetI(cur.GetV() / cur.GetR());
            }
            if (cur.IsParallel())
            {
                Parallel parallel = cur.gameObject.GetComponent<Parallel>();
                for (int i = 0; i < parallel.GetInnerStart().Count; i++)
                {
                    if (parallel.GetInnerStart()[i].GetVisit() != visit)
                    {
                        parallel.GetInnerStart()[i].SetVisit(visit);
                        q.Enqueue(parallel.GetInnerStart()[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < cur.plus.links.Count; i++)
                {
                    if (cur.plus.links[i].GetComponent().GetVisit() != visit)
                    {
                        cur.plus.links[i].GetComponent().SetVisit(visit);
                        q.Enqueue(cur.plus.links[i].GetComponent());
                    }
                }
            }
            
        }

    }
    // ȸ���� �� ��ǰ�� ����, ����, ������ �ʱ�ȭ �ϴ� �Լ� (BFS) + Parallel ��ǰ ����
    // �������� ������ �����ϰ� ���� 0���� �ʱ�ȭ, �׿��� ��ǰ�� ������ �����ϰ� ���� 0���� �ʱ�ȭ
    public void clearComponent(ComponentClass root)
    {
        Debug.Log("clearComponent ����");
        Queue<ComponentClass> q = new Queue<ComponentClass>();
        bool visit = !root.GetVisit();
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
            int count = cur.plus.links.Count;
            //Debug.Log("count : " + count);
            for (int i = 0; i < cur.plus.links.Count; i++)
            {

                ComponentClass tmp = cur.plus.links[i].GetComponent();
                if (tmp.IsParallel())
                {
                    Debug.Log("Parallel ����");
                    //Debug.Log("���� �� cur.plus.links.Count : " + cur.plus.links.Count);
                    //Debug.Log("���� �� cur.plus.links.link[i] : " + tmp.gameObject.name);
                    Parallel parallel = tmp.gameObject.GetComponent<Parallel>();
                    parallel.DeleteParallel();
                    //Debug.Log("���� �� cur.plus.links.Count : " + cur.plus.links.Count);
                    //Debug.Log("���� �� cur.plus.links.link[i] : " + cur.plus.links[i].GetComponent().transform.parent.gameObject.name);
                    //cur.plus.links.Remove(tmp.minus);
                }
            }
            count = cur.plus.links.Count;
            //Debug.Log("count : " + count);
            for (int i = 0; i < cur.plus.links.Count; i++)
            {
                ComponentClass tmp = cur.plus.links[i].GetComponent();
                if (tmp.GetVisit() != visit)
                {
                    tmp.SetVisit(visit);
                    q.Enqueue(tmp);
                }

            }

            if (cur.gameObject.GetComponent<Power>() != null)
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
        double r = 0;
        double v = 0;
        double i = 0;
        success = findParallel(root);
        if (!success)
        {
            Debug.Log("ȸ�ο� ���� ����");
            return false;
        }
        r = calcEntireR(root, ref success);

        if (success)
        {
            Debug.Log("��ü ���� : " + r);
        }
        else
        {
            Debug.Log("CircuitManager Error : ���� ���ϱ� ����");
            return false;
        }
        v = calcEntireV(root, ref success);
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

        calcComponent(root, r, v);
        clearComponent(root);
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
        for (int i = 0; i < next.Count; i++)
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
        /*        Debug.Log("calc ����");
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
                }*/
        Debug.Log("calc ����");
        bool success = true;
        circuit(root, ref success);


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

