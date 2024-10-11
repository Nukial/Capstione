using UnityEngine;

[CreateAssetMenu(fileName = "NewDrinkItem", menuName = "Items/Drink Item")]
public class DrinkItemData : ItemData
{
    [Header("Drink Specific Data")]
    public int thirstRestored; // Lượng khát được hồi phục khi tiêu thụ
    public int healthRestored; // Lượng máu hồi phục (nếu có)
    public float consumeTime;  // Thời gian cần để tiêu thụ item
}
