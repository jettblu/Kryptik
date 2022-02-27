var bip = require("../ext/bip39.min.js")


export type Options = {
    strength?: number
    path?: string
    mnemonic?: string | null
    chainTicker?: string
    isCreation?: boolean
}

export const defaultOptions = {
    // default path is BIP-44 ethereum coin type, where depth 5 is the address index
    path: "m/44'/60'/0'/0",
    strength: 128,
    mnemonic: null,
    chainTicker: "Eth",
    isCreation: true
}

export function normalizeMnemonic(mnemonic: string): string {
    return mnemonic.trim().toLowerCase().replace(/\r/, " ").replace(/ +/, " ")
}

export function validateAndFormatMnemonic(
    mnemonic: string,
    wordlist?: string[]
): string | null {
    const normalized = normalizeMnemonic(mnemonic)

    if (bip.validateMnemonic(normalized, wordlist)) {
        return normalized
    }
    return null
}

export function normalizeHexAddress(address: string | Buffer): string {
    const addressString =
        typeof address === "object" && !("toLowerCase" in address)
            ? address.toString("hex")
            : address
    const noPrefix = addressString.replace(/^0x/, "")
    const even = noPrefix.length % 2 === 0 ? noPrefix : `0${noPrefix}`
    return `0x${Buffer.from(even, "hex").toString("hex")}`
}


