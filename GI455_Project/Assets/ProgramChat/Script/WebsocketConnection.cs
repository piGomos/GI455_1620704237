using System;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using UnityEngine.SceneManagement;

namespace ProgramChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        private WebSocket websocket;
        public InputField userName;
        public InputField inputMassage;
        //public Transform chatContent;
        //public GameObject chatMessage;
        public Text massage;

        void Start()
        {
            websocket = new WebSocket("ws://127.0.0.1:40000");

            websocket.OnMessage += OnMessage;
            
            websocket.Connect();
        }
        
        public void BottonConnect()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        public void MessageBox()
        {
            websocket.Send($"{inputMassage.text}\n");
            
            /*if (Input.GetKeyDown(KeyCode.Return))
            {
                //GameObject newMessage = Instantiate(chatMessage, chatContent);
            }
            
            //Text content = newMessage.GetComponent<Text>();

            //content.text = string.Format(content.text, userName, massage);*/
        }
        
        public void OnDestroy()
        {
            if (websocket != null)
            {
                websocket.Close();
            }
        }

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log("Message from server : " + messageEventArgs.Data);
            massage.text += messageEventArgs.Data;
        }
    }
}
