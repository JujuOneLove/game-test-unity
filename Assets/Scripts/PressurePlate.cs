using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private GameObject plateform;
    private void OnTriggerEnter(Collider other)
    {
        plateform.SetActive(false);
    }
    
    private void OnTriggerExit(Collider other)
    {
        plateform.SetActive(true);
    }
}
