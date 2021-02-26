const sqlite = require('sqlite3').verbose();
var websocket = require('ws');

var callbackInitServer = ()=>{
    console.log("Server is running.");
}

let db = new sqlite.Database('./db/chatDB.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{

    if(err) throw err;

    console.log('Connected to database.');

    db.all("SELECT * FROM UserData", (err,row)=>{

        if (err) {
            console.log(err);
        }
        else{
            console.table(row);
        }
    })

})

var websocketServer = new websocket.Server({port:40000}, callbackInitServer);

//array add คนกับห้องเข้าไป
var wsList = [];
var roomList = [];
/*
    roomName: "xxxxx" 
    wsList: []

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
                console.log("client request CreateRoom ["+toJson.roomName+"]");
                
                var isFoundRoom = false;
                for (var i = 0; i < roomList.length; i++)
                {
                    if (roomList[i].roomName == toJson.roomName)
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
                        roomName: toJson.roomName,
                        name: toJson.name,
                        data: "fail"
                    }
                    ws.send(JSON.stringify(resultData));
                }
                else{
                    console.log("Create room : room is not found");
                    
                    //New เป็น object
                    var newRoom = {
                        roomName: toJson.roomName,
                        wsList: []
                    }
                    
                    newRoom.wsList.push(ws);
                    roomList.push(newRoom);
                    
                    var resultData = {
                        eventName: toJson.eventName,
                        roomName: toJson.roomName,
                        name: toJson.name,
                        data: "success"
                    }
                    
                    ws.send(JSON.stringify(resultData));
                    
                }
            }
            else if (toJson.eventName == "JoinRoom")//JoinRoom
            {
                console.log("client request Joinroom ["+toJson.roomName+"]");

                var isFoundRoom = false;
                var number = -1;
                for (var i = 0; i < roomList.length; i++)
                {
                    if (roomList[i].roomName == toJson.roomName)
                    {
                        isFoundRoom = true;
                        number = i;
                        break;
                    }
                }

                if(isFoundRoom)
                {
                    console.log("Join room : room is found");

                    var resultData = {
                        eventName: toJson.eventName,
                        roomName: toJson.roomName,
                        name: toJson.name,
                        data: "success"
                    }
                    
                    ws.send(JSON.stringify(resultData));
                    roomList[i].wsList.push(ws);
                }
                else{
                    console.log("Join room : room is not found");
                    
                    var resultData = {
                        eventName: toJson.eventName,
                        roomName: toJson.roomName,
                        name: toJson.name,
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
                            roomList[i].wsList.splice(j, 1);
                            
                            if (roomList[i].wsList.length <= 0)
                            {
                                roomList.splice(i, 1);
                            }
                            break;
                        }
                    }
                }
                
            }
            else if (toJson.eventName == "SendMessage")//sandMessage
            {
                Boardcast(ws, toJson)
            }
            else if (toJson.eventName == "Register")//Register
            {
                console.log("client request Register [ username: "+toJson.userName+" password: "+toJson.password+" name: "+toJson.name+"]")

                db.all("INSERT INTO UserData (Username, Password, Name) VALUES ('"+toJson.userName+"', '"+toJson.password+"', '"+toJson.name+"')", (err,row)=>{

                    if (err) {
                        
                        console.log("Register fail: Username exists");
                    
                        var resultData = {
                            eventName: toJson.eventName,
                            roomName: toJson.roomName,
                            name: toJson.name,
                            data: "fail"
                        }

                        ws.send(JSON.stringify(resultData));
                    }
                    else
                    {
                        console.log("Register success");
                    
                        var resultData = {
                            eventName: toJson.eventName,
                            roomName: toJson.roomName,
                            name: toJson.name,
                            data: "success"
                        }

                        ws.send(JSON.stringify(resultData));
                    }
                })
            }
            else if (toJson.eventName == "Login")//Register
            {
            
                console.log("client request Login [ username: "+toJson.userName+" password: "+toJson.password+"]")

                db.all("SELECT * FROM UserData WHERE Username='"+toJson.userName+"' AND Password='"+toJson.password+"'", (err,row)=>{

                    if (err) {
                        
                        console.log(err);
                    }
                    else if (row.length > 0) 
                    {
                        //console.log(row);
                        //console.log(row[0].Name);

                        console.log("Register success");
                    
                        var resultData = {
                            eventName: toJson.eventName,
                            roomName: toJson.roomName,
                            name: row[0].Name,
                            data: "success"
                        }

                        ws.send(JSON.stringify(resultData));
                    }
                    else
                    {
                        console.log("Register fail: Username exists");
                    
                        var resultData = {
                            eventName: toJson.eventName,
                            roomName: toJson.roomName,
                            name: toJson.name,
                            data: "fail"
                        }

                        ws.send(JSON.stringify(resultData));
                    }
                })
            }
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
                    roomList[i].wsList.splice(j, 1);

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

 function Boardcast(ws, message) 
 {
    var selectRoomIndex = -1; 
    
    for (var i = 0; i < roomList.length; i++)
     {
        for(var j = 0; j < roomList[i].wsList.length; j++){
            
            if (ws == roomList[i].wsList[j]) {
                
                selectRoomIndex = i;
                break;
            }
        }
     }

    for(var i = 0; i < roomList[selectRoomIndex].wsList.length; i++){

        var resultData = {
            eventName: "SendMessage",
            roomName: message.roomName,
            name: message.name,
            data: message.data
        }
        roomList[selectRoomIndex].wsList[i].send(JSON.stringify(resultData));
    }
 }
