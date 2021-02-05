var websocket = require('ws');

var callbackInitServer = ()=>{
    console.log("Server is running.");
}

var websocketServer = new websocket.Server({port:40000}, callbackInitServer);

var wsList = [];

websocketServer.on("connection", (ws, rq)=>{
    
    console.log("client connected.");
    
    wsList.push(ws);
    
    ws.on("message", (data)=>{
        
        console.log("send from clinent : " + data);
        Boardcast(data);
        
    })
    
    ws.on("close", ()=>{
        
        wsList = ArrayRemove(wsList, ws);
        console.log("client disconnected");
        
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
