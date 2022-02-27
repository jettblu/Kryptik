import HDSeedLoop, {SerializedSeedLoop} from "../keyManager/seedLoop.js"
import { defaultOptions } from "../keyManager/utils.js"


// make sure seed loop setup works as expected
console.log("Creating seed loop....")
var seedLoop: HDSeedLoop = new HDSeedLoop(defaultOptions)
console.log("Seed loop:")
console.log(seedLoop)

var serializedSeedLoop: SerializedSeedLoop = seedLoop.serializeSync();

console.log(serializedSeedLoop);

