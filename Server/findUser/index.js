const socketIO = require('socket.io');

const Room = require('./Room');

const io = socketIO(52200);
console.log('server started');
var players = [];
var playersHealth = [];
var hp = {
    roomId: []
};
var count = {
    roomId: [],
    id: []
};
var timer = 0;
var playerCount = 0;
var ranks = [];
var rooms = [];
io.on("connection", (socket) => {
    socket.emit("register", { id: socket.id });
    socket.on("rank", function (data) {
        if (ranks.length > 0) {
            ranks.forEach(element => {
                if (element["id"] != socket.id) {
                    ranks.push({ id: socket.id, rank: data.rank });
                }
                else {
                    console.log("same");
                }
            });
        }
        else {
            ranks.push({ id: socket.id, rank: data.rank });
        }
    });

    var myInt = setInterval(function () {
        if (ranks.length > 0) {
            ranks.forEach((element, index) => {
                if (element["id"] == socket.id) {
                    ranks.forEach((element2, index2) => {
                        if (element["id"] != element2["id"]) {
                            if (Math.abs(element["rank"] - element2["rank"]) < 500) {
                                let yourId = element["id"];
                                let enemyId = element2["id"];
                                ranks.splice(index, 1);
                                ranks.forEach((element3,index3) => {
                                    if(element3["id"] == element2["id"])
                                    {
                                        ranks.splice(index3, 1);
                                    }
                                });
                                room = new Room();
                                room.yourSocketId = yourId;
                                room.enemySocketId = enemyId;
                                socket.broadcast.emit("findMatch", room);
                                socket.emit("findMatch", room);
                            }
                        }
                    });
                }

            });
        }
        console.log(ranks);
    }, 1500);
    socket.on("disconnect", function () {
        socket.broadcast.emit("disconnected", { id: socket.id });
        ranks.forEach((element,index) => {
            if(element["id"] == socket.id)
            {
                ranks.splice(index,1);
            }
        });
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
        playerCount--;
        delete players[socket.id];
        console.log("disconnected");
    });
});


