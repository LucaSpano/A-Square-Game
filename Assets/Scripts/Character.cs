using System;
using System.Collections;
using System.Collections.Generic;
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
	
	public List<Character> Characters = new List<Character>();
	float _invincibilityStart;

	SpriteRenderer[] _walkingRenderers = new SpriteRenderer[4];
	SpriteRenderer[] _jumpingRenderers = new SpriteRenderer[4];
	CharacterInput _inputs;

	float _currentWalkBend = 0f;
	[SerializeField] float _horizontalAnimSpeed;
	[SerializeField] bool _jellyBend;
	[SerializeField] float _scaleUpFactor = 1.2f;
	Vector3 _startScale;
	[SerializeField] public float SpeedDown = 1f;

	void Awake()
	{
		_startScale = transform.localScale;
		_inputs = GetComponent<CharacterInput>();
	}
	
	public void Add(Character charater)
	{		
		if (Characters.Contains(charater)) {
			Debug.LogError("Player " + charater.PrimaryCharacterIndex + "already contained in character " + name);
		}
				
		Characters.Add(charater);
		
		UpdateGraphics();

		if (Characters.Count >= 4) {
			StopAllCoroutines();
			StartCoroutine(WaitAndBoom(5f));
		}
	}

	public void Boom()
	{
		SoundManager.instance.Play(_explodeSound, transform.position);
		var chars = Characters.ToArray();
		var pos = transform.position;
		for (var index = 0; index < chars.Length; index++) {
			var character = chars[index];
			
			var rot = Quaternion.Euler(0f, 0f, 360f/chars.Length * index);
			var offset = (rot * Vector3.left * 0.2f);
			character.transform.position = pos + offset;
			
			character.gameObject.SetActive(true);
			character.GetComponent<Rigidbody2D>().AddForce(offset * 100f + Vector3.up * 5f, ForceMode2D.Impulse);

			character.TriggerInvincibilityFrame();
			
			if (character == this) {
				continue;
			}

			character.UpdateGraphics();
			Characters.Remove(character);
		}

		UpdateGraphics();
	}

	void Start()
	{
		Add(this);
	}
	
	void UpdateGraphics()
	{
		UpdateScale();
		
		for (int i = 0; i < _sliceGroups.Length; i++) {
			var groupActive = i < Characters.Count;
			var group = _sliceGroups[i];
			group.SetActive(groupActive);
			if (groupActive) {
				for (int sliceIndex = 0; sliceIndex < 2; sliceIndex++) {
					var slice = group.transform.GetChild(sliceIndex);
					var spriteRenderer = slice.GetComponent<SpriteRenderer>();
					spriteRenderer.color = ColorForPlayerIndex(Characters[i].PrimaryCharacterIndex);
					spriteRenderer.sortingOrder = i;
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

	void UpdateScale()
	{
		transform.localScale = _startScale * (1f + (Characters.Count - 1) * _scaleUpFactor);
	}

	void Update()
	{
		var inputs = _inputs.PoolInputs();
		_currentWalkBend = Mathf.MoveTowards(_currentWalkBend, inputs.horizontal, Time.deltaTime * _horizontalAnimSpeed);
		UpdateWalk(Mathf.Sin(Time.time * 20f) * 0.2f + _currentWalkBend * 0.5f);
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
		
		if (character && !IsInvincible() && !character.IsInvincible()) {
			if (HotAngle(transform.position, other.transform.position)) {
				if (character.Characters.Count == 1) {
					SoundManager.instance.Play(_implodeSound, transform.position);
					Add(character);
					character.gameObject.SetActive(false);
				}
				else {
					character.Boom();
				}
			}
		}
	}

	bool HotAngle(Vector3 a, Vector3 b)
	{
		var delta = a - b;
		if (delta.y < 0f) {
			return false;
		}		
		var ratio = delta.x / delta.y;
		
		return ratio < 1f;
	}

	IEnumerator WaitAndBoom(float f)
	{
		yield return new WaitForSeconds(f);
		Boom();
	}
}
