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
    [SerializeField] private float distanceWithFloor = 1.1f;
    [SerializeField] private Rigidbody rigidbody;

    private Controls _controls;

    private Vector3 _moveDirection;
    private bool _isMove, _isDancing, _initTargetTimeDancing, _canHurt = false;
    private float _targetTimeDancing;

    private bool _isJumping;

    [SerializeField] private Animator anim;
    [SerializeField] private Camera camera;
    [SerializeField] private float rotateSpeed;

    [SerializeField] private GameObject playerModel;
    private float _tolerance = 0f;
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
        _controls = new Controls();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        InvokeRepeating("Speak", 0f, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movementInput = _controls.Player.Movement.ReadValue<Vector2>();
        MoveCamera(movementInput);

        ActionsDancingScream();

        Animation(movementInput);
    }

    private void FixedUpdate()
    {
        Vector2 movementInput = _controls.Player.Movement.ReadValue<Vector2>();

        Move(movementInput);
    }

    private void LateUpdate()
    {
        Vector2 movementInput = _controls.Player.Movement.ReadValue<Vector2>();
    }

    private void OnEnable() => _controls.Player.Enable();

    private void OnDisable() => _controls.Player.Disable();
    
    public void Jump()
    {
        if (Grounded())
        {
            _isJumping = true;
        }
    }

    private void Speak()
    {
        if (Grounded() && !_source.isPlaying)
        {
            if (Random.Range(0, 100) % 2 != 0)
            {
                _source.PlayOneShot(speak1);
            }
            else _source.PlayOneShot(speak2);
        }
    }

    private void Scream()
    {
        if (!_source.isPlaying && transform.position.y <= -5f)
        {
            _source.PlayOneShot(scream);
        }
    }

    private void Move(Vector2 movementInput)
    {
        if (_isJumping)
        {
            _isJumping = false;
            rigidbody.AddForce(Vector3.up * jumpForce);
        }

        _isMove = Mathf.Abs(movementInput.y + Mathf.Abs(movementInput.x)) > 0f;
        _moveDirection = transform.TransformDirection(new Vector3(movementInput.x, 0f, movementInput.y));
        _moveDirection = _moveDirection.normalized * moveSpeed;
       
        rigidbody.MovePosition(transform.position + _moveDirection * Time.fixedDeltaTime);
    }

    private void Animation(Vector2 movementInput)
    {
        anim.SetBool(IsGrounded, Grounded());
        anim.SetBool(IsDancing, _isDancing);
        anim.SetFloat(Speed, (Mathf.Abs(movementInput.y) + Mathf.Abs(movementInput.x)));
    }

    private void MoveCamera(Vector2 movementInput)
    {
        //Move the player in different directions based on look direction
        if (Math.Abs(movementInput.x) > _tolerance || Math.Abs(movementInput.y) > _tolerance)
        {
            transform.rotation = Quaternion.Euler(0f, camera.transform.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(_moveDirection.x, 0f, _moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation,
                rotateSpeed * Time.deltaTime);
        }
    }

    private void ActionsDancingScream()
    {
        if (Grounded())
        {
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

            if (_canHurt)
            {
                _source.PlayOneShot(hurt);
                _canHurt = false;
            }
        }
        else
        {
            Scream();
            _initTargetTimeDancing = false;
            _isDancing = false;
            _canHurt = true;
        }
    }
    
    private bool Grounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, Mathf.Infinity))
            return hit.distance <= distanceWithFloor;

        return false;
    }
}