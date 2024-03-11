using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS 를 돌면서 연결된 블록들의 firstMove 를 false 로 바꿈
public class GSetFirstMove : BfsOffset
{
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.firstMove = false;
    }
}
