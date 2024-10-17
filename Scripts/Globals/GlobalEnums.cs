public enum E_StationType
{
    NONE = 0,
    COMPUTER,
    FEEDING,
    CLEANING,
    SLIME_COLLECTION,
    CLOCKOUT,
    CANISTER_STORAGE,
    HAPPINESS,
    ELEVATOR_PANEL,
    ABANDONED_ROOM_CODE,
    SUPERVISOR_PICKUP,
    SUPERVISOR_ROOM_SWIPE,
    POISON_CREATURE,
    POISON_INJECTOR_PICKUP
}

public enum E_IngredientList
{
    BRAIN = 0,
    EYEBALLS,
    LUNGS,
    HEART,
    LIVER,
    STOMACH,
    KIDNEYS,
    SKIN,
    NAILS
}

public enum E_AreasToClean
{
    C1 = 0,
    C2,
    C3,
    B1,
    B2,
    B3,
    A1,
    A2,
    A3
}

public enum E_NeedMetAmount
{
    NONE = 0,
    HALF,
    MOST,
    ALL
}

public enum E_EndState
{
    NONE = 0,
    FIRED,
    PLAYER_WIN,
    CREATURE_WIN
}