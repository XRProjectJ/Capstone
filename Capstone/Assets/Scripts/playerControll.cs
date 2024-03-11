using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ŭ���� ����� �����̵��� �ϴ� ��ũ��Ʈ
// ����Ű�� �̵�
// w,s,d,a �� ȸ��
// h,l �� ���� �̵�
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
