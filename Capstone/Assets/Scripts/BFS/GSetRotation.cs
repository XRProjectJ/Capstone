using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS 를 돌면서 연결된 블록들을 회전 시킴

public class GSetRotation : BfsGCube
{
    // 회전 시키는 방법 3가지
    // 1. (부분적 성공) 회전 중심 객체로 이동시킨 뒤 회전 시킨 후 회전 된 회전축을 기준으로 원위치
    // -> 회전 축이 바뀌면 이상한 회전이 이루어짐
    // -> 오일러 각도로 중간에 변화를 줘서 생기는 문제라 생각하고 쿼터니언으로만 시도해보았으나 동일한 현상
    // 2. (실패) 회전 중심 객체를 중심으로 원 운동 시키기 (회전은 곧 원 운동이라는 생각으로 접근)
    // -> 연결된 객체의 기존 위치 정보가 일부 소실되는 문제
    // 3. (실패) opengl 에서 처럼 회전 중심 객체의 월드변환 행렬에 연결된 객체의 월드변환 행렬 곱하기
    // -> 유니티에서 어떻게 코드로 옮기는 지 모르겠음 
    override public void bfsMainFunction(GCubeClass obj, GCubeClass root)
    {

        obj.firstMove = false;
        obj.mBlock.transform.Translate(obj.distance);
        Quaternion prevRot = root.getPrevRotQua();
        // 이전에 한 회전을 취소한 후 새로운 회전을 취함 => 여기서 문제가 생기는 듯함 : 변화량 구하는 법이 필요
        /*Quaternion parentRot = Quaternion.Inverse(prevRot) * root.mBlock.transform.rotation;
        Quaternion myRot = obj.mBlock.transform.rotation;
        obj.mBlock.transform.rotation = parentRot * myRot;*/
        
        // 이전 블록의 회전을 반영못함
        obj.mBlock.transform.rotation = root.mBlock.transform.rotation;

        obj.mBlock.transform.Translate(-obj.distance);

    }
    public void bfsMainFunction2(GCubeClass obj, GCubeClass root)
    {
        obj.firstMove = false;
        Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f) - Quaternion.ToEulerAngles(root.mBlock.transform.rotation);
        float posX = obj.distance.magnitude * Mathf.Cos(offset.y) * Mathf.Cos(offset.x);
        float posY = obj.distance.magnitude * Mathf.Sin(offset.x);
        float posZ = obj.distance.magnitude * Mathf.Sin(offset.y) * Mathf.Cos(offset.x);
        obj.mBlock.transform.position = root.mBlock.transform.position + new Vector3(posX, posY, posZ);
       
        Vector3 rotOffset = Quaternion.ToEulerAngles(root.mBlock.transform.rotation) - root.getPrevRot();
        Quaternion parentRot = Quaternion.EulerAngles(rotOffset);
        Quaternion myRot = obj.mBlock.transform.rotation;
        obj.mBlock.transform.rotation = parentRot * myRot;

        //obj.mBlock.transform.localPosition += obj.distance;
    }
    public void bfsMainFunction3(GCubeClass obj, GCubeClass root)
    {
        // 행렬 곱셈을 통해 부모-자식 관계 따라하기

        Matrix4x4 rootWorldMat = root.mBlock.transform.localToWorldMatrix;
        Matrix4x4 objWorldMat = obj.mBlock.transform.localToWorldMatrix;
        objWorldMat *= rootWorldMat;
        obj.firstMove = false;
        //obj.mBlock.transform.localToWorldMatrix = objWorldMat;

        /*Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f) - Quaternion.ToEulerAngles(root.mBlock.transform.rotation);
        float posX = obj.distance.magnitude * Mathf.Cos(offset.y) * Mathf.Cos(offset.x);
        float posY = obj.distance.magnitude * Mathf.Sin(offset.x);
        float posZ = obj.distance.magnitude * Mathf.Sin(offset.y) * Mathf.Cos(offset.x);
        obj.mBlock.transform.position = root.mBlock.transform.position + new Vector3(posX, posY, posZ);

        Vector3 rotOffset = Quaternion.ToEulerAngles(root.mBlock.transform.rotation) - root.getPrevRot();
        Quaternion parentRot = Quaternion.EulerAngles(rotOffset);
        Quaternion myRot = obj.mBlock.transform.rotation;
        obj.mBlock.transform.rotation = parentRot * myRot;*/

        //obj.mBlock.transform.localPosition += obj.distance;
    }
}
