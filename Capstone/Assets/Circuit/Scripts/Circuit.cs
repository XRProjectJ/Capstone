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

// 순회를 담당하는 스크립트

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

    // 병렬의 시작과 끝을 맞추는 함수
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
    // 회로의 병렬 부분을 찾고 병렬인 부분을 하나의 저항으로 바꾸는 함수 (DFS)
    public bool findParallel(ComponentClass root)
    {
        Debug.Log("findParallel 시작");
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
    // 병렬을 하나의 저항(Parallel 부품)으로 만드는 함수
    public Parallel createParallelComponent(ComponentClass start, ComponentClass end)
    {
        Debug.Log("createParallelComponent 시작");
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
    // 회로 전체 저항을 계산하는 함수
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
    // 회로 전체 전압을 구하는 함수
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
    // 부품별 전압, 전류 계산하는 함수
    public void calcComponent(ComponentClass root, double entireR, double entireV)
    {
        Debug.Log("calcComponent 시작");
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
            // 병렬에 속하지 않는 부품은 전체 회로의 저항과 전압으로 계산
            if (cur.GetRootParallel() == null)
            {
                calcR = entireR;
                calcV = entireV;
            }
            // 병렬에 속하는 부품은 병렬 가지의 저항과 병렬 회로에 걸리는 전압으로 계산
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
            // Parallel 부품인 경우 내부도 순회 해야함
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
    // 회로의 각 부품의 전압, 전류, 저항을 초기화 하는 함수 (BFS) + Parallel 부품 삭제
    // 건전지는 전압을 제외하고 전부 0으로 초기화, 그외의 부품은 저항을 제외하고 전부 0으로 초기화
    public void clearComponent(ComponentClass root)
    {
        Debug.Log("clearComponent 시작");
        Queue<ComponentClass> q = new Queue<ComponentClass>();
        bool visit = !root.GetVisit();
        root.SetVisit(visit);
        q.Enqueue(root);
        // Parallel 부품 없애는 부분
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
    // 순회의 시작점을 찾아줌 (null 인 경우는 직렬만 있는 경우)
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
    // 계산과 관련된 함수를 모두 호출하는 함수
    public bool circuit(ComponentClass root, ref bool success)
    {
        double r = 0;
        double v = 0;
        double i = 0;
        success = findParallel(root);
        if (!success)
        {
            Debug.Log("회로에 문제 있음");
            return false;
        }
        ComponentClass startingComponent = findStartingComponent();
        if(startingComponent == null) {
            startingComponent = root;
        }
        r = calcEntireR(startingComponent, ref success);

        if (success)
        {
            Debug.Log("전체 저항 : " + r);
            //Rt.text = "R = " + r;
        }
        else
        {
            Debug.Log("CircuitManager Error : 저항 구하기 실패");
            return false;
        }
        v = calcEntireV(startingComponent, ref success);
        if (success)
        {
            Debug.Log("전체 전압 : " + v);
            //Vt.text = "V = " + v;
        }
        else
        {
            Debug.Log("CircuitManager Error : 전압 구하기 실패");
            return false;
        }
        i = v / r;
        Debug.Log("전체 전류 : " + i);
        //It.text = "I = " + i;

        calcComponent(startingComponent, r, v);
        clearComponent(startingComponent);
        return success;

    }
    public void calc(ComponentClass root)
    {
        Debug.Log("calc 시작");
        bool success = true;
        circuit(root, ref success);
    }

    private GameObject obj;
    private ComponentClass root;
    // 회로 부품이 클릭되면 순회시작
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

