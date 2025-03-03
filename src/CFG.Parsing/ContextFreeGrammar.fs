namespace CFG.Parsing

module ContextFreeGrammar =
    type Literal = Literal of string
    type IdentifierPart = IdentifierPart of string
    type Identifier = Identifier of IdentifierPart list
    type DefinitionPart =
        | LiteralDefinitionPart of Literal
        | IdentifierDefinitionPart of Identifier
    type DefinitionChoice = DefinitionChoice of DefinitionPart list
    type Definition = Definition of DefinitionChoice list
    type Rule = {
        Identifier: Identifier
        Definition: Definition
    }
    type Grammar = Grammar of Rule list