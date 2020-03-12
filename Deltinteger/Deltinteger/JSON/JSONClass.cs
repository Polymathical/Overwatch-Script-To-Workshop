using Deltin.Deltinteger;
using Deltin.Deltinteger.Elements;
using Deltin.Deltinteger.LanguageServer;
using Deltin.Deltinteger.Parse;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;

class JSONType: CodeType {
    List<JSONType> Children = new List<JSONType>();
    List<(InternalVar var, JProperty prop)> Properties = new List<(InternalVar var, JProperty prop)>();

    private Scope staticScope = new Scope("JSON");

    public JSONType(JObject jsonData) : base("JSON"){
        foreach(JProperty prop in jsonData.Children<JProperty>()){
            switch (prop.Value.Type)
            {
                case JTokenType.String:
                case JTokenType.Boolean:
                //case JTokenType.Array:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Null:
                    Properties.Add((CreateInternalVar(prop.Name, "A JSON Property."), prop));
                    break;
                default:
                    Children.Add(new JSONType((JObject)prop.Value));
                    break;
            }
        }
    }


    private InternalVar CreateInternalVar(string name, string documentation)
    {
        // Create the variable.
        InternalVar newInternalVar = new InternalVar(name, CompletionItemKind.Property);

        // Make the variable unsettable.
        newInternalVar.IsSettable = false;

        // Set the documentation.
        newInternalVar.Documentation = documentation;

        staticScope.AddNativeVariable(newInternalVar);

        return newInternalVar;
    }



    public override void AddObjectVariablesToAssigner(IWorkshopTree reference, VarIndexAssigner assigner) {
        foreach (var p in Properties) {
            switch (p.prop.Type)
            {
                case JTokenType.String:
                    assigner.Add(p.var, new V_CustomString(p.prop.Value.ToObject<string>()));
                    break;
                case JTokenType.Boolean:
                    throw new NotImplementedException();
                case JTokenType.Float:
                    assigner.Add(p.var, new V_Number(p.prop.Value.ToObject<float>()));
                    break;
                case JTokenType.Integer:
                    assigner.Add(p.var, new V_Number(p.prop.Value.ToObject<int>()));
                    break;
                case JTokenType.Null:
                    assigner.Add(p.var, new V_Null());
                    break;
            }
        }
    }
    public override CompletionItem GetCompletion() => new CompletionItem()
    {
        Label = Name,
        Kind = CompletionItemKind.Struct
    };


    public override Scope ReturningScope() => staticScope;
}


//class JSONConstructor : Constructor
//{
//        public JSONConstructor(JSONType jsonType) : base(jsonType, null, AccessLevel.Public)
//        {
//            Parameters = new CodeParameter[] {
//                new JSONFileParameter("jsonFile", "path of the JSON file you want to access. Must be a `.json` file.")
//            };
//            Documentation = Extras.GetMarkupContent("Creates a macro out of a '.json' file.");
//        }

//        public override void Parse(ActionSet actionSet, IWorkshopTree[] parameterValues, object[] additionalParameterData) => throw new NotImplementedException();
//    }

//class JSONFileParameter : FileParameter {
//    public JSONFileParameter(string parameterName, string description) : base(parameterName, description, ".json") {}

//        public override object Validate(ScriptFile script, IExpression value, DocRange valueRange){
//            string filepath = base.Validate(script, value, valueRange) as string;
//            if (filepath == null) return null;


//            JObject jsonData;

//            try{
//                ImportedScript file = FileGetter.GetImportedFile(new Uri(filepath));
//                file.Update();

//                jsonData = JObject.Parse(file.Content);
//            } catch (InvalidOperationException){
//                script.Diagnostics.Error("Failed to deserialize the JSON file.", valueRange);
//                return null;
//            }

//            return jsonData;
//        }

//}