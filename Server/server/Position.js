var shortId = require('shortid');
module.exports = class Position
{
    constructor(){
        this.insId = shortId.generate();
        this.id ="";
        this.unitid = 0;
        this.placeId = 0;
        this.health= 0;
        this.attackSpeed = 0;
        this.attackDamage = 0;
        this.abilityPower = 0;
        this.armour = 0;
        this.resistance = 0;
    }
}