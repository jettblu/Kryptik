export function normalizeMnemonic(mnemonic: string): string {
    return mnemonic.trim().toLowerCase().replace(/\r/, " ").replace(/ +/, " ")
}

export function test() {
    console.log("TEST!")
}