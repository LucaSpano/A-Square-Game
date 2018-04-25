﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	[SerializeField] float _speed;
	[SerializeField] float _jumpStartVel = 8f;
	[SerializeField] float _jumpBoostAccel = 1f;
	
	CharacterInput _inputs;
	Rigidbody2D _rigidBody;

	bool _lastJumpInput = false;

	float _lastGroundTime = 0.0f;
	float _coyoteTime = 0.2f;

	const float _teleportWidth = 9.5f;
	const float _teleportMargin = 0.05f;
	
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
			if (!_lastJumpInput && Time.time - _lastGroundTime < _coyoteTime) {
				currentVel.y = _jumpStartVel;
			}
			
			currentVel.y += _jumpBoostAccel * Time.deltaTime;
		}

		_lastJumpInput = inputs.jump;

		_rigidBody.velocity = currentVel;

		var pos = transform.position;
		if (transform.position.y < -5f) {
			pos.y = 5.2f;
		}
		
		if (transform.position.y > 5.3f) {
			pos.y = -4.9f;
		}
		
		if (transform.position.x < -_teleportWidth) {
			pos.x = _teleportWidth - _teleportMargin;
		}
		
		if (transform.position.x > _teleportWidth) {
			pos.x = -_teleportWidth + _teleportMargin ;
		}

		transform.position = pos;
	}

	void OnCollisionStay2D()
	{
		_lastGroundTime = Time.time;
	}
}
