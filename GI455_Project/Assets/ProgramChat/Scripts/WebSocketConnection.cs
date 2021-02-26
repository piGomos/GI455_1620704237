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
        /*public class MessageData
        {
            public string name;
            public string message;
        }*/

        /*struct MessageData
        {
            public string name;
            public string message;
            public string massageEvent;

            public MessageData(string massageEvent, string name, string message)
            {
                this.massageEvent = massageEvent;
                this.name = name;
                this.message = message;
            }
        }*/

        //CreateRoom[0] = "Event" 
        //TestRoom[1] = "data"
        class SocketEvent
        {
            public string eventName;
            public string roomName;
            public string data;
            public string name;
            public string userName;
            public string password;
            

            //การสร้าง constructor เพื่อสร้าง object
            public SocketEvent(string eventName, string roomName, string name, string userName, string password, string data)
            {
                this.eventName = eventName;
                this.roomName = roomName;
                this.userName = userName;
                this.password = password;
                this.name = name;
                this.data = data;
            }
        }
        
        public GameObject rootConnection;
        public GameObject rootMessage;
        public GameObject rootLobby;
        public GameObject rootCreateRoom;
        public GameObject rootJoinRoom;
        public GameObject rootPopup;
        public GameObject rootstart;
        public GameObject rootRegister;
        public GameObject rootLogin;

        public InputField registerUsername;
        public InputField registerPassword;
        public InputField inputName;
        public InputField loginUsername;
        public InputField loginPassword;
        public InputField inputCreateRoom;
        public InputField inputJoinRoom;
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
            rootstart.SetActive(false);
            rootRegister.SetActive(false);
            rootLogin.SetActive(false);
        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:40000/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            rootConnection.SetActive(false);
            //rootLobby.SetActive(true);
            rootLogin.SetActive(true);
        }

        public void Register()
        {
            string _inputName = inputName.text;
            string _registerUsername = registerUsername.text;
            string _registerPassword= registerPassword.text;
            
            if (_inputName != "" && _registerUsername != "" && _registerPassword != "")
            {
                SocketEvent socketEvent = new SocketEvent("Register", "", _inputName, _registerUsername, _registerPassword, "");
                string jsonStr = JsonUtility.ToJson(socketEvent);
                ws.Send(jsonStr);
            }
            else
            {
                rootPopup.SetActive(true);
                popUpText.text = "Please fill in the information";
            }
        }

        public void ReceiveRegister()
        {
            if (tempMessageString != "")
            {
                SocketEvent socketEventData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (socketEventData.data == "success")
                {
                    rootRegister.SetActive(false);
                    rootLogin.SetActive(true);
                }
                else if (socketEventData.data == "fail")
                {
                    rootPopup.SetActive(true);
                    popUpText.text = socketEventData.eventName + ": Duplicate Username";
                }
                tempMessageString = "";
            }
        }

        public void Login()
        {
            string _loginUsername = loginUsername.text;
            string _loginPassword = loginPassword.text;

            if (_loginUsername != "" && _loginPassword != "" )
            {
                SocketEvent socketEvent = new SocketEvent("Login", "", "", _loginUsername, _loginPassword, "");
                string jsonStr = JsonUtility.ToJson(socketEvent);
                ws.Send(jsonStr);
            }
            else
            {
                rootPopup.SetActive(true);
                popUpText.text = "Please fill in the information";
            }
        }

        public void ReceiveLogin()
        {
            if (tempMessageString != "")
            {
                SocketEvent socketEventData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (socketEventData.data == "success")
                {
                    rootLobby.SetActive(true);
                    rootLogin.SetActive(false);

                    inputName.text = socketEventData.name;
                }
                else if (socketEventData.data == "fail")
                {
                    rootPopup.SetActive(true);
                    popUpText.text = socketEventData.eventName + ": The username or password is incorrect";
                }
                tempMessageString = "";
            }
        }

        public void CreateRoom()
        {
            //ดูว่า connect รึยัง(open พร้อมรับส่งข้อมูล)
            if (ws.ReadyState == WebSocketState.Open)
            {
                string createRoom = inputCreateRoom.text;
                
                if (createRoom != "")
                {
                    SocketEvent socketEvent = new SocketEvent("CreateRoom", createRoom, "", "", "", "");
                    string jsonStr = JsonUtility.ToJson(socketEvent);
                    ws.Send(jsonStr);
                }
                else
                {
                    rootPopup.SetActive(true);
                    popUpText.text = "Please fill in the information";
                }
                inputCreateRoom.text = "";
            }
        }

        public void MessageDataCreateRoom()
        {
            if (tempMessageString != "")
            {
                SocketEvent socketEventData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (socketEventData.data == "success")
                {
                    rootMessage.SetActive(true);
                    rootCreateRoom.SetActive(false);

                    nameRoomText.text = "Room: " + socketEventData.roomName;
                }
                else if (socketEventData.data == "fail")
                {
                    rootPopup.SetActive(true);
                    popUpText.text = socketEventData.eventName + ": Duplicate Roomname";
                }
                tempMessageString = "";
            }
        }

        public void JoinRoom()
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                string joinName = inputJoinRoom.text;
                
                if (joinName != "")
                {
                    SocketEvent socketEvent = new SocketEvent("JoinRoom", joinName, "", "", "", "");
                    string jsonStr = JsonUtility.ToJson(socketEvent);
                    ws.Send(jsonStr);
                }
                else
                {
                    rootPopup.SetActive(true);
                    popUpText.text = "Please fill in the information";
                }
                inputJoinRoom.text = "";
            }
        }
        
        public void MessageDataJoinRoom()
        {
            if (tempMessageString != "")
            {
                SocketEvent socketEventData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (socketEventData.data == "success")
                {
                    rootMessage.SetActive(true);
                    rootJoinRoom.SetActive(false);

                    nameRoomText.text = "Room: " + socketEventData.roomName;
                }
                else if (socketEventData.data == "fail")
                {
                    rootPopup.SetActive(true);
                    popUpText.text = socketEventData.eventName + ": Can't find the room";
                }
                tempMessageString = "";
            }
        }

        public void LeaveRoom()
        {
            SocketEvent socketEventJoinRoom = new SocketEvent("LeaveRoom", "", "", "", "", "");
            string jsonToStr = JsonUtility.ToJson(socketEventJoinRoom);
            ws.Send(jsonToStr);

            sendText.text = "";
            receiveText.text = "";
            rootMessage.SetActive(false);
            rootLobby.SetActive(true);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent newMessageData = new SocketEvent("SendMessage", "", inputName.text, "", "", inputText.text);
                string toJsonStr = JsonUtility.ToJson(newMessageData);
                ws.Send(toJsonStr);
                inputText.text = "";
            }
        }

        public void Messagerecive()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (receiveMessageData.name == inputName.text)
                {
                    sendText.text += receiveMessageData.name + ": " + receiveMessageData.data + "\n";
                }
                else
                {
                    receiveText.text += receiveMessageData.name + ": " + receiveMessageData.data + "\n";
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
            if (tempMessageString != "")
            {
                SocketEvent newData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (newData.eventName == "CreateRoom")
                {
                    MessageDataCreateRoom();
                }
                else if (newData.eventName == "JoinRoom")
                {
                    MessageDataJoinRoom();
                }
                else if (newData.eventName == "SendMessage")
                {
                    Messagerecive();
                }
                else if (newData.eventName == "Register")
                {
                    ReceiveRegister();
                }
                else if (newData.eventName == "Login")
                {
                    ReceiveLogin();
                }
            }  
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);
            tempMessageString = messageEventArgs.Data;
        }
        
        public void LobbyCreateRoom()
        {
            rootLobby.SetActive(false);
            rootCreateRoom.SetActive(true);
        }
        
        public void LobbyJoinRoom()
        {
            rootLobby.SetActive(false);
            rootJoinRoom.SetActive(true);
        }

        public void ToRegister()
        {
           rootLogin.SetActive(false);
           rootRegister.SetActive(true);
        }
        
        public void ExitPopUp()
        {
            rootPopup.SetActive(false);
        }
    }
}


