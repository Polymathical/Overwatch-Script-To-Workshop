using System;
using Deltin.Deltinteger.Elements;
using CompletionItem = OmniSharp.Extensions.LanguageServer.Protocol.Models.CompletionItem;
using CompletionItemKind = OmniSharp.Extensions.LanguageServer.Protocol.Models.CompletionItemKind;

namespace Deltin.Deltinteger.Parse
{
    public interface IInitOperations
    {
        void InitOperations();
    }

    public class ObjectType : CodeType, IInitOperations
    {
        public static readonly ObjectType Instance = new ObjectType();

        private ObjectType() : base("Object")
        {
            CanBeExtended = true;
        }

        public void InitOperations()
        {
            Operations = new TypeOperation[] {
                new TypeOperation(TypeOperator.Equal, this, BooleanType.Instance, (l, r) => new V_Compare(l, Operators.Equal, r)),
                new TypeOperation(TypeOperator.NotEqual, this, BooleanType.Instance, (l, r) => new V_Compare(l, Operators.NotEqual, r))
            };
        }

        public override CompletionItem GetCompletion() => new CompletionItem() {
            Label = Name,
            Kind = CompletionItemKind.Struct
        };
        public override Scope ReturningScope() => null;
    }

    public class NullType : CodeType
    {
        public static readonly NullType Instance = new NullType();

        private NullType() : base("?") {}

        public override bool Implements(CodeType type) => type.Implements(ObjectType.Instance);
        public override CompletionItem GetCompletion() => new CompletionItem() {
            Label = Name,
            Kind = CompletionItemKind.Struct
        };
        public override Scope ReturningScope() => null;
    }

    public class NumberType : CodeType, IInitOperations
    {
        public static readonly NumberType Instance = new NumberType();

        private NumberType() : base("Number")
        {
            CanBeExtended = false;
            Inherit(ObjectType.Instance, null, null);
        }

        public void InitOperations()
        {
            Operations = new TypeOperation[] {
                new TypeOperation(TypeOperator.Add, this, this, TypeOperation.Add), // Number + number
                new TypeOperation(TypeOperator.Subtract, this, this, TypeOperation.Subtract), // Number - number
                new TypeOperation(TypeOperator.Multiply, this, this, TypeOperation.Multiply), // Number * number
                new TypeOperation(TypeOperator.Divide, this, this, TypeOperation.Divide), // Number / number
                new TypeOperation(TypeOperator.Modulo, this, this, TypeOperation.Modulo), // Number % number
                new TypeOperation(TypeOperator.Multiply, VectorType.Instance, VectorType.Instance, TypeOperation.Multiply), // Number * vector
                new TypeOperation(TypeOperator.LessThan, this, BooleanType.Instance, (l, r) => new V_Compare(l, Operators.LessThan, r)), // Number < number
                new TypeOperation(TypeOperator.LessThanOrEqual, this, BooleanType.Instance, (l, r) => new V_Compare(l, Operators.LessThanOrEqual, r)), // Number <= number
                new TypeOperation(TypeOperator.GreaterThanOrEqual, this, BooleanType.Instance, (l, r) => new V_Compare(l, Operators.GreaterThanOrEqual, r)), // Number >= number
                new TypeOperation(TypeOperator.GreaterThan, this, BooleanType.Instance, (l, r) => new V_Compare(l, Operators.GreaterThan, r)), // Number > number
            };
        }

        public override CompletionItem GetCompletion() => new CompletionItem() {
            Label = Name,
            Kind = CompletionItemKind.Struct
        };
        public override Scope ReturningScope() => null;
    }

    public class TeamType : CodeType
    {
        public static readonly TeamType Instance = new TeamType();

        private TeamType() : base("Team")
        {
            CanBeExtended = false;
            Inherit(ObjectType.Instance, null, null);
        }

        public override CompletionItem GetCompletion() => new CompletionItem() {
            Label = Name,
            Kind = CompletionItemKind.Struct
        };
        public override Scope ReturningScope() => null;
    }

    public class BooleanType : CodeType
    {
        public static readonly BooleanType Instance = new BooleanType();

        private BooleanType() : base("Boolean")
        {
            CanBeExtended = false;
            Inherit(ObjectType.Instance, null, null);

            Operations = new TypeOperation[] {
                new TypeOperation(TypeOperator.And, this, this, (l, r) => Element.Part<V_And>(l, r)),
                new TypeOperation(TypeOperator.Or, this, this, (l, r) => Element.Part<V_Or>(l, r)),
            };
        }

        public override CompletionItem GetCompletion() => new CompletionItem() {
            Label = Name,
            Kind = CompletionItemKind.Struct
        };
        public override Scope ReturningScope() => null;
    }
}