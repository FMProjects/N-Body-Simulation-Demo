using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualCam : MonoBehaviour
{
    [SerializeField] float speed=1;
    [SerializeField] float sensitivity=1;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float scaledSpeed = Input.GetKey(KeyCode.Mouse0)? speed * 2 : speed;


            if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.forward * Time.deltaTime * scaledSpeed);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.back * Time.deltaTime * scaledSpeed);
        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * Time.deltaTime * scaledSpeed);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * Time.deltaTime * scaledSpeed);
        if (Input.GetKey(KeyCode.LeftShift))
            transform.Translate(Vector3.down * Time.deltaTime * scaledSpeed);
        if (Input.GetKey(KeyCode.Space))
            transform.Translate(Vector3.up * Time.deltaTime * scaledSpeed);

        transform.rotation*= Quaternion.Euler(-Input.GetAxis("Mouse Y")*sensitivity,Input.GetAxis("Mouse X")*sensitivity, 0);

    }
}
