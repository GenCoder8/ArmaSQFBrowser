isVehParachute =
{
 ((tolower (typeof _this)) find "parachute") >= 0
};

chutePlrs = [];

getRidOfParachute =
{
params ["_plr"];

 _plr setVariable ["disableChuteTime", time];

 chutePlrs pushback _plr;

};


addMissionEventHandler ["EachFrame",
{

hintSilent format ["<%1>", count chutePlrs];

{

_plr = _x;

_timer = _plr getVariable ["disableChuteTime",-1];

if(_timer > 0) then
{

_veh = vehicle _plr;

if(_veh call isVehParachute) then
{
 systemChat "moving out";
 _plr moveout _veh;

 deleteVehicle _veh;
};

if((time - _timer) > 5) then
{
 _plr setVariable ["disableChuteTime",-1];
 chutePlrs deleteAt _forEachIndex; 
 systemChat "Done";
};

};

} foreachReversed chutePlrs;

}];

addMissionEventHandler ["EntityRespawned",
{
params ["_plr", "_oldPlr"];

if(!isPlayer _plr) exitwith {};

_plr call getRidOfParachute;
}];
