using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayManager : MonoBehaviour
{
    private GameObject objectToActivate; // 활성화할 게임 오브젝트
    public string spawnPointName;
    public float delayInSeconds = 3f; // 기다릴 시간 (초)

    private void Start()
    {
        objectToActivate = this.gameObject;
        objectToActivate.SetActive(false);
        
        // 지정한 시간 후에 게임 오브젝트를 활성화
        Invoke("ActivateObject", delayInSeconds);
    }

    private void ActivateObject()
    {
        objectToActivate.transform.position = GameObject.Find(spawnPointName).transform.position; 
        objectToActivate.SetActive(true);
    }
}
