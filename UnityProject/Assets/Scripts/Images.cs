using UnityEngine;

[CreateAssetMenu(fileName = "Images", menuName = "Scriptable Objects/Images", order = 11)]
public class Images : ScriptableObject {
	public Sprite[] sprites;

	public Sprite SelectImage() => sprites[Random.Range(0, sprites.Length)];
}
