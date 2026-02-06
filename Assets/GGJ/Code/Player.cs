using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float health = 10f;
    public float speed = 5f;

    public InputActionReference move;

    public float moveMaxRange = 40f;

    private Vector2 _moveDirection;

    // Update is called once per frame
    void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
        transform.Translate(new Vector3(_moveDirection.x, 0, _moveDirection.y).normalized * speed * Time.deltaTime);

        // clamp position, can't move outside border
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -moveMaxRange, moveMaxRange), transform.position.y, Mathf.Clamp(transform.position.z, -moveMaxRange, moveMaxRange));
    }
}
