using UnityEngine;
using UnityEngine.XR;

public class WeaponHolder : MonoBehaviour
{
    public WeaponBase currentWeapon;
    public Transform hand;

    public WeaponBase SetWeapon(WeaponData data)
    {
        // 기존 무기 제거
        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);

        GameObject obj = Instantiate(data.weaponPrefab, hand);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity; // 초기화

        currentWeapon = obj.GetComponent<WeaponBase>();
        currentWeapon.Initialize(data);

        return currentWeapon;
    }
    public void Attack()
    {
        currentWeapon?.Attack();
    }
}
