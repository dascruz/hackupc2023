using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using SimpleTools.DialogueSystem;
using TMPro;
using UnityEngine.UI;

public class PacketReceiver : MonoBehaviour {
    
    Dialogue _dialogue;

    [SerializeField] NetworkButton _networkButton;

    [SerializeField] Button[] buttons;
    [SerializeField] TMP_Text[] buttonsText;
    List<string> _options;

    [ButtonMethod]
    void Call() {
        _networkButton.StartGame();
    }

    void ShowOptions() {
        Debug.Log("time to show options");
    }
    
    void Start() {
        DialogueManager.instance.finishAnimatingCallback += ShowOptions;
    }
    
    public void Receive(string packet) {
        Packet pack = JsonUtility.FromJson<Packet>(packet);
        _options = new List<string>(pack.options);
        _dialogue = ScriptableObject.CreateInstance<Dialogue>();
        _dialogue.InsertPacket(new [] {packet});
        DialogueManager.instance.Dialogue(_dialogue);
    }

    void Update() {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        
        DialogueManager.instance.Dialogue(_dialogue);
    }
}
