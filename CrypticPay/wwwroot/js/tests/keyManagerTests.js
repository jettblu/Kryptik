import HDSeedLoop from "../keyManager/seedLoop.js";
import { defaultOptions } from "../keyManager/utils.js";
// make sure seed loop setup works as expected
console.log("Creating seed loop....");
var seedLoop = new HDSeedLoop(defaultOptions);
console.log("Seed loop:");
console.log(seedLoop);
var serializedSeedLoop = seedLoop.serializeSync();
console.log(serializedSeedLoop);
console.log("One way trip complete!");
var seedLoopRoundTrip = HDSeedLoop.deserialize(serializedSeedLoop);
console.log("Deserialized seed loop:");
console.log(seedLoopRoundTrip);
//# sourceMappingURL=keyManagerTests.js.map