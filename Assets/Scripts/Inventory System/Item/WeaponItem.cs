using UnityEngine;

public class WeaponItem : Item
{
    public int AttackPower { get; private set; }

    public WeaponItem(WeaponItemData data) : base(data)
    {
        AttackPower = data.attackPower;
    }

    // Các phương thức khác liên quan đến WeaponItem
}
