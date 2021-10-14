// script used with community/test for testing purposes


var runSim = function () {
    console.log("Sim started!");
    var mnem = bip.generateMnemonic();
    console.log(mnem);
}

$("#testBtn").on('click', createShares);