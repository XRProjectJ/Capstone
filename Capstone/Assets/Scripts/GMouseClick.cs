using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ����� ���� ���� ���콺�� Ŭ������ ��ü�Ͽ� ���α׷��� ��������
// ���콺 Ŭ���� �̷������ Ŭ���� ����� firstMove �� true �� �ϰ� ����� ��������ϵ��� firstMove �� false �� �ٲ� 
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
