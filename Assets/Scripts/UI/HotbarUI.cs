using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    public Hotbar hotbar;
    public GameObject[] slots;
    public Color selectedColor;
    public Color defaultColor;

    void Start()
    {
        if (hotbar == null)
        {
            Debug.LogError("Tham chiếu đến Hotbar bị thiếu trong HotbarUI.");
        }

        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("Các slot trong HotbarUI chưa được gán.");
        }
    }

    void Update()
    {
        UpdateHotbarUI();
    }

    void UpdateHotbarUI()
    {
        if (hotbar == null || slots == null)
            return;

        for (int i = 0; i < hotbar.hotbarSize; i++)
        {
            if (slots.Length > i)
            {
                Item item = hotbar.GetItemInSlot(i);

                // Lấy thành phần Image của hình nền và biểu tượng
                Image backgroundImage = slots[i].GetComponent<Image>();
                Transform iconTransform = slots[i].transform.Find("Icon");
                Image iconImage = null;

                if (iconTransform != null)
                {
                    iconImage = iconTransform.GetComponent<Image>();
                }
                else
                {
                    Debug.LogWarning($"Không tìm thấy 'Icon' trong slot {i}");
                }

                if (iconImage != null)
                {
                    if (item != null)
                    {
                        // Gán sprite cho icon của vật phẩm
                        iconImage.sprite = item.Data.icon;
                        iconImage.enabled = true;
                    }
                    else
                    {
                        iconImage.sprite = null;
                        iconImage.enabled = false;
                    }
                }

                // Làm nổi bật ô đang được chọn
                if (backgroundImage != null)
                {
                    if (i == hotbar.GetCurrenIndex())
                    {
                        backgroundImage.color = selectedColor;
                    }
                    else
                    {
                        backgroundImage.color = defaultColor;
                    }
                }
            }
        }
    }
}
