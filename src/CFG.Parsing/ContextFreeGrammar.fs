namespace CFG.Parsing

open SingleLineString

module ContextFreeGrammar =
    type Epsilon = Epsilon
    type Literal = Literal of SingleLineString
    type IdentifierPart = IdentifierPart of SingleLineString
    type Identifier = Identifier of IdentifierPart list
    type DefinitionPart =
        | LiteralDefinitionPart of Literal
        | IdentifierDefinitionPart of Identifier
        | EpsilonDefinitionPart of Epsilon
    type DefinitionChoice = DefinitionChoice of DefinitionPart list
    type Definition = Definition of DefinitionChoice list
    type Rule = {
        Identifier: Identifier
        Definition: Definition
    }
    type Grammar = Grammar of Rule list