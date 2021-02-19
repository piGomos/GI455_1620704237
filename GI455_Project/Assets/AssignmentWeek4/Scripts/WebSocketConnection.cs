using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEditor;
using UnityEngine.UI;

namespace ChatWebSocketWithJson
{
    public class WebSocketConnection : MonoBehaviour
    {
        public class MessageData
        {
            public string username;
            public string message;
        }

        //CreateRoom[0] = "Event" 
        //TestRoom[1] = "data"
        struct SocketEvent
        {
            public string eventName;
            public string data;

            //การสร้าง constructor เพื่อสร้าง object
            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }
        
        public GameObject rootConnection;
        public GameObject rootMessage;
        public GameObject rootLobby;
        public GameObject rootCreateRoom;
        public GameObject rootJoinRoom;
        public GameObject rootPopup;

        public InputField inputCreateRoom;
        public InputField inputJoinRoom;
        public InputField inputUsername;
        public InputField inputText;
        public Text sendText;
        public Text receiveText;
        public Text nameRoomText;
        public Text popUpText;
       
        
        private WebSocket ws;

        private string tempMessageString;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootMessage.SetActive(false);
            rootLobby.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:40000/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            ButtonLobbyRoom();
        }

        public void CreateRoom()
        {
            //ดูว่า connect รึยัง(open พร้อมรับส่งข้อมูล)
            if (ws.ReadyState == WebSocketState.Open)
            {
                string roomName = inputCreateRoom.text;
                SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);
                string jsonStr = JsonUtility.ToJson(socketEvent);
                ws.Send(jsonStr);

                nameRoomText.text = roomName;
            }
        }

        public void MessageDataCreateRoom()
        {
            if (tempMessageString != "")
            {
                SocketEvent socketEventData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (socketEventData.data == "success")
                {
                    rootConnection.SetActive(false);
                    rootMessage.SetActive(true);
                    rootLobby.SetActive(false);
                    rootCreateRoom.SetActive(false);
                    rootJoinRoom.SetActive(false);
                }
                else if (socketEventData.data == "fail")
                {
                    rootPopup.SetActive(true);
                    popUpText.text = socketEventData.eventName + ": fail";
                }
            }
        }

        public void JoinRoom()
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                string joinName = inputJoinRoom.text;
                SocketEvent socketEventJoinRoom = new SocketEvent("JoinRoom", joinName);
                string jsonString = JsonUtility.ToJson(socketEventJoinRoom);
                ws.Send(jsonString);
                
                nameRoomText.text = joinName;
            }
            
        }
        
        public void MessageDataJoinRoom()
        {
            if (tempMessageString != "")
            {
                SocketEvent socketEventData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (socketEventData.data == "success")
                {
                    rootConnection.SetActive(false);
                    rootMessage.SetActive(true);
                    rootLobby.SetActive(false);
                    rootCreateRoom.SetActive(false);
                    rootJoinRoom.SetActive(false);
                }
                else if (socketEventData.data == "fail")
                {
                    rootPopup.SetActive(true);
                    popUpText.text = socketEventData.eventName + ": fail";
                }
            }
        }

        public void LeaveRoom()
        {
            SocketEvent socketEventJoinRoom = new SocketEvent("LeaveRoom", "");
            string jsonToStr = JsonUtility.ToJson(socketEventJoinRoom);
            ws.Send(jsonToStr);
            ButtonLobbyRoom();
        }

        public void ButtonLobbyRoom()
        {
            rootConnection.SetActive(false);
            rootMessage.SetActive(false);
            rootLobby.SetActive(true);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;
            
            MessageData newMessageData = new MessageData();
            newMessageData.username = inputUsername.text;
            newMessageData.message = inputText.text;

            string toJsonStr = JsonUtility.ToJson(newMessageData);
            
            ws.Send(toJsonStr);
            inputText.text = "";
        }

        public void Messagerecive()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                MessageData receiveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);

                if (receiveMessageData.username == inputUsername.text)
                {
                    sendText.text += receiveMessageData.username + ": " + receiveMessageData.message + "\n";
                }
                else
                {
                    receiveText.text += receiveMessageData.username + ": " + receiveMessageData.message + "\n";
                }
                tempMessageString = "";
            }
        }

        private void OnDestroy()
        {
            if (ws != null)
                ws.Close();
        }

        private void Update()
        {
            MessageDataCreateRoom();

            MessageDataJoinRoom();
            
            //Messagerecive();
            
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);
            tempMessageString = messageEventArgs.Data;
        }
        
        public void LobbyCreateRoom()
        {
            rootConnection.SetActive(false);
            rootMessage.SetActive(false);
            rootLobby.SetActive(false);
            rootCreateRoom.SetActive(true);
            rootJoinRoom.SetActive(false);
        }
        
        public void LobbyJoinRoom()
        {
            rootConnection.SetActive(false);
            rootMessage.SetActive(false);
            rootLobby.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(true);
        }
        
        public void ExitPopUp()
        {
            rootPopup.SetActive(false);
        }
    }
}


