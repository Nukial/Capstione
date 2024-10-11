using UnityEngine;

[CreateAssetMenu(fileName = "NewFoodItem", menuName = "Items/Food Item")]
public class FoodItemData : ItemData
{
    [Header("Food Specific Data")]
    public int healthRestored;    // Lượng máu hồi phục khi tiêu thụ
    public int hungerRestored;    // Lượng độ no hồi phục khi tiêu thụ
    public float consumeTime;     // Thời gian cần để tiêu thụ item
}
