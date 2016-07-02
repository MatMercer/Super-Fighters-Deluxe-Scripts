/*    
  __  __                                          _     
 |  \/  |      /\                                ( )    
 | \  / |_ __ /  \   _ __  _   _  ___  _ __   ___|/ ___ 
 | |\/| | '__/ /\ \ | '_ \| | | |/ _ \| '_ \ / _ \ / __|
 | |  | | | / ____ \| | | | |_| | (_) | | | |  __/ \__ \
 |_|  |_|_|/_/    \_\_| |_|\__, |\___/|_| |_|\___| |___/
                            __/ |                       
  _____       _          __|___/         _       _      
 |  __ \     (_)        / ____|         (_)     | |     
 | |__) |__ _ _ _ __   | (___   ___ _ __ _ _ __ | |_    
 |  _  // _` | | '_ \   \___ \ / __| '__| | '_ \| __|   
 | | \ \ (_| | | | | |  ____) | (__| |  | | |_) | |_    
 |_|  \_\__,_|_|_| |_| |_____/ \___|_|  |_| .__/ \__|   
                                          | |           
                                          |_|        
                             000      00
                           0000000   0000
              0      00  00000000000000000
            0000 0  000000000000000000000000       0
         000000000000000000000000000000000000000 000
        0000000000000000000000000000000000000000000000
    000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000
              / / / / / / / / / / / / / / / /
            / / / / / / / / / / / / / / /
            / / / / / / / / / / / / / / /
          / / / / / / / / / / / / / /
          / / / / / / / / / / / / /
        / / / / / / / / / / / /
        / / / / / / / / / /

         ...........RUN FOR YOUR LIVES!.
                                                  
*/

// Settings vars
// Stop the rain on gameover?
bool stopOnGameOver = true;

// Primary tick speed ratio,
// affects the rain strenght
int[] tickRand = {200, 900};

// Changes the 'rain stage' after
// some rand time ratio, making it 
// heavier or not
int[] rainStageRand = {3000, 5000};

// Drops life time, makes 
// them disapear after
// XXX miliseconds
int dropsLifeTime = 2000;

// Destroys or remove the
// objects when they are
// removed
static bool destroyOnDeath = false;

// Remove or not junk
// objects
bool removeJunk = true;

// Sets the drops speed rand
int[] rainDropsSpeed = {-20, 20};

// Sets the drop angular velocity
// (only for objects, players wont be
// affected)
int[] rainDropsAngularSpeed = {-10, 10};

// Objects that will rain list
// "PLAYER" is a special flag
// to make a player rain
// Remove the "//" in the line
// to select which objects.
// The last object MUST NOT HAVE
// A COMMA IN THE END
// as follow: array {
//      "firsts",
//      "last"    
// }
string[] rainDrops = {
      "PLAYER",
//    "AcidVat00Top",
//    "AcidVat00A",
//    "AcidVat00B",
//    "AcidVat00C",
//    "Button00",
//    "ButtonPole00",
//    "Lever00",
//    "AtlasStatue00",
//    "AtlasStatue00_D",
//    "AtlasStatue00Globe",
//    "Balloon00",
//    "Balloon00_D",
//    "BambooScaffold00A",
//    "BambooScaffold00B",
//    "BambooScaffold00C",
//    "BambooScaffold00D",
//    "BambooStick00A",
//    "BambooStick00B",
//    "BambooStick00C",
//    "BambooStick00D",
    "Barrel00",
    "BarrelExplosive",
//    "BarrelWreck",
//    "BigMachine00A",
//    "BigMachine00Top",
//    "BlastDoor00A",
//    "BlastDoor00B",
//    "BlastDoor00C",
    "Bottle00",
//    "Bottle00Broken",
//    "BridgePlank00",
//    "BridgePlank01",
//    "Computer00",
//    "MetalBucket00",
//    "Monitor00",
//    "Monitor00_D",
//    "Car01A",
    "CarWheel00",
    "CardboardBox00",
//    "CardboardBox00_D",
//    "CargoContainer00A",
//    "CargoContainer01A",
//    "CargoContainer01B",
//    "CashRegister00",
    "Chair00",
//    "Chandelier00",
//    "Chandelier01A",
//    "Chandelier01B",
//    "Chandelier01C",
//    "Chandelier01D",
//    "ComfyChair",
//    "Concrete01ADynamic",
//    "Concrete01DDynamic",
//    "Concrete01EDynamic",
//    "ConcretePipe01",
//    "ConveyorBelt00A",
//    "ConveyorBelt00B",
//    "ConveyorBelt00C",
//    "CueStick00",
//    "CueStick00Shaft",
//    "CueStick00Debris",
    "CrabCan00",
//    "CrabCan00_D",
//    "Boot00",
    "Gascan00",
//    "GasLamp00_D",
//    "GasMask00",
//    "Helmet00",
//    "Concrete01Weak",
    "Crate00",
    "Crate01",
    "Crate02",
//    "CrumpledPaper00",
//    "Cup00",
//    "Desk00",
//    "Cage00",
//    "Cage00_D",
//    "Dove00",
//    "Duct00C_D",
//    "DrinkingGlass00",
//    "DeskLamp00",
//    "Elevator02A",
//    "Elevator02B",
//    "Elevator02C",
//    "Elevator02D",
//    "Elevator03A",
//    "Elevator03B",
//    "Elevator04A",
//    "FerrisWheel00",
//    "FerrisWheelCart00",
//    "CarnivalCart00",
//    "CarnivalCart01",
//    "CarnivalCart01_D",
//    "CarnivalFerrisWheel",
//    "FileCab00",
//    "Gargoyle00",
//    "Gargoyle00_D",
//    "HangingCrate00",
//    "HangingCrate01",
//    "HangingCrateHolder",
//    "HangingLamp00",
//    "HangingLamp00_D",
//    "Lamp00",
//    "Lamp00_D",
//    "Lamp01",
//    "Lamp01_D",
//    "Lifeboat00",
//    "Hook00",
//    "Hook01",
//    "Lift00A",
//    "Lift00B",
//    "Lift00C",
//    "Lift00D",
//    "MetalHatch00A",
//    "MetalHatch00B",
//    "MetalDesk00",
//    "MetalShelf00A",
//    "MetalShelf00B",
//    "MetalRailing00",
//    "MetalRailing00_D",
//    "MetalTable00",
    "MoneyClip00",
//    "Padlock00",
//    "Padlock00_D",
//    "Pallet00",
//    "PaperBinder00",
//    "PaperLantern00",
   "PaperStack00",
//    "Plank00",
//    "Plank02",
//    "Plank01",
//    "Piano00",
//    "Piano00_D",
//    "Piston00A",
//    "Piston00B",
    "PropaneTank",
//    "Pulley00",
//    "Pulley00Weak",
//    "Pulley01",
//    "Sampan00",
//    "Safe00",
//    "Shelf00",
//    "SearchLight00",
//    "SearchLight00Holder",
//    "Spotlight00A",
//    "Spotlight00A_D",
//    "Spotlight00B",
    "Suitcase00",
//    "Suitcase00Debris1",
//    "Suitcase00Debris2",
//    "SupplyCrate00",
//    "SteamshipWheel00",
//    "StoneWeak00A",
//    "StoneWeak00B",
//    "StoneWeak00C",
//    "SubwayTrain00",
//    "SurveillanceCamera00A",
//    "SwivelChair01",
//    "SwivelChair02",
    "Table00",
//    "Teapot00",
//    "TinRoof00",
//    "TinRoof00_D",
//    "TrainCar00A",
//    "TrainCarLocomotive00A",
//    "TrainingTarget00",
    "Trashbag00",
//    "Trashcan00",
//    "Trashcan00_D",
//    "Trashcan00Lid",
//    "Truck00B",
//    "TruckWheel",
//    "Van00",
//    "WarningLamp00",
//    "WarningLamp01",
//    "WaterTrough00",
//    "WindMill00",
//    "WindMillSail00",
//    "WindMillSail00_D",
//    "WindMillSail00_D2",
//    "WiringTube00A",
//    "WiringTube00A_D",
//    "WiringTube00B",
//    "WoodPlat01WeakA",
//    "WoodPlat01WeakB",
//    "WoodAwning00A",
//    "WoodAwning00B",
//    "WoodAwning00B_D",
//    "WoodBarrel00",
//    "WoodBarrel01",
//    "WoodLattice00A",
//    "WoodRailing00",
//    "WoodSupport00A",
//    "WoodSupport00B",
//    "LightBulb00",
    "LightSign00E",
//    "LightSign00E_D",
    "LightSign00H",
//    "LightSign00H_D",
    "LightSign00L",
//    "LightSign00L_D",
    "LightSign00O",
//    "LightSign00O_D",
    "LightSign00T",
//    "LightSign00T_D",
//    "LightSign00Debris1",
    "XmasPresent00"
//    "XmasTree",
//    "XmasTree_D",
//    "Giblet00",
//    "Giblet01",
//    "Giblet02",
//    "Giblet03",
//    "Giblet04",
//    "StoneDebris00A",
//    "StoneDebris00B",
//    "StoneDebris00C",
//    "StoneDebris00D",
//    "StoneDebris00E",
//    "MetalDebris00A",
//    "MetalDebris00B",
//    "MetalDebris00C",
//    "MetalDebris00D",
//    "MetalDebris00E",
//    "WoodBarrelDebris00A",
//    "WoodBarrelDebris00B",
//    "WoodBarrelDebris00C",
//    "WoodDebris00A",
//    "WoodDebris00B",
//    "WoodDebris00C",
//    "WoodDebris00D",
//    "WoodDebris00E",
//    "WoodDebrisTable00A",
//    "WoodDebrisTable00B",
//    "ChairLeg",
//    "GlassShard00A"
};

// Script vars

// A list of all players skins loaded
// in the current game
private static List<IProfile> pProfileList = new List<IProfile>();

// Death sentences object, used to delete objects after some time
private static List<Hit> hits = new List<Hit>();

// Tick trigger
private IObjectTimerTrigger tickTrigger;

// Randomize time trigger, used to
// make the rain more dynamic
private IObjectTimerTrigger randomizeTimeTrigger;

// A black list to remove
// junk objects
string[] blackList = {
    "CueStick00Debris",
    "Suitcase00Debris1",
    "Suitcase00Debris2",
    "LightSign00Debris1",
    "StoneDebris00A",
    "StoneDebris00B",
    "StoneDebris00C",
    "StoneDebris00D",
    "StoneDebris00E",
    "MetalDebris00A",
    "MetalDebris00B",
    "MetalDebris00C",
    "MetalDebris00D",
    "MetalDebris00E",
    "WoodBarrelDebris00A",
    "WoodBarrelDebris00B",
    "WoodBarrelDebris00C",
    "WoodDebris00A",
    "WoodDebris00B",
    "WoodDebris00C",
    "WoodDebris00D",
    "WoodDebris00E",
    "WoodDebrisTable00A",
    "WoodDebrisTable00B",
    "GlassShard00A",
    "AtlasStatue00_D",
    "Balloon00_D",
    "Monitor00_D",
    "CardboardBox00_D",
    "CrabCan00_D",
    "GasLamp00_D",
    "Cage00_D",
    "Duct00C_D",
    "CarnivalCart01_D",
    "Gargoyle00_D",
    "HangingLamp00_D",
    "Lamp00_D",
    "Lamp01_D",
    "MetalRailing00_D",
    "Padlock00_D",
    "Piano00_D",
    "Spotlight00A_D",
    "TinRoof00_D",
    "Trashcan00_D",
    "WindMillSail00_D",
    "WindMillSail00_D2",
    "WiringTube00A_D",
    "WoodAwning00B_D",
    "LightSign00E_D",
    "LightSign00H_D",
    "LightSign00L_D",
    "LightSign00O_D",
    "LightSign00T_D",
    "XmasTree_D",
    "Bottle00Broken",
    "Giblet00",
    "Giblet01",
    "Giblet02",
    "Giblet03",
    "Giblet04"
};

//Rand
Random rand = new Random();

// Constants
const float DISTANCE_FROM_TOP = 50f;

public void OnStartup(){
    // Get the profiles, from players
    // & custom ones
    foreach(IObjectPlayerProfileInfo pInfo in Game.GetObjectsByName("PlayerProfileInfo")){
        pProfileList.Add(pInfo.GetProfile());
    }

    foreach (IPlayer ply in Game.GetPlayers()){
        pProfileList.Add(ply.GetProfile());
    }

    // Setup the triggers
    tickTrigger = Utils.SetTimer("Tick", "", 0, rand.Next(tickRand[0], tickRand[1]));
    randomizeTimeTrigger = Utils.SetTimer("RandomizeDelay", "", rainStageRand[0], rainStageRand[1]);
    Utils.SetTimer("DeleteTash", "", 0, 20000);
    Utils.SetTimer("CheckHits", "", 0, 100);

    Utils.SendMessage("Welcome to rain script!");
    Utils.SendMessage("Made by: MrAnyone.");
    Utils.SendMessage("Contact me at: m3rc3r99@gmail.com");
    Utils.SendMessage("There are a LOT of settings you can setup.");


    //Game.CreatePlayer(Game.GetSingleObjectByCustomID("spawn").GetWorldPosition());
}

// Main tick
public void Tick(TriggerArgs args){
    //Stop on gameover?
    if(!(Game.IsGameOver && stopOnGameOver)) {
        SpawnDrop(rainDrops[rand.Next(0, rainDrops.Length)]);
    }
}

// Tells which object will be spawned
// PLAYER flags spawns a corpse
public void SpawnDrop(string name) {
    if(name == "PLAYER") {
        SpawnRandomPlayer();
    }
    else {
        SpawnObject(name);
    }
}

// Randomize the rain
public void RandomizeDelay(TriggerArgs args){
    tickTrigger.SetIntervalTime(rand.Next(tickRand[0], tickRand[1]));
    randomizeTimeTrigger.SetIntervalTime(rand.Next(rainStageRand[0], rainStageRand[1]));
}

// Spawns a dead players
// based in the map top
public void SpawnRandomPlayer(){
    IProfile cProfile = pProfileList[rand.Next(pProfileList.Count)];
    IPlayer cPly = Game.CreatePlayer(new Vector2((float)rand.Next((int)Game.GetBorderArea().Left, (int)Game.GetBorderArea().Right), (int)Game.GetBorderArea().Top + DISTANCE_FROM_TOP));
    cPly.SetProfile(cProfile);
    cPly.SetLinearVelocity(new Vector2((float)rand.Next(rainDropsSpeed[0], rainDropsSpeed[1]), (float)rand.Next(rainDropsSpeed[0], rainDropsSpeed[1])));
    cPly.Kill();

    AddHit((IObject)cPly);
}

// Spawns a object
// based in the map top
public void SpawnObject(string name) {
    IObject obj = Game.CreateObject(name, new Vector2((float)rand.Next((int)Game.GetBorderArea().Left, (int)Game.GetBorderArea().Right), (int)Game.GetBorderArea().Top + DISTANCE_FROM_TOP), 0, new Vector2((float)rand.Next(rainDropsSpeed[0], rainDropsSpeed[1]), (float)rand.Next(rainDropsSpeed[0], rainDropsSpeed[1])), (float)rand.Next(rainDropsAngularSpeed[0], rainDropsAngularSpeed[1]));
    AddHit(obj);
}

// Adds a object to be removed
// after some time in the game
public void AddHit(IObject obj) {
    hits.Add(new Hit(obj, dropsLifeTime));
}

// Checks if some objects must
// be removed after the delays
public void CheckHits(TriggerArgs args){
    try{
        foreach (Hit hit in hits){
            if(hit.MustDie()){
                hit.Kill();
                hits.Remove(hit);
            }
        }
    }catch(Exception e){
        return;
    }
}

// Deletes trash objects
// from the map
public void DeleteTash(TriggerArgs args){
    if(removeJunk) {
        foreach (IObject obj in Game.GetObjectsByName(blackList)){
            obj.Remove();
        }
    }
}

// A class used to control the objects
// life time
private class Hit{
    private IObject obj;
    private float lifetime;
    private float controlTime;

    public Hit(IObject obj, float lifetime){
        this.obj = obj;
        this.lifetime = Game.TotalElapsedGameTime + lifetime;
        this.controlTime = lifetime;
    }

    public bool MustDie(){
        if(Game.TotalElapsedGameTime - this.lifetime > this.controlTime){
            return true;
        }
        else {
            return false;
        }
    }

    public void Kill(){
        if(destroyOnDeath) {
            this.obj.Destroy();
        }
        else {
            this.obj.Remove();
        }
    }
}

// Utils class
private class Utils{
    //Sets a timer
    public static IObjectTimerTrigger SetTimer(string met, string id, int times, int delay){
        IObjectTimerTrigger trigger = (IObjectTimerTrigger)Game.CreateObject("TimerTrigger");
        trigger.CustomId = id;
        trigger.SetScriptMethod(met);
        trigger.SetIntervalTime(delay);
        trigger.SetRepeatCount(times);
        trigger.Trigger();
        return trigger;
    }
    public static void SendMessage(string msg){
        Game.RunCommand("/MSG |RAIN MOD| " + msg);
    }
}