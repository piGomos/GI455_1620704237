const sqlite = require('sqlite3').verbose();

let db = new sqlite.Database('./db/chatDB.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{

    //ถ้าไม่มี server (สั่งปิด)
    if(err) throw err;
    /*if (err) {
        console.log(err);
    }*/

    console.log('Connected to database.');

    //จะไม่เก็บรหัสผ่านไว้ใน database ตรงๆ 
    var id = "test5";
    var password = "6699";
    var name = "test4";

    var sqlSelect = "SELECT * FROM UserData WHERE UserID='"+id+"' AND Password='"+password+"'";
    //Add data ทุกครั้งที่เปลี่ยน Structure ใหม่ก็ควรเพิ่มเข้าไปด้วย
    var sqlInsert = "INSERT INTO UserData (UserID, Password, Name, Money) VALUES ('"+id+"', '"+password+"', '"+name+"', '100')"
    //เปลี่ยนค่าที่เราต้องการ(เขียนทับข้อมูลชุดเก่า)
    var sqlUpdate = "UPDATE UserData SET Money='500' WHERE UserID='"+id+"'";
    
    //Update
    /*db.all("SELECT Money FROM UserData WHERE UserID='"+id+"'", (err,row)=>{

        if (err) {
            console.log(err);
        }
        else{

            if (row.length > 0) {
                
                var currentMoney = row[0].Money;
                currentMoney += 100;

                db.all("UPDATE UserData SET Money='"+currentMoney+"' WHERE UserID='"+id+"'", (err, row)=>{

                    if(err){

                        console.log(err);

                    }
                    else{

                        var result = {
                            status:true,
                            money:currentMoney
                        }

                        console.log(JSON.stringify(result));
                    }
                })
            }
            else
            {
                console.log("UserID not found");
            }
        }

        console.log(row);
    });*/
    
    //ระบบ LogIn
    /*db.all(sqlSelect, (err,row)=>{

        if (err) {
            console.log("Register fail");
        }
        else{
            console.log("Register succuss");
        }

        console.log(row);
    })*/
    
    //เจาะจงข้อมูล
    db.all(sqlSelect, (err,row)=>{
        //"SELECT * FROM UserData WHERE UserID='"+id+"' AND Password='"+password+"'"
        //'SELECT * FROM UserData WHERE Name=\'test2\''
        //"SELECT * FROM UserData WHERE Name='test2'"
        //"SELECT * FROM UserData WHERE Name='test2' AND UserID='test1556'"
        //ไม่ได้สั่งปิดแอพ
        if(err){
            console.log(err);
        }

        if (row.length > 0) 
        {
            console.log("Login succuss")
        }
        else
        {
            console.log("Login fail")
        }

        console.log(row);
    });

    /*db.all('SELECT * FROM UserData', (err,row)=>{

        //ไม่ได้สั่งปิดแอพ
        if(err){
            console.log(err);
        }

        console.table(row);

        //console.log(row);
        console.log(row[0].Name);//or console.log(row[0]["Name"]);

    });*/
    
});