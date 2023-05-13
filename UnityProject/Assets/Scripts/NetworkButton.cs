using System.Collections.Generic;
using ChatGPTWrapper;
using MyBox;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkButton : NetworkBehaviour {

	[SerializeField] ChatGPTConversation conversation;
	[SerializeField] PacketReceiver packetReceiver;
	
	[HideInInspector] public List<string> options;

	public void StartGame() {
		SendChatMessageServerRpc("Tell me a joke followed by 5 different joke options.");
	}

	public void Click(int index) {
		SendChatMessageServerRpc(options[index]);
		options.Clear();
	}

	[ClientRpc]
	void ReceiveChatMessageClientRpc(string response) {
		//Write to send to dialogue
		packetReceiver.Receive(response);
	}

	[ServerRpc(RequireOwnership = false)]
	void SendChatMessageServerRpc(string option) {
		conversation.SendToChatGPT(option);
		conversation.chatGPTResponse.AddListener(ReceiveChatMessageClientRpc);
	}
}