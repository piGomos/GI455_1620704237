using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Seacrhbar : MonoBehaviour
{
    public InputField game;
    public Text textBox;

    public void SeacrhGame()
    {
        var gameName = game.text;
        
        switch (gameName)
        {
            case "Overwatch":
                textBox.text = game.text + " is found";
                break;
            case "Maplestory":
                textBox.text = game.text + " is found";
                break;
            case "Thesims":
                textBox.text = game.text + " is found";
                break;
            case "Pangya":
                textBox.text = game.text + " is found";
                break;
            case "Minecraft":
                textBox.text = game.text + " is found";
                break;
            default:
                textBox.text = game.text + " is not found";
                break;
        }
        
    }
    
}
