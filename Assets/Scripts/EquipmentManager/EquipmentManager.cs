using System;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public Transform rightHandSocket;
    public Transform leftHandSocket;
    public GameObject currentRightHandItem;
    public GameObject currentLeftHandItem;

    // Định nghĩa sự kiện
    public event Action OnEquipmentChanged;

    // Gắn đồ vật vào tay phải
    public void EquipRightHand(GameObject itemPrefab)
    {
        if (currentRightHandItem != null)
        {
            Destroy(currentRightHandItem);
        }
        currentRightHandItem = Instantiate(itemPrefab, rightHandSocket);
        currentRightHandItem.transform.localPosition = Vector3.zero;
        currentRightHandItem.transform.localRotation = Quaternion.identity;

        // Kích hoạt sự kiện
        OnEquipmentChanged?.Invoke();
    }

    // Tháo đồ vật khỏi tay phải
    public void UnequipRightHand()
    {
        if (currentRightHandItem != null)
        {
            Destroy(currentRightHandItem);
            currentRightHandItem = null;

            // Kích hoạt sự kiện
            OnEquipmentChanged?.Invoke();
        }
    }

    // Gắn đồ vật vào tay trái
    public void EquipLeftHand(GameObject itemPrefab)
    {
        if (currentLeftHandItem != null)
        {
            Destroy(currentLeftHandItem);
        }
        currentLeftHandItem = Instantiate(itemPrefab, leftHandSocket);
        currentLeftHandItem.transform.localPosition = Vector3.zero;
        currentLeftHandItem.transform.localRotation = Quaternion.identity;
    }




    // Tháo đồ vật khỏi tay trái
    public void UnequipLeftHand()
    {
        if (currentLeftHandItem != null)
        {
            Destroy(currentLeftHandItem);
            currentLeftHandItem = null;
        }
    }
}
