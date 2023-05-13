using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyChatBehaviour : NetworkBehaviour {
	[SerializeField] private ChatMessage chatMessagePrefab;
	[SerializeField] private Transform messageParent;
	[SerializeField] private TMP_InputField chatInputField;

	private const int MaxNumberOfMessagesInList = 20;
	private List<ChatMessage> _messages;

	private const float MinIntervalBetweenChatMessages = 1f;
	private float _clientSendTimer;

	private void Start() {
		_messages = new List<ChatMessage>();
	}

	private void Update() {
		_clientSendTimer += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Return)) {
			if (chatInputField.text.Length > 0 && _clientSendTimer > MinIntervalBetweenChatMessages) {
				SendMessage();
				chatInputField.DeactivateInputField(clearSelection: true);
			}
			else {
				chatInputField.Select();
				chatInputField.ActivateInputField();
			}
		}
	}

	public void SendMessage() {
		string message = chatInputField.text;
		chatInputField.text = "";

		if (string.IsNullOrWhiteSpace(message)) {
			return;
		}

		_clientSendTimer = 0;
		SendChatMessageServerRpc(message, NetworkManager.Singleton.LocalClientId);
	}

	private void AddMessage(string message, ulong senderPlayerId) {
		var msg = Instantiate(chatMessagePrefab, messageParent);
		msg.SetMessage("potato", message);

		_messages.Add(msg);

		if (_messages.Count > MaxNumberOfMessagesInList) {
			Destroy(_messages[0]);
			_messages.RemoveAt(0);
		}
	}

	[ClientRpc]
	private void ReceiveChatMessageClientRpc(string message, ulong senderPlayerId) {
		AddMessage(message, senderPlayerId);
	}

	[ServerRpc(RequireOwnership = false)]
	private void SendChatMessageServerRpc(string message, ulong senderPlayerId) {
		ReceiveChatMessageClientRpc(message, senderPlayerId);
	}
}