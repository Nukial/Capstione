using UnityEngine;

public class DrinkItem : Item
{
    public DrinkItemData DrinkData { get; private set; }

    public DrinkItem(DrinkItemData data) : base(data)
    {
        DrinkData = data;
    }

    public void Consume(PlayerStats player)
    {
        if (player == null)
        {
            Debug.LogError("Player is null. Cannot consume drink.");
            return;
        }

        // Hồi phục khát cho player
        player.RestoreThirst(DrinkData.thirstRestored);

        // Hồi phục máu nếu có
        if (DrinkData.healthRestored > 0)
        {
            player.RestoreHealth(DrinkData.healthRestored);
        }

        // Xử lý thời gian tiêu thụ nếu cần (có thể thêm hiệu ứng, animation, v.v.)

        // Sau khi tiêu thụ, bạn có thể xóa item khỏi inventory
        // Điều này thường được xử lý ở nơi gọi phương thức Consume
    }
}
