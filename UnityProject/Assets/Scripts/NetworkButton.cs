using System.Collections.Generic;
using ChatGPTWrapper;
using MyBox;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkButton : NetworkBehaviour {

	[SerializeField] ChatGPTConversation conversation;
	[SerializeField] PacketReceiver packetReceiver;

	[SerializeField] GameObject loadingAnimation;
	
	[HideInInspector] public List<string> options;

	public void StartGame() {
		SendChatMessageServerRpc("start");
	}

	public static int CurrentPlayer = 0;

	public void Click(int index) {
		//SendChatMessageServerRpc(options[index]);
		SendChatMessageServerRpc($"generate?player_id={CurrentPlayer}&player_input={index}");
		options.Clear();
	}

	[ClientRpc]
	void ReceiveChatMessageClientRpc(string response) {
		//Write to send to dialogue
		loadingAnimation.SetActive(false);
		packetReceiver.Receive(response);
	}

	[ClientRpc]
	void SendChatAnimationClientRpc() {
		packetReceiver.ClearOptions();
		loadingAnimation.SetActive(true);
	}

	[ServerRpc(RequireOwnership = false)]
	void SendChatMessageServerRpc(string option) {
		SendChatAnimationClientRpc();
		PlayerManager.Instance.SelectNextPlayer();
		conversation.SendToChatGPT(option);
		conversation.chatGPTResponse.AddListener(ReceiveChatMessageClientRpc);
	}
}