var bip = require("../ext/bip39.min.js");
export const defaultOptions = {
    // default path is BIP-44 ethereum coin type, where depth 5 is the address index
    path: "m/44'/60'/0'/0",
    strength: 128,
    mnemonic: null,
    chainTicker: "Eth",
    isCreation: true
};
export function normalizeMnemonic(mnemonic) {
    return mnemonic.trim().toLowerCase().replace(/\r/, " ").replace(/ +/, " ");
}
export function validateAndFormatMnemonic(mnemonic, wordlist) {
    const normalized = normalizeMnemonic(mnemonic);
    if (bip.validateMnemonic(normalized, wordlist)) {
        return normalized;
    }
    return null;
}
export function normalizeHexAddress(address) {
    const addressString = typeof address === "object" && !("toLowerCase" in address)
        ? address.toString("hex")
        : address;
    const noPrefix = addressString.replace(/^0x/, "");
    const even = noPrefix.length % 2 === 0 ? noPrefix : `0${noPrefix}`;
    return `0x${Buffer.from(even, "hex").toString("hex")}`;
}
//# sourceMappingURL=utils.js.map