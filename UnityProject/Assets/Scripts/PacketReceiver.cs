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

    public static PacketReceiver Instance;

    void Awake() {
        Instance = this;
    }

    public void ChangeInteractiveState(bool active) {
        foreach (Button button in buttons) {
            button.interactable = active;
        }
    }

    [ButtonMethod]
    void Call() {
        _networkButton.StartGame();
    }

    public void ClearOptions() {
        foreach (Button t in buttons) {
            t.gameObject.SetActive(false);
        }
    }
    
    void ShowOptions() {
        for (int i = 0; i < _options.Count; i++) {
            buttonsText[i].text = _options[i];
            buttons[i].gameObject.SetActive(true);
            buttons[i].gameObject.SetActive(true);
        }

        for (int i = _options.Count; i < buttons.Length; i++) {
            buttons[i].gameObject.SetActive(false);
        }
    }
    
    void Start() {
        DialogueManager.instance.finishAnimatingCallback += ShowOptions;
    }
    
    public void Receive(string packet) {
        var pack = JsonUtility.FromJson<CustomRes>(packet);
        _options = pack.Options;
        _dialogue = ScriptableObject.CreateInstance<Dialogue>();
        _dialogue.InsertPacket(pack.Text.ToArray());
        DialogueManager.instance.Dialogue(_dialogue);
    }

    void Update() {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        
        DialogueManager.instance.Dialogue(_dialogue);
    }
}
