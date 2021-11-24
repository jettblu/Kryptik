// script used with community/test for testing purposes


var runSim = function () {
    console.log("Sim started!");
    var remoteData = createShares();
    generateEncryptorTest(remoteData.seedShareHex, "Boom shakalaka");
}

$("#testBtn").on('click', runSim);

