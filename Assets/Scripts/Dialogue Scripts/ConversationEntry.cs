using UnityEngine;
using System.Collections;

[System.Serializable]
public class ConversationEntry {

    //Variables that each conversation must have

	public Sprite CharacterDisplayPicture;

	[TextArea(3, 5)]
    public string ConversationText;
}
