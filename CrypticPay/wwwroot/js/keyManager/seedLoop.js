var __classPrivateFieldGet = (this && this.__classPrivateFieldGet) || function (receiver, state, kind, f) {
    if (kind === "a" && !f) throw new TypeError("Private accessor was defined without a getter");
    if (typeof state === "function" ? receiver !== state || !f : !state.has(receiver)) throw new TypeError("Cannot read private member from an object whose class did not declare it");
    return kind === "m" ? f : kind === "a" ? f.call(receiver) : f ? f.value : state.get(receiver);
};
var _HDSeedLoop_instances, _HDSeedLoop_populateLoopKeyrings;
// the seed loop holds the seed and keyrings that share the common seed. Each keyring is responsible for a different coin.
var bip = require("../ext/bip39.min.js");
import { supportedChains } from "./chain.js";
import HDKeyring from "./keyRing.js";
import { validateAndFormatMnemonic, defaultOptions } from "./utils.js";
export default class HDSeedLoop {
    constructor(options = {}) {
        _HDSeedLoop_instances.add(this);
        this.keyrings = [];
        this.chainToKeyring = {};
        const hdOptions = Object.assign(Object.assign({}, defaultOptions), options);
        this.mnemonic = validateAndFormatMnemonic(hdOptions.mnemonic || bip.generateMnemonic(hdOptions.strength));
        if (!this.mnemonic) {
            throw new Error("Invalid mnemonic.");
        }
        // only populate with new keyrings if keyrings haven't already been created and serialized
        if (hdOptions.isCreation) {
            __classPrivateFieldGet(this, _HDSeedLoop_instances, "m", _HDSeedLoop_populateLoopKeyrings).call(this);
        }
    }
    // SERIALIZE CODE
    serializeSync() {
        let serializedKeyRings = [];
        // serialize the key ring for every coin that's on the seed loop and add to serialized list output
        for (let ticker in this.chainToKeyring) {
            var keyring = this.chainToKeyring[ticker];
            var serializedKeyRing = keyring.serializeSync();
            serializedKeyRings.push(serializedKeyRing);
        }
        return {
            version: 1,
            mnemonic: this.mnemonic,
            keyrings: serializedKeyRings
        };
    }
    async serialize() {
        return this.serializeSync();
    }
    // add keyring to dictionary and list of fellow key rings
    async addKeyRing(keyring) {
        this.keyrings.push(keyring);
        this.chainToKeyring[keyring.chain.ticker] = keyring;
    }
    // DESERIALIZE CODE
    static deserialize(obj) {
        const { version, mnemonic, keyrings } = obj;
        if (version !== 1) {
            throw new Error(`Unknown serialization version ${obj.version}`);
        }
        // create loop options with prexisting mnemonic
        // TODO add null check for mnemonic
        let loopOptions = {
            // default path is BIP-44 ethereum coin type, where depth 5 is the address index
            strength: 128,
            mnemonic: mnemonic,
            isCreation: false
        };
        // create seed loop that will eventually be returned
        var seedLoopNew = new HDSeedLoop(loopOptions);
        // deserialize keyrings
        keyrings.forEach(function (serializedKeyRing) {
            var keyRing = HDKeyring.deserialize(serializedKeyRing);
            seedLoopNew.addKeyRing(keyRing);
        });
        return seedLoopNew;
    }
    async getKeyRing(chain) {
        return this.chainToKeyring[chain.ticker];
    }
}
_HDSeedLoop_instances = new WeakSet(), _HDSeedLoop_populateLoopKeyrings = function _HDSeedLoop_populateLoopKeyrings() {
    for (let ticker in supportedChains) {
        let chain = supportedChains[ticker];
        let ringOptions = {
            // default path is BIP-44 ethereum coin type, where depth 5 is the address index
            path: chain.path,
            strength: 128,
            mnemonic: this.mnemonic,
            chainTicker: chain.ticker
        };
        // create new key ring for chain given setup options
        var keyRing = new HDKeyring(ringOptions);
        // add key ring to seed loop 
        this.addKeyRing(keyRing);
    }
};
//# sourceMappingURL=seedLoop.js.map