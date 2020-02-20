using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool useOffsetValues;

    [SerializeField] private float rotateSpeed;
    [SerializeField] private Transform pivot;

    [SerializeField] private float maxViewAngle;
    [SerializeField] private float minViewAngle;

    [SerializeField] private bool InvertY;

    // Start is called before the first frame update
    void Start()
    {
        if(!useOffsetValues)
		{
            offset = target.position - transform.position;
        }

        pivot.transform.position = target.transform.position;
        //pivot.transform.parent = target.transform;
        pivot.transform.parent = null;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        pivot.transform.position = target.transform.position;
        // Get the x position of the mouse and rotate the target

        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        pivot.Rotate(0, horizontal, 0);

        // Get the y position of the mouse and rotate the pivot
        float vertical = Input.GetAxis("Mouse Y") * rotateSpeed;
        //pivot.Rotate(-vertical, 0, 0);
        if(InvertY)
		{
            pivot.Rotate(vertical, 0, 0);
        } else
		{
            pivot.Rotate(-vertical, 0, 0);
        }

		//Liit up/down camera rotation
		if (pivot.rotation.eulerAngles.x > maxViewAngle && pivot.rotation.eulerAngles.x < 180f)
		{
            pivot.rotation = Quaternion.Euler(maxViewAngle, 0, 0);

        }

        if(pivot.rotation.eulerAngles.x > 180f && pivot.rotation.eulerAngles.x < 360f + minViewAngle)
		{
            pivot.rotation = Quaternion.Euler(315f + minViewAngle, 0, 0);
		}

        //Move the camera based on the current rotation of the target and the original offset
        float desiredYAngle = pivot.eulerAngles.y;
        float desiredXAngle = pivot.eulerAngles.x;

        Quaternion rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
        transform.position = target.position - (rotation * offset);

        //transform.position = target.position - offset;

  //      if(transform.position.y < target.position.y)
		//{
  //          transform.position = new Vector3(transform.position.x, target.position.y - .5f, transform.position.z);
		//}
        transform.LookAt(target);
        
    }
}
