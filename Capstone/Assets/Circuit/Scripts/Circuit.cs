using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

// 순회를 담당하는 스크립트

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
    // 병렬을 하나의 저항(Parallel 부품)으로 만드는 함수
    public void createParallelComponent(ComponentClass start, ComponentClass end)
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
    // 회로의 각 부품의 전압, 전류, 저항을 초기화 하는 함수 (BFS) + Parallel 부품 삭제
    // 건전지는 전압을 제외하고 전부 0으로 초기화, 그외의 부품은 저항을 제외하고 전부 0으로 초기화
    public void clearComponent(ComponentClass root)
    {
        Debug.Log("clearComponent 시작");
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
                    Debug.Log("Parallel 삭제");
                    //Debug.Log("삭제 전 cur.plus.links.Count : " + cur.plus.links.Count);
                    //Debug.Log("삭제 전 cur.plus.links.link[i] : " + tmp.gameObject.name);
                    Parallel parallel = tmp.gameObject.GetComponent<Parallel>();
                    parallel.DeleteParallel();
                    //Debug.Log("삭제 후 cur.plus.links.Count : " + cur.plus.links.Count);
                    //Debug.Log("삭제 후 cur.plus.links.link[i] : " + cur.plus.links[i].GetComponent().transform.parent.gameObject.name);
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
        r = calcEntireR(root, ref success);

        if (success)
        {
            Debug.Log("전체 저항 : " + r);
        }
        else
        {
            Debug.Log("CircuitManager Error : 저항 구하기 실패");
            return false;
        }
        v = calcEntireV(root, ref success);
        if (success)
        {
            Debug.Log("전체 전압 : " + v);
        }
        else
        {
            Debug.Log("CircuitManager Error : 전압 구하기 실패");
            return false;
        }
        i = v / r;
        Debug.Log("전체 전류 : " + i);

        calcComponent(root, r, v);
        clearComponent(root);
        return success;

    }

    // 순회 할 시작점을 만들어줌 (회로에는 보이지 않으나 코드상에 논리적으로 존재)
    // 병렬이 끝나는 지점에 만들 때 사용
    ComponentClass createStartingComponent(ComponentClass parallel, List<ComponentClass> next, bool visit)
    {
        Debug.Log("createStartingComponent 시작");
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
    // 순회 할 시작점을 만들어줌 (회로에는 보이지 않으나 코드상에 논리적으로 존재)
    // 병렬의 시작점에 만들 때 사용
    ComponentClass createStartingComponent2(ComponentClass parallel, List<ComponentClass> next, bool visit)
    {
        Debug.Log("createStartingComponent2 시작");
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

    // 순회를 시작할 지점을 자동으로 찾아줌 (마땅한게 없으면 자체적으로 생성)
    ComponentClass findStartingPoint(ComponentClass root, ref bool success, bool visit)
    {
        // FindVisit 을 다시 false 로 만들기 위해 FindVisit 을 true 로 바꾼 부품을 저장할 리스트
        List<ComponentClass> clearList = new List<ComponentClass>();
        ComponentClass result = root;
        // toggle 이 false 인 경우 :
        // 1. 직렬만 있는 경우
        // 2. 아래에 나올 반복문에서 병렬의 끝이 시작보다 먼저 나와 순회의 시작점까지 되돌아왔음에도
        // createStartingComponent() 를 호출 못한 경우
        bool toggle = false;
        Debug.Log("findStartingPoint 시작");
        if (root.plus.links.Count <= 0)
        {
            success = false;
            toggle = false;
            return result;
        }
        ComponentClass parallel = root.plus.links[0].GetComponent();
        // 괄호 문제처럼 병렬의 시작은 push하고 병렬의 끝은 pop한다
        Stack<PlusAttachment> parallelComponent = new Stack<PlusAttachment>();
        // 스택을 썼는지 안썼는지 : 안썼으면 직렬만 있는 경우임
        bool stackUse = false;
        while (parallel != root && parallel != null && parallel.GetFindVisit() != true)
        {
            parallel.SetFindVisit(true);
            clearList.Add(parallel);
            //Debug.Log("현재 위치 : " + parallel.transform.parent.transform.name);
            // 병렬의 끝이 나온 경우
            if (parallel.plus.GetIsEndOfParallel())
            {
                // 순회 순서상 병렬의 끝이 먼저 올 수 있음 => 회로가 잘못된 것이 아님
                if (parallelComponent.Count > 0)
                {
                    parallelComponent.Pop();
                    stackUse = true;
                }
            }
            // 병렬의 시작점이 나온 경우
            else if (parallel.plus.GetIsStartOfParallel())
            {
                parallelComponent.Push(parallel.plus);
                stackUse = true;
            }
            // 스택을 썼는지 확인하고 (만약 스택을 안썼으면 아직 병렬의 시작점과 끝점이 안나온 것일 수 있음)
            // 스택을 썼는데도 스택이 비어 있으면 병렬의 가장 바깥에 있다고 간주
            if (stackUse && parallelComponent.Count <= 0)
            {
                List<ComponentClass> next = new List<ComponentClass>();
                // 병렬의 시작점에 순회시작점을 만드는 경우
                if (parallel.plus.GetIsStartOfParallel())
                {
                    for (int i = 0; i < parallel.plus.links.Count; i++)
                    {
                        next.Add(parallel.plus.links[i].GetComponent());
                    }
                    result = createStartingComponent(parallel, next, visit);
                }
                // 병렬의 끝점에 순회시작점을 만드는 경우
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
            // 중간에 끊겨 있는 회로
            if (parallel.plus.links.Count <= 0)
            {
                success = false;
                toggle = false;
                return result;
            }
            // 이미 방문을 했던 부품이면 다시 방문하지 않음 
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
        // 직렬만 있거나 위의 반복문에서 마땅한 시작점을 못찾은 경우
        if (!toggle)
        {
            // 병렬이 있으나 반복문에서 마땅한 시작점을 못찾은 경우
            if (stackUse)
            {
                // 가장 맨처음에 만나는 병렬의 끝점에 시작점 생성
                while (!parallel.minus.GetIsEndOfParallel())
                {
                    parallel = parallel.plus.links[0].GetComponent();
                }
            }
            List<ComponentClass> next = new List<ComponentClass>();
            next.Add(parallel.plus.links[0].GetComponent());
            result = createStartingComponent(parallel, next, visit);
        }
        // FindVisit 을 초기화 해줌으로써 다시 사용할 수 있게 함
        for (int i = 0; i < clearList.Count; i++)
        {
            clearList[i].SetFindVisit(false);
        }
        return result;
    }
    public void calc(ComponentClass root)
    {
        /*        Debug.Log("calc 시작");
                bool success = true;
                bool visit = root.GetVisit();
                ComponentClass start = findStartingPoint(root, ref success, visit);
                circuit(start, ref success);
                //success 가 false 면 회로가 중간에 끊겨 있다는 것이고 시작점이 만들어지지 않았음
                if (success)
                {
                    // 모든 순회가 끝나면 만들었던 시작점 제거
                    Destroy(start.plus.links[0].GetComponent().transform.gameObject);
                    Destroy(start.minus.links[0].GetComponent().transform.gameObject);
                    Destroy(start.plus.links[0].GetComponent().plus.links[0].GetComponent().transform.gameObject);
                    Destroy(start.minus.links[0].GetComponent().minus.links[0].GetComponent().transform.gameObject);
                    Destroy(start.transform.gameObject);
                }*/
        Debug.Log("calc 시작");
        bool success = true;
        circuit(root, ref success);


    }
    // 회로 부품이 클릭되면 순회시작
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

