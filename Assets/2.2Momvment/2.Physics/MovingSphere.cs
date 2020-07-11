using UnityEngine;

namespace _2_2
{

    public class MovingSphere : MonoBehaviour
    {

        [SerializeField, Range(0f, 100f)]
        float maxSpeed = 10f;

        Vector3 velocity, desiredVelocity;
		bool desiredJump;

        [SerializeField, Range(0f, 100f)]
        float maxAcceleration = 10f;

		[SerializeField, Range(0f, 10f)]
		float jumpHeight = 2f;

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

			desiredJump |= Input.GetButtonDown("Jump");

			desiredVelocity =
				new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
		}

		void FixedUpdate()
		{
			velocity = body.velocity;
            if (desiredJump)
            {
				desiredJump = false;
				Jump();
            }
			float maxSpeedChange = maxAcceleration * Time.deltaTime;
			velocity.x =
				Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
			velocity.z =
				Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
			body.velocity = velocity;

			onGround = false;
		}

		void Jump()
        {
            if (onGround)
            {
				velocity.y += Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
			}
		}

		bool onGround;

		void OnCollisionEnter(Collision collision)
		{
			EvaluateCollision(collision);
		}

		void OnCollisionStay(Collision collision)
		{
			EvaluateCollision(collision);
		}

		void EvaluateCollision(Collision collision) {
			for (int i = 0; i < collision.contactCount; i++)
			{
				Vector3 normal = collision.GetContact(i).normal;
				onGround |= normal.y >= 0.9f;
			}
		}
	}
}