using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public WeaponHolder weaponHolder;

    [Header("기본 무기")]
    public WeaponData defaultWeapon;
    public WeaponBase activeWeapon;

    // 플레이어가 가진 무기 목록
    public List<WeaponData> inventory = new List<WeaponData>();
    private int currentIndex = 0; // 현재 장착 무기 인덱스

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // ★ 게임 시작 시 기본 무기가 자동 지급되도록
        if (defaultWeapon != null)
        {
            AddWeapon(defaultWeapon);
        }
    }

    public void AddWeapon(WeaponData newWeapon)
    {
        if (!inventory.Contains(newWeapon))
            inventory.Add(newWeapon);

        EquipWeapon(newWeapon); // WeaponHolder에서 새 무기 Instantiate
    }


    public void EquipWeapon(WeaponData data)
    {
        activeWeapon = weaponHolder.SetWeapon(data);

        PlayerController pc = FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            pc.currentWeapon = activeWeapon;
        }

        // 현재 인덱스 업데이트
        currentIndex = inventory.IndexOf(data);
    }

    // 다음 무기로 교체
    public void SwitchWeapon()
    {
        if (inventory.Count <= 1) return;

        currentIndex++;
        if (currentIndex >= inventory.Count)
            currentIndex = 0;

        EquipWeapon(inventory[currentIndex]);
    }

    public void UpgradeWeapon(WeaponData target)
    {
        target.level++;
    }
}
