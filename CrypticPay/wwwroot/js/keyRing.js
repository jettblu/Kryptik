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
var _HDKeyring_instances, _HDKeyring_hdNode, _HDKeyring_addressIndex, _HDKeyring_wallets, _HDKeyring_addressToWallet, _HDKeyring_mnemonic, _HDKeyring_deriveChildWallet;
import { generateMnemonic } from "bip39";
import { Wallet } from "@ethersproject/wallet";
import { HDNode } from "@ethersproject/hdnode";
import { normalizeHexAddress, validateAndFormatMnemonic } from "./utils";
export { normalizeHexAddress, normalizeMnemonic, validateAndFormatMnemonic, } from "./utils";
const defaultOptions = {
    // default path is BIP-44 ethereum coin type, where depth 5 is the address index
    path: "m/44'/60'/0'/0",
    strength: 256,
    mnemonic: null,
};
export default class HDKeyring {
    // constructor that builds hdkeyring on init
    constructor(options = {}) {
        _HDKeyring_instances.add(this);
        _HDKeyring_hdNode.set(this, void 0);
        _HDKeyring_addressIndex.set(this, void 0);
        _HDKeyring_wallets.set(this, void 0);
        _HDKeyring_addressToWallet.set(this, void 0);
        _HDKeyring_mnemonic.set(this, void 0);
        const hdOptions = Object.assign(Object.assign({}, defaultOptions), options);
        const mnemonic = validateAndFormatMnemonic(hdOptions.mnemonic || generateMnemonic(hdOptions.strength));
        if (!mnemonic) {
            throw new Error("Invalid mnemonic.");
        }
        __classPrivateFieldSet(this, _HDKeyring_mnemonic, mnemonic, "f");
        this.path = hdOptions.path;
        __classPrivateFieldSet(this, _HDKeyring_hdNode, HDNode.fromMnemonic(mnemonic, undefined, "en").derivePath(this.path), "f");
        this.id = __classPrivateFieldGet(this, _HDKeyring_hdNode, "f").fingerprint;
        __classPrivateFieldSet(this, _HDKeyring_addressIndex, 0, "f");
        __classPrivateFieldSet(this, _HDKeyring_wallets, [], "f");
        __classPrivateFieldSet(this, _HDKeyring_addressToWallet, {}, "f");
    }
    serializeSync() {
        return {
            version: 1,
            id: this.id,
            mnemonic: __classPrivateFieldGet(this, _HDKeyring_mnemonic, "f"),
            keyringType: HDKeyring.type,
            path: this.path,
            addressIndex: __classPrivateFieldGet(this, _HDKeyring_addressIndex, "f"),
        };
    }
    async serialize() {
        return this.serializeSync();
    }
    static deserialize(obj) {
        const { version, keyringType, mnemonic, path, addressIndex } = obj;
        if (version !== 1) {
            throw new Error(`Unknown serialization version ${obj.version}`);
        }
        if (keyringType !== HDKeyring.type) {
            throw new Error("HDKeyring only supports BIP-32/44 style HD wallets.");
        }
        const keyring = new HDKeyring({
            mnemonic,
            path,
        });
        keyring.addAddressesSync(addressIndex);
        return keyring;
    }
    async signTransaction(address, transaction) {
        const normAddress = normalizeHexAddress(address);
        if (!__classPrivateFieldGet(this, _HDKeyring_addressToWallet, "f")[normAddress]) {
            throw new Error("Address not found!");
        }
        return __classPrivateFieldGet(this, _HDKeyring_addressToWallet, "f")[normAddress].signTransaction(transaction);
    }
    async signTypedData(address, domain, types, value) {
        const normAddress = normalizeHexAddress(address);
        if (!__classPrivateFieldGet(this, _HDKeyring_addressToWallet, "f")[normAddress]) {
            throw new Error("Address not found!");
        }
        // eslint-disable-next-line no-underscore-dangle
        return __classPrivateFieldGet(this, _HDKeyring_addressToWallet, "f")[normAddress]._signTypedData(domain, types, value);
    }
    async signMessage(address, message) {
        const normAddress = normalizeHexAddress(address);
        if (!__classPrivateFieldGet(this, _HDKeyring_addressToWallet, "f")[normAddress]) {
            throw new Error("Address not found!");
        }
        return __classPrivateFieldGet(this, _HDKeyring_addressToWallet, "f")[normAddress].signMessage(message);
    }
    addAddressesSync(numNewAccounts = 1) {
        const numAddresses = __classPrivateFieldGet(this, _HDKeyring_addressIndex, "f");
        for (let i = 0; i < numNewAccounts; i += 1) {
            __classPrivateFieldGet(this, _HDKeyring_instances, "m", _HDKeyring_deriveChildWallet).call(this, i + numAddresses);
        }
        __classPrivateFieldSet(this, _HDKeyring_addressIndex, __classPrivateFieldGet(this, _HDKeyring_addressIndex, "f") + numNewAccounts, "f");
        const addresses = this.getAddressesSync();
        return addresses.slice(-numNewAccounts);
    }
    async addAddresses(numNewAccounts = 1) {
        return this.addAddressesSync(numNewAccounts);
    }
    getAddressesSync() {
        return __classPrivateFieldGet(this, _HDKeyring_wallets, "f").map((w) => normalizeHexAddress(w.address));
    }
    async getAddresses() {
        return this.getAddressesSync();
    }
}
_HDKeyring_hdNode = new WeakMap(), _HDKeyring_addressIndex = new WeakMap(), _HDKeyring_wallets = new WeakMap(), _HDKeyring_addressToWallet = new WeakMap(), _HDKeyring_mnemonic = new WeakMap(), _HDKeyring_instances = new WeakSet(), _HDKeyring_deriveChildWallet = function _HDKeyring_deriveChildWallet(index) {
    const newPath = `${index}`;
    const childNode = __classPrivateFieldGet(this, _HDKeyring_hdNode, "f").derivePath(newPath);
    const wallet = new Wallet(childNode);
    __classPrivateFieldGet(this, _HDKeyring_wallets, "f").push(wallet);
    const address = normalizeHexAddress(wallet.address);
    __classPrivateFieldGet(this, _HDKeyring_addressToWallet, "f")[address] = wallet;
};
HDKeyring.type = "bip32";
//# sourceMappingURL=keyRing.js.map