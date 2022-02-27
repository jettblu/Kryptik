var bip = require("../ext/bip39.min.js")
import { Wallet } from "@ethersproject/wallet"
import { HDNode } from "@ethersproject/hdnode"
import { TransactionRequest } from "@ethersproject/abstract-provider"
import { TypedDataDomain, TypedDataField } from "@ethersproject/abstract-signer"
import { normalizeHexAddress, validateAndFormatMnemonic, Options, defaultOptions } from "./utils"
import { Chain, chainFromTicker } from "./chain.js"


export {
    normalizeHexAddress,
    normalizeMnemonic
} from "./utils"


export type SerializedHDKeyring = {
    version: number
    id: string
    mnemonic: string
    path: string
    keyringType: string
    addressIndex: number
    chainTicker: string
}

export interface Keyring<T> {
    serialize(): Promise<T>
    getAddresses(): Promise<string[]>
    addAddresses(n?: number): Promise<string[]>
    signTransaction(
        address: string,
        transaction: TransactionRequest
    ): Promise<string>
    signTypedData(
        address: string,
        domain: TypedDataDomain,
        types: Record<string, Array<TypedDataField>>,
        value: Record<string, unknown>
    ): Promise<string>
    signMessage(address: string, message: string): Promise<string>
}

export interface KeyringClass<T> {
    new(): Keyring<T>
    deserialize(serializedKeyring: T): Promise<Keyring<T>>
}



export default class HDKeyring implements Keyring<SerializedHDKeyring> {
    static readonly type: string = "bip32"

    readonly path: string

    readonly id: string

    #hdNode: HDNode

    #addressIndex: number

    #wallets: Wallet[]

    #addressToWallet: { [address: string]: Wallet }

    #mnemonic: string
    readonly chain: Chain

    // constructor that builds hdkeyring on init
    constructor(options: Options = {}) {
        const hdOptions: Required<Options> = {
            ...defaultOptions,
            ...options,
        }

        const mnemonic = validateAndFormatMnemonic(
            hdOptions.mnemonic || bip.generateMnemonic(hdOptions.strength)
        )

        if (!mnemonic) {
            throw new Error("Invalid mnemonic.")
        }

        this.#mnemonic = mnemonic
        this.chain = chainFromTicker(hdOptions.chainTicker)
        this.path = hdOptions.path
        this.#hdNode = HDNode.fromMnemonic(mnemonic, undefined, "en").derivePath(
            this.path
        )
        this.id = this.#hdNode.fingerprint
        this.#addressIndex = 0
        this.#wallets = []
        this.#addressToWallet = {}
    }

    serializeSync(): SerializedHDKeyring {
        return {
            version: 1,
            id: this.id,
            mnemonic: this.#mnemonic,
            keyringType: HDKeyring.type,
            path: this.path,
            addressIndex: this.#addressIndex,
            chainTicker: this.chain.ticker
        }
    }

    async serialize(): Promise<SerializedHDKeyring> {
        return this.serializeSync()
    }

    static deserialize(obj: SerializedHDKeyring): HDKeyring {
        const { version, keyringType, mnemonic, path, addressIndex, chainTicker } = obj
        if (version !== 1) {
            throw new Error(`Unknown serialization version ${obj.version}`)
        }

        if (keyringType !== HDKeyring.type) {
            throw new Error("HDKeyring only supports BIP-32/44 style HD wallets.")
        }

        const keyring = new HDKeyring({
            mnemonic,
            path,
            chainTicker
        })

        keyring.addAddressesSync(addressIndex)

        return keyring
    }

    async signTransaction(
        address: string,
        transaction: TransactionRequest
    ): Promise<string> {
        const normAddress = normalizeHexAddress(address)
        if (!this.#addressToWallet[normAddress]) {
            throw new Error("Address not found!")
        }
        return this.#addressToWallet[normAddress].signTransaction(transaction)
    }

    async signTypedData(
        address: string,
        domain: TypedDataDomain,
        types: Record<string, Array<TypedDataField>>,
        value: Record<string, unknown>
    ): Promise<string> {
        const normAddress = normalizeHexAddress(address)
        if (!this.#addressToWallet[normAddress]) {
            throw new Error("Address not found!")
        }
        // eslint-disable-next-line no-underscore-dangle
        return this.#addressToWallet[normAddress]._signTypedData(
            domain,
            types,
            value
        )
    }

    async signMessage(address: string, message: string): Promise<string> {
        const normAddress = normalizeHexAddress(address)
        if (!this.#addressToWallet[normAddress]) {
            throw new Error("Address not found!")
        }
        return this.#addressToWallet[normAddress].signMessage(message)
    }

    addAddressesSync(numNewAccounts = 1): string[] {
        const numAddresses = this.#addressIndex

        for (let i = 0; i < numNewAccounts; i += 1) {
            this.#deriveChildWallet(i + numAddresses)
        }

        this.#addressIndex += numNewAccounts
        const addresses = this.getAddressesSync()
        return addresses.slice(-numNewAccounts)
    }

    async addAddresses(numNewAccounts = 1): Promise<string[]> {
        return this.addAddressesSync(numNewAccounts)
    }

    #deriveChildWallet(index: number): void {
        const newPath = `${index}`

        const childNode = this.#hdNode.derivePath(newPath)
        const wallet = new Wallet(childNode)

        this.#wallets.push(wallet)
        const address = normalizeHexAddress(wallet.address)
        this.#addressToWallet[address] = wallet
    }

    getAddressesSync(): string[] {
        return this.#wallets.map((w) => normalizeHexAddress(w.address))
    }

    async getAddresses(): Promise<string[]> {
        return this.getAddressesSync()
    }
}

