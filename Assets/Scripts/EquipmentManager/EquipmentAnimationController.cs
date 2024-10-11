using UnityEngine;
using UnityEngine.InputSystem;

public class EquipmentAnimationController : MonoBehaviour
{
    public EquipmentManager equipmentManager;
    public Animator animator;
    public Hotbar hotbar;

    void Start()
    {
        // Đăng ký sự kiện khi trang bị thay đổi
        equipmentManager.OnEquipmentChanged += UpdateAnimations;
        UpdateAnimations(); // Cập nhật lần đầu
    }

    void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi đối tượng bị hủy
        equipmentManager.OnEquipmentChanged -= UpdateAnimations;
    }

    void UpdateAnimations()
    {
        // Kiểm tra trang bị trên tay phải
        if (equipmentManager.currentRightHandItem != null)
        {
            Item item = hotbar.GetCurrentEquippedItem();

            if (item != null)
            {
                switch (item.Data.itemType)
                {
                    case ItemType.Sword:
                        animator.SetBool("isFishing", false);
                        animator.SetBool("isHoldingPlow", false);
                        animator.SetBool("isHoldingSword", true);
                        animator.SetBool("isHoldingPickaxe", false);
                        animator.SetBool("isHoldingHammer", false);
                        break;
                    case ItemType.FishingRod:
                        animator.SetBool("isFishing", true);
                        animator.SetBool("isHoldingPlow", false);
                        animator.SetBool("isHoldingSword", true);
                        animator.SetBool("isHoldingPickaxe", false);
                        animator.SetBool("isHoldingHammer", false);
                        break;
                    case ItemType.Plow:
                        animator.SetBool("isFishing", false);
                        animator.SetBool("isHoldingPlow", true);
                        animator.SetBool("isHoldingSword", false);
                        animator.SetBool("isHoldingPickaxe", false);
                        animator.SetBool("isHoldingHammer", false);
                        break;
                    case ItemType.Pickaxe:
                        animator.SetBool("isFishing", false);
                        animator.SetBool("isHoldingPlow", false);
                        animator.SetBool("isHoldingSword", false);
                        animator.SetBool("isHoldingPickaxe", true);
                        animator.SetBool("isHoldingHammer", false);
                        break;
                     case ItemType.Hammer:
                        animator.SetBool("isFishing", false);
                        animator.SetBool("isHoldingPlow", false);
                        animator.SetBool("isHoldingHammer", true);
                        animator.SetBool("isHoldingPickaxe", false);
                        animator.SetBool("isHoldingSword", false);
                        break;
                    default:
                        animator.SetBool("isFishing", false);
                        animator.SetBool("isHoldingPlow", false);
                        animator.SetBool("isHoldingHammer", false);
                        animator.SetBool("isHoldingPickaxe", false);
                        animator.SetBool("isHoldingSword", false);
                        break;
                }
            }
        }
        else
        {
            // Không có trang bị trên tay
            animator.SetBool("isFishing", false);
            animator.SetBool("isHoldingSword", false);
            animator.SetBool("isHoldingPickaxe", false);
            animator.SetBool("isHoldingHammer", false);
            animator.SetBool("isHoldingPlow", false);
        }

        // Tương tự cho tay trái nếu cần
    }
}
