// creates shares for a given secret
// returns xpub and 1 seed share for remote storage
// one seed share is saved on client
// uniqueId is used to differentiate if multiple local shares
var createShares = function (unqiqueId) {
    console.log("Creating shares");
    var mnemonic = bip.generateMnemonic();
    console.log(mnemonic);
    // retrieve seed from mnemonic to be used for xpub generation
    var seed = bip.mnemonicToSeedSync(mnemonic);
    console.log("Seed:");
    console.log(seed);
    var hdk = hdkey.fromMasterSeed(seed);
    console.log("Hdk:");
    console.log(hdk);
    // get extended key for remote storage
    var extPubKey = hdk.publicExtendedKey;
    console.log("Extended public key:");
    console.log(extPubKey);

    // convert mnemonic to hex
    var mnemonicHex = shamir.str2hex(mnemonic);
    var shares = shamir.share(mnemonicHex, 4, 2);
    console.log(shares);
    // Add unique identifier so there can be multiple accounts on same browser
    var seedStorageName = "seedShare" + uniqueId;
    // save one share to browser's local memory
    localStorage.setItem(seedStorageName, shares[0]);
    // use another share for remote storage
    var remoteShare = shares[1];
    // create return object
    remoteData = {
        seedShareHex: remoteShare,
        xpub: extPubKey
    };
    console.log("Shares created.");
    var mnemonicRes = combineShares(localStorage.getItem(seedStorageName), remoteShare);
    console.log(mnemonicRes);
    return remoteData;
}


// combines shares to regenerate seed
var combineShares = function (share1, share2) {
    // combine 2 shares
    comb = shamir.combine([share1, share2])
    // return hex version of combined shares
    return comb;
}

// decrypt message w/ client seed
var decryptCipher= function (seed, encrypted) {
    // create hd key from client seed
    var hdk = hdkey.fromMasterSeed(buffer.Buffer.from(seed, 'hex'));
    var path = sessionStorage.getItem("keypath");
    var childKey = hdk.derive(path);
    console.log("Decryption key:");
    console.log(childKey);
    return crypt.decrypt(childKey.privateKey, encrypted);
}

// encrypt smessage with client seed
var encryptMessageWithSeed = function (seed, plainText){
    var hdk = hdkey.fromMasterSeed(buffer.Buffer.from(seed, 'hex'));
    var path = sessionStorage.getItem("keypath");
    // child key used to encrypt and decrypt messages
    var childKey = hdk.derive(path);
    console.log("Encryption key:");
    console.log(childKey);
    return crypt.encrypt(childKey.publicKey, buffer.Buffer.from(plainText));
}

// encrypt smessage with recipient public key
var encryptMessageWithPub = function (xpubKeyString, plainText) {
    console.log("Extended pub key passed in:");
    console.log(xpubKeyString);
    // path already derived on server
    var childKey = hdkey.fromExtendedKey(xpubKeyString);
    console.log(childKey);
    // child key used to encrypt and decrypt messages
    /*var keyBuffer = buffer.Buffer.from(pubKeyString);
    console.log("Pub key buffer passed in:");
    console.log(keyBuffer);*/
    return crypt.encrypt(childKey.publicKey, buffer.Buffer.from(plainText));
}

var generateEncryptorTest = function (seed, message, path) {
    hdk = hdkey.fromMasterSeed(buffer.Buffer.from(seed, 'hex'));
    // child key used to encrypt and decrypt messages
    var childKey = hdk.derive(path);
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

// returns current user's local seed share
var getLocalShare = function () {
    var uniqueId = sessionStorage.getItem("uname");
    var remoteStorageName = "seedShare" + uniqueId;
    var shareLocal = localStorage.getItem(remoteStorageName);
    return shareLocal;
}
        