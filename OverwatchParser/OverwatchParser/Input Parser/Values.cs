﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Deltin.OverwatchParser.Elements
{
    [ElementData("Absolute Value", ValueType.Number)]
    [Parameter("Value", ValueType.Number, typeof(V_Number))]
    public class V_AbsoluteValue : Element {}

    [ElementData("Add", ValueType.Any)]
    [Parameter("Value", ValueType.Any, typeof(V_Number))]
    [Parameter("Value", ValueType.Any, typeof(V_Number))]
    public class V_Add : Element {}

    [ElementData("All Dead Players", ValueType.Player)]
    [Parameter("Team", ValueType.Team, typeof(V_Team))]
    public class V_AllDeadPlayers : Element {}

    [ElementData("All Heroes", ValueType.Hero)]
    public class V_AllHeroes : Element {}

    [ElementData("All Living Players", ValueType.Player)]
    [Parameter("Team", ValueType.Team, typeof(V_Team))]
    public class V_AllLivingPlayers : Element {}

    [ElementData("All Players", ValueType.Player)]
    [Parameter("Team", ValueType.Team, typeof(V_Team))]
    public class V_AllPlayers : Element {}

    [ElementData("All Players Not On Objective", ValueType.Player)]
    [Parameter("Team", ValueType.Team, typeof(V_Team))]
    public class V_AllPlayersNotOnObjective : Element {}

    [ElementData("All Players On Objective", ValueType.Player)]
    [Parameter("Team", ValueType.Team, typeof(V_Team))]
    public class V_AllPlayersOnObjective : Element {}

    [ElementData("Allowed Heroes", ValueType.Hero)]
    [Parameter("Player", ValueType.Player, typeof(V_EventPlayer))]
    public class V_AllowedHeroes : Element {}

    [ElementData("Altitude Of", ValueType.Number)]
    [Parameter("Player", ValueType.Player, typeof(V_EventPlayer))]
    public class V_AltitudeOf : Element {}

    [ElementData("And", ValueType.Boolean)]
    [Parameter("Value", ValueType.Boolean, typeof(V_True))]
    [Parameter("Value", ValueType.Boolean, typeof(V_True))]
    public class V_And : Element {}

    [ElementData("Angle Difference", ValueType.Number)]
    [Parameter("Angle", ValueType.Number, typeof(V_Number))]
    [Parameter("Angle", ValueType.Number, typeof(V_Number))]
    public class V_AngleDifference : Element {}

    [ElementData("Append To Array", ValueType.Any)]
    [Parameter("Array", ValueType.Any, typeof(V_AllPlayers))]
    [Parameter("Value", ValueType.Any, typeof(V_Number))]
    public class V_AppendToArray : Element {}

    [ElementData("Array Contains", ValueType.Boolean)]
    [Parameter("Array", ValueType.Any, typeof(V_AllPlayers))]
    [Parameter("Value", ValueType.Any, typeof(V_Number))]
    public class V_ArrayContains : Element { }

    [ElementData("Array Slice", ValueType.Any)]
    [Parameter("Array", ValueType.Any, typeof(V_GlobalVariable))]
    [Parameter("Start Index", ValueType.Number, typeof(V_Number))]
    [Parameter("Count", ValueType.Number, typeof(V_Number))]
    public class V_ArraySlice : Element {}

    [ElementData("Attacker", ValueType.Player)]
    public class V_Attacker : Element {}

    [ElementData("Backward", ValueType.Vector)]
    public class V_Backward : Element {}

    [ElementData("Closest Player To", ValueType.Player)]
    [Parameter("Center", ValueType.VectorAndPlayer, typeof(V_Vector))]
    [Parameter("Team", ValueType.Team, typeof(V_Team))]
    public class V_ClosestPlayerTo : Element {}

    [ElementData("Compare", ValueType.Boolean)]
    [Parameter("Value", ValueType.Any, typeof(V_Number))]
    [Parameter("", typeof(Operators))]
    [Parameter("Value", ValueType.Any, typeof(V_Number))]
    public class V_Compare : Element {}

    [ElementData("Control Point Scoring Percentage", ValueType.Number)]
    [Parameter("Team", ValueType.Team, typeof(V_Team))]
    public class V_ControlPointScoringPercentage : Element {}

    [ElementData("Control Point Scoring Team", ValueType.Team)]
    public class V_ControlPointScoringTeam : Element {}

    [ElementData("Event Player", ValueType.Player, 0)]
    public class V_EventPlayer : Element {}

    [ElementData("Global Variable", ValueType.Any, 0)]
    [Parameter("Variable", typeof(Variable))]
    public class V_GlobalVariable : Element {}

    [ElementData("Null", ValueType.Player, 0)]
    public class V_Null : Element {}

    [ElementData("Number", ValueType.Number, 0)]
    public class V_Number : Element
    {
        public V_Number(int value)
        {
            this.value = value;
        }
        public V_Number() : this(0) { }

        int value;

        protected override void AfterParameters()
        {
            InputHandler.Input.KeyPress(Keys.Down);
            Thread.Sleep(InputHandler.SmallStep);

            var keys = InputHandler.GetNumberKeys(value);
            for (int i = 0; i < keys.Length; i++)
            {
                InputHandler.Input.KeyDown(keys[i]);
                Thread.Sleep(InputHandler.SmallStep);
            }

            InputHandler.Input.KeyPress(Keys.Enter);
            Thread.Sleep(InputHandler.SmallStep);
        }
    }

    [ElementData("Player Variable", ValueType.Any, 0)]
    [Parameter("Player", ValueType.Player, typeof(V_EventPlayer))]
    [Parameter("Variable", typeof(Variable))]
    public class V_PlayerVariable : Element {}

    [ElementData("String", ValueType.String, 1)]
    [Parameter("{0}", ValueType.Any, typeof(V_Number))]
    [Parameter("{1}", ValueType.Any, typeof(V_Number))]
    [Parameter("{2}", ValueType.Any, typeof(V_Number))]
    public class V_String : Element
    {
        public V_String(string text, params Element[] stringValues) : base(NullifyEmptyValues(stringValues))
        {
            textID = Array.IndexOf(Constants.Strings, text);
            if (textID == -1)
                throw new Exception();
        }
        public V_String() : this(Constants.DEFAULT_STRING) { }

        int textID;

        protected override void BeforeParameters()
        {
            Thread.Sleep(InputHandler.BigStep);

            // Select "string" option
            InputHandler.Input.KeyPress(Keys.Down);
            Thread.Sleep(InputHandler.SmallStep);

            // Open the string list
            InputHandler.Input.KeyPress(Keys.Space);
            Thread.Sleep(InputHandler.BigStep);

            // Leave the search field input
            InputHandler.Input.KeyPress(Keys.Enter);
            Thread.Sleep(InputHandler.SmallStep);

            // Select the selected string by textID.
            for (int i = 0; i < textID; i++)
            {
                InputHandler.Input.KeyPress(Keys.Down);
                Thread.Sleep(InputHandler.SmallStep);
            }

            // Select the string
            InputHandler.Input.KeyPress(Keys.Space);
            Thread.Sleep(InputHandler.BigStep);
        }

        public static Element BuildString(params Element[] strings)
        {
            if (strings.Length == 0)
                throw new ArgumentException($"There needs to be at least 1 string in the {nameof(strings)} array.");

            if (strings.Length == 1)
                return strings[0];

            if (strings.Length == 2)
                return new V_String("{0} {1}", strings);

            if (strings.Length == 3)
                return new V_String("{0} {1} {2}", strings);

            if (strings.Length > 3)
                return new V_String("{0} {1} {2}", strings[0], strings[1], BuildString(strings.Skip(2).ToArray()));

            throw new Exception();
        }
        private static Element[] NullifyEmptyValues(Element[] stringValues)
        {
            var stringList = stringValues.ToList();
            while (stringList.Count < 3)
                stringList.Add(new V_Null());

            return stringList.ToArray();
        }
    }

    [ElementData("Team", ValueType.Team, 4)]
    [Parameter("Team", typeof(TeamSelector))]
    public class V_Team : Element {}

    [ElementData("True", ValueType.Boolean, 2)]
    public class V_True : Element {}

    [ElementData("Vector", ValueType.Vector, 1)]
    [Parameter("X", ValueType.Number, typeof(V_Number))]
    [Parameter("Y", ValueType.Number, typeof(V_Number))]
    [Parameter("Z", ValueType.Number, typeof(V_Number))]
    public class V_Vector : Element {}
}