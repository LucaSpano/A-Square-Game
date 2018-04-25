using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	[SerializeField] float _speed;
	
	CharacterInput _inputs;
	Rigidbody2D _rigidBody;

	void Awake()
	{
		_inputs = GetComponent<CharacterInput>();
		_rigidBody = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate()
	{
		var inputs =_inputs.PoolInputs();

		if (GetComponent<Character>().IsInvincible()) {
			return;
		}
		
		var currentVel = _rigidBody.velocity;
		currentVel.x = inputs.horizontal * _speed;

		if (inputs.jump) {
			currentVel.y = 10f;
		}

		_rigidBody.velocity = currentVel;

		var pos = transform.position;
		if (transform.position.y < -5f) {
			pos.y = 5.2f;
		}
		
		if (transform.position.y > 5.3f) {
			pos.y = -4.9f;
		}
		
		if (transform.position.x < -10.05f) {
			pos.x = 10f;
		}
		
		if (transform.position.x > 10.05f) {
			pos.x = -10f;
		}

		transform.position = pos;
	}
}
