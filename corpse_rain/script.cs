private static List<IProfile> pProfileList = new List<IProfile>();
private static List<DeathSentence> DeathSentences = new List<DeathSentence>();
private IProfile cProfile;
private IPlayer cPly;
private IObjectTimerTrigger tTrigger;
private IObjectTimerTrigger randTimeTTrigger;

private int deadPlyCount;
//Rand
Random rand = new Random();

public void OnStartup(){
    Game.SetAllowedCameraModes(CameraMode.Static);
    Game.SetCurrentCameraMode(CameraMode.Static);
    foreach(IObjectPlayerProfileInfo pInfo in Game.GetObjectsByName("PlayerProfileInfo")){
        pProfileList.Add(pInfo.GetProfile());
    }

    foreach (IPlayer ply in Game.GetPlayers()){
        pProfileList.Add(ply.GetProfile());
    }

    tTrigger = Utils.SetTimer("Tick", "", 0, rand.Next(100, 900));
    randTimeTTrigger = Utils.SetTimer("RandomizeDelay", "", 0, 3000);
    Utils.SetTimer("DeleteGlibets", "", 0, 5000);
    Utils.SetTimer("CheckDeathSentences", "", 0, 200);

    //Game.CreatePlayer(Game.GetSingleObjectByCustomID("spawn").GetWorldPosition());
}

public void Tick(TriggerArgs args){
    SpawnRandomPlayer(args);
}

public void RandomizeDelay(TriggerArgs args){
    tTrigger.SetIntervalTime(rand.Next(100, 900));
    randTimeTTrigger.SetIntervalTime(rand.Next(3000, 5000));
}

public void SpawnRandomPlayer(TriggerArgs args){
    if(Game.IsGameOver){
        return;
    }
    cProfile = pProfileList[rand.Next(pProfileList.Count)];
    cPly = Game.CreatePlayer(new Vector2((float)rand.Next(-140, 140), 160f));
    cPly.SetProfile(cProfile);
    cPly.SetLinearVelocity(new Vector2((float)rand.Next(-20, 20), (float)rand.Next(-20, 10)));
    cPly.Kill();

    DeathSentences.Add(new DeathSentence((IObject)cPly, 2000));
}

public void CheckDeathSentences(TriggerArgs args){
    try{
        foreach (DeathSentence death in DeathSentences){
            if(death.HasDied()){
                death.RemoveObject();
                DeathSentences.Remove(death);
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

    public bool HasDied(){
        if(Game.TotalElapsedGameTime - this.lifetime > this.controlTime){
            return true;
        }
        else {
            return false;
        }
    }

    public void RemoveObject(){
        this.obj.Remove();
        this.obj.Destroy();
    }
}

private class Utils{
    //Sets a timer
    private static IObjectTimerTrigger tTrigger;
    public static IObjectTimerTrigger SetTimer(string met, string id, int times, int delay){
        tTrigger = (IObjectTimerTrigger)Game.CreateObject("TimerTrigger");
        tTrigger.CustomId = id;
        tTrigger.SetScriptMethod(met);
        tTrigger.SetIntervalTime(delay);
        tTrigger.SetRepeatCount(times);
        tTrigger.Trigger();
        return tTrigger;
    }
}