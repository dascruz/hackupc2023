using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour {
	public override void OnNetworkSpawn() {
		base.OnNetworkSpawn();

		PlayerManager.Instance.Players.Add(this);
	}

	public void SelectPlayer(bool selected) {
		PacketReceiver.Instance.ChangeInteractiveState(selected);
		Debug.Log("You are now selected");
	}
}