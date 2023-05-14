using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleTools.DialogueSystem {
	[CreateAssetMenu(fileName = "New Dialogue", menuName = "Simple Tools/Dialogue", order = 11)]
	public class Dialogue : ScriptableObject {

		public DialogueBox[] sentences;

		public void InsertPacket(string[] packText) {
			List<string> l = packText.ToList();

			sentences = new DialogueBox[l.Count];
			for (int i = 0; i < sentences.Length; i++) {
				sentences[i] = new DialogueBox {
					displayName = false,
					sentence = l[i]
				};
			}
		}
	}

	[System.Serializable]
	public class DialogueBox {
		public bool displayName;
		public string characterName;
		public Sprite characterImage;
		[TextArea(5, 10)] public string sentence;
	}
}