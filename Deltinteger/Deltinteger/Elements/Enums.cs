﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Deltin.Deltinteger.Elements
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumParameter : Attribute
    {
        public EnumParameter() 
        { 

        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumValue : Attribute
    {
        public EnumValue(string codeName, string workshopName)
        {
            CodeName = codeName;
            WorkshopName = workshopName;
        }
        public string CodeName { get; private set; }
        public string WorkshopName { get; private set; }

        public static string GetWorkshopName(object enumValue)
        {
            var enumType = enumValue.GetType();
            var memInfo = enumType.GetMember(enumValue.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(EnumValue), false);
            return ((EnumValue)attributes.ElementAtOrDefault(0))?.WorkshopName ?? Extras.AddSpacesToSentence(enumValue.ToString(), false);
         }

         public static string[] GetCodeValues(Type enumType)
         {
             return enumType.GetFields()
                .Select(v => v.GetCustomAttribute<EnumValue>().CodeName)
                .ToArray();
         }
    }

    public enum RuleEvent
    {
        [EnumValue(null, "Ongoing - Global")]
        Ongoing_Global,
        [EnumValue(null, "Ongoing - Each Player")]
        Ongoing_EachPlayer,

        [EnumValue(null, "Player earned elimination")]
        Player_Earned_Elimination,
        [EnumValue(null, "Player dealt final blow")]
        Player_Dealt_Final_Blow,

        [EnumValue(null, "Player dealt damage")]
        Player_Dealt_Damage,
        [EnumValue(null, "Player took damage")]
        Player_Took_Damage,

        [EnumValue(null, "Player died")]
        Player_Died
    }

    public enum PlayerSelector
    {
        All,
        Slot0,
        Slot1,
        Slot2,
        Slot3,
        Slot4,
        Slot5,
        Slot6,
        Slot7,
        Slot8,
        Slot9,
        Slot10,
        Slot11,
        // Why isn't it alphabetical? we will never know.
        Reaper,
        Tracer,
        Mercy,
        Hanzo,
        Torbjorn,
        Reinhardt,
        Pharah,
        Winston,
        Widowmaker,
        Bastion,
        Symmetra,
        Zenyatta,
        Gengi,
        Roadhog,
        Mccree,
        Junkrat,
        Zarya,
        Soldier76,
        Lucio,
        Dva,
        Mei,
        Sombra,
        Doomfist,
        Ana,
        Orisa,
        Brigitte,
        Moira,
        WreckingBall,
        Ashe,
        Baptiste
    }

    public enum Operators
    {
        [EnumValue(null, "==")]
        Equal,
        [EnumValue(null, "!=")]
        NotEqual,
        [EnumValue(null, "<")]
        LessThan,
        [EnumValue(null, "<=")]
        LessThanOrEqual,
        [EnumValue(null, ">")]
        GreaterThan,
        [EnumValue(null, ">=")]
        GreaterThanOrEqual
    }

    [EnumParameter]
    public enum Variable
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z
    }

    [EnumParameter]
    public enum Operation
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        RaiseToPower,
        Min,
        Max,
        AppendToArray,
        RemoveFromArrayByValue,
        RemoveFromArrayByIndex
    }

    [EnumParameter]
    public enum Button
    {
        PrimaryFire,
        SecondaryFire,
        Ability1,
        Ability2,
        Ultimate,
        Interact,
        Jump,
        Crouch
    }

    [EnumParameter]
    public enum Relative
    {
        ToWorld,
        ToPlayer
    }

    [EnumParameter]
    public enum ContraryMotion
    {
        Cancel,
        Incorporate
    }

    [EnumParameter]
    public enum ChaseReevaluation
    {
        DestinationAndRate,
        None
    }

    [EnumParameter]
    public enum Status
    {
        Hacked,
        Burning,
        KnockedDown,
        Asleep,
        Frozen,
        Unkillable,
        Invincible,
        PhasedOut,
        Rooted,
        Stunned
    }

    [EnumParameter]
    public enum TeamSelector
    {
        All,
        Team1,
        Team2,
    }

    [EnumParameter]
    public enum WaitBehavior
    {
        IgnoreCondition,
        AbortWhenFalse,
        RestartWhenTrue
    }

    [EnumParameter]
    public enum Effects
    {
        Sphere,
        LightShaft,
        Orb,
        Ring,
        Cloud,
        Sparkles,
        GoodAura,
        BadAura,
        EnergySound,
        PickupSound,
        GoodAuraSound,
        BadAuraSound,
        SparklesSound,
        SmokeSound,
        DecalSound,
        BeaconSound
    }

    [EnumParameter]
    public enum Color
    {
        White,
        Yellow,
        Green,
        Purple,
        Red,
        Blue,
        Team1,
        Team2
    }

    [EnumParameter]
    public enum EffectRev
    {
        VisibleToPositionAndRadius,
        PositionAndRadius,
        VisibleTo,
        None
    }

    [EnumParameter]
    public enum Rounding
    {
        Up,
        Down,
        Nearest
    }

    [EnumParameter]
    public enum Communication
    {
        VoiceLineUp,
        VoiceLineLeft,
        VoiceLineRight,
        VoiceLineDown,
        EmoteUp,
        EmoteLeft,
        EmoteRight,
        EmoteDown,
        UltimateStatus,
        Hello,
        NeedHealing,
        GroupUp,
        Thanks,
        Acknowledge
    }

    [EnumParameter]
    public enum Location
    {
        Left,
        Top,
        Right
    }

    [EnumParameter]
    public enum StringRev
    {
        VisibleToAndString,
        String
    }

    [EnumParameter]
    public enum Icon
    {
        [EnumValue(null, "Arrow: Down")]
        ArrowDown,
        [EnumValue(null, "Arrow: Left")]
        ArrowLeft,
        [EnumValue(null, "Arrow: Right")]
        ArrowRight,
        [EnumValue(null, "Arrow: Up")]
        ArrowUp,
        Aterisk,
        Bolt,
        Checkmark,
        Circle,
        Club,
        Diamond,
        Dizzy,
        ExclamationMark,
        Eye,
        Fire,
        Flag,
        Halo,
        Happy,
        Heart,
        Moon,
        No,
        Plus,
        Poison1,
        Poison2,
        QuestionMark,
        Radioactive,
        Recycle,
        RingThick,
        RingThin,
        Sad,
        Skull,
        Spade,
        Spiral,
        Stop,
        Trashcan,
        Warning,
        X
    }

    [EnumParameter]
    public enum IconRev
    {
        VisibleToAndPosition,
        Position,
        VisibleTo,
        None
    }

    [EnumParameter]
    public enum PlayEffects
    {
        GoodExplosion,
        BadExplosion,
        RingExplosion,
        GoodPickupEffect,
        BadPickupEffect,
        DebuffImpactSound,
        BuffImpactSound,
        RingExplosionSound,
        BuffExplosionSound,
        ExplosionSound
    }

    [EnumParameter]
    public enum Hero
    {
        Ana,
        Ashe,
        Baptiste,
        Bastion,
        Brigitte,
        Dva,
        Doomfist,
        Genji,
        Hanzo,
        Junkrat,
        Lucio,
        Mccree,
        Mei,
        Mercy,
        Moira,
        Orisa,
        Pharah,
        Reaper,
        Reinhardt,
        Roadhog,
        Soldier76,
        Sombra,
        Symmetra,
        Torbjorn,
        Tracer,
        Widowmaker,
        Winston,
        WreckingBall,
        Zarya,
        Zenyatta
    }

    [EnumParameter]
    public enum InvisibleTo
    {
        All,
        Enemies,
        None
    }

    [EnumParameter]
    public enum AccelerateRev
    {
        DirectionRateAndMaxSpeed,
        None
    }

    [EnumParameter]
    public enum ModRev
    {
        ReceiversDamagersAndDamagePercent,
        ReceiversAndDamagers,
        None
    }

    [EnumParameter]
    public enum FacingRev
    {
        DirectionAndTurnRate,
        None
    }

    [EnumParameter]
    public enum BarrierLOS
    {
        [EnumValue(null, "Barriers Do Not Block LOS")]
        NoBarriersBlock,
        [EnumValue(null, "Enemy Barriers Block LOS")]
        EnemyBarriersBlock,
        [EnumValue(null, "All Barriers Block LOS")]
        AllBarriersBlock
    }

    [EnumParameter]
    public enum Transformation
    {
        Rotation,
        RotationAndTranslation
    }

    [EnumParameter]
    public enum RadiusLOS
    {
        Off,
        Surfaces,
        SurfacesAndEnemyBarriers,
        SurfacesAndAllBarriers
    }

    [EnumParameter]
    public enum LocalVector
    {
        Rotation,
        RotationAndTranslation
    }

    [EnumParameter]
    public enum Clipping
    {
        ClipAgainstSurfaces,
        DoNotClip
    }

    [EnumParameter]
    public enum InworldTextRev
    {
        VisibleToPositionAndString,
        VisibleToAndString,
        String
    }
}
