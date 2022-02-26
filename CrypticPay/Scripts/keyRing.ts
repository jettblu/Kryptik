import { generateMnemonic } from "bip39"
import { Wallet } from "@ethersproject/wallet"
import { HDNode } from "@ethersproject/hdnode"
import { TransactionRequest } from "@ethersproject/abstract-provider"
import { TypedDataDomain, TypedDataField } from "@ethersproject/abstract-signer"


export type Options = {
    strength?: number
    path?: string
    mnemonic?: string | null
}

const defaultEthOptions = {
    // default path is BIP-44, where depth 5 is the address index
    path: "m/44'/60'/0'/0",
    strength: 256,
    mnemonic: null,
}


