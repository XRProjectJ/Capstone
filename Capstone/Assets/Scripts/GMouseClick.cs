using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 손으로 블록을 집는 것을 마우스의 클릭으로 대체하여 프로그램을 구성했음
// 마우스 클릭이 이루어지면 클릭된 블록의 firstMove 를 true 로 하고 연결된 나머지블록들은 firstMove 를 false 로 바꿈 
public class GMouseClick : MonoBehaviour
{
    private static Camera detachManager;
    private GSetFirstMove setFirstMove;
    private GSetDistance setDistance;
    private static GDetach detach;

    private void Start()
    {
        setFirstMove = this.transform.parent.GetComponentInChildren<GSetFirstMove>();
        setDistance = this.transform.parent.GetComponentInChildren<GSetDistance>();
        detachManager = Camera.main;
        if (detachManager)
        {
            detach = detachManager.GetComponent<GDetach>();
        }
    }

    private void OnMouseDown()
    {
        GCubeClass startObject = this.transform.parent.GetComponentInChildren<GCubeClass>();
        startObject.firstMove = true;
        setFirstMove.bfs(startObject, null);
        setDistance.bfs(startObject, startObject.transform.position);
        //setDistance.bfs(startObject, startObject.transform.localPosition);
        detach.add(startObject);
        startObject.distance = new Vector3(0.0f, 0.0f, 0.0f);
    }
}
