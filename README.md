# CFG.NET

Parsing and generating types from context-free grammars using F#.

## What are context-free grammars, and why are they useful?

You can think of a [context-free grammar (CFG)](https://en.wikipedia.org/wiki/Context-free_grammar) as a set of rules, written following a consistent notation, that can describe or define pretty much anything.

### Context-free grammars as specifications

The description above is quite vague, so consider the following example of a context-free grammar that uses [Backus-Naur Form (BNF)](https://en.wikipedia.org/wiki/Backus%E2%80%93Naur_form) as a notation for its rules.

```
<number> = <non-zero-digit> | <number> <digit>
<digit> = "0" | <non-zero-digit>
<non-zero-digit> = "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
```

This simple context-free grammar says that a `number` is either a `non-zero-digit` (a single digit 1~9 but not 0), or that it is a `number` followed by any `digit` (which may include 0). This means that `5` is a valid number according to the rules, but it also means that `50` (which is `5` followed by a `0`) is itself also a valid number. Let's define this same context-free grammar using another notation called [Extended Backus-Naur Form (EBNF)](https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form), commonly used by the [ANTLR](https://en.wikipedia.org/wiki/ANTLR) project to describe programming languages. This notation uses the `*` symbol to mean *zero or more occurrences*, which makes things a bit more readable than standard BNF.

```
grammar Number;

number
    : nonZeroDigit digit*
    ;

digit
    : '0' | nonZeroDigit
    ;

nonZeroDigit
    : '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9'
    ;
```

Whichever notation you choose, you can define complex context-free grammars to describe the rules of pretty much anything, as long as you can express it with text. This makes context-free grammars very useful, because if you follow the rules of a grammar, you can only generate text that is valid according to the rules of the grammar.

### Rules are easily broken

Many systems rely on the correctness of text-based structures, such as [source code](https://en.wikipedia.org/wiki/Source_code) for programming, configuration files using [JSON](https://en.wikipedia.org/wiki/JSON) or [YAML](https://en.wikipedia.org/wiki/YAML), and so on. Without software to validate and check for correctness, it's easy to make typos, meaning that the text you've written doesn't satisfy a certain specification. What's worse is that you'll often have no idea where the typo is, making it hard to find and correct mistakes.

When you start from a specification, however, you can repeatedly apply the rules of the context-free grammar that's used to represent the specification, which means that no matter what you do, the end result will always be a correct and valid output that satisfies the constraints and rules of the grammar, and consequently, the specification.

Given the specification for a number above, the value `01` would be invalid. But starting from the definition of a number, you would never be able to generate a number that starts with `0`, because the rules specifically disallow it.

## How to generate types from a CFG, and what's the purpose of it?

