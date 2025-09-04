using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 2f;
    public float zoomSpeed = 10f;
    [Header("�����˳�����")]
    public Vector3 thirdPersonOffset = new Vector3(1.3f, -0.5f, -2f);

    [Header("��һ�˳�����")]
    public Vector3 firstPersonOffset = new Vector3(0, 1.6f, 0);

    private bool isFirstPerson = false;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private float zoomFactor = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //���������
    }

    void Update()
    {
        // �л��ӽ�
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
        }

        // �����������
        xRotation += Input.GetAxis("Mouse X") * rotationSpeed;
        yRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;
        yRotation = Mathf.Clamp(yRotation, -80f, 80f);
    }

    void LateUpdate()
    {
        if (player == null) return;
        // ���������λ�ú���ת
        Quaternion rotation = Quaternion.Euler(yRotation, xRotation, 0);
        Quaternion rotationPlayer = Quaternion.Euler(0, xRotation, 0);
        if (isFirstPerson)
        {
            // ��һ�˳��ӽ�
            this.transform.position = player.position + rotation * firstPersonOffset;
            this.transform.rotation = rotation;
            player.rotation = rotationPlayer;
        }
        else
        {
            // �����˳��ӽ�
            zoomFactor = Mathf.Lerp(zoomFactor, 1, Time.deltaTime * zoomSpeed);
            this.transform.position = rotation * thirdPersonOffset * zoomFactor + player.position;
            this.transform.rotation = rotation;
            player.rotation = rotationPlayer;
        }
    }
}
