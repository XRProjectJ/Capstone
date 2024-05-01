using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

// 순회를 담당하는 스크립트

public class CircuitManager : MonoBehaviour
{
    struct ParallelR
    {
        public double R;
        public ComponentClass start;
    };
    // 각 부품의 전압, 전류를 구할 때 사용
    private List<ParallelR> parallelRs = new List<ParallelR>();
    //[SerializeField] private ComponentClass root;
    [SerializeField] private GameObject startComponent;
    [SerializeField] private Camera cam;


    // 회로의 전체 저항을 구하는 함수 (순환 호출로 순회)
    // 첫번째 매개변수 : 순회를 시작한 부품
    // 두번째 매개변수 : 현재 순회중인 부품
    // 세번째 매개변수 : 병렬 연결 이후 다음으로 순회해야하는 부품
    // 네번째 매개변수 : 순회의 성공 여부
    // 다섯번째 매개변수 : 현재 부품이 방문했으면 이 매개변수의 값을 visit 으로 가지고 있어야함 (방문 여부가 아님)
    public double calcEntireR(ComponentClass root, ComponentClass node, ref ComponentClass nextOfParallel, ref bool success, bool visit)
    {
        if (success == false)
        {
            //Debug.Log("실패");
            return 0;
        }
        // 회로가 중간에 끊겨있는 경우
        if (node.plus.links.Count <= 0)
        {
            //Debug.Log("중간에 끊김");
            Debug.Log(node.transform.parent.transform.name);
            success = false;
            return 0;
        }

        // 순회를 모두 성공적으로 마쳤을 때
        if (node == root && node.GetVisit() == visit)
        {
            //Debug.Log("순회 성공");
            return 0;
        }
        double result = 0;
        // 지금 부품이 이미 방문한 부품일 때
        if (node.GetVisit() == visit)
        {
            //Debug.Log("이미 방문");
            return 0;
        }
        // 지금 부품이 병렬의 끝일 때
        else if (node.plus.GetIsEndOfParallel())
        {
            //Debug.Log("병렬의 끝");
            node.SetVisit(visit);

            nextOfParallel = node.plus.links[0].GetComponent();

            // 자신의 값을 리턴
            result = node.GetR();
            return result;
        }
        // 지금 부품이 병렬의 시작일 때
        if (node.plus.GetIsStartOfParallel())
        {
            //Debug.Log("병렬의 시작");
            node.SetVisit(visit);
            // 병렬 연결의 내부의 저항 값
            double sumOfParallelR = 0;
            // 병렬 연결 이후의 값을 전부 저장
            double totalR = 0;
            for (int i = 0; i < node.plus.links.Count; i++)
            {
                sumOfParallelR += 1 / calcEntireR(root, node.plus.links[i].GetComponent(), ref nextOfParallel, ref success, visit);
            }
            // 각 부품의 전압, 전류를 구할 때 사용
            ParallelR tmp = new ParallelR();
            tmp.R = 1 / sumOfParallelR;
            tmp.start = node;
            parallelRs.Add(tmp);

            // 병렬 내부를 전부 순회 했으면 병렬 외부의 순회를 다시 진행
            totalR = calcEntireR(root, nextOfParallel, ref nextOfParallel, ref success, visit);

            result = node.GetR() + 1 / sumOfParallelR + totalR;
            return result;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            //Debug.Log("직렬");
            //Debug.Log("이름 : " + node.transform.name);
            node.SetVisit(visit);

            result = node.GetR() + calcEntireR(root, node.plus.links[0].GetComponent(), ref nextOfParallel, ref success, visit);
            return result;
        }

        
    }
    //회로의 전체 전압 구하는 함수
    // 첫번째 매개변수 : 순회를 시작한 부품
    // 두번째 매개변수 : 현재 순회중인 부품
    // 세번째 매개변수 : 병렬 이후의 전압값들
    // 네번째 매개변수 : 순회의 성공 여부
    // 다섯번째 매개변수 : 현재 부품이 방문했으면 이 매개변수의 값을 visit 으로 가지고 있어야함 (방문 여부가 아님)
    public double calcEntireV(ComponentClass root, ComponentClass node, ref ComponentClass nextOfParallel, ref bool success, bool visit)
    {
        if (success == false)
        {
            return 0;
        }
        // 회로가 중간에 끊겨있는 경우
        if (node.plus.links.Count <= 0)
        {
            Debug.Log(node.transform.parent.transform.name);
            success = false;
            return 0;
        }

        // 순회를 모두 성공적으로 마쳤을 때
        if (node == root && node.GetVisit() == visit)
        {
            return 0;
        }
        double result = 0;
        // 지금 부품이 이미 방문한 부품일 때
        if (node.GetVisit() == visit)
        {
            return 0;
        }
        // 지금 부품이 병렬의 끝일 때
        else if (node.plus.GetIsEndOfParallel())
        {
            node.SetVisit(visit);

            nextOfParallel = node.plus.links[0].GetComponent();

            // 자신의 값을 리턴
            result = node.GetV();
            return result;
        }
        // 지금 부품이 병렬의 시작일 때
        if (node.plus.GetIsStartOfParallel())
        {
            node.SetVisit(visit);
            // 병렬 연결의 내부의 전압 값
            double maxV = 0;
            // 병렬 연결 이후의 값을 전부 저장
            double totalV = 0;
            for (int i = 0; i < node.plus.links.Count; i++)
            {

                double tmpV = calcEntireV(root, node.plus.links[i].GetComponent(), ref nextOfParallel, ref success, visit);
                if(tmpV > maxV)
                {
                    maxV = tmpV;
                }
            }

            // 병렬 내부를 전부 순회 했으면 병렬 외부의 순회를 다시 진행
            totalV = calcEntireV(root, nextOfParallel, ref nextOfParallel, ref success, visit);

            result = node.GetV() + maxV + totalV;
            return result;
        }
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            node.SetVisit(visit);

            result = node.GetV() + calcEntireV(root, node.plus.links[0].GetComponent(), ref nextOfParallel, ref success, visit);
            return result;
        }


    }

    // 병렬 내부의 직렬(가지)의 저항의 합을 계산 (순환호출)
    private double calcRSerialInParallel(ComponentClass node)
    {
        if (node.plus.GetIsEndOfParallel())
        {
            return node.GetR();
        }
        return node.GetR() + calcRSerialInParallel(node.plus.links[0].GetComponent());
    }
    
    // 회로의 각 부품의 전압, 전류를 구하는 함수 (순환 호출로 순회)
    // sumOfParallelR 이 0이 아니면 병렬 내부를 방문중임을 암시, sumOfSerialR 이 0이 아니면 병렬의 가지 중 시작점이 아닌 곳을 방문중임을 암시
    public void calcComponent(ComponentClass root, ComponentClass node, double entireR, double entireV, ref bool success, bool visit, double sumOfParallelR = 0, double sumOfSerialR = 0)
    {
        //Debug.Log(node.transform.name);
        if (success == false)
        {
            return;
        }
        // 회로가 중간에 끊겨있는 경우
        if (node.plus.links.Count <= 0)
        {
            success = false;
            return;
        }

        // 순회를 모두 성공적으로 마쳤을 때
        if (node == root && node.GetVisit() == visit)
        {
            return;
        }
        // 건전지면 전압, 저항, 전류를 계산안함
        if (node.transform.GetComponent<Power>() != null)
        {
            node.SetVisit(visit);
            calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit);
            return;
        }
        // 지금 부품이 이미 방문한 부품일 때
        if (node.GetVisit() == visit)
        {
            return;
        }
        // 지금 부품이 병렬의 끝일 때
        else if (node.plus.GetIsEndOfParallel())
        {
            node.SetVisit(visit);
            double I = entireV / entireR;
            node.SetV(I * node.GetR());
            node.SetI(node.GetV() / node.GetR());
            calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit);

            return;
        }
        // 지금 부품이 병렬의 시작일 때
        if (node.plus.GetIsStartOfParallel())
        {
            node.SetVisit(visit);

            // 현재 병렬 연결의 전체 저항을 찾기
            for (int i = 0; i < parallelRs.Count; i++)
            {
                if (parallelRs[i].start == node)
                {
                    //sumOfParallelR 이 0 이 아니라면 병렬 내부임을 나타냄 -> 다음 부품에 적용
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
        // 지금 부품이 직렬 연결이면서 방문하지 않은 부품일 때 (병렬 내부는 직렬과 같음)
        else
        {
            node.SetVisit(visit);
            double I = 0;

            // 병렬 내부의 직렬 중 (가지 중) 시작점인 부품
            if (sumOfParallelR != 0 && sumOfSerialR == 0)
            {
                double tmpV = 0;
                double tmpI = 0;
                double tmpR = 0;

                I = entireV / entireR;

                tmpV = I * sumOfParallelR;
                tmpR = calcRSerialInParallel(node);
                //sumOfSerialR 이 0이 아니라면 병렬의 직렬 (가지) 중 첫번째 부품이 아니란 것을 의미 -> 다음 부품에 적용
                sumOfSerialR = tmpR;
                tmpI = tmpV / tmpR;
                node.SetI(tmpI);
                node.SetV(tmpI * node.GetR());
                calcComponent(root, node.plus.links[0].GetComponent(), entireR, entireV, ref success, visit, sumOfParallelR, sumOfSerialR);
            }
            // 병렬 내부의 직렬 중 시작 점이 아닌 부품
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
            // 일반적인 직렬
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
    // 회로의 각 부품의 전압, 전류, 저항을 초기화 하는 함수 (BFS)
    // 건전지는 전압을 제외하고 전부 0으로 초기화, 그외의 부품은 저항을 제외하고 전부 0으로 초기화
    public void clearComponent(ComponentClass root, bool visit)
    {
        Debug.Log("clearComponent 시작");
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

    // 계산과 관련된 함수를 모두 호출하는 함수
    public bool circuit(ComponentClass root, ref bool success)
    {
        ComponentClass next = null;
        double r = 0;
        double v = 0;
        double i = 0;
        // 방문을 했는지 확인하기 위한 값 : true, false 로 고정되어 있다면 순회마다 매번 초기화 시켜줘야함
        bool visit = !root.GetVisit();

        r = calcEntireR(root, root, ref next, ref success, visit);

        if (success)
        {
            Debug.Log("전체 저항 : " + r);
        }
        else
        {
            Debug.Log("CircuitManager Error : 저항 구하기 실패");
            return false;
        }

        visit = !root.GetVisit();
        next = null;
        v = calcEntireV(root, root, ref next, ref success, visit);
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

        visit = !root.GetVisit();
        calcComponent(root, root, r, v, ref success, visit);
        parallelRs.Clear();
        visit = !root.GetVisit();
        clearComponent(root, visit);
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
        for(int i=0; i <  next.Count; i++)
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
        Debug.Log("calc 시작");
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
        }
        

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
