var websocket = require('ws');

var callbackInitServer = ()=>{
    console.log("Server is running.");
}

var websocketServer = new websocket.Server({port:40000}, callbackInitServer);

//array add คนกับห้องเข้าไป
var wsList = [];
var roomList = [];
/*
    roomName: "xxxxx"
    wsList

 */
websocketServer.on("connection", (ws, rq)=>{
    
    {
        //lobbyzone
        //ส่วนที่ข้อมูลขึ้นไปยัง Server หลังจากอยู่ใน lobby ข้อมูลอยู่ใน data(string)
        ws.on("message", (data)=>{

            console.log(data);
            
            var toJson = JSON.parse(data);
            //console.log(toJson["eventName"]);
            //console.log(toJson.eventName);
            
            //CreateRoom
            if (toJson.eventName == "CreateRoom")
            {
                console.log("client request CreateRoom ["+toJson.data+"]");
                
                var isFoundRoom = false;
                for (var i = 0; i < roomList.length; i++)
                {
                    if (roomList[i].roomName == toJson.data)
                    {
                        isFoundRoom = true;
                        break;
                    }
                }
                
                if(isFoundRoom)
                {
                    console.log("Create room : room is found");
                    
                    var resultData = {
                        eventName: toJson.eventName,
                        data: "fail"
                    }
                    ws.send(JSON.stringify(resultData));
                }
                else{
                    console.log("Create room : room is not found");
                    
                    //New เป็น object
                    var newRoom = {
                        roomName: toJson.data,
                        wsList: []
                    }
                    
                    newRoom.wsList.push(ws);
                    roomList.push(newRoom);
                    
                    var resultData = {
                        eventName: toJson.eventName,
                        data: "success"
                    }
                    
                    ws.send(JSON.stringify(resultData));
                    
                }
            }
            else if (toJson.eventName == "JoinRoom")//JoinRoom
            {
                console.log("client request Joinroom ["+toJson.data+"]");

                var isFoundRoom = false;
                for (var i = 0; i < roomList.length; i++)
                {
                    if (roomList[i].roomName == toJson.data)
                    {
                        isFoundRoom = true;
                        break;
                    }
                }

                if(isFoundRoom)
                {
                    console.log("Join room : room is found");

                    var resultData = {
                        eventName: toJson.eventName,
                        data: "success"
                    }
                    
                    ws.send(JSON.stringify(resultData));
                }
                else{
                    console.log("Join room : room is not found");
                    
                    var resultData = {
                        eventName: toJson.eventName,
                        data: "fail"
                    }

                    ws.send(JSON.stringify(resultData));

                }
            }
            else if (toJson.eventName == "LeaveRoom")//LeaveRoom
            {
                
                for (var i = 0; i < roomList.length; i++)
                {
                    for (var j = 0; j < roomList[i].wsList.length; j++)
                    {
                        if(roomList[i].wsList[j] == ws)
                        {
                            roomList[i].wsList[j].splice(j, 1);
                            
                            if (roomList[i].wsList.length <= 0)
                            {
                                roomList.splice(i, 1);
                            }
                            break;
                        }
                    }
                }
                
            }
            //console.log("send from clinent : " + data);
            //Boardcast(data);
        })
        
    }
    
    console.log("client connected.");
    
    wsList.push(ws);
    
    ws.on("close", ()=>{
        
        wsList = ArrayRemove(wsList, ws);
        console.log("client disconnected");

        for (var i = 0; i < roomList.length; i++)
        {
            for (var j = 0; j < roomList[i].wsList.length; j++)
            {
                if(roomList[i].wsList[j] == ws)
                {
                    roomList[i].wsList[j].splice(j, 1);

                    if (roomList[i].wsList.length <= 0)
                    {
                        roomList.splice(i, 1);
                    }
                   
                    break;
                }
            }
        }
        
    });
});

function ArrayRemove(arr, value) 
{
    return arr.filter((element)=>{
        return element != value;
    })
}

 function Boardcast(data) 
 {
     for (var i = 0; i < wsList.length; i++)
     {
         wsList[i].send(data);
     }
 }
