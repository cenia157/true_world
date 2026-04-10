using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public int damage = 10;
    private Collider hitCol;

    private HashSet<Monster> hitMonsters = new HashSet<Monster>();

    void Start()
    {
        hitCol = GetComponent<Collider>();
        hitCol.enabled = false;
    }

    public void EnableHitbox()
    {
        hitMonsters.Clear();
        hitCol.enabled = true;
    }

    public void DisableHitbox()
    {
        hitCol.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitCol.enabled) return;

        Monster monster = other.GetComponent<Monster>();
        if (monster == null) return;

        if (hitMonsters.Contains(monster)) return;

        hitMonsters.Add(monster);
        monster.TakeDamage(damage);
    }
}