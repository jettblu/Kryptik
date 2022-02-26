import { validateMnemonic } from "bip39";
export function normalizeMnemonic(mnemonic) {
    return mnemonic.trim().toLowerCase().replace(/\r/, " ").replace(/ +/, " ");
}
export function validateAndFormatMnemonic(mnemonic, wordlist) {
    var normalized = normalizeMnemonic(mnemonic);
    if (validateMnemonic(normalized, wordlist)) {
        return normalized;
    }
    return null;
}
export function normalizeHexAddress(address) {
    var addressString = typeof address === "object" && !("toLowerCase" in address)
        ? address.toString("hex")
        : address;
    var noPrefix = addressString.replace(/^0x/, "");
    var even = noPrefix.length % 2 === 0 ? noPrefix : "0".concat(noPrefix);
    return "0x".concat(Buffer.from(even, "hex").toString("hex"));
}
//# sourceMappingURL=utils.js.map