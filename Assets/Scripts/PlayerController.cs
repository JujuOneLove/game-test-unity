using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    //public Rigidbody theRB;
    public float jumpForce;
    public CharacterController controller;

    private Vector3 _moveDirection;
    public float gravityScale;

    public Animator anim;
    public Transform pivot;
    public float rotateSpeed;

    public GameObject playerModel;
    private int _tolerance = 0;

    // Start is called before the first frame update
    void Start() {
        //theRB = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //theRB.velocity = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, theRB.velocity.y, Input.GetAxis("Vertical") * moveSpeed);

        // if(Input.GetButtonDown("Jump")){
        //     theRB.velocity = new Vector3(theRB.velocity.x, jumpForce, theRB.velocity.z);
        // }

        //moveDirection = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, moveDirection.y, Input.GetAxis("Vertical") * moveSpeed);
        float yStore = _moveDirection.y;
        _moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        _moveDirection = _moveDirection.normalized * moveSpeed;
        _moveDirection.y = yStore;

        if(controller.isGrounded)
        {
            _moveDirection.y = 0f;
            if (Input.GetButtonDown("Jump"))
            {
                _moveDirection.y = jumpForce;
            }
        }

        _moveDirection.y = _moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(_moveDirection * Time.deltaTime);

        //Move the player in different directions based on look direction
        if(Math.Abs(Input.GetAxis("Horizontal")) > _tolerance || Math.Abs(Input.GetAxis("Vertical")) > _tolerance)
		{
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(_moveDirection.x, 0f, _moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
		}

        anim.SetBool("isGrounded", controller.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal"))));
    }
}
