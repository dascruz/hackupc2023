using UnityEngine;

public class ChatButton : MonoBehaviour {

    [SerializeField] GameObject chat;
    
    bool _currentState;
    public void ChangeChatState() {
        _currentState ^= true;
        chat.SetActive(_currentState);
    }
}
