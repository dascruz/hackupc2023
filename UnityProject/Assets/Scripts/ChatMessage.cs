using System;
using TMPro;
using UnityEngine;

public class ChatMessage : MonoBehaviour {
	
	[SerializeField] private TMP_Text textField;
	public RectTransform RectTransform { get; private set; }

	void Awake() {
		RectTransform = GetComponent<RectTransform>();
	}

	public void SetMessage(string message) {
		textField.text = message;
	}
}