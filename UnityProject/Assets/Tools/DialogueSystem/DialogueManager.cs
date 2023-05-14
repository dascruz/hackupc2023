using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleTools.DialogueSystem {
	public class DialogueManager : MonoBehaviour {

		DialogueVertexAnimator dialogueVertexAnimator;

		Queue<string> sentences;
		Queue<bool> displayNames;
		Queue<string> characterNames;
		Queue<Sprite> characterImages;
		bool talking;

		public DialogueItems dialogueItems;

		public static DialogueManager instance;

		Dialogue _currentDialogue;

		public Action finishAnimatingCallback;
		
		void Awake() {
			instance = this;
			sentences = new Queue<string>();
			displayNames = new Queue<bool>();
			characterNames = new Queue<string>();
			characterImages = new Queue<Sprite>();

			dialogueVertexAnimator = new DialogueVertexAnimator(dialogueItems.textBox);
		}

		public bool Dialogue(Dialogue dialogue) {
			if (_currentDialogue == dialogue) return Dialogue(dialogue, string.Empty);
			
			_currentDialogue = dialogue;
			talking = false;

			return Dialogue(dialogue, string.Empty);
		}

		public bool Dialogue(Dialogue dialogue, params string[] sounds) {
			dialogueVertexAnimator.SetAudioSourceGroup(sounds);

			if (!talking) {
				sentences.Clear();
				if (dialogue.sentences.Length != 0) {
					foreach (DialogueBox sentence in dialogue.sentences) {
						sentences.Enqueue(sentence.sentence);
						displayNames.Enqueue(sentence.displayName);
						characterNames.Enqueue(sentence.characterName);
						characterImages.Enqueue(sentence.characterImage);
					}
				} else {
					sentences.Enqueue("I am error. No text has been added");
				}
				talking = true;

				if (sentences.Count == 0) {
					talking = false;
					return false;
				}

				bool last = sentences.Count == 1;
				string sentenceToShow = sentences.Peek();
				bool displayName = displayNames.Peek();
				string characterName = characterNames.Peek();
				Sprite characterImage = characterImages.Peek();
				if (PlayDialogue(sentenceToShow, displayName, characterName, characterImage, last)) {
					sentences.Dequeue();
					displayNames.Dequeue();
					characterNames.Dequeue();
					characterImages.Dequeue();
				}
				return true;
			} else {
				if (sentences.Count == 0) {
					talking = false;
					return false;
				}

				bool last = sentences.Count == 1;
				string sentenceToShow = sentences.Peek();
				bool displayName = displayNames.Peek();
				string characterName = characterNames.Peek();
				Sprite characterImage = characterImages.Peek();
				if (PlayDialogue(sentenceToShow, displayName, characterName, characterImage, last)) {
					sentences.Dequeue();
					displayNames.Dequeue();
					characterNames.Dequeue();
					characterImages.Dequeue();
				}
				return true;
			}
		}

		private Coroutine typeRoutine = null;
		bool PlayDialogue(string message, bool displayName = false, string characterName = "", Sprite characterImage = null, bool last = false) {
			if (dialogueVertexAnimator.IsMessageAnimating()) {
				dialogueVertexAnimator.SkipToEndOfCurrentMessage();
				return false; //Next message hasn't been shown because the current one is still animating.
			}
			this.EnsureCoroutineStopped(ref typeRoutine);
			dialogueVertexAnimator.textAnimating = false;
			List<DialogueCommand> commands = DialogueUtility.ProcessInputString(message, out string totalTextMessage);
			typeRoutine = last ? 
				StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, finishAnimatingCallback)) : 
				StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, null));

			dialogueItems.characterImage.sprite = characterImage;
			dialogueItems.characterName.text = displayName ? characterName : "???";
			return true; //Next message shown successfully
		}
	}

	[System.Serializable]
	public struct DialogueItems {
		public Image characterImage;
		public TMP_Text characterName;
		public TMP_Text textBox;
		public Canvas canvas;
	}
}