using UnityEngine;

public class DestroyObjectX : MonoBehaviour
{
    void Start()
    {
        // Уничтожаем эффект взрыва из иерархии во время игры через 2 секунды после появления.
        Destroy(gameObject, 2);
    }
}
