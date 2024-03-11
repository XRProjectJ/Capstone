using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 회전을 하는 방법 
// 1. 회전의 중심이 되는 어떤 블록을 A 라고 하면, A 에 연결된 모든 블록을 A 의 위치로 옮김
// 2. 회전 중심이 되는 A 가 회전한 만큼 연결된 모든 블록을 로컬공간에서 회전시킴
// 3. 회전이 된 상태에서 1번 과정에서 이동한 만큼 그대로 이동하면 됨
public class GRotationProcess : BfsOffset
{
    public GSetRotation setRotation;
    private void Start()
    {
        setRotation = GetComponent<GSetRotation>();
    }
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.firstMove = false;
        obj.mBlock.transform.Translate(obj.distance);
        Quaternion parentRot = Quaternion.EulerAngles(offset);
        Quaternion myRot = obj.mBlock.transform.rotation;
        obj.mBlock.transform.rotation = parentRot * myRot;
        obj.mBlock.transform.Translate(-obj.distance);

    }
}
