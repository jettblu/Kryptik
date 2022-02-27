export interface HDCoin {
    getPath(): string
}


// SLIP-0044 specified coin types
let chainCodes = {
    btc: 0, ltc: 2, doge: 3, eth: 60, xmr: 128, zec: 133, bch: 145, sol: 501, pokt: 635, bnb: 714, avax: 9000, avaxc: 9005, one: 1023
};




export class Chain implements HDCoin {
    readonly name: string
    readonly ticker: string
    // base path used for hdnode derivation
    readonly path?: string
    // BIP-44 coin code
    readonly code: number

    constructor(name: string, ticker: string) {
        this.name = name
        this.ticker = ticker.toLowerCase();
        // ensure coin ticker is in coinTypes before we search
        if (!(this.ticker in chainCodes)) {
            throw new Error(`${this.ticker}: Coin path not found!`)
        }
        this.path = this.getPath();
    }

    // builds coin path based on BIP-44 standard
    getPath(): string{
        let coinType = chainCodes[this.ticker];
        let path = `m/44'/${coinType}'/0'/0`;
        return path;
    }

}


export let supportedChains: { [name: string]: Chain } = {}
supportedChains.btc = new Chain("Bitcoin", "btc")
supportedChains.eth = new Chain("Ethereum", "eth")
supportedChains.sol =  new Chain("Solana", "sol")
supportedChains.avaxc = new Chain("Avalanche C Chain",  "avaxc")


// return chain that matches ticker
export function chainFromTicker(ticker: string): Chain{
    return supportedChains[ticker]
}

