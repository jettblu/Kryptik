# Kryptik

## What is Kryptik?
Kryptik is an online platform for sending and collecting cryptocurrency. The Kryptik website simplifies digital ownership, by providing a simple cryptocurrency wallet— compatible with multiple blockchains— and a social NFT marketplace. All software is free and open source, with the belief that better blockchain technology will lead to a better world. 

Kryptik operations are currently supported by a Carnegie Mellon research grant. You can join the waitlist for early access [here](https://kryptik.app/).

<img src="https://jetthays.com/media/external/kryptikSearch.png" alt="Wallet Page screenshot" width="150">

## Key Management
To give users full control over their crypto assets, Kryptik separates client-server responsibilities. The client is responsible for key management and transactions; the server helps with key recovery and address sharing. This is accomplished via shamir secret sharing.
	
## Wallet 
Kryptik uses a hierarchical deterministic (HD) wallet derived from a BIP32 seed. Each blockchain has its own keyring and keyrings that share a common seed hang from a 'seed loop'. Kryptik currently supports the following currencies:
* Avalanche
* Ethereum
* Harmony
* Solana

Note: UTXO based blockchains like Bitcoin will be addded soon.

## Decentralized File Storage
Kryptik provides access to IPFS, allowing users to store and display decentralized media. This is an essential first step to enabling an NFT marketplace where unique files can be bought and sold via a social feed. 


## Tech Stack
Backend: C#, SignalR

Front end: Razor, Javascript, Ajax, CSS

Database: SQL and Azure

Blockchain Infrastructure: Pocket Network




