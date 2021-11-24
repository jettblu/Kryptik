// creates shares for a given secret
// returns xpub and 1 seed share for remote storage
// one seed share is saved on client
var createShares = function () {
    console.log("Creating shares");
    var mnemonic = bip.generateMnemonic();
    console.log(mnemonic);
    // retrieve seed from mnemonic to be used for xpub generation
    var seed = bip.mnemonicToSeed(mnemonic);
    var hdk = hdkey.fromMasterSeed(seed);
    // get extended key for remote storage
    var extPubKey = hdk.publicExtendedKey;
    console.log("Extended public key.");
    
    console.log(extPubKey);
    // convert mnemonic to hex
    var mnemonicHex = shamir.str2hex(mnemonic);
    var shares = shamir.share(mnemonicHex, 4, 2);
    console.log(shares);
    // save one share to browser's local memory
    localStorage.setItem("seedShare", shares[0]);
    // use another share for remote storage
    var remoteShare = shares[1];
    // create return object
    remoteData = {
        seedShareHex: remoteShare,
        xpub: extPubKey
    };
    console.log("Shares created.");
    var mnemonicRes = combineShares(localStorage.getItem("seedShare"), remoteShare);
    console.log(mnemonicRes);
    return remoteData;
}


// combines shares to regenerate seed
var combineShares = function (share1, share2) {
    // combine 2 shares
    comb = shamir.combine([share1, share2])
    //convert Hex back to regular string
    res = shamir.hex2str(comb)
    return res;
}

var generateEncryptorTest = function (seed, message) {
    hdk = hdkey.fromMasterSeed(buffer.Buffer.from(seed, 'hex'));
    // child key used to encrypt and decrypt messages
    var childKey = hdk.derive("m/0/1/2/3/4");
    console.log(childKey.publicKey);
    // encrypt with public key
   /* crypt.encrypt(childKey.publicKey, buffer.Buffer.from(message)).then(function (encrypted) {
            // decrypt message
            crypt.decrypt(childKey.privateKey, encrypted).then(function (plaintext) {
            console.log("Message to part A:", plaintext.toString());
            });*/
    console.log(childKey.privateKey)
    crypt.encrypt(childKey.publicKey, buffer.Buffer.from(message)).then(function (encrypted) {
   // A decrypting the message.
   crypt.decrypt(childKey.privateKey, encrypted).then(function (plaintext) {
       console.log("Message to part A:", plaintext.toString());
   });
    });
  }

        