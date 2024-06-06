using UnityEngine;
using UnityEngine.EventSystems;

public class MoveController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public Vector2 direction;
    public Vector3 _direction;
    public Transform folder;
    public float speed = 2.5f;

    public void OnPointerDown(PointerEventData eventData) => _direction = new Vector3(direction.x, 0, direction.y);

    public void OnPointerUp(PointerEventData eventData) => _direction = Vector3.zero;

    private void Update() => folder.transform.position += _direction * Time.deltaTime * speed;
}
