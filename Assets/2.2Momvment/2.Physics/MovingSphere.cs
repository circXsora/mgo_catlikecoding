using UnityEngine;

namespace _2_2
{

    public class MovingSphere : MonoBehaviour
    {

        [SerializeField, Range(0f, 100f)]
        float maxSpeed = 10f;

        Vector3 velocity;

        [SerializeField, Range(0f, 100f)]
        float maxAcceleration = 10f;

        Rigidbody body;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        void Update()
        {
            Vector2 playerInput;
            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput = Vector2.ClampMagnitude(playerInput, 1f);

            Vector3 desiredVelocity =
                new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

            velocity = body.velocity;

            float maxSpeedChange = maxAcceleration * Time.deltaTime;
            velocity.x =
                Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z =
                Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

            body.velocity = velocity;
        }
    }
}