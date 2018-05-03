using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Character : MonoBehaviour
{
	public int PrimaryCharacterIndex = 0;
	public bool freeFall;
	
	[SerializeField] GameObject[] _sliceGroups;
	[SerializeField] Color[]      _colors;
	[SerializeField] AudioClip _explodeSound;
	[SerializeField] AudioClip _implodeSound;

	[SerializeField] Sprite[] _walkingSprites;
	
	List<Character> _characters = new List<Character>();
	float _invincibilityStart;

	SpriteRenderer[] _walkingRenderers = new SpriteRenderer[4];
	SpriteRenderer[] _jumpingRenderers = new SpriteRenderer[4];
	CharacterInput _inputs;

	float _currentWalkBend = 0f;
	[SerializeField] float _horizontalAnimSpeed;
	[SerializeField] bool _jellyBend;

	void Awake()
	{
		_inputs = GetComponent<CharacterInput>();
	}
	
	public void Add(Character charater)
	{
		Debug.Log("Implode ?");
		SoundManager.instance.Play(_implodeSound, transform.position);
		
		if (_characters.Contains(charater)) {
			Debug.LogError("Player " + charater.PrimaryCharacterIndex + "already contained in character " + name);
		}
				
		_characters.Add(charater);
		
		UpdateGraphics();
	}

	public void Boom()
	{
		Debug.Log("Explode");
		SoundManager.instance.Play(_explodeSound, transform.position);
		var chars = _characters.ToArray();
		var pos = transform.position;
		for (var index = 0; index < chars.Length; index++) {
			var character = chars[index];
			
			var rot = Quaternion.Euler(0f, 0f, 360f/chars.Length * index);
			var offset = (rot * Vector3.left * 0.2f);
			character.transform.position = pos + offset;
			
			character.GetComponent<Rigidbody2D>().AddForce(offset * 50f + Vector3.up * 5f, ForceMode2D.Impulse);

			character.TriggerInvincibilityFrame();
			
			if (character == this) {
				continue;
			}

			character.gameObject.SetActive(true);
			character.UpdateGraphics();
			_characters.Remove(character);
		}

		UpdateGraphics();
	}

	void Start()
	{
		Add(this);
	}
	
	void UpdateGraphics()
	{
		for (int i = 0; i < _sliceGroups.Length; i++) {
			var groupActive = i < _characters.Count;
			var group = _sliceGroups[i];
			group.SetActive(groupActive);
			if (groupActive) {
				for (int sliceIndex = 0; sliceIndex < 2; sliceIndex++) {
					var slice = group.transform.GetChild(sliceIndex);
					var spriteRenderer = slice.GetComponent<SpriteRenderer>();
					spriteRenderer.color = ColorForPlayerIndex(_characters[i].PrimaryCharacterIndex);
					spriteRenderer.sortingOrder = sliceIndex;
					if (sliceIndex == 0) {
						_walkingRenderers[i] = spriteRenderer;
					}
					else {
						_jumpingRenderers[i] = spriteRenderer;
					}
				}
			}
		}
	}

	void Update()
	{
		var inputs = _inputs.PoolInputs();
		_currentWalkBend = Mathf.MoveTowards(_currentWalkBend, inputs.horizontal, Time.deltaTime * _horizontalAnimSpeed);
		UpdateWalk(_currentWalkBend);
	}
	
	void UpdateAnimState(bool walking)
	{
		foreach (var r in _walkingRenderers) {
			if (r == null)
				continue;
			r.enabled = walking;
		}
		
		foreach (var r in _jumpingRenderers) {
			if (r == null)
				continue;
			r.enabled = !walking;
		}
	}

	void UpdateWalk(float direction)
	{
		UpdateAnimState(true);
		var frame = Mathf.RoundToInt(_walkingSprites.Length * Mathf.Abs(direction));
		frame = Mathf.Clamp(frame, 0, _walkingSprites.Length - 1);
		var sprite = _walkingSprites[frame];
		
		foreach (var r in _walkingRenderers) {
			if (r == null)
				continue;
			if (Mathf.Abs(direction) > 0.01f) {
				r.flipX = _jellyBend == direction > 0f;
			}
			r.sprite = sprite;
		}
	}

	void TriggerInvincibilityFrame()
	{
		Debug.Log("Trigger for " + name);
		_invincibilityStart = Time.time;
	}
	
	Color ColorForPlayerIndex(int index)
	{
		return _colors[index];
	}

	public bool IsInvincible()
	{
		return Time.time < _invincibilityStart + 1f;
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		var character = other.gameObject.GetComponent<Character>();
		
		if (character && other.relativeVelocity.y > 0f && !IsInvincible() && !character.IsInvincible()) {
			if (character._characters.Count == 1) {
				Add(character);
				character.gameObject.SetActive(false);
			}
			else {
				character.Boom();
			}
		}
	}
}
