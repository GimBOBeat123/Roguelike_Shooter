using UnityEngine;

public enum WeaponType
{
    Melee,   // 검, 방망이
    Gun,     // 총
    Bow     // 활
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "SO/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("무기 공통 데이터")]
    public string weaponName;
    public Sprite sprite;
    public WeaponType weaponType;

    public GameObject weaponPrefab;


    [Header("공격 공통 스텟")]
    public float damage;
    public float attackSpeed;
    public float attackDistance;

    [Header("원거리 무기 전용")]
    public float bulletSpeed;
    public GameObject bulletPrefab;

    [Header("무기 레벨")]
    public int level;
}
