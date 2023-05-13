using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour {
    [SerializeField] Button server;
    [SerializeField] Button host;
    [SerializeField] Button client;

    void Awake() {
        server.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
        host.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        client.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }
}
