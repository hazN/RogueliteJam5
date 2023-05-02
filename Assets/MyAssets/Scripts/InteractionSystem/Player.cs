using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask pickableLayerMask;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private TextMeshProUGUI pickUpText;
    [SerializeField][Min(1)] private float pickUpDistance = 3f;
    [SerializeField] private GameObject WeaponHolster;
    [SerializeField] private GameObject EquipedWeapon;
    [SerializeField] private InputActionReference interactionInput, dropInput, useInput;

    private RaycastHit hit;
    private void Start()
    {
        interactionInput.action.performed += Interact;
        dropInput.action.performed += Drop;
        useInput.action.performed += Use;
    }

    private void Use(InputAction.CallbackContext obj)
    {
    }

    private void Drop(InputAction.CallbackContext obj)
    {
        if (EquipedWeapon != null)
        {
            EquipedWeapon.transform.SetParent(null);
            EquipedWeapon.GetComponent<Rigidbody>().isKinematic = false;
            EquipedWeapon.GetComponent<CapsuleCollider>().enabled = true;
            EquipedWeapon.GetComponent<BoxCollider>().enabled = false;
            EquipedWeapon.gameObject.tag = "Untagged";
            EquipedWeapon.gameObject.layer = 11;
            EquipedWeapon = null;
        }
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        if (hit.collider != null)
        {
            Weapon weapon = hit.collider.GetComponent<Weapon>();
            if (weapon != null && EquipedWeapon == null)
            {
                EquipedWeapon = hit.collider.gameObject;
                EquipedWeapon.transform.SetParent(WeaponHolster.transform, false);
                EquipedWeapon.transform.localPosition = Vector3.zero;
                EquipedWeapon.transform.localRotation = Quaternion.identity;
                EquipedWeapon.GetComponent<Rigidbody>().isKinematic = true;
                EquipedWeapon.GetComponent<Collider>().enabled = false;
            }
        }
    }

    void Update()
    {
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * pickUpDistance, Color.red);
        if (hit.collider != null)
        {
            hit.collider.GetComponent<Highlight>().ToggleHighlight(false);
            pickUpText.gameObject.SetActive(false);
        }
        if (EquipedWeapon != null)
        {
            return;
        }
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, pickUpDistance, pickableLayerMask))
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            pickUpText.gameObject.SetActive(true);
            pickUpText.transform.position = Camera.main.WorldToScreenPoint(hit.collider.transform.position + Vector3.up * 1.1f);
            pickUpText.text = "Press E to Pick Up " + hit.collider.name;
        }
    }
}
