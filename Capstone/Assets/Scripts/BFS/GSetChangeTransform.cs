using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS 를 돌면서 연결된 블록들의 hasChanged 를 false 로 바꿈
// hasChanged 는 유니티에서 기본적으로 제공되는 속성
public class GSetChangeTransform : BfsOffset
{
    override public void coreFunction(GCubeClass obj, Vector3 offset)
    {
        obj.mBlock.transform.hasChanged =false;
    }
}
