//00010110101001010101010101011010101010100010100101010110011010011010010101010101010011010101001101010101010//
//10100                                                                                                 10101//
//10100 A small script that allows you to eat your opponent's dead body!                                10101//
//10100 What could be better than eating your enemy after a hard fight? It also heals you!              10101//
//10100 Made by Motto73, but feel free to use anything and edit and no need to give credit              10101//
//10100 If you want to remove the credits just delete the text in the OnStartup method.                 10101//
//10100                                                                                                 10101//
//10010100011010011010101010101010101010101010101010110101010110101010011010101010101010101010101010101010101//

//   __  __            _             __  __                                          
//  |  \/  |Made by_ _| |_          |  \/  |      /\                                 
//  | \  / | ___  | |_   _|#73      | \  / |_ __ /  \   _ __  _   _  ___  _ __   ___ 
//  | |\/| |/ _ \|   || |/ _ \  And | |\/| | '__/ /\ \ | '_ \| | | |/ _ \| '_ \ / _ \
//  | |  | | (_)  | | | | (_) |     | |  | | | / ____ \| | | | |_| | (_) | | | |  __/
//  |_|  |_|\___/ |_| |_|\___/      |_|  |_|_|/_/    \_\_| |_|\__, |\___/|_| |_|\___|
//                                                             __/ |                 
//                                                            |___/                  
         

//=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====//
//=====<>=====<>=====||             Variable fields                ||=====<>=====<>=====//
//=====<>=====<>=====|| Please set these variables to the values that best fits you.       ||=====<>=====<>=====//
//=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====//

float Nutrition_Info = 10;                  //The value that each giblet heals.

Color Text_Color = Color.Yellow;                //The color to display instructions.

float Max_Hp_Limit = 101;                   //The max HP until eating gets limited.100 is normal, over 100 is unlimited.

float Hp_sound = 30f;                       //The HP sound effect sound

string Full_Text = "I'm full.";             //The text to show if full.

bool Use_Random_Comments = true;                //To show random comments or not.

int Comment_Max_Factor = 80;                    //How often show the comment.0=always 101=never.

bool Show_Vars = false;

bool Show_Instructions = true;

//=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====//
//=====<>=====<>=====||                 Comments                   ||=====<>=====<>=====//
//=====<>=====<>=====||         You can delete or add your own comments.           ||=====<>=====<>=====//
//=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====//
public string[] comments ={
   "Om nom nom",
   "Om nom",
   "Om nom nom nom",
   "So tasty!",
   "Better than nothing",
   "Why am I doing this?"
   };

//=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====//
//=====<>=====<>=====||                 Code                   ||=====<>=====<>=====//
//=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====<>=====//

/**
 *
 * A list of things that must be clickable
 *
 */
public string[] deadStuff ={
    "Giblet00",
    "Giblet01",
    "Giblet02",
    "Giblet03",
    "Giblet04"
   };

static Random rnd = new Random();

/**
 *
 * OnStartup event
 *
 */
public void OnStartup() {
    IObjectOnPlayerDeathTrigger ded = (IObjectOnPlayerDeathTrigger)Game.CreateObject("OnPlayerDeathTrigger");
    ded.SetScriptMethod("Death");
    ded.SetActivateOnStartup(false);
    Game.RunCommand("/MSG Cannibals mod by Motto73 & MrAnyone");    //Delete this line if you dont want the credits.
    string Variables = string.Format("Current set variables: \nUse comments: {0}\nNutrition informaton: {1}\nComment factor: {2}", Use_Random_Comments, Nutrition_Info.ToString(), Comment_Max_Factor.ToString());
    if (!Show_Vars) Variables = "";
    string Instructions = ("Eat the meat!\nPlayers will always gib!");
    if (!Show_Instructions) Instructions = "";
    Game.ShowPopupMessage(Instructions + "\n" + Variables, Text_Color);
    //CreateTimer(200,0,"checkPositions","Timer0");
    Utils.SetTimer("SetDeadStuffClickabe", "SetDeadStuffClickabeTick", -1, 1500);
}

/**
 *
 * Death listener, used to gib the players
 * all the time.
 *
 */
public void Death(TriggerArgs args) {
    if (args.Sender != null) {
        IPlayer ply = args.Sender as IPlayer;

        ply.Gib();
    }
}

/**
 *
 * Sets the players and glibets clickable in the map
 * too many calls may be slow and causes lag.
 *
 */
public void SetDeadStuffClickabe(TriggerArgs args) {
    foreach (IObject obj in Game.GetObjectsByName(deadStuff).Take(20)) {
        Utils.MakeObjectClickable(obj, "Eat", true);
    }
}

/**
 *
 * Called by an clickable glibet
 * adds life to the player and shows effects
 *
 */
public void Eat(TriggerArgs args) {
    IObjectActivateTrigger obj = args.Caller as IObjectActivateTrigger;
    IPlayer ply = args.Sender as IPlayer;
    float hp = ply.GetHealth();
    if (hp < Max_Hp_Limit) {
        ply.SetHealth(hp + Nutrition_Info);
        obj.Destroy();
        Utils.ShowMeatEffect(ply);
        Game.PlaySound("GetHealthSmall", ply.GetWorldPosition(), Hp_sound);
        int i = rnd.Next(0, 100);
        if (i >= Comment_Max_Factor) {
            string str = comments[rnd.Next(0, comments.Count())];
            Game.PlayEffect("CFTXT", ply.GetWorldPosition() + new Vector2(0, 16), str);
        }
    }
}

/**
 * Utils class
 * The functions are used
 * to automate some
 * actions in the script.
 */
public static class Utils {
    static List<int> clickableObjects = new List<int>();

    /**
     * Used to set a timer trigger
     */
    public static IObjectTimerTrigger SetTimer(string method, string customId, int repeatCount, int interval) {
        IObjectTimerTrigger trigger = (IObjectTimerTrigger)Game.CreateObject("TimerTrigger");
        trigger.CustomId = customId;
        trigger.SetScriptMethod(method);
        trigger.SetIntervalTime(interval);
        trigger.SetRepeatCount(repeatCount);
        trigger.Trigger();
        return trigger;
    }

    /**
     * Sends a custom message to the chat
     */
    public static void SendMessage(string msg) {
        Game.RunCommand("/MSG " + msg);
    }

    /**
     * Sends a exception message to the console
     */
    public static void SendExceptionMessage(System.Exception e, string at) {
        Game.WriteToConsole("EXCEPTION: " + e + " AT: " + at);
    }

    /**
     * Sends a work in progress
     * message for something that isn't
     * ready in the map
     */
    public static void SendWorkInProgressMessage(Vector2 whereToSend) {
        Game.PlayEffect("CFTXT", whereToSend, "Beta version!\nMrAnyone is working...");
    }

    /**
     * Generates a clicable object
     * runs the specific method when triggered
     * destroys or not the triggers
     */
    public static IObject CreateClickableObject(string objName, Vector2 location, string method, bool destroyTriggers) {
        // Creates the object
        IObject obj = (IObject)Game.CreateObject(objName, location);

        // Creates a weldjoint
        IObjectWeldJoint welder = (IObjectWeldJoint)Game.CreateObject("WeldJoint", location);
        welder.SetBodyType(BodyType.Dynamic);

        // The button that triggers the method
        IObjectActivateTrigger invButton = (IObjectActivateTrigger)Game.CreateObject("ActivateTrigger", location);
        invButton.SetBodyType(BodyType.Dynamic);
        // What method will it run when trigered?
        invButton.SetScriptMethod(method);

        // Joins everyone
        welder.AddTargetObject(obj);
        welder.AddTargetObject(invButton);

        // Used to higlight the obj
        invButton.SetHighlightObject(obj);

        // Used to destroy the "clickable family"
        if (destroyTriggers) {
            // Destroy the other objects, like the trigger
            IObjectDestroyTargets destroyer = (IObjectDestroyTargets)Game.CreateObject("DestroyTargets", location);
            destroyer.SetBodyType(BodyType.Dynamic);
            welder.AddTargetObject(destroyer);
            destroyer.AddTriggerDestroyObject(obj);
            destroyer.AddObjectToDestroy(welder);
            destroyer.AddObjectToDestroy(invButton);
            destroyer.AddObjectToDestroy(destroyer);
        }

        return obj;
    }

    /**
     * Makes an object clickable
     * runs the specific method when triggered
     * destroys or not the triggers
     */
    public static void MakeObjectClickable(IObject obj, string method, bool destroyTriggers) {
        if (!clickableObjects.Contains(obj.UniqueID)) {
            Utils.CleanClickableObjects();
            clickableObjects.Add(obj.UniqueID);
            // Creates a weldjoint
            IObjectWeldJoint welder = (IObjectWeldJoint)Game.CreateObject("WeldJoint", obj.GetWorldPosition());
            welder.SetBodyType(BodyType.Dynamic);

            // The button that triggers the method
            IObjectActivateTrigger invButton = (IObjectActivateTrigger)Game.CreateObject("ActivateTrigger", obj.GetWorldPosition());
            invButton.SetBodyType(BodyType.Dynamic);
            // What method will it run when trigered?
            invButton.SetScriptMethod(method);

            // Joins everyone
            welder.AddTargetObject(obj);
            welder.AddTargetObject(invButton);

            // Used to higlight the obj
            invButton.SetHighlightObject(obj);

            // Used to destroy the "clickable family"
            if (destroyTriggers) {
                // Destroy the other objects, like the trigger
                IObjectDestroyTargets destroyer = (IObjectDestroyTargets)Game.CreateObject("DestroyTargets", obj.GetWorldPosition());
                destroyer.SetBodyType(BodyType.Dynamic);
                welder.AddTargetObject(destroyer);
                destroyer.AddTriggerDestroyObject(invButton);
                destroyer.AddTriggerDestroyObject(obj);
                destroyer.AddObjectToDestroy(obj);
                destroyer.AddObjectToDestroy(welder);
                destroyer.AddObjectToDestroy(invButton);
                destroyer.AddObjectToDestroy(destroyer);
            }
        }
    }


    /**
    * Sends a message based in
    * a triggers args
    */
    public static void SendMessageFromTriggerArgs(TriggerArgs args, string message) {
        IObject caller = args.Caller as IObject;
        Game.PlayEffect("CFTXT", caller.GetWorldPosition(), message);
    }

    /**
     *
     * Cleans the clickableObject, preventing too many memory usage
     *
     */
    public static void CleanClickableObjects() {
        try {
            foreach (int str in clickableObjects) {
                if (Game.GetObject(str) == null) {
                    clickableObjects.Remove(str);
                }
            }
        } catch (System.Exception e) {
            Utils.SendExceptionMessage(e, "Utils.CleanClickableObjects");
        }
    }

    /**
     *
     * Shows a meat exploding effect, to looks like
     * the player is eating it
     *
     */
    public static void ShowMeatEffect(IObject obj) {
        Vector2 pos = obj.GetWorldPosition();
        Game.PlayEffect("BLD", pos + new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10)));
        Game.PlayEffect("TR_B", pos + new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10)));
        Game.PlayEffect("TR_D", pos + new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10)));
        Game.PlayEffect("TR_B", pos + new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10)));
        Game.PlayEffect("TR_D", pos + new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10)));
        Game.PlayEffect("TR_B", pos + new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10)));
        Game.PlayEffect("TR_B", pos + new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10)));
    }

}