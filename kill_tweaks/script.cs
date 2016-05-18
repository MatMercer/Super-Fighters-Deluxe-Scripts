//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
//CONFIG AREA

const float MAX_GENERAL_DAMAGE = -1;
const float MAX_FIRE_DAMAGE = -1;
const float MAX_PROJECTILE_DAMAGE = -1;
const float MAX_MELEE_DAMAGE = -1;
const float MAX_FALL_DAMAGE = -1;
const float MAX_EXPLOSION_DAMAGE = -1;
const int 	MAX_JUMPS = -1;
const int 	MAX_ROLLS = -1;
const int 	MAX_DIVES = -1;	
const int 	MAX_PROJECTILES_HITBY = -1;	
const int   MAX_BLOCKED_ATTACKS = -1;

const bool GIB_PLAYERS = false;
const bool DO_EXPLOSION_ON_DEATH = true;
const bool DO_FIRE_ON_DEATH = true;

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
//DEFAULT CONFIG
/*
const float MAX_GENERAL_DAMAGE = 0;
const float MAX_FIRE_DAMAGE = -1;
const float MAX_PROJECTILE_DAMAGE = -1;
const float MAX_MELEE_DAMAGE = -1;
const float MAX_FALL_DAMAGE = -1;
const float MAX_EXPLOSION_DAMAGE = -1;
const int 	MAX_JUMPS = -1;
const int 	MAX_ROLLS = -1;
const int 	MAX_DIVES = -1;	
const int 	MAX_PROJECTILES_HITBY = -1;	
const int   MAX_BLOCKED_ATTACKS = -1;

const bool GIB_PLAYERS = false;
const bool DO_EXPLOSION_ON_DEATH = true;
const bool DO_FIRE_ON_DEATH = true;
*/
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//

//Rand
Random rand = new Random();

//Version
const string version = "1.5 BETA";

//Called on death
IObjectOnPlayerDeathTrigger dTrigger;

//Don't generate dupe objects
IPlayer cPly;

//Sets a timer
IObjectTimerTrigger tTrigger;
public IObjectTimerTrigger SetTimer(string met, string id, int times, int delay){
	tTrigger = (IObjectTimerTrigger)Game.CreateObject("TimerTrigger");
	tTrigger.CustomId = id;
	tTrigger.SetScriptMethod(met);
	tTrigger.SetIntervalTime(delay);
	tTrigger.SetRepeatCount(times);
	tTrigger.Trigger();
	return tTrigger;
}

public void OnStartup(){
	UpdateSettingsBools();
	SetTimer("FastTick", "@mranyone.FastTick", 0, 100);
	SetTimer("SlowTick", "@mranyone.SlowTick", 0, 1000);
	SetTimer("StartupMessageTick", "@mranyone.StartupMessageTick", 15, 1000);
	SetTimer("ClearStartupMessage", "@mranyone.ClearStartupMessage", 1, 16000);
	dTrigger = (IObjectOnPlayerDeathTrigger)Game.CreateObject("OnPlayerDeathTrigger");
	dTrigger.SetScriptMethod("OnDeath");
	SendMessage("Version " + version);
	SendMessage("By MrAnyone");
	SendMessage("A lot of things is customizable! Setup it in the config area of the script!");
}

public void FastTick(TriggerArgs args){
	DamageTick();
}

public void SlowTick(TriggerArgs args){
}

//Explodes or do something else when a player is damaged
bool kill = false; //Used to control if the player should be killed
bool careToGeneralDamage = true;
bool careToFireDamage = true;
bool careToProjectileDamage = true;
bool careToMeleeDamage = true;
bool careToFallDamage = true;
bool careToExplosionDamage = true;
bool careToJumps = true;
bool careToRolls = true;
bool careToDives = true;
bool careToProjectilesHitBy = true;
bool careToBlockedAttacks = true;
public void DamageTick(){
	foreach(IPlayer player in Game.GetPlayers()){
		kill = false;
		//invert this, makes things faster... because access player.Statistics all the time is cpu consuming
		//careToGeneralDamage player.Statistics.TotalDamageTaken > MAX_GENERAL_DAMAGE

		if(player.Statistics.TotalDamageTaken > MAX_GENERAL_DAMAGE && careToGeneralDamage ||
			player.Statistics.TotalFireDamageTaken > MAX_FIRE_DAMAGE && careToFireDamage ||
			player.Statistics.TotalProjectileDamageTaken > MAX_PROJECTILE_DAMAGE && careToProjectileDamage ||
			player.Statistics.TotalMeleeDamageTaken > MAX_MELEE_DAMAGE && careToMeleeDamage ||
			player.Statistics.TotalFallDamageTaken > MAX_FALL_DAMAGE && careToFallDamage ||
			player.Statistics.TotalExplosionDamageTaken > MAX_EXPLOSION_DAMAGE && careToExplosionDamage ||
			player.Statistics.TotalJumps > MAX_JUMPS && careToJumps ||
			player.Statistics.TotalRolls > MAX_ROLLS && careToRolls ||
			player.Statistics.TotalDives > MAX_DIVES && careToDives ||
			player.Statistics.TotalBlockedAttacks > MAX_BLOCKED_ATTACKS && careToBlockedAttacks ||
			player.Statistics.TotalProjectilesHitBy > MAX_PROJECTILES_HITBY && careToProjectilesHitBy){
			kill = true;
		}
		if(kill)
			KillMethod(player);
	}
}

bool isItEnabled = false;
public void UpdateSettingsBools(){
	if(MAX_GENERAL_DAMAGE < 0){
		careToGeneralDamage = false;
	}

	if(MAX_FIRE_DAMAGE < 0){
		careToFireDamage = false;
	}

	if(MAX_PROJECTILE_DAMAGE < 0){
		careToProjectileDamage = false;
	}

	if(MAX_MELEE_DAMAGE < 0){
		careToMeleeDamage = false;
	}

	if(MAX_FALL_DAMAGE < 0){
		careToFallDamage = false;
	}

	if(MAX_EXPLOSION_DAMAGE < 0){
		careToExplosionDamage = false;
	}

	if(MAX_JUMPS < 0){
		careToJumps = false;
	}

	if(MAX_ROLLS < 0){
		careToRolls = false;
	}

	if(MAX_DIVES < 0){
		careToDives = false;
	}

	if(MAX_PROJECTILES_HITBY < 0){
		careToProjectilesHitBy = false;
	}

	if(MAX_BLOCKED_ATTACKS < 0){
		careToBlockedAttacks = false;
	}

	if(careToGeneralDamage ||
		careToFireDamage ||
		careToProjectileDamage ||
		careToExplosionDamage ||
		careToMeleeDamage ||
		careToFallDamage ||
		careToBlockedAttacks ||
		careToProjectilesHitBy ||
		careToJumps ||
		careToRolls ||
		careToDives){
		isItEnabled = true;
	}
}

public string GenerateStartupMessage(){
	string msg = "INSTANT DEATH SETTINGS:\nTIP: in the config area, set 1 value to -1 to disable it!\n";
	
	if(!isItEnabled){
		msg = "The current settings doesn't change nothing!\nPlease change the values in the config area.\n\nLike, to insta kill when someone jumps\nset the variable \"MAX_JUMPS\" to 0\n\nEverything that is less than 0 will\ntell the script to ignore it";
		return msg;
	}

	if(careToGeneralDamage){
		msg += (MAX_GENERAL_DAMAGE > 0) ? "\nIf you have more than " + MAX_GENERAL_DAMAGE.ToString() + " of any damage you die!" : "\nYou insta die to any damage!";
	}

	if(MAX_GENERAL_DAMAGE > 0 || !careToGeneralDamage){

		if(careToFireDamage){
			msg += (MAX_FIRE_DAMAGE > 0 ) ? "\nIf you have more than " + MAX_FIRE_DAMAGE.ToString() + " of fire damage you die!" : "\nYou insta die to fire!";
		}
	
		if(careToProjectileDamage){
			msg += (MAX_PROJECTILE_DAMAGE > 0 ) ? "\nIf you have more than " + MAX_PROJECTILE_DAMAGE.ToString() + " of projectile damage you die!" : "\nYou insta die to guns!";
		}
	
		if(careToMeleeDamage){
			msg += (MAX_MELEE_DAMAGE > 0 ) ? "\nIf you have more than " + MAX_MELEE_DAMAGE.ToString() + " of melee damage you die!" : "\nYou insta die to melee attacks!";
		}
	
		if(careToFallDamage){
			msg += (MAX_FALL_DAMAGE > 0 ) ? "\nIf you have more than " + MAX_FALL_DAMAGE.ToString() + " of fall damage you die!" : "\nYou insta die if you fall!";
		}
	
		if(careToExplosionDamage){
			msg += (MAX_EXPLOSION_DAMAGE > 0 ) ? "\nIf you have more than " + MAX_EXPLOSION_DAMAGE.ToString() + " of explosion damage you die!" : "\nYou insta die to explosions!";
		}

	}

	if(careToJumps){
		msg += (MAX_JUMPS > 0 ) ? "\nIf you jump more than " + MAX_JUMPS.ToString() + " times you die!" : "\nIf you jump you die!";
	}

	if(careToRolls){
		msg += (MAX_ROLLS > 0 ) ? "\nIf you roll more than " + MAX_ROLLS.ToString() + " times you die!" : "\nIf you roll you die!";
	}

	if(careToDives){
		msg += (MAX_DIVES > 0 ) ? "\nIf you dive more than " + MAX_DIVES.ToString() + " times you die!" : "\nIf you dive you die!";
	}

	if(careToProjectilesHitBy){
		msg += (MAX_PROJECTILES_HITBY > 0 ) ? "\nIf you be shoot more than " + MAX_PROJECTILES_HITBY.ToString() + " times you die!" : "\nYou die to any projectile!";
	}

	if(careToBlockedAttacks){
		msg += (MAX_BLOCKED_ATTACKS > 0 ) ? "\nIf you block in melee more than " + MAX_BLOCKED_ATTACKS.ToString() + " times you die!" : "\nYou die if you block in melee!";
	}

	if(DO_EXPLOSION_ON_DEATH){
		msg += "\nYou explode when you die!";
	}

	if(DO_FIRE_ON_DEATH){
		msg += "\nYou make fire when you die!";
	}
	return msg;
}

public void StartupMessageTick(TriggerArgs args){
	Game.ShowPopupMessage(GenerateStartupMessage(), new Color((byte)rand.Next(50, 256), (byte)rand.Next(50, 256), (byte)rand.Next(50, 256)));
}

public void ClearStartupMessage(TriggerArgs args){
	Game.HidePopupMessage();
}

public void KillMethod(IPlayer player){
	if(GIB_PLAYERS){
		player.Gib();
	}
	else{
		player.Kill();
	}
}

public void SendMessage(string msg){
	Game.RunCommand("/MSG |INSTA KILL| :" + msg);
}

Vector2 lastPos;
public void OnDeath(TriggerArgs args){
	cPly = (IPlayer)args.Sender;
	lastPos = cPly.GetWorldPosition();
	if(DO_EXPLOSION_ON_DEATH){
		Game.TriggerExplosion(lastPos);
	}

	if(DO_FIRE_ON_DEATH){
		Game.SpawnFireNodes(
		lastPos,
		rand.Next(5, 30),
		new Vector2(rand.Next(-10, 10), rand.Next(0, 20)),
		1,
		5,
		FireNodeType.Flamethrower);
	}
}