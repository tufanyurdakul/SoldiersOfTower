var shortId = require('shortid');
module.exports = class Room
{
    constructor(){
        this.roomId = shortId.generate();
        this.yourSocketId = "";
        this.enemySocketId = "";
    }
}