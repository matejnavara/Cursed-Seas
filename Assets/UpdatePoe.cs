using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UpdatePoe : MonoBehaviour
{
    public Player Player;
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        if(Player == null)
        {
            Debug.LogError("UpdatePoe script needs player attached");
        }
    }

    // Update is called once per frame
    void Update()
    {
        text.text = Player.PiecesOfEight.ToString();
    }
}
