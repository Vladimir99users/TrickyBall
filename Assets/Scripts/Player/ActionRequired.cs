using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ActionRequired : MonoBehaviour,IInteractable
{
    [SerializeField] private LayerMask _whatIsMoney;
    [SerializeField] private LayerMask _whatIsTrap;
    [SerializeField] private UI _uiAction;
    private CircleCollider2D _circleCollider => GetComponent<CircleCollider2D>();
    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.TryGetComponent<Money>(out Money money))
        {
            Debug.Log("Money");
            _uiAction.OnCollectMoney?.Invoke(money.Price);
            Destroy(col.gameObject);
        }

        if(col.gameObject.TryGetComponent<Trap>(out Trap trap))
        {
             Debug.Log("Trap");
            _uiAction.OnActionTrap?.Invoke();
        }
    }
}
