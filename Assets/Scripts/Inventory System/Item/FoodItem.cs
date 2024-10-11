using Unity.VisualScripting;
using UnityEngine;

public class FoodItem : Item
{
    public FoodItemData FoodData { get; private set; }

    public FoodItem(FoodItemData data) : base(data)
    {
        FoodData = data;
    }

    public void Consume(PlayerStats playerStats)
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is null. Cannot consume food.");
            return;
        }

        // Hồi phục máu cho người chơi
        if (FoodData.healthRestored > 0)
        {
            playerStats.RestoreHealth(FoodData.healthRestored);
        }

        // Hồi phục độ no cho người chơi
        if (FoodData.hungerRestored > 0)
        {
            playerStats.RestoreHunger(FoodData.hungerRestored);
        }

        // Xử lý thời gian tiêu thụ nếu cần thiết
        // Ví dụ: Bắt đầu Coroutine để chờ thời gian tiêu thụ

        Debug.Log($"Player đã tiêu thụ {FoodData.itemName}");
    }
}
