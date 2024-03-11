using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS 를 자주 사용하다보니 template method 로 만들었음
// BFS 가 필요한 경우 이 스크립트를 상속한 다음 coreFunction 에 원하는 기능을 넣으면 됨
// 이 스크립트는 Vector3 offset 매개변수를 받는 BFS 용도
public abstract class BfsOffset : BfsTemplate<Vector3?>
{
    public Vector3 offsetSettingTemplate(Vector3? nullableOffset = null)
    {
        Vector3 offset;
        Vector3 defaultOffset = new Vector3(0.0f, 0.0f, 0.0f);
        offset = nullableOffset ?? defaultOffset;
        return offset;
    }
    public abstract void coreFunction(GCubeClass obj, Vector3 offset);
    override public void bfsMainFunction(GCubeClass obj, Vector3? nullableOffset = null)
    {
        Vector3 offset = offsetSettingTemplate(nullableOffset);
        coreFunction(obj, offset);
    }
}
