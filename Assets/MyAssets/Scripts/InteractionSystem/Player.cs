using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask pickableLayerMask;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private TextMeshProUGUI pickUpText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField][Min(1)] private float pickUpDistance = 3f;
    [SerializeField] private GameObject WeaponHolster;
    [SerializeField] private GameObject EquipedWeapon;
    [SerializeField] private InputActionReference interactionInput, dropInput, useInput;
    [SerializeField] private PlayerCombat playerCombat;
    private RaycastHit hit;

    private void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
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
            // Check if its a weapon
            Weapon weapon = hit.collider.GetComponent<Weapon>();
            if (weapon != null)
            {
                if (EquipedWeapon != null)
                {
                    Drop(obj);
                }
                EquipedWeapon = hit.collider.gameObject;
                EquipedWeapon.transform.SetParent(WeaponHolster.transform, false);
                EquipedWeapon.transform.localPosition = Vector3.zero;
                EquipedWeapon.transform.localRotation = Quaternion.identity;
                EquipedWeapon.GetComponent<Rigidbody>().isKinematic = true;
                EquipedWeapon.GetComponent<Collider>().enabled = false;
                EquipedWeapon.layer = 0;
                return;
            }
            // Check if its a consumable(mushroom)
            Consumable consumable = hit.collider.GetComponent<Consumable>();
            if (consumable != null)
            {
                consumable.Consume();
                pickUpText.gameObject.SetActive(false);
            }
            // Check if its a door
            if (hit.collider.GetComponent(typeof(Door)) is Door door)
            {
                if (door.isActive)
                {
                    door.OpenDoor();
                }
            }
            if (hit.collider.gameObject.tag == "Shopkeeper")
            {
                // Open shop
                if (GameObject.Find("Shop").TryGetComponent(out ShopUI shop))
                {
                    shop.OpenShop();
                }
            }
            if (hit.collider.gameObject.tag == "QuitTotem")
            {
                gameObject.transform.root.GetComponentInChildren<PlayerCombat>().SavePlayerStats();
                Application.Quit();
            }
        }
    }

    private void Update()
    {
        coinsText.text = playerCombat.stats.coins.ToString();
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * pickUpDistance, Color.red);
        pickUpText.gameObject.SetActive(false);
        if (Physics.SphereCast(playerCameraTransform.position, 0.5f, playerCameraTransform.forward, out hit, pickUpDistance, pickableLayerMask))
        {
            pickUpText.gameObject.SetActive(true);
            pickUpText.transform.position = Camera.main.WorldToScreenPoint(hit.collider.transform.position + Vector3.up * 1.1f);
            pickUpText.transform.rotation = Quaternion.identity;
            if (hit.collider.GetComponent<Weapon>() != null)
            {
                pickUpText.transform.rotation = Quaternion.identity;
                pickUpText.text = "Press E to Pick Up <br>" + hit.collider.name + " (" + hit.collider.GetComponent<Weapon>().damage.x.ToString("0.00") + "-" + hit.collider.GetComponent<Weapon>().damage.y.ToString("0.00") + " dmg)";
            }
            else if (hit.collider.GetComponent(typeof(Door)) is Door door)
            {
                if (door.isActive)
                {
                    pickUpText.text = "Press E to Enter Door";
                }
            }
            else if (hit.collider.gameObject.tag == "Shopkeeper")
            {
                pickUpText.text = "Press E to Open Shop";
            }
            else if (hit.collider.gameObject.tag == "QuitTotem")
            {
                pickUpText.text = "Press E to Save and Quit";
            }
            else
            {
                pickUpText.text = "Press E to Eat ";
            }
        }
    }

    public void addCoins(int goldValue)
    {
        playerCombat.stats.coins += goldValue;
    }

    public int getCoins()
    {
        return playerCombat.stats.coins;
    }

    public bool removeCoins(int coinsToRemove)
    {
        if (coinsToRemove <= playerCombat.stats.coins)
        {
            playerCombat.stats.coins -= coinsToRemove;
            return true;
        }
        return false;
    }
}