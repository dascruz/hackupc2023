using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DialogueControl : NetworkBehaviour {

    public static DialogueControl instance;
    
    // Start is called before the first frame update
    void Awake() {
        instance = this;
    }

    public void ReceiveRequest() {
        //if (!IsHost) {
            GameObject.CreatePrimitive(PrimitiveType.Cube);
            Debug.Log("potato");
        //}
    }
}
