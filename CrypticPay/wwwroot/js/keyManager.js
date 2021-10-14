// creates shares for a given secret
// returns xpub and 1 seed share for remote storage
// one seed share is saved on client
function createShares() {
    var mnemonic = generateMnemonic(wordlists.english);
    // retrieve seed from mnemonic to be used
    var seed = mnemonicToSeed(mnemonic);
    var hdkey = fromMasterSeed(Buffer.from(seed, 'hex'));
    var extPubKey = hdkey.publicExtendedKey();
    // convert the seed into a hex string
    var seedHex = secrets.str2hex(seed);
    var shares = secrets.share(seedHex, 4, 2);
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
    return remoteData;
}
