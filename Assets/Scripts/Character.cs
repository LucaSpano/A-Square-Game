using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public int PrimaryCharacterIndex = 0;
	public bool freeFall;
	
	[SerializeField] GameObject[] _sliceGroups;
	[SerializeField] Color[]      _colors;
	
	List<Character> _characters = new List<Character>();
	[SerializeField] float _invincibilityStart;

	public void Add(Character charater)
	{
		if (_characters.Contains(charater)) {
			Debug.LogError("Player " + charater.PrimaryCharacterIndex + "already contained in character " + name);
		}
				
		_characters.Add(charater);
		
		UpdateGraphics();
	}

	public void Boom()
	{
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
		for (int i = 0; i < 3; i++) {
			var groupActive = i + 1 == _characters.Count;
			var group = _sliceGroups[i];
			group.SetActive(groupActive);
			if (groupActive) {
				for (int sliceIndex = 0; sliceIndex < group.transform.childCount; sliceIndex++) {
					var slice = group.transform.GetChild(sliceIndex);
					slice.GetComponent<SpriteRenderer>().color = ColorForPlayerIndex(_characters[sliceIndex].PrimaryCharacterIndex);
				}
			}
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
