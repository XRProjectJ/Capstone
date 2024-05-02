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

    public double calcParallelR(ComponentClass root, bool visit)
    {
        ComponentClass node = root;
        double result = 0;
        while(!node.plus.GetIsEndOfParallel())
        {
            node.SetVisit(visit);
            if (node.plus.GetIsStartOfParallel() && node.GetVisit() != visit)
            {
                for (int i = 0; i < node.plus.links.Count; i++)
                {
                    result += 1 / calcParallelR(node.plus.links[i].GetComponent(), visit);
                }

            }
            else if(node.GetVisit() != visit)
            {
                result += node.GetR();
            }
            node = node.plus.links[0].GetComponent();
        }
        return result;
    }
    public double calcEntireR(ComponentClass root, bool visit, ref bool success)
    {
        ComponentClass node = root.plus.links[0].GetComponent();
        root.SetVisit(visit);
        double result = root.GetR();
        while(node != root)
        {
            node.SetVisit(visit);
            if (node.plus.GetIsStartOfParallel() && node.GetVisit() != visit)
            {
                for(int i=0; i < node.plus.links.Count; i++)
                {
                    result += 1 / calcParallelR(node.plus.links[i].GetComponent(), visit);
                }

            }
            else if(node.GetVisit() != visit)
            {
                result += node.GetR();
            }
            node = node.plus.links[0].GetComponent();
        }
        return result;
    }
    public double calcParallelV(ComponentClass root, bool visit)
    {
        ComponentClass node = root;
        double result = 0;
        while (!node.plus.GetIsEndOfParallel())
        {
            node.SetVisit(visit);
            if (node.plus.GetIsStartOfParallel() && node.GetVisit() != visit)
            {
                double max = 0;
                for (int i = 0; i < node.plus.links.Count; i++)
                {
                    double tmp = calcParallelV(node.plus.links[i].GetComponent(), visit);
                    if (max < tmp)
                    {
                        max = tmp;
                    }
                }
                result += max;
            }
            else if (node.GetVisit() != visit)
            {
                result += node.GetV();
            }
            node = node.plus.links[0].GetComponent();
        }
        return result;
    }
    public double calcEntireV(ComponentClass root, bool visit, ref bool success)
    {
        ComponentClass node = root.plus.links[0].GetComponent();
        root.SetVisit(visit);
        double result = root.GetV();
        while (node != root)
        {
            node.SetVisit(visit);
            if (node.plus.GetIsStartOfParallel() && node.GetVisit() != visit)
            {
                double max = 0;
                for (int i = 0; i < node.plus.links.Count; i++)
                {
                    double tmp = calcParallelV(node.plus.links[i].GetComponent(), visit);
                    if (max < tmp)
                    {
                        max = tmp;
                    }
                }
                result += max;

            }
            else if (node.GetVisit() != visit)
            {
                result += node.GetV();
            }
            node = node.plus.links[0].GetComponent();
        }
        return result;
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

        r = calcEntireR(root,  visit, ref success);

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
        v = calcEntireV(root, visit, ref success);
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
        //calcComponent(root, root, r, v, ref success, visit);
        //calcComponentManager(root, visit, r, v);
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

