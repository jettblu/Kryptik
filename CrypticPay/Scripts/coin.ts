export interface HDCoin {
    getPath(): string
}


// SLIP-0044 specified coin types
let coinTypes = { btc: 0, ltc: 2, doge: 3, eth: 60, xmr: 128, zec: 133, bch: 145, sol: 501, pokt:635, bnb: 714, avax: 9000, axaxc:9005, one: 1023};


export class Coin implements HDCoin {
    readonly Name: string
    readonly Ticker: string
    // base path used for hdnode derivation
    readonly Path: string

    constructor(name: string, ticker: string) {
        this.Name = name
        this.Ticker = ticker.toLowerCase();
        // ensure coin ticker is in coinTypes before we search
        if (!(this.Ticker in coinTypes)) {
            throw new Error("Coin path not found!")
        }
        this.Path = this.getPath();
    }

    // builds coin path based on BIP-44 standard
    getPath(): string{
        let coinType = coinTypes[this.Ticker];
        let path = `m/44'/${coinType}'/0'/0`;
        return path;
    }
}