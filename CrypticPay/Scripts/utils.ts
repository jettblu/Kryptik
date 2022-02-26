﻿import { validateMnemonic } from "bip39"

export function normalizeMnemonic(mnemonic: string): string {
    return mnemonic.trim().toLowerCase().replace(/\r/, " ").replace(/ +/, " ")
}

export function validateAndFormatMnemonic(
    mnemonic: string,
    wordlist?: string[]
): string | null {
    const normalized = normalizeMnemonic(mnemonic)

    if (validateMnemonic(normalized, wordlist)) {
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