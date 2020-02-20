using System;
using System.Collections;
using System.Collections.Generic;
using Unity.UNetWeaver;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private CharacterController controller;

    private Vector3 _moveDirection;
    private bool _isMove, _isDancing, _initTargetTimeDancing, _canHurt = false;
    private float _targetTimeDancing;
    [SerializeField] private float gravityScale;

    [SerializeField] private Animator anim;
    [SerializeField] private Transform pivot;
    [SerializeField] private float rotateSpeed;

    [SerializeField] private GameObject playerModel;
    private int _tolerance = 0;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int IsDancing = Animator.StringToHash("isDancing");

    [SerializeField] private AudioClip scream;
    [SerializeField] private AudioClip hurt;
    [SerializeField] private AudioClip speak1;
    [SerializeField] private AudioClip speak2;

    private AudioSource _source;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        InvokeRepeating("Speak", 0f, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        float yStore = _moveDirection.y;
        _moveDirection = (transform.forward * Input.GetAxis("Vertical")) +
                         (transform.right * Input.GetAxis("Horizontal"));
        _moveDirection = _moveDirection.normalized * moveSpeed;
        _moveDirection.y = yStore;
        _isMove = Mathf.Abs(Input.GetAxis("Vertical") + Mathf.Abs(Input.GetAxis("Horizontal"))) > 0f;

        if (controller.isGrounded)
        {
            _moveDirection.y = 0f;
            if (Input.GetButtonDown("Jump"))
            {
                _moveDirection.y = jumpForce;
            }
            
            if (!_isMove && !_isDancing && !_initTargetTimeDancing)
            {
                _targetTimeDancing = 10f;
                _initTargetTimeDancing = true;
            }
            else if (!_isMove && !_isDancing && _initTargetTimeDancing)
            {
                _targetTimeDancing -= Time.deltaTime;
            }
            else _initTargetTimeDancing = false;

            _isDancing = _targetTimeDancing <= 0.0f && !_isMove;
        }
        else
        {
            if (!_source.isPlaying && transform.position.y <= -5f)
            {
                _source.PlayOneShot(scream);
            }
            _initTargetTimeDancing = false;
            _isDancing = false;
        }

        _moveDirection.y = _moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(_moveDirection * Time.deltaTime);

        //Move the player in different directions based on look direction
        if (Math.Abs(Input.GetAxis("Horizontal")) > _tolerance || Math.Abs(Input.GetAxis("Vertical")) > _tolerance)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(_moveDirection.x, 0f, _moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation,
                rotateSpeed * Time.deltaTime);
        }

        anim.SetBool(IsGrounded, controller.isGrounded);
        anim.SetBool(IsDancing, _isDancing);
        anim.SetFloat(Speed, (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal"))));
    }

    void Speak()
    {
        if (controller.isGrounded && !_source.isPlaying)
        {
            if (Random.Range(0, 100) % 2 != 0)
            {
                _source.PlayOneShot(speak1);
            }
            else _source.PlayOneShot(speak2);
        }
    }
}