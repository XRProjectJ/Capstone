using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ��ȸ�� ����ϴ� ��ũ��Ʈ

public class Circuit : MonoBehaviour
{
    public TextMeshProUGUI Rt,Vt,It;

    struct ParallelPair
    {
        public ComponentClass start;
        public ComponentClass end;
    };
    //[SerializeField] private ComponentClass root;
    [SerializeField] private GameObject startComponent;
    [SerializeField] private GameObject parallelComponent;
    [SerializeField] private Camera cam;
    public List<Parallel> parallels = new List<Parallel>();

    public GameObject resultPanel;

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
        Debug.Log("startParallel.Count : " + startParallel.Count);
        while (startParallel.Count > 0)
        {
            ComponentClass tmp = startParallel.Pop();
            findPair(tmp);
            ParallelPair pair = new ParallelPair();
            pair.start = tmp;
            pair.end = tmp.GetPairOfStart();
            pairs.Add(pair);
        }
        Debug.Log("pairs.Count : " + pairs.Count);
        for (int i = 0; i < pairs.Count; i++)
        {
            ComponentClass start = pairs[i].start;
            ComponentClass end = pairs[i].end;
            Debug.Log("start : " + start.transform.parent.name);
            Debug.Log("end : " + end.transform.parent.name);
            Parallel tmp = createParallelComponent(start, end);
            parallels.Add(tmp);
        }
        return true;
    }
    // ������ �ϳ��� ����(Parallel ��ǰ)���� ����� �Լ�
    public Parallel createParallelComponent(ComponentClass start, ComponentClass end)
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

        return node;
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
            if(cur.plus.links.Count == 0)
            {
                success = false;
                break;
            }
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
            if (cur.plus.links.Count == 0)
            {
                success = false;
                break;
            }
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
            
            if (cur.gameObject.GetComponent<Power>() == null)
            {
                cur.SetV(calcI * cur.GetR());
                cur.SetI(cur.GetV() / cur.GetR());
            }
            cur.Do(entireV / entireR);
            // Parallel ��ǰ�� ��� ���ε� ��ȸ �ؾ���
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
        // Parallel ��ǰ ���ִ� �κ�
        int count = parallels.Count;
        for(int i= 0; i < count; i++)
        {
            parallels[i].DeleteParallel();
        }
        parallels.Clear();
        while (q.Count > 0)
        {
            ComponentClass cur = q.Dequeue();
            double prevR = cur.GetR();
            double prevV = cur.GetV();
            double prevI = cur.GetI();
            cur.SetShowR(prevR);
            cur.SetShowI(prevI);
            cur.SetShowV(prevV);
            cur.SetPairOfEnd(null);
            cur.SetPairOfStart(null);
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
    // ��ȸ�� �������� ã���� (null �� ���� ���ĸ� �ִ� ���)
    public ComponentClass findStartingComponent()
    {

        for(int i=0; i < parallels.Count; i++)
        {
            if (parallels[i].GetRootParallel() == null)
            {
                return parallels[i].GetComponent<ComponentClass>();
            }
        }
        return null;
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
        ComponentClass startingComponent = findStartingComponent();
        if(startingComponent == null) {
            startingComponent = root;
        }
        r = calcEntireR(startingComponent, ref success);

        if (success)
        {
            Debug.Log("��ü ���� : " + r);
            //Rt.text = "R = " + r;
        }
        else
        {
            Debug.Log("CircuitManager Error : ���� ���ϱ� ����");
            return false;
        }
        v = calcEntireV(startingComponent, ref success);
        if (success)
        {
            Debug.Log("��ü ���� : " + v);
            //Vt.text = "V = " + v;
        }
        else
        {
            Debug.Log("CircuitManager Error : ���� ���ϱ� ����");
            return false;
        }
        i = v / r;
        Debug.Log("��ü ���� : " + i);
        //It.text = "I = " + i;

        calcComponent(startingComponent, r, v);
        clearComponent(startingComponent);
        return success;

    }
    public void calc(ComponentClass root)
    {
        Debug.Log("calc ����");
        bool success = true;
        circuit(root, ref success);
    }

    private GameObject obj;
    private ComponentClass root;
    // ȸ�� ��ǰ�� Ŭ���Ǹ� ��ȸ����
    private void click()
    {
        if (GameObject.FindWithTag("Hovering") != null)
        {
            obj = GameObject.FindWithTag("Hovering");
            Debug.Log("obj.name : " + obj.transform.name);
            root = obj.GetComponentInChildren<ComponentClass>();
            
        }

        if (Input.GetMouseButtonDown(0))
        {
        
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
        
                obj = hit.transform.gameObject;
                Debug.Log("obj.name : " + obj.transform.name);
                root = obj.GetComponentInChildren<ComponentClass>();
                if (root)
                {
                    Debug.Log("click");
                    calc(root);
                }
            }
        }
    }

    public void circuitStart()
    {
        if (root)
        {
            calc(root);
            Debug.Log("start");
            resultPanel.SetActive(true);
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

