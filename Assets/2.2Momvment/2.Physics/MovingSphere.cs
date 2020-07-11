﻿using UnityEngine;

namespace _2_2
{

    public class MovingSphere : MonoBehaviour
    {

        [SerializeField, Range(0f, 100f)]
        float maxSpeed = 10f;

        Vector3 velocity, desiredVelocity;
		bool desiredJump;

        [SerializeField, Range(0f, 100f)]
        float maxAcceleration = 10f, maxAirAcceleration = 1f;

		[SerializeField, Range(0f, 10f)]
		float jumpHeight = 2f;

		Rigidbody body;

		[SerializeField, Range(0, 5)]
		int maxAirJumps = 0;

		int jumpPhase;

		[SerializeField, Range(0f, 90f)]
		float maxGroundAngle = 25f;

		float minGroundDotProduct;

		void OnValidate()
		{
			minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
		}

		void Awake()
        {
            body = GetComponent<Rigidbody>();
			OnValidate();
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

		void UpdateState()
		{
			velocity = body.velocity;
			if (onGround)
			{
				jumpPhase = 0;
			}
            else
            {
				contactNormal = Vector3.up;
            }
		}

		void FixedUpdate()
		{
			UpdateState();

			if (desiredJump)
            {
				desiredJump = false;
				Jump();
            }
			float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
			float maxSpeedChange = acceleration * Time.deltaTime;
			velocity.x =
				Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
			velocity.z =
				Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
			body.velocity = velocity;

			onGround = false;
		}

		void Jump()
        {
            if (onGround || jumpPhase < maxAirJumps)
			{
				jumpPhase += 1;
				float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
				float alignedSpeed = Vector3.Dot(velocity, contactNormal);
				if (alignedSpeed > 0f)
				{
					jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
				}
				velocity += contactNormal * jumpSpeed;
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
		Vector3 contactNormal;
		void EvaluateCollision(Collision collision) {
			for (int i = 0; i < collision.contactCount; i++)
			{
				Vector3 normal = collision.GetContact(i).normal;

				if (normal.y >= minGroundDotProduct)
				{
					onGround = true;
					contactNormal = normal;
				}
			}
		}
	}
}