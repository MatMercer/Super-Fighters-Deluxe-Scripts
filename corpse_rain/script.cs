// Settings vars
// Stop teh rain on gameover?
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

// Objects that will rain list
// "PLAYER" is a special flag
// to make a player rain
// Remove the "//" in the line
// to select which objects
string[] rainDrops = {
    "PLAYER",
    "BarrelExplosive"
};

// Sets the drops speed rand
int[] rainDropsSpeed = {-20, 20};

// Sets the drop angular velocity
// (only for objects, players wont be
// affected)
int[] rainDropsAngularSpeed = {-10, 10};

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
    Utils.SetTimer("DeleteGlibets", "", 0, 5000);
    Utils.SetTimer("CheckHits", "", 0, 100);

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
// TODO: add all objects, not only glibets
public void DeleteGlibets(TriggerArgs args){
    foreach (IObject obj in Game.GetObjectsByName("Giblet00")){
        obj.Remove();
    }

    foreach (IObject obj in Game.GetObjectsByName("Giblet01")){
        obj.Remove();
    }

    foreach (IObject obj in Game.GetObjectsByName("Giblet02")){
        obj.Remove();
    }

    foreach (IObject obj in Game.GetObjectsByName("Giblet03")){
        obj.Remove();
    }

    foreach (IObject obj in Game.GetObjectsByName("Giblet04")){
        obj.Remove();
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
}