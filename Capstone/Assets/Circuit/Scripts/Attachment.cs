using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ��ǰ ������ ����� �Ǳ� ���� ���� �ֻ��� Ŭ����
// ������ ���÷� �ʶ��Ʈ�� ���ΰ� �ִ� ���� �κп� ������ ������ �ȵ�
// ������ ���� �κ��� ������ ���� �ش� Ŭ������ �ڽĵ�
public abstract class Attachment : MonoBehaviour
{
    // Attachment �� ������ �ִ� ��ǰ
    [SerializeField] protected ComponentClass component;
    // Attachment �� ����� �ٸ� Attachment �� ����
    protected int linkSize = 0;
    
    public ComponentClass GetComponent()
    {
        return component;
    }
    
}
