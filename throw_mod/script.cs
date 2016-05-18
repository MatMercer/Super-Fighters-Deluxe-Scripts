//#################################### CONFIG AREA ####################################//
//THROW_POWER is relative to velocity of a throw object
//Too high values causes insane behavior
//------------------------------------
//THROW_POWER é relacionado com a força que um player joga o objeto
//Valores alto demais causam velocidade insanas

//VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//
private const float THROW_POWER = 5;
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

//MAX_MASS (MAX_MASS) that a player can carry.
//A safe for example, have 80 mass.
//Set it to -1 if you want to don't have a limit. (dangerous, causes a massive mess)
//------------------------------------
//Peso maximo q um player pode carregar
//Um confre por exemplo tem 80 de mass.
//Coloque -1 em seu valor para não ter limite de peso. (perigoso, causa coisas absuradas)

//VVVVVVVVVVVVVVVVVVVVVVVVVVVV//
private const int MAX_MASS = 70;
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

//This makes projectiles (from guns, bullets) hit carried objects.
//Useful if you want to use one as a shield or something.
//------------------------------------
//Isso faz com que objetos que estao sendo carregados possam ser acertados por tiros.
//É util pois possibilita que alguem faça um escudo com um objeto.

//VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//
private const bool ALLOW_PROJECTILES_TO_HIT_CARRIED_OBJECTS = false;
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

//#####################################################################################//

private static List<Carry> Carries = new List<Carry>();
private static List<Projectile> Projectiles = new List<Projectile>();

//Rand
Random rand = new Random();

//Sets a timer
IObjectTimerTrigger tTrigger;
private IObjectTimerTrigger SetTimer(string met, string id, int times, int delay){
    tTrigger = (IObjectTimerTrigger)Game.CreateObject("TimerTrigger");
    tTrigger.CustomId = id;
    tTrigger.SetScriptMethod(met);
    tTrigger.SetIntervalTime(delay);
    tTrigger.SetRepeatCount(times);
    tTrigger.Trigger();
    return tTrigger;
}

private class Projectile{
    private IObject obj;
    private float lifetime;
    private float controlTime;

    public Projectile(IObject obj, float lifetime){
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

    public void DestroyObject(){
        this.obj.Destroy();
    }
}

private class Carry {
    private string id;
    private bool isCarrying = false;
    private IPlayer controller;
    private float lastUse;
    private IObjectTrigger pickupTrigger;
    private IObject pickedObject = null;
    private float pickedObjectWidth;
    private float pickedObjectHeight;
    private float pickedObjectMass;
    private IObjectAlterCollisionTile altColisonTile = null;

    public Carry(IPlayer ply, string id){
        this.controller = ply;
        this.lastUse = Game.TotalElapsedGameTime;
        this.id = id;
    }

    public void PickObject(){
        this.lastUse = Game.TotalElapsedGameTime;
        this.pickupTrigger = ((IObjectTrigger)Game.CreateObject(
            "AreaTrigger",
            this.controller.GetWorldPosition() + new Vector2(this.controller.FacingDirection * 15f, 2f),
            0
        ));
        this.pickupTrigger.CustomId = this.id;
        this.pickupTrigger.SetScriptMethod("TryPickup");
        Projectiles.Add(new Projectile(pickupTrigger, 100));
    }

    public void DropObject(){
        if(this.pickedObject == null){
            this.isCarrying = false;
            return;
        }
        this.lastUse = Game.TotalElapsedGameTime;
        pickedObject.SetBodyType(BodyType.Dynamic);
        this.altColisonTile.RemoveTargetObject(this.pickedObject);
        this.altColisonTile.Destroy();
        this.altColisonTile = null;
        this.pickedObject = null;
    }

    //-1 do throw down
    public void ThrowObject(int upOrDown){
        pickedObject.SetBodyType(BodyType.Dynamic);
        this.pickedObject.SetLinearVelocity(new Vector2((THROW_POWER/100)/pickedObjectMass * 
            this.controller.FacingDirection, 
                (THROW_POWER/100)/pickedObjectMass * 
                    upOrDown));
        DropObject();
    }

    public IPlayer GetPlayer(){
        return this.controller;
    }

    public string GetId(){
        return this.id;
    }

    public bool HasMsPassedSinceLastUse(float time){
        if(Game.TotalElapsedGameTime - this.lastUse >= time){
            return true;
        }
        else {
            return false;
        }
    }

    public bool IsCarrying(){
        return this.isCarrying;
    }

    public void SetPickupObject(IObject obj, bool isPlayer){
        if(isPlayer){
            this.pickedObject = obj;
            this.pickedObjectWidth = 6f;
            this.pickedObjectHeight = 7f;
            this.pickedObjectMass = 0.008f;
        }
        else{
            if(obj.GetMass() * 1000 > MAX_MASS || MAX_MASS < 0){
                Game.PlayEffect("CFTXT", this.controller.GetWorldPosition() + new Vector2(3f *  this.controller.FacingDirection, 10f), "Too heavy!");
                return;
            }

            this.pickedObject = obj;
            this.pickedObjectMass = pickedObject.GetMass();
            this.pickedObjectWidth = pickedObject.GetAABB().Width;
            this.pickedObjectHeight = pickedObject.GetAABB().Height;
            if (this.pickedObjectMass < 0.003f){
                this.pickedObjectMass = 0.003f;
            }
        
        if(this.altColisonTile == null){
            ConfigureAltColisonTileForNoColision((IObjectAlterCollisionTile)Game.CreateObject("AlterCollisionTile", this.controller.GetWorldPosition()));
        }
        if(this.altColisonTile.GetTargetObjects().Length == 0){
            this.altColisonTile.AddTargetObject(this.pickedObject);
            }
        }
    }

    public void UpdatePickupObjectPos(){
        if(this.pickedObject == null){
            this.isCarrying = false;
            return;
        }

        //Game.WriteToConsole(pickedObject.GetMass().ToString());

        if(this.pickedObject.IsRemoved){
            this.altColisonTile.RemoveTargetObject(this.pickedObject);
            this.altColisonTile.Destroy();
            this.altColisonTile = null;
            DropObject();
            this.isCarrying = false;
            return;
        }

        if(this.controller.IsCrouching){
            this.pickedObject.SetWorldPosition(this.controller.GetWorldPosition() + 
                new Vector2(this.controller.FacingDirection * 
                    this.pickedObjectWidth * 2.5f, this.pickedObjectHeight/2));
        }
        else{
            this.pickedObject.SetWorldPosition(this.controller.GetWorldPosition() + 
            new Vector2(this.controller.FacingDirection * 
                this.pickedObjectWidth * 2.5f, this.pickedObjectHeight * 1.3f));
        }
        this.pickedObject.SetLinearVelocity(Vector2.Zero);
        this.pickedObject.SetAngle(0f);
        this.isCarrying = true;
        this.pickedObject.SetBodyType(BodyType.Static);

        //IObject[] test = this.altColisonTile.GetTargetObjects();
        //Game.WriteToConsole(this.altColisonTile.GetDisabledMaskBits().ToString());
        //Game.WriteToConsole(test[0].Name);
    }

    private void ConfigureAltColisonTileForNoColision(IObjectAlterCollisionTile tile){
        tile.SetDisablePlayerMelee(true);
        if(!ALLOW_PROJECTILES_TO_HIT_CARRIED_OBJECTS){
            tile.SetDisableProjectileHit(true);
        }
        tile.SetDisableCollisionTargetObjects(true);
        tile.SetDisabledCategoryBits(0xFFFF);
        tile.SetDisabledMaskBits(0xFFFF);
        tile.SetDisabledAboveBits(0xFFFF);
        tile.CustomId = this.id;
        this.altColisonTile = tile;
    }
}

public void OnStartup(){
    if(Game.GetObjectsByCustomId("@mranyone.MovementTick").Length > 0){
        Game.ShowPopupMessage("2 or more instances of carry mod running! Aborting...\nPlease only run ONE carry script at a time.", Color.Red);
        return;
    }
    SetTimer("MovementTick", "@mranyone.MovementTick", 0, 100);
    SetTimer("CheckProjectiles", "@mranyone.CheckProjectiles", 0, 222);
    SetTimer("PickupTick", "@mranyone.PickupTick", 0, 100);
    //SpawnCpu();
    foreach (IPlayer ply in Game.GetPlayers()){
        if(!ply.IsBot){
            if(!Utils.IsPlayerAlreadyInCarryObject(ply)){
                Carries.Add(new Carry(ply, ply.GetUser().Name + "@mranyone.CarryObject"));
            }
        }
    }
}

public void MovementTick(TriggerArgs args){
    foreach (Carry car in Carries) {
        if(car.GetPlayer().IsBlocking && car.GetPlayer().IsWalking && car.HasMsPassedSinceLastUse(700) && !car.IsCarrying()){
            car.PickObject();
            Game.PlayEffect("CFTXT", car.GetPlayer().GetWorldPosition() + new Vector2(3f * car.GetPlayer().FacingDirection, 10f), "Pickup");
        }
        else if(car.GetPlayer().IsBlocking && car.GetPlayer().IsWalking && car.HasMsPassedSinceLastUse(300) && car.IsCarrying()){
            car.DropObject();
            Game.PlayEffect("CFTXT", car.GetPlayer().GetWorldPosition() + new Vector2(3f * car.GetPlayer().FacingDirection, 10f), "Drop");
        }

        if(car.GetPlayer().IsMeleeAttacking && car.GetPlayer().IsWalking && car.GetPlayer().IsOnGround && car.HasMsPassedSinceLastUse(400) && car.IsCarrying()){
            car.ThrowObject(1);
            Game.PlayEffect("CFTXT", car.GetPlayer().GetWorldPosition() + new Vector2(3f * car.GetPlayer().FacingDirection, 10f), "Throw up");
        }
        else if(car.GetPlayer().IsJumpAttacking && car.GetPlayer().IsWalking && !car.GetPlayer().IsOnGround && car.HasMsPassedSinceLastUse(400) && car.IsCarrying()){
            car.ThrowObject(-1);
            Game.PlayEffect("CFTXT", car.GetPlayer().GetWorldPosition() + new Vector2(3f * car.GetPlayer().FacingDirection, 10f), "Throw down");
        }

        if(car.GetPlayer().IsDiving || car.GetPlayer().IsClimbing || car.GetPlayer().IsLayingOnGround || car.GetPlayer().IsTakingCover || car.GetPlayer().IsDead && car.IsCarrying()){
            car.DropObject();
        }
    }
}

public void CheckProjectiles(TriggerArgs args){
    try{
        foreach (Projectile proj in Projectiles){
            if(proj.HasDied()){
            proj.DestroyObject();
            Projectiles.Remove(proj);
            }
        }
    }catch(Exception e){
        return;
    }
}

public void PickupTick(TriggerArgs args){
    try{
        foreach (Carry car in Carries){
            car.UpdatePickupObjectPos();
        }
    }catch(Exception e){
        return;
    }
}

public void TryPickup(TriggerArgs args){
    IObject caller = (IObject)args.Caller;
    Carry car = Utils.GetCarryById(caller.CustomId);

    if (args.Sender is IPlayer){
        IPlayer plySender = (IPlayer)args.Sender;
        if(!plySender.IsDead){
            return;
        }
        else{
            if(!car.IsCarrying()){
                car.SetPickupObject(plySender, true);
            }
            return;
        }
    }
    IObject objSender = (IObject)args.Sender;

    if(car == null){
        return;
    }
    if(!car.IsCarrying()){
        car.SetPickupObject(objSender, false);
    }
    //sender.SetLinearVelocity(new Vector2(0, 10f));
}

private class Utils{
    public static Carry GetCarryById(string id){
        foreach (Carry car in Carries){
            if(car.GetId() == id){
                return car;
            }
        }
        return null;
    }

    public static bool IsPlayerAlreadyInCarryObject(IPlayer ply){
        foreach (Carry car in Carries){
            if(car.GetPlayer() == ply){
                return true;
            }
        }
        return false;
    }
}

//IPlayer cPly;
//private void SpawnCpu(){
//    cPly = Game.CreatePlayer(Game.GetSingleObjectByCustomID("spawn").GetWorldPosition());
//    cPly.SetBotType(BotType.TutorialA);
//}