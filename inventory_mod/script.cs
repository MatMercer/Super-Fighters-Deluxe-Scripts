//#################################### CONFIG AREA ####################################//
//THROW_POWER is relative to the velocity of a throw object
//Too high values causes insane behavior
//------------------------------------
//THROW_POWER é relacionado com a força que um player joga o objeto
//Valores alto demais causam velocidade insanas

//VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//
private const float THROW_POWER = 5;
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

//The max objects a player can have in his inventory.
//Negative values like -1 makes it infinite.
//----------------------------------
//Quantidade maxima que um player pode ter em seu inventario.
//Valores negativos como -1 fazem com que a quantidade maxima seja infinita.
//VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV//
private const int MAX_OBJECTS_PER_INV = 5;
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

//MAX_MASS (MAX_MASS) that a player can carry.
//A safe for example, have 80 mass.
//Set it to -1 if you want to don't have a limit.
//------------------------------------
//Peso maximo que um player pode carregar
//Um confre por exemplo tem 80 de mass.
//Coloque -1 em seu valor para não ter limite de peso.
//VVVVVVVVVVVVVVVVVVVVVVVVVVVV//
private const int MAX_MASS = 60;
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

//Ignore if an object os not supposed to be picked
//Causes strange behavior
//------------------------------------
//Ignora se um objeto nao pode ser colocado no inventario
//Causa coisas estranhas
//VVVVVVVVVVVVVVVVVVVVVVVVVVVV//
private const bool IGNORE_BLACKLIST = false;
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

//Destroys the objects when a player die
//This can cause explosive behavior if the player 
//has something explosive in his inventory
//------------------------------------
//Destroi os objetos dentro do inventario quando um player morre
//Voce pode fazer homens bomba dessa forma! Carregando coisas explosivas
//VVVVVVVVVVVVVVVVVVVVVVVVVVVV//
private const bool DESTROY_OBJECTS_ON_DEATH = true;
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

//Removes the objects when a player die
//------------------------------------
//Remove os objetos dentro do inventario quando um player morre
private const bool REMOVE_OBJECTS_ON_DEATH = false;
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

//#####################################################################################//

//DECLARATIONS -------------------------- DECLARATIONS//
private static List<InvControl> InvControls = new List<InvControl>();
private static List<DeathSentence> DeathSentences = new List<DeathSentence>();
private static Vector2 textAlign = new Vector2(0f, 38f);
private static Vector2 textShadowAlign = new Vector2(0.6f, 37.6f);

private static IObject trainCar;

private static IObjectOnPlayerDeathTrigger dTrigger;

private static Random rand = new Random();

private static string ver = "1.0 BETA";
//DECLARATIONS -------------------------- DECLARATIONS//

public void OnStartup(){
    if(!(Game.GetSingleObjectByCustomID("@mranyone.MidTick") == null)){
        Game.ShowPopupMessage("Two or more instances of inventory mods running!\nAborting...\nPlease only run ONE inventory script at a time.", Color.Red);
        return;
    }

    Utils.SetTimer("MidTick", "@mranyone.MidTick", 0, 200);
    Utils.SetTimer("FastTick", "@mranyone.FastTick", 0, 100);
    foreach (IPlayer ply in Game.GetPlayers()){
        //Never and ever make dupe controllers
        if(!ply.IsBot && Utils.GetInvControlByPly(ply) == null){
            InvControls.Add(new InvControl(ply, ply.GetUser().Name + "@mranyone.InvControlObject"));
        }
    }
    trainCar = Game.CreateObject("TrainCar00A", Game.GetBorderArea().TopRight + new Vector2(0f, 120f) - new Vector2(Game.GetBorderArea().Width/2, 0f));
    trainCar.SetBodyType(BodyType.Static);
    dTrigger = (IObjectOnPlayerDeathTrigger)Game.CreateObject("OnPlayerDeathTrigger");
    dTrigger.CustomId = "@mranyone.DeathTrigger";
    dTrigger.SetScriptMethod("OnDeath");
    Utils.SendModMessage("Inventory Mod!");
    Utils.SendModMessage("Version: " + ver);
    Utils.SendModMessage("By: MrAnyone");
    Game.CreatePlayer(Game.GetSingleObjectByCustomID("spawn").GetWorldPosition());
}

private class InvControl{
    private Inventory inv;
    private IPlayer controller;
    private string id;
    private static IObjectTrigger pickupTrigger;
    private float lastUse;

    public InvControl(IPlayer controller, string id){
        this.controller = controller;
        this.id = id;
        this.inv = new Inventory(controller, MAX_OBJECTS_PER_INV);
    }

    public IPlayer GetController(){
        return this.controller;
    }

    public string GetId(){
        return this.id;
    }

    public void PickObject(IObject obj){
        if(obj == null){
            this.lastUse = Game.TotalElapsedGameTime;

            pickupTrigger = ((IObjectTrigger)Game.CreateObject(
                "AreaTrigger",
                this.controller.GetWorldPosition() + new Vector2(this.controller.FacingDirection * 15f, 2f),
                0
                ));
            pickupTrigger.CustomId = this.id; 
            pickupTrigger.SetScriptMethod("TryPickup");
                //Used to destroy a object after some milisecs
            DeathSentences.Add(new DeathSentence(pickupTrigger, 100));
        }
        else{
            this.lastUse = Game.TotalElapsedGameTime;
            if(Utils.IsObjectBlackListed(obj)){
                //Utils.PlayTextEffect("Can't Pickup That!", this.controller);
                return;
            }
            if(this.inv.IsFull()){
                Utils.PlayTextEffect("Full Inventory!", this.controller);
                return;
            }
            if(obj.GetMass() * 1000 > MAX_MASS && !(MAX_MASS < 0)){
                Utils.PlayTextEffect("Too Heavy!", this.controller);
                return;
            }
            this.inv.AddObj(obj);
        }
    }

    public void NextInventoryIndex(){
        this.lastUse = Game.TotalElapsedGameTime;
        this.inv.NextIndex();
    }

    public void UpdateInventoryTextStatus(){
        this.inv.UpdateTextStatus();
    }

    public void DropInventoryObject(){
        this.lastUse = Game.TotalElapsedGameTime;
        this.inv.DropObj(false, false);
    }

    public void ThrowInventoryObject(bool up){
        this.lastUse = Game.TotalElapsedGameTime;
        this.inv.ThrowObj(up);
    }

    public void Kill(){
        this.inv.KillIt();
    }

    //Used to delay things
    public bool HasMsPassedSinceLastUse(float time){
        if(Game.TotalElapsedGameTime - this.lastUse >= time){
            return true;
        }
        else {
            return false;
        }
    }

    private class Inventory{
        private List<IObject> iObjects = new List<IObject>();
        private IPlayer controller;
        private int size;
        private int currentIndex = 0;
        private IObjectText text = null;
        private IObjectText textShadow = null;
        private string[] controlStringArray;
        private int upOrDown;
        private float pickedObjectMass;
        private Color textColor = new Color(250, (byte)rand.Next(0, 250), 0);
        private Color textShadowColor = Color.Black;
        
        public Inventory(IPlayer controller, int size){
            this.controller = controller;
            this.size = size;
            Game.WriteToConsole("Its ready");
        }

        public void AddObj(IObject obj){
            if(obj is IPlayer)
                Game.WriteToConsole("TEEEEEEEEEEEEEST");
            Game.RemoveCameraFocus(obj);
            obj.SetLinearVelocity(Vector2.Zero);
            obj.SetBodyType(BodyType.Static);
            obj.SetWorldPosition(Game.GetBorderArea().TopRight + new Vector2(0f, 100f) - new Vector2(Game.GetBorderArea().Width/2, 0f));
            this.iObjects.Add(obj);
            currentIndex = this.iObjects.IndexOf(obj);
            Utils.PlayTextEffect("Pickup", this.controller);
        }

        public void DropObj(bool destroy, bool rmv){
            if(this.iObjects.Count < 1){
                Utils.PlayTextEffect("Empty Inventory!", this.controller);
                return;
            }
            
            this.iObjects[this.currentIndex].SetWorldPosition(this.controller.GetWorldPosition() + 
                new Vector2(this.controller.FacingDirection * 
                    this.iObjects[this.currentIndex].GetAABB().Width, this.iObjects[this.currentIndex].GetAABB().Height/2));
            this.iObjects[this.currentIndex].SetBodyType(BodyType.Dynamic);
            if(rmv){
                this.iObjects[this.currentIndex].Remove();
            }
            if(destroy){
                this.iObjects[this.currentIndex].Destroy();
                this.iObjects[this.currentIndex].SetWorldPosition(this.controller.GetWorldPosition());
            }
            this.iObjects.Remove(this.iObjects[this.currentIndex]);

            if(this.currentIndex - 1 < 0){
                this.currentIndex = 1;
            }
            this.currentIndex--;
            Utils.PlayTextEffect("Drop", this.controller);
        }

        public void ThrowObj(bool up){
            if(this.iObjects.Count < 1){
                Utils.PlayTextEffect("Empty Inventory!", this.controller);
                return;
            }
            if(up){
                this.upOrDown = 1;
                Utils.PlayTextEffect("Throw Up", this.controller);
            }
            else{
                this.upOrDown = -1;
                Utils.PlayTextEffect("Throw Down", this.controller);
            }

            this.iObjects[this.currentIndex].SetWorldPosition(this.controller.GetWorldPosition() + 
                new Vector2(this.controller.FacingDirection * 
                    this.iObjects[this.currentIndex].GetAABB().Width, this.iObjects[this.currentIndex].GetAABB().Height/2));
            this.iObjects[this.currentIndex].SetBodyType(BodyType.Dynamic);
            
            this.pickedObjectMass = this.iObjects[this.currentIndex].GetMass();

            if(this.pickedObjectMass < 0.003f){
                this.pickedObjectMass = 0.008f;
            }

            this.iObjects[this.currentIndex].SetLinearVelocity(new Vector2((THROW_POWER/100)/this.pickedObjectMass * 
            this.controller.FacingDirection, 
                (THROW_POWER/100)/this.pickedObjectMass * upOrDown));


            this.iObjects.Remove(this.iObjects[this.currentIndex]);
            if(this.currentIndex - 1 < 0){
                this.currentIndex = 1;
            }
            this.currentIndex--;
        }

        public void NextIndex(){
            if(this.iObjects.Count < 1){
                Utils.PlayTextEffect("Empty Inventory!", this.controller);
                return;
            }
            if(currentIndex + 1 == this.iObjects.Count){
                this.currentIndex = 0;
            }
            else{
                this.currentIndex++;
            }
            Utils.PlayTextEffect("Next Item", this.controller);
        }

        public void UpdateTextStatus(){
            if(this.text == null && this.textShadow == null){
                this.textShadow = (IObjectText)Game.CreateObject("Text", this.controller.GetWorldPosition() + textShadowAlign, 0f);
                this.textShadow.SetTextAlignment(TextAlignment.Middle);
                this.textShadow.SetTextScale(0.8f);
                this.textShadow.SetTextColor(this.textShadowColor);

                this.text = (IObjectText)Game.CreateObject("Text", this.controller.GetWorldPosition() + textAlign, 0f);
                this.text.SetTextAlignment(TextAlignment.Middle);
                this.text.SetTextScale(0.8f);
                this.text.SetTextColor(this.textColor);
            }
            if(!this.controller.IsDead){
                text.SetWorldPosition(this.controller.GetWorldPosition() + textAlign);
                textShadow.SetWorldPosition(this.controller.GetWorldPosition() + textShadowAlign);
            }
            else {
                text.SetWorldPosition(Game.GetBorderArea().TopRight + new Vector2(0f, 120f));
                textShadow.SetWorldPosition(Game.GetBorderArea().TopRight + new Vector2(0f, 120f));
            }

            if(this.iObjects.Count < 1){
                this.text.SetText("[EMPTY]");
                this.textShadow.SetText("[EMPTY]");
            }
            else{
                try{
                    if(this.iObjects[this.currentIndex].IsRemoved){
                        DropObj(false, false);
                    }

                    this.controlStringArray = this.iObjects[this.currentIndex].Name.Split('0');

                    this.text.SetText("[" + (this.currentIndex + 1).ToString() + "-" + this.controlStringArray[0] + "]");
                    this.textShadow.SetText("[" + (this.currentIndex + 1).ToString() + "-" + this.controlStringArray[0] + "]");

                }
                catch(Exception e){

                }
            }
        }

        public bool IsFull(){
            if(this.iObjects.Count == this.size && !(this.size < 0)){
                return true;
            }
            else{
                return false;
            }
        }

        public void KillIt(){
            while(this.iObjects.Count > 0){
                DropObj(DESTROY_OBJECTS_ON_DEATH, REMOVE_OBJECTS_ON_DEATH);
            }
        }
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

    public static InvControl GetInvControlByPly(IPlayer ply){
        foreach (InvControl invc in InvControls){
            if(invc.GetController() == ply){
                return invc;
            }
        }
        return null;
    }

    public static InvControl GetInvControlById(string id){
        foreach (InvControl invc in InvControls){
            if (invc.GetId() == id){
                return invc;
            }
        }
        return null;
    }

    public static void PlayTextEffect(string msg, IPlayer ply){
        Game.PlayEffect("CFTXT", ply.GetWorldPosition() + new Vector2(3f * ply.FacingDirection, 10f), msg);
    }

    private static string[] controlStringArray;
    public static bool IsObjectBlackListed(IObject obj){
        if(IGNORE_BLACKLIST){
            return false;
        }

        if(!(obj.Sizeable == SizeableType.None) || obj.IsBurning || obj.DestructionInitiated){
            return true;
        }
        controlStringArray = obj.Name.Split('0');

        switch (controlStringArray[0]){
            case "BambooStick":
                return true;
            case "Plank":
                return true;
            case "TruckWheel":
                return true;
            case "WpnGrenadesThrown":
                return true;
            case "WpnMineThrown":
                return true;
            case "HangingLamp":
                return true;
            case "Chandelier":
                return true;
            case "WoodDebris":
                return true;
            case "GlassShard":
                return true;
            case "Hook":
                return true;
            case "Elevator":
                return true;
            case "MetalDebris":
                return true;
            case "Pulley":
                return true;
            case "FerrisWheelCart":
                return true;
            case "CarnivalCart":
                return true;
            case "HangingCrate":
                return true;
            case "Giblet":
                return true;
            case "ConcretePipe":
                return true;
            case "Lift":
                return true;
            case "TinRoof":
                return true;
            case "Piano":
                return true;
            case "WindMillSail":
                return true;
            case "CarWheel":
                return true;
            case "Car":
                return true;
            case "Duct":
                return true;
            case "Truck":
                return true;
            case "SteamshipWheel":
                return true;
            case "Lifeboat":
                return true;
            case "SubwayTrain":
                return true;
            case "CargoContainer":
                return true;
            case "TrainCarLocomotive":
                return true;
            case "HangingCrateHolder":
                return true;
            case "Pallet":
                return true;
            case "Balloon":
                return true;
        }
        return false;
    }

    public static void SendModMessage(string msg){
        Game.RunCommand("/MSG |INV MOD| " + msg);
    }
}

public void MidTick(TriggerArgs args){
    CheckDeathSentences();
}

public void FastTick(TriggerArgs args){
    MovementTick();
    DsiplayTick();
}

//Try to pickup something
private static InvControl invc;
public void TryPickup(TriggerArgs args){
    IObject caller = (IObject)args.Caller;
    invc = Utils.GetInvControlById(caller.CustomId);

    caller.Remove();

    if(invc == null){
        return;
    }

    if (args.Sender is IPlayer){
        IPlayer plySender = (IPlayer)args.Sender;
        if(!plySender.IsDead){
            return;
        }
        else{
            invc.PickObject(plySender);
            return;
        }
    }

    IObject objSender = (IObject)args.Sender;
    invc.PickObject(objSender);
}

private void CheckDeathSentences(){
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

private void DsiplayTick(){
    foreach (InvControl invc in InvControls){
        invc.UpdateInventoryTextStatus();
    }
}

private static IPlayer cPly;
private void MovementTick(){
    foreach (InvControl invc in InvControls){
        cPly = invc.GetController();
        if(cPly.IsWalking && cPly.IsBlocking && invc.HasMsPassedSinceLastUse(300)){
            invc.PickObject(null);
        }
        else if(cPly.IsBlocking && invc.HasMsPassedSinceLastUse(250)){
            invc.NextInventoryIndex();
        }

        if(cPly.IsWalking && cPly.IsMeleeAttacking && cPly.IsCrouching && invc.HasMsPassedSinceLastUse(500)){
            invc.DropInventoryObject();
        }
        else if(cPly.IsWalking && cPly.IsMeleeAttacking && invc.HasMsPassedSinceLastUse(500)){
            invc.ThrowInventoryObject(true);
        }

        if(cPly.IsWalking && cPly.IsJumpAttacking && invc.HasMsPassedSinceLastUse(500)){
            invc.ThrowInventoryObject(false);
        }
    }
}

public void OnDeath(TriggerArgs args){
    IPlayer plySender = (IPlayer)args.Sender;
    InvControl invc = Utils.GetInvControlByPly(plySender);

    invc.Kill();
}