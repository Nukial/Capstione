using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    // Sức khỏe
    public int maxHealth = 100;
    public int currentHealth;

    // Độ no
    public int maxHunger = 100;
    public int currentHunger;
    public float hungerDecreaseRate = 1f; // Tốc độ giảm độ no theo thời gian

    // Khát
    public int maxThirst = 100;
    public int currentThirst;
    public float thirstDecreaseRate = 1.5f; // Tốc độ tăng khát theo thời gian

    // Năng lượng
    public int maxEnergy = 100;
    public int currentEnergy;
    public float energyDecreaseRate = 0.5f; // Tốc độ giảm năng lượng theo thời gian

    // Các cài đặt khác
    public float statUpdateInterval = 1f; // Khoảng thời gian cập nhật các chỉ số

    private Coroutine statCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentEnergy = maxEnergy;

        // Bắt đầu Coroutine để cập nhật các chỉ số
        statCoroutine = StartCoroutine(UpdateStats());
    }

    private IEnumerator UpdateStats()
    {
        while (true)
        {
            yield return new WaitForSeconds(statUpdateInterval);

            // Giảm độ no và khát theo thời gian
            DecreaseHunger(hungerDecreaseRate);
            IncreaseThirst(thirstDecreaseRate);
            DecreaseEnergy(energyDecreaseRate);

            // Kiểm tra tình trạng sức khỏe
            CheckHealthStatus();
        }
    }

    private void DecreaseHunger(float amount)
    {
        currentHunger -= Mathf.RoundToInt(amount);
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
    }

    private void IncreaseThirst(float amount)
    {
        currentThirst -= Mathf.RoundToInt(amount);
        currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);
    }

    private void DecreaseEnergy(float amount)
    {
        currentEnergy -= Mathf.RoundToInt(amount);
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    public void RestoreHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Sức khỏe đã hồi phục: {amount}. Sức khỏe hiện tại: {currentHealth}");
    }

    public void RestoreHunger(int amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        Debug.Log($"Độ no đã hồi phục: {amount}. Độ no hiện tại: {currentHunger}");
    }

    public void RestoreThirst(int amount)
    {
        currentThirst += amount;
        currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);
        Debug.Log($"Đã uống nước: {amount}. Khát hiện tại: {currentThirst}");
    }

    public void RestoreEnergy(int amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        Debug.Log($"Năng lượng đã hồi phục: {amount}. Năng lượng hiện tại: {currentEnergy}");
    }

    private void CheckHealthStatus()
    {
        // Nếu độ no hoặc khát bằng 0, sức khỏe sẽ giảm
        if (currentHunger <= 0 || currentThirst <= 0)
        {
            TakeDamage(5); // Mỗi lần trừ 5 máu
        }

        // Nếu sức khỏe giảm về 0, người chơi chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Bị mất máu: {amount}. Sức khỏe hiện tại: {currentHealth}");
    }

    private void Die()
    {
        Debug.Log("Người chơi đã chết!");
        // Xử lý chết (hiển thị màn hình game over, load lại scene, v.v.)
        // Bạn có thể thêm các xử lý cần thiết ở đây
    }

    // Gọi hàm này khi người chơi ăn thức ăn
    public void ConsumeFood(FoodItem foodItem)
    {
        RestoreHealth(foodItem.FoodData.healthRestored);
        RestoreHunger(foodItem.FoodData.hungerRestored);
        // Bạn có thể thêm các hiệu ứng khác nếu cần
    }

    // Gọi hàm này khi người chơi uống nước
    public void ConsumeDrink(DrinkItem drinkItem)
    {
        RestoreThirst(drinkItem.DrinkData.thirstRestored);
        // Bạn có thể thêm các hiệu ứng khác nếu cần
    }

    // Bạn có thể thêm các phương thức khác để quản lý tiêu hóa, mệt mỏi, v.v.
}
