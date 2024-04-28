using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayManager : MonoBehaviour
{
    private GameObject objectToActivate; // Ȱ��ȭ�� ���� ������Ʈ
    public string spawnPointName;
    public float delayInSeconds = 3f; // ��ٸ� �ð� (��)

    private void Start()
    {
        objectToActivate = this.gameObject;
        objectToActivate.SetActive(false);
        
        // ������ �ð� �Ŀ� ���� ������Ʈ�� Ȱ��ȭ
        Invoke("ActivateObject", delayInSeconds);
    }

    private void ActivateObject()
    {
        objectToActivate.transform.position = GameObject.Find(spawnPointName).transform.position; 
        objectToActivate.SetActive(true);
    }
}
