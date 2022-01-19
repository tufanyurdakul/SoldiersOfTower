const socketIO = require('socket.io');
const Position = require('./Position');
const Changes = require('./changes');
const Health = require('./Health');
const PlayerData = require('./PlayerData');
const Room = require('./Room');

const io = socketIO(52300);
console.log('server started');



var timer = 0;
var rooms = [];
var players = [];
io.on("connection", (socket) => {
    players[socket.id] = socket.id;
   
    socket.on("Join Room", function (data) {
        let isThere = false;
        if (rooms.length > 0) {
            rooms.forEach(element => {
                if (element["room"] == data.roomId && element["id"] == data.roomId) {
                    isThere = true;
                }
            });
            if (!isThere) {
                socket.join(data.roomId);
                rooms.push({ room: data.roomId,id:data.myId, count: 0 });
            }
        }
        else {
            socket.join(data.roomId);
            rooms.push({ room: data.roomId,id:data.myId, count: 0 });
        }
    });

    socket.on("register2", function (data) {
        rooms.forEach(element => {
            if (element["room"] == data.roomId) {
                element["count"]++;
                socket.emit("register", { id: socket.id});
                socket.emit("size", { size: element["count"] });
                socket.to(data.roomId).emit("size", { size: element["count"] });
            }
        });
    });
    socket.on("x2", function (data) {
        socket.to(data.roomId).emit("x2", data);
        var myInt = setInterval(function () {
            socket.to(data.roomId).emit("timer", { time: timer });
        }, 1000);
    });
    socket.on("healths", function (data) {
        socket.to(data.roomId).emit("healths", data);
    });

    socket.on("spawn", function (data) {
        var position = new Position();
        position.id = data.id;
        position.unitid = data.unitId;
        position.placeId = data.placeId;
        position.health = data.health;
        position.attackSpeed = data.attackSpeed;
        position.attackDamage = data.attackDamage;
        position.abilityPower = data.abilityPower;
        position.armour = data.armour;
        position.resistance = data.resistance;
        console.log(data);
        socket.to(data.roomId).emit("spawn", position);
        socket.emit("spawn",position);
    });
    socket.on("attackBaseSignal", function (data) {
        socket.to(data.roomId).emit("attackBaseSignal", data);
    });
    socket.on("attack", function (data) {
        socket.to(data.roomId).emit("attack", data);
        socket.emit("attack", data);
        console.log(data);
    });
    socket.on("kill",function(data)
    {
        socket.to(data.roomId).emit("kill");
        socket.emit("kill",data);
    });
    socket.on("lifeSteal",function(data)
    {
        socket.emit("lifeSteal",data);
        socket.to(data.roomId).emit("lifeSteal",data);
    });
    socket.on("attackCount",function(data)
    {
        socket.emit("attackCount",data);
        socket.to(data.roomId).emit("attackCount",data);
    });
    socket.on("attackBase", function (data) {
        socket.emit("attackBase",data);
        socket.to(data.roomId).emit("attackBase",data);
    });
   
    
    socket.on("diss", function (data) {
      
    });
    socket.on("disconnect", function () {
        // hp.forEach((element, index) => {
        //     if (element["key"] == socket.id) {
        //         count.splice(index, 1);
        //     }
        // });
        // count.forEach((element, index) => {
        //     if (element == socket.id) {
        //         count.splice(index, 1);
        //     }
        // });
        // playersHealth.forEach((element, index) => {
        //     if (element["id"] == socket.id) {
        //         playersHealth.splice(index, 1);
        //     }
        // });
        rooms.forEach((element, index) => {
                if (element["id"] == socket.id) {
                    count.splice(index, 1);
                }
            });
            
        delete players[socket.id];
        console.log("disconnected");
    });
});


