using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 클릭된 블록을 움직이도록 하는 스크립트
// 방향키는 이동
// w,s,d,a 는 회전
// h,l 은 상하 이동
public class playerControll : MonoBehaviour
{
    private GCubeClass cube;
    // Start is called before the first frame update
    void Start()
    {
        cube = this.GetComponentInChildren<GCubeClass>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) && cube.firstMove)
        {
            transform.Translate(transform.forward * 2.0f * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.DownArrow) && cube.firstMove)
        {
            transform.Translate(transform.forward * -2.0f * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.RightArrow) && cube.firstMove)
        {
            transform.Translate(transform.right * 2.0f * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.LeftArrow) && cube.firstMove)
        {
            transform.Translate(transform.right * -2.0f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.H) && cube.firstMove)
        {
            transform.Translate(Vector3.up * 2.0f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.L) && cube.firstMove)
        {
            transform.Translate(Vector3.up * -2.0f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W) && cube.firstMove)
        {
            transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f) * 5.0f * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.S) && cube.firstMove)
        {
            transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f) * -5.0f * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.D) && cube.firstMove)
        {
            transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f) * -5.0f * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.A) && cube.firstMove)
        {
            transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f) * 5.0f * Time.deltaTime);
        }
    }
}
