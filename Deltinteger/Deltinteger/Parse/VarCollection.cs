using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deltin.Deltinteger.Elements;
using Deltin.Deltinteger.LanguageServer;

namespace Deltin.Deltinteger.Parse
{
    public class VarCollection
    {
        public WorkshopArrayBuilder ArrayBuilder { get; private set; }

        // Indicates the workshop variables to store the extended collections at.
        private WorkshopVariable global;
        private WorkshopVariable player;

        // Reserved IDs and names.
        private readonly List<int> reservedGlobalIDs = new List<int>();
        private readonly List<int> reservedPlayerIDs = new List<int>();
        private readonly List<string> reservedGlobalNames = new List<string>();
        private readonly List<string> reservedPlayerNames = new List<string>();

        // Variables
        private readonly List<WorkshopVariable> globalVariables = new List<WorkshopVariable>();
        private readonly List<WorkshopVariable> playerVariables = new List<WorkshopVariable>();
        // Variables in the extended collections
        private readonly IndexReference[] extendedGlobalVariables = new IndexReference[Constants.MAX_ARRAY_LENGTH];
        private readonly IndexReference[] extendedPlayerVariables = new IndexReference[Constants.MAX_ARRAY_LENGTH];

        public VarCollection() {}

        public void Setup()
        {
            global      = AssignWorkshopVariable("_extendedGlobalCollection", true);
            player      = AssignWorkshopVariable("_extendedPlayerCollection", false);
            var builder = AssignWorkshopVariable("_arrayBuilder", true);

            IndexReference store = Assign("_arrayBuilderStore", true, true);
            ArrayBuilder = new WorkshopArrayBuilder(builder, store);
            // The store shouldn't require an instance of the WorkshopArrayBuilder, but if for some reason it does uncomment the line below.
            // store.ArrayBuilder = arrayBuilder;
        }

        public void Reserve(int id, bool isGlobal, FileDiagnostics diagnostics, DocRange range)
        {
            // Throw a syntax error if the ID was already reserved.
            if (reserveList(isGlobal).Contains(id))
            {
                string msg = string.Format("The id {0} is already reserved in the {1} collection.", id, isGlobal ? "global" : "player");

                if (range != null)
                    diagnostics.Error(msg, range);
                else
                    throw new Exception(msg);
            }
        
            // Add the ID to the reserved list.
            reserveList(isGlobal).Add(id);
        }

        List<int> reserveList(bool isGlobal) => isGlobal ? reservedGlobalIDs : reservedPlayerIDs;
        List<WorkshopVariable> variableList(bool isGlobal) => isGlobal ? globalVariables : playerVariables;
        IndexReference[] extendedVariableList(bool isGlobal) => isGlobal ? extendedGlobalVariables : extendedPlayerVariables;

        public string WorkshopNameFromCodeName(bool isGlobal, string name)
        {
            StringBuilder valid = new StringBuilder();

            // Remove invalid characters and replace ' ' with '_'.
            for (int i = 0; i < name.Length; i++)
                if (name[i] == ' ')
                    valid.Append('_');
                else if (WorkshopVariable.ValidVariableCharacters.Contains(name[i]))
                    valid.Append(name[i]);
                
            string newName = valid.ToString();

            if (newName.Length > Constants.MAX_VARIABLE_NAME_LENGTH)
                newName = newName.Substring(0, Constants.MAX_VARIABLE_NAME_LENGTH);

            // Add a number to the end of the variable name if a variable with the same name was already created.
            if (NameTaken(isGlobal, newName))
            {
                int num = 0;
                while (NameTaken(isGlobal, NewName(newName, num))) num++;
                newName = NewName(newName, num);
            }
            return newName.ToString();
        }

        private bool NameTaken(bool isGlobal, string name)
        {
            return variableList(isGlobal).Any(gv => gv != null && gv.Name == name) || (isGlobal ? reservedGlobalNames : reservedPlayerNames).Contains(name);
        }

        private string NewName(string baseName, int indent)
        {
            return baseName.Substring(0, Math.Min(baseName.Length, Constants.MAX_VARIABLE_NAME_LENGTH - (indent.ToString().Length + 1))) + "_" + indent;
        }
    
        private WorkshopVariable AssignWorkshopVariable(string name, bool isGlobal)
        {
            int id = NextFreeID(isGlobal);
            WorkshopVariable workshopVariable = new WorkshopVariable(isGlobal, id, WorkshopNameFromCodeName(isGlobal, name));
            variableList(isGlobal).Add(workshopVariable);
            return workshopVariable;
        }
        
        private int NextFreeID(bool isGlobal)
        {
            // Get the next free ID.
            int id = -1;
            var collection = variableList(isGlobal);
            for (int i = 0; i < Constants.NUMBER_OF_VARIABLES; i++)
                // Make sure the ID is not reserved.
                if (!variableList(isGlobal).Any(var => var.ID == i) && !reserveList(isGlobal).Contains(i))
                {
                    id = i;
                    break;
                }

            // If ID still equals -1, there are no more free variables.
            if (id == -1)
                throw new Exception();
            return id;
        }

        private int NextFreeExtended(bool isGlobal)
        {
            int index = Array.IndexOf(extendedVariableList(isGlobal), null);
            if (index == -1) throw new Exception();
            return index;
        }
    
        public IndexReference Assign(string name, bool isGlobal, bool extended)
        {
            if (!extended)
                return new IndexReference(ArrayBuilder, AssignWorkshopVariable(name, isGlobal));
            else
            {
                int index = NextFreeExtended(isGlobal);
                IndexReference reference = new IndexReference(ArrayBuilder, isGlobal ? global : player, new V_Number(index));
                extendedVariableList(isGlobal)[index] = reference;
                return reference;
            }
        }

        public IndexReference Assign(Var var, bool isGlobal)
        {
            // variableIsGlobal will equal isGlobal if var.VariableType is dynamic. Otherwise, it will equal is var.VariableType global.
            bool variableIsGlobal = var.VariableType == VariableType.Dynamic ? isGlobal : var.VariableType == VariableType.Global;

            if (!var.InExtendedCollection)
            {
                if (var.ID == -1)
                    return new IndexReference(ArrayBuilder, AssignWorkshopVariable(var.Name, variableIsGlobal));
                else
                {
                    WorkshopVariable workshopVariable = new WorkshopVariable(variableIsGlobal, var.ID, WorkshopNameFromCodeName(variableIsGlobal, var.Name));
                    variableList(isGlobal).Add(workshopVariable);
                    return new IndexReference(ArrayBuilder, workshopVariable);
                }
            }
            else
            {
                int index = NextFreeExtended(isGlobal);
                IndexReference reference = new IndexReference(ArrayBuilder, isGlobal ? global : player, new V_Number(index));
                extendedVariableList(isGlobal)[index] = reference;
                return reference;
            }
        }
    
        public void ToWorkshop(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("variables");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine(Extras.Indent(1, false) + "global:");
            WriteCollection(stringBuilder, variableList(true));
            stringBuilder.AppendLine(Extras.Indent(1, false) + "player:");
            WriteCollection(stringBuilder, variableList(false));
            stringBuilder.AppendLine("}");

            bool anyExtendedGlobal = extendedVariableList(true).Any(v => v != null);
            bool anyExtendedPlayer = extendedVariableList(false).Any(v => v != null);
            if (anyExtendedGlobal || anyExtendedPlayer)
            {
                // TODO: Write extended variables.
            }
        }
        private void WriteCollection(StringBuilder stringBuilder, List<WorkshopVariable> collection)
        {
            foreach (var var in collection) stringBuilder.AppendLine(Extras.Indent(2, false) + var.ID + ": " + var.Name);
        }
    }
}