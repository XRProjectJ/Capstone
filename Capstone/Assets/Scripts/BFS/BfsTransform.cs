using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS 를 자주 사용하다보니 template method 로 만들었음
// BFS 가 필요한 경우 이 스크립트를 상속한 다음 bfsMainFunction 에 원하는 기능을 넣으면 됨
// 이 스크립트는 Transform root 매개변수를 받는 BFS 용도
public abstract class BfsTransform : BfsTemplate<Transform>
{
    override public abstract void bfsMainFunction(GCubeClass obj, Transform root);
}
