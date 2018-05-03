using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	[SerializeField] float _speed;
	[SerializeField] float _jumpStartVel = 8f;
	[SerializeField] float _jumpBoostAccel = 1f;
	[SerializeField] float _dashVel = 12f;
	[SerializeField] float _accel = 20f;

	[SerializeField] AudioClip _jumpSound;
	[SerializeField] AudioClip _landingSound;
	
	CharacterInput _inputs;
	Rigidbody2D _rigidBody;

	CharacterInput.InputState _lastInputs = new CharacterInput.InputState();

	float _lastGroundTime = 0.0f;
	float _coyoteTime = 0.2f;
	Character _character;

	const float _teleportWidth = 9.5f;
	const float _teleportMargin = 0.05f;
	
	void Awake()
	{
		_inputs = GetComponent<CharacterInput>();
		_rigidBody = GetComponent<Rigidbody2D>();
		_character = GetComponent<Character>();
	}

	void FixedUpdate()
	{
		var inputs =_inputs.PoolInputs();

		if (GetComponent<Character>().IsInvincible()) {
			return;
		}
		
		var currentVel = _rigidBody.velocity;
		var slowDownFac = (1f - (_character.Characters.Count - 1f) * _character.SpeedDown);
		currentVel.x = Mathf.MoveTowards(currentVel.x, inputs.horizontal * _speed * slowDownFac, Time.deltaTime * _accel);

		if (inputs.jump) {
			if (!_lastInputs.jump && Time.time - _lastGroundTime < _coyoteTime) {
				SoundManager.instance.Play(_jumpSound, transform.position, 1f, 1f);
				currentVel.y = _jumpStartVel;
			}
			
			currentVel.y += _jumpBoostAccel * Time.deltaTime;
		}

		if (inputs.dash && !_lastInputs.dash) {
			currentVel.x = 0f;
			currentVel.y = -_dashVel;
		}
		

		_lastInputs = inputs;

		_rigidBody.velocity = currentVel;
		
		var pos = transform.position;
		if (transform.position.y < -5.4f) {
			pos.y = 5.1f;
		}
		
		if (transform.position.y > 5.2f) {
			pos.y = -5.3f;
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
		if (Time.time - _lastGroundTime > 0.1f) {
			SoundManager.instance.Play(_landingSound, transform.position, 1f, 1f);
		}
		
		_lastGroundTime = Time.time;
	}
}
