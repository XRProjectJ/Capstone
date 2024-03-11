using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS 를 돌면서 연결된 블록들의 위치를 offset 만큼 이동시킴
public class GSetPosition : BfsOffset
{
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.mBlock.transform.position += offset;
    }
}
