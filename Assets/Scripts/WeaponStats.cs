using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon", order = 0)]
public class WeaponStats : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public float fireRate = 3f;
    public int weaponDamage = 1;
    public Vector2 spread;
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public int bulletsPerShot = 1;
    public Color bulletTint = Color.white;
    public AudioClip[] shootingClips;
    public bool piercing;
    public float range = 20f;
    public Vector3 firingPoint;
    public int ammo = 10;
    public Color laser = Color.clear;
}