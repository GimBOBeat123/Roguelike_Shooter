using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponData;

    private bool canPickUp = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Enter: " + other.name);
        if (other.CompareTag("Player"))
            canPickUp = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Trigger Exit: " + other.name);
        if (other.CompareTag("Player"))
            canPickUp = false;
    }


    private void Update()
    {
        if (canPickUp)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("Picking up " + weaponData.weaponName);
                WeaponManager.Instance.AddWeapon(weaponData);
                Destroy(gameObject);
            }
        }
    }

}
