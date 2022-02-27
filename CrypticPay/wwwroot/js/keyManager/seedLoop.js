var __classPrivateFieldSet = (this && this.__classPrivateFieldSet) || function (receiver, state, value, kind, f) {
    if (kind === "m") throw new TypeError("Private method is not writable");
    if (kind === "a" && !f) throw new TypeError("Private accessor was defined without a setter");
    if (typeof state === "function" ? receiver !== state || !f : !state.has(receiver)) throw new TypeError("Cannot write private member to an object whose class did not declare it");
    return (kind === "a" ? f.call(receiver, value) : f ? f.value = value : state.set(receiver, value)), value;
};
var __classPrivateFieldGet = (this && this.__classPrivateFieldGet) || function (receiver, state, kind, f) {
    if (kind === "a" && !f) throw new TypeError("Private accessor was defined without a getter");
    if (typeof state === "function" ? receiver !== state || !f : !state.has(receiver)) throw new TypeError("Cannot read private member from an object whose class did not declare it");
    return kind === "m" ? f : kind === "a" ? f.call(receiver) : f ? f.value : state.get(receiver);
};
var _HDSeedLoop_instances, _HDSeedLoop_mnemonic, _HDSeedLoop_keyrings, _HDSeedLoop_chainToKeyring, _HDSeedLoop_populateLoopKeyrings;
// the seed loop holds the seed and keyrings that share the common seed. Each keyring is responsible for a different coin.
var bip = require("../ext/bip39.min.js");
import { supportedChains } from "./chain.js";
import HDKeyring from "./keyRing.js";
import { validateAndFormatMnemonic, defaultOptions } from "./utils.js";
export default class HDSeedLoop {
    constructor(options = {}) {
        _HDSeedLoop_instances.add(this);
        _HDSeedLoop_mnemonic.set(this, void 0);
        _HDSeedLoop_keyrings.set(this, []);
        _HDSeedLoop_chainToKeyring.set(this, {});
        const hdOptions = Object.assign(Object.assign({}, defaultOptions), options);
        __classPrivateFieldSet(this, _HDSeedLoop_mnemonic, validateAndFormatMnemonic(hdOptions.mnemonic || bip.generateMnemonic(hdOptions.strength)), "f");
        if (!__classPrivateFieldGet(this, _HDSeedLoop_mnemonic, "f")) {
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
        for (let ticker in __classPrivateFieldGet(this, _HDSeedLoop_chainToKeyring, "f")) {
            var keyring = __classPrivateFieldGet(this, _HDSeedLoop_chainToKeyring, "f")[ticker];
            var serializedKeyRing = keyring.serializeSync();
            serializedKeyRings.push(serializedKeyRing);
        }
        return {
            version: 1,
            mnemonic: __classPrivateFieldGet(this, _HDSeedLoop_mnemonic, "f"),
            keyrings: serializedKeyRings
        };
    }
    async serialize() {
        return this.serializeSync();
    }
    // add keyring to dictionary and list of fellow key rings
    async addKeyRing(keyring) {
        __classPrivateFieldGet(this, _HDSeedLoop_keyrings, "f").push(keyring);
        __classPrivateFieldGet(this, _HDSeedLoop_chainToKeyring, "f")[keyring.chain.ticker] = keyring;
    }
    // DESERIALIZE CODE
    static deserialize(obj) {
        const { version, mnemonic, keyrings } = obj;
        if (version !== 1) {
            throw new Error(`Unknown serialization version ${obj.version}`);
        }
        // deserialize keyrings
        keyrings.forEach(function (serializedKeyRing) {
            var hdKeyRing = HDKeyring.deserialize(serializedKeyRing);
        });
        return;
    }
    async getKeyRing(chain) {
        return __classPrivateFieldGet(this, _HDSeedLoop_chainToKeyring, "f")[chain.ticker];
    }
}
_HDSeedLoop_mnemonic = new WeakMap(), _HDSeedLoop_keyrings = new WeakMap(), _HDSeedLoop_chainToKeyring = new WeakMap(), _HDSeedLoop_instances = new WeakSet(), _HDSeedLoop_populateLoopKeyrings = function _HDSeedLoop_populateLoopKeyrings() {
    for (let ticker in supportedChains) {
        let chain = supportedChains[ticker];
        let ringOptions = {
            // default path is BIP-44 ethereum coin type, where depth 5 is the address index
            path: chain.path,
            strength: 128,
            mnemonic: __classPrivateFieldGet(this, _HDSeedLoop_mnemonic, "f"),
            chainTicker: chain.ticker
        };
        // create new key ring for chain given setup options
        var keyRing = new HDKeyring(ringOptions);
        // add key ring to seed loop 
        this.addKeyRing(keyRing);
    }
};
//# sourceMappingURL=seedLoop.js.map