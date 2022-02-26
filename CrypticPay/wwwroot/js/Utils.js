export function normalizeMnemonic(mnemonic) {
    return mnemonic.trim().toLowerCase().replace(/\r/, " ").replace(/ +/, " ");
}
export function test() {
    console.log("TEST!");
}
//# sourceMappingURL=utils.js.map