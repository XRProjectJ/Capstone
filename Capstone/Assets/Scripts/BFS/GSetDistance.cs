using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 블록 덩어리에서 선택된 블록과 그렇지 않은 블록들의 거리를 구함 - > 회전을 하기 위해서 필요
public class GSetDistance : BfsOffset
{
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.distance = offset - obj.transform.position;
        //obj.distance = offset - obj.transform.localPosition;
    }
}
