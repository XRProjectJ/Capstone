using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 크기 변환을 수행하는 스크립트
// -> 현재로써는 크기가 변했을 때 블록의 위치가 움직이지 않아서 블록의 간격이 달라져 보이는 문제가 있음 
public class GSetScale : BfsOffset
{
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.transform.parent.localScale = offset;
    }
}
