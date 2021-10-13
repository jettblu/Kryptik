import { generateMnemonic, mnemonicToSeed, wordlists } from "../lib/bip39/src";
import * as HDKey from "../lib/hdkey/lib/hdkey";
import { fromMasterSeed } from "../lib/hdkey/lib/hdkey";


// creates shares of a given 
var createShares = function () {
    var mnemonic = generateMnemonic(wordlists.english);
    // retrieve seed from mnemonic to be used
    var seed = mnemonicToSeed(mnemonic);
    var hdkey = fromMasterSeed(Buffer.from(seed, 'hex'));
    // initial key pair created with index 0
    var childKey = hdkey.deriveChild(0);
    var extPubKey = hdkey.publicExtendedKey();
    // convert the private key into a hex string
    var privHex = secrets.str2hex(childKey.privateExtendedKey) 
    var shares = secrets.share(pwHex, 4, 2);
    // save one share to browser's local memory
    localStorage.setItem("keyShare", shares[0]);
    // save another share for remote storage
    var remoteShare = shares[1];

}