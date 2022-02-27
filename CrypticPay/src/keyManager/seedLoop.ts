// the seed loop holds the seed and keyrings that share the common seed. Each keyring is responsible for a different coin.
var bip = require("../ext/bip39.min.js")
import { Chain, supportedChains } from "./chain.js"
import HDKeyring from "./keyRing.js"
import HDKeyRing, { Keyring, SerializedHDKeyring } from "./keyRing.js"
import { normalizeHexAddress, validateAndFormatMnemonic, Options, defaultOptions } from "./utils.js"


export type SerializedSeedLoop = {
    version: number
    mnemonic: string
    keyrings: SerializedHDKeyring[]
}


export interface SeedLoop<T> {
    serialize(): Promise<T>
    getKeyRing(coin: Chain): Promise<HDKeyRing>
}

export interface KeyringClass<T> {
    new(): SeedLoop<T>
    deserialize(serializedKeyring: T): Promise<SeedLoop<T>>
}


export default class HDSeedLoop implements SeedLoop<SerializedSeedLoop>{
    #mnemonic: string
    #keyrings: HDKeyring[] = []
    #chainToKeyring = {}

    constructor(options: Options = {}) {
        const hdOptions: Required<Options> = {
            ...defaultOptions,
            ...options,
        }


        this.#mnemonic = validateAndFormatMnemonic(
            hdOptions.mnemonic || bip.generateMnemonic(hdOptions.strength)
        )

        if (!this.#mnemonic) {
            throw new Error("Invalid mnemonic.")
        }

        // only populate with new keyrings if keyrings haven't already been created and serialized
        if (hdOptions.isCreation) {
            this.#populateLoopKeyrings();
        }
        
    }

    // populate seed loop with keyrings for supported chains
    #populateLoopKeyrings() {
        for (let ticker in supportedChains) {
            let chain: Chain = supportedChains[ticker]
            let ringOptions = {
                // default path is BIP-44 ethereum coin type, where depth 5 is the address index
                path: chain.path,
                strength: 128,
                mnemonic: this.#mnemonic,
                chainTicker: chain.ticker
            }
            // create new key ring for chain given setup options
            var keyRing: HDKeyring = new HDKeyring(ringOptions);
            // add key ring to seed loop 
            this.addKeyRing(keyRing);
        }
    }

    // SERIALIZE CODE
    serializeSync(): SerializedSeedLoop{
        let serializedKeyRings: SerializedHDKeyring[] = []
        // serialize the key ring for every coin that's on the seed loop and add to serialized list output
        for (let ticker in this.#chainToKeyring)
        {
            var keyring: HDKeyring = this.#chainToKeyring[ticker]
            var serializedKeyRing: SerializedHDKeyring = keyring.serializeSync()
            serializedKeyRings.push(serializedKeyRing)
        }

        return {
            version: 1,
            mnemonic: this.#mnemonic,
            keyrings: serializedKeyRings
        }
    }

    async serialize(): Promise<SerializedSeedLoop> {
        return this.serializeSync()
    }

    // add keyring to dictionary and list of fellow key rings
    async addKeyRing(keyring: HDKeyring) {
        this.#keyrings.push(keyring)
        this.#chainToKeyring[keyring.chain.ticker] = keyring
    }

    // DESERIALIZE CODE
    static deserialize(obj: SerializedSeedLoop): HDSeedLoop {
        const { version, mnemonic, keyrings } = obj
        if (version !== 1) {
            throw new Error(`Unknown serialization version ${obj.version}`)
        }

        // deserialize keyrings
        keyrings.forEach(function (serializedKeyRing) {
            var hdKeyRing: HDKeyring = HDKeyring.deserialize(serializedKeyRing);
        })
        
        
        return 
    }


    async getKeyRing(chain: Chain): Promise<HDKeyRing> {
        return this.#chainToKeyring[chain.ticker];
    }
}

