// Settings vars
// Primary tick speed ratio,
// affects the rain delay
int[] tickRand = {200, 900};

// Changes the 'rain stage' after
// some rand time ratio, making it 
// heavier or not
int[] rainStageRand = {3000, 5000};

// Drops life time, makes 
// them disapear after
// XXX miliseconds
int dropsLifeTime = 2000;

// Objects that will rain list
// "PLAYER" is a special flag
// to make a player rain
// Remove the "//" in the line
// to select which objects
string[] rainDrops = {
    "PLAYER"
};

// Script vars

// A list of all players skins loaded
// in the current game
private static List<IProfile> pProfileList = new List<IProfile>();

// Death sentences object, used to delete objects after some time
private static List<DeathSentence> DeathSentences = new List<DeathSentence>();

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
    foreach(IObjectPlayerProfileInfo pInfo in Game.GetObjectsByName("PlayerProfileInfo")){
        pProfileList.Add(pInfo.GetProfile());
    }

    foreach (IPlayer ply in Game.GetPlayers()){
        pProfileList.Add(ply.GetProfile());
    }

    tickTrigger = Utils.SetTimer("Tick", "", 0, rand.Next(tickRand[0], tickRand[1]));
    randomizeTimeTrigger = Utils.SetTimer("RandomizeDelay", "", rainStageRand[0], rainStageRand[1]);
    Utils.SetTimer("DeleteGlibets", "", 0, 5000);
    Utils.SetTimer("CheckDeathSentences", "", 0, 100);

    //Game.CreatePlayer(Game.GetSingleObjectByCustomID("spawn").GetWorldPosition());
}

public void Tick(TriggerArgs args){
   SpawnDrop(rainDrops[rand.Next(0, rainDrops.Length)]);
}

public void SpawnDrop(string name) {
    if(name == "PLAYER") {
        SpawnRandomPlayer();
    }
}

public void RandomizeDelay(TriggerArgs args){
    tickTrigger.SetIntervalTime(rand.Next(tickRand[0], tickRand[1]));
    randomizeTimeTrigger.SetIntervalTime(rand.Next(rainStageRand[0], rainStageRand[1]));
}

public void SpawnRandomPlayer(){
    if(Game.IsGameOver){
        return;
    }
    IProfile cProfile = pProfileList[rand.Next(pProfileList.Count)];
    IPlayer cPly = Game.CreatePlayer(new Vector2((float)rand.Next((int)Game.GetBorderArea().Left, (int)Game.GetBorderArea().Right), (int)Game.GetBorderArea().Top + DISTANCE_FROM_TOP));
    cPly.SetProfile(cProfile);
    cPly.SetLinearVelocity(new Vector2((float)rand.Next(-20, 20), (float)rand.Next(-20, 10)));
    cPly.Kill();

    DeathSentences.Add(new DeathSentence((IObject)cPly, dropsLifeTime));
}

public void CheckDeathSentences(TriggerArgs args){
    try{
        foreach (DeathSentence hit in DeathSentences){
            if(hit.MustDie()){
                hit.Kill();
                DeathSentences.Remove(hit);
            }
        }
    }catch(Exception e){
        return;
    }
}

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

private class DeathSentence{
    private IObject obj;
    private float lifetime;
    private float controlTime;

    public DeathSentence(IObject obj, float lifetime){
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
        this.obj.Remove();
        this.obj.Destroy();
    }
}

private class Utils{
    //Sets a timer
    private static IObjectTimerTrigger tickTrigger;
    public static IObjectTimerTrigger SetTimer(string met, string id, int times, int delay){
        tickTrigger = (IObjectTimerTrigger)Game.CreateObject("TimerTrigger");
        tickTrigger.CustomId = id;
        tickTrigger.SetScriptMethod(met);
        tickTrigger.SetIntervalTime(delay);
        tickTrigger.SetRepeatCount(times);
        tickTrigger.Trigger();
        return tickTrigger;
    }
}