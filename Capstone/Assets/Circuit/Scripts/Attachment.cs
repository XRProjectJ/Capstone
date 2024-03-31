using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 부품 연결이 제대로 되기 위해 만든 최상위 클래스
// 전구를 예시로 필라멘트를 감싸고 있는 유리 부분에 전선이 붙으면 안됨
// 전선이 붙을 부분을 구현한 것이 해당 클래스의 자식들
public abstract class Attachment : MonoBehaviour
{
    // Attachment 를 가지고 있는 부품
    [SerializeField] protected ComponentClass component;
    // Attachment 에 연결된 다른 Attachment 의 개수
    protected int linkSize = 0;
    
    public ComponentClass GetComponent()
    {
        return component;
    }
    
}
