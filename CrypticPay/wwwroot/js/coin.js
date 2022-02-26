// SLIP-0044 specified coin types
var coinTypes = { btc: 0, ltc: 2, doge: 3, eth: 60, xmr: 128, zec: 133, bch: 145, sol: 501, pokt: 635, bnb: 714, avax: 9000, axaxc: 9005, one: 1023 };
var Coin = /** @class */ (function () {
    function Coin(name, ticker) {
        this.Name = name;
        this.Ticker = ticker.toLowerCase();
        // ensure coin ticker is in coinTypes before we search
        if (!(this.Ticker in coinTypes)) {
            throw new Error("Coin path not found!");
        }
        this.Path = this.getPath();
    }
    // builds coin path based on BIP-44 standard
    Coin.prototype.getPath = function () {
        var coinType = coinTypes[this.Ticker];
        var path = "m/44'/".concat(coinType, "'/0'/0");
        return path;
    };
    return Coin;
}());
export { Coin };
//# sourceMappingURL=coin.js.map