using System.Text;
using System.Linq;
using Deltin.Deltinteger.Decompiler.TextToElement;
using Deltin.Deltinteger.Elements;

namespace Deltin.Deltinteger.Decompiler.ElementToCode
{
    public class CodeFormattingOptions
    {
        public bool SameLineOpeningBrace = false;
        public bool IndentWithTabs = false;
        public int SpaceIndentCount = 4;
    }

    public class WorkshopDecompiler
    {
        public Workshop Workshop { get; }
        public CodeFormattingOptions Options { get; }
        public int IndentLevel { get; private set; }
        private readonly StringBuilder _builder = new StringBuilder();
        private bool _space = false;

        public WorkshopDecompiler(Workshop workshop, CodeFormattingOptions options)
        {
            Workshop = workshop;
            Options = options;
        }

        public void AddBlock(bool startBlock = true)
        {
            if (Options.SameLineOpeningBrace)
            {
                if (startBlock) Append(" {");
                NewLine();
                Indent();
            }
            else
            {
                if (startBlock)
                {
                    NewLine();
                    Append("{");
                }
                NewLine();
                Indent();
            }
        }

        public void Indent() => IndentLevel++;
        public void Outdent() => IndentLevel--;

        public void Append(string text)
        {
            if (_space)
            {
                _space = false;
                _builder.Append(new string(Options.IndentWithTabs ? '\t' : ' ', IndentLevel * (Options.IndentWithTabs ? 1 : Options.SpaceIndentCount)));
            }
            _builder.Append(text);
        }

        public void NewLine()
        {
            _builder.AppendLine();
            _space = true;
        }

        public string Decompile()
        {
            // Variables
            foreach (var variable in Workshop.Variables)
            {
                Append((variable.IsGlobal ? "globalvar" : "playervar") + " define " + GetVariableName(variable.Name, variable.IsGlobal) + ";");
                NewLine();
            }
            NewLine();
            
            // Rules
            foreach (var rule in Workshop.Rules)
                new DecompileRule(this, rule).Decompile();
            
            return _builder.ToString();
        }

        public override string ToString() => _builder.ToString();

        public string GetVariableName(string baseName, bool isGlobal)
        {
            if (!isGlobal && Workshop.Variables.Any(v => v.IsGlobal && v.Name == baseName))
                baseName = "p_" + baseName;
            
            return baseName;
        }
    }

    public class DecompileRule
    {
        public WorkshopDecompiler Decompiler { get; }
        public TTERule Rule { get; }
        public int CurrentAction { get; private set; }
        public bool IsFinished => CurrentAction >= Rule.Actions.Length;
        public ITTEAction Current => Rule.Actions[CurrentAction];

        public DecompileRule(WorkshopDecompiler decompiler, TTERule rule)
        {
            Decompiler = decompiler;
            Rule = rule;
        }
        
        public void Decompile()
        {
            if (Rule.EventInfo.Event != RuleEvent.Subroutine)
            {
                if (Rule.Disabled) Decompiler.Append("disabled ");
                Decompiler.Append("rule: \"" + Rule.Name + "\"");

                if (Rule.EventInfo.Event != RuleEvent.OngoingGlobal)
                {
                    Decompiler.NewLine();
                    Decompiler.Append("Event." + EnumData.GetEnumValue(Rule.EventInfo.Event).CodeName);
                    // Write the event.
                    if (Rule.EventInfo.Team != Team.All)
                    {
                        Decompiler.NewLine();
                        Decompiler.Append("Team." + EnumData.GetEnumValue(Rule.EventInfo.Team).CodeName);
                    }
                    // Write the player.
                    if (Rule.EventInfo.Player != PlayerSelector.All)
                    {
                        Decompiler.NewLine();
                        Decompiler.Append("Player." + EnumData.GetEnumValue(Rule.EventInfo.Player).CodeName);
                    }
                }
            }
            else
            {
                Decompiler.Append("void " + Rule.EventInfo.SubroutineName + "() \"" + Rule.Name + "\"");
            }

            Decompiler.AddBlock();

            while (CurrentAction < Rule.Actions.Length)
                DecompileCurrentAction();

            Decompiler.Outdent();
            Decompiler.Append("}");
            Decompiler.NewLine();
            Decompiler.NewLine();
        }

        public void DecompileCurrentAction()
        {
            Rule.Actions[CurrentAction].Decompile(this);
        }

        public void Append(string text) => Decompiler.Append(text);
        public void NewLine() => Decompiler.NewLine();
        public void AddBlock(bool startBlock = true) => Decompiler.AddBlock(startBlock);
        public void Outdent() => Decompiler.Outdent();
        public void Advance() {
            CurrentAction++;
        }
        public void EndAction()
        {
            Append(";");
            NewLine();
            Advance();
        }
        public void AddComment(ITTEAction action)
        {
            if (action.Comment == null) return;
            if (action.Disabled) Append("// ");
            else Append("# ");
            Append(action.Comment);
            NewLine();
        }
    }
}