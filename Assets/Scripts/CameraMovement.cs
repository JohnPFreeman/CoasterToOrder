using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraSpeed = 50f;
    public float cameraRotateSpeed = 1f;
    public float cameraZoomSpeed = 100f;

    private Vector3 cameraMovement;
    private float currScrollSpeed;

    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = new Vector3(35f, 90f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        cameraMovement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            cameraMovement += transform.forward;

        }
        if (Input.GetKey(KeyCode.A))
        {
            cameraMovement -= transform.right;

        }
        if (Input.GetKey(KeyCode.S))
        {
            cameraMovement -= transform.forward;

        }
        if (Input.GetKey(KeyCode.D))
        {
            cameraMovement += transform.right;

        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -cameraRotateSpeed * Time.deltaTime, 0, Space.World);

        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, cameraRotateSpeed * Time.deltaTime, 0, Space.World);

        }

        cameraMovement[1] = 0;
        cameraMovement = cameraMovement.normalized * cameraSpeed * Time.deltaTime;

        transform.position += cameraMovement;


        currScrollSpeed = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * cameraZoomSpeed * currScrollSpeed * Time.deltaTime;


    }
}
