using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]
public class Money : MonoBehaviour
{
    [SerializeField] private int _price;
    public int Price => _price;

}
