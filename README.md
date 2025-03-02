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

This context-free grammar says that a `number` is either a `non-zero-digit` (a single digit 1~9 but not 0), or that it is a `number` followed by any `digit` (which may include 0). This means that `5` is a valid number according to the rules, but it also means that `50` (which is `5` followed by a `0`) is itself also a valid number. Note how 0 by itself is not a valid number according to these rules.

Let's define this same context-free grammar using another notation called [Extended Backus-Naur Form (EBNF)](https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form), commonly used by the [ANTLR](https://en.wikipedia.org/wiki/ANTLR) project to describe programming languages. This notation uses the `*` symbol to mean *zero or more occurrences*, which makes things a bit more readable than standard BNF.

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

Every rule of a context-free grammar can be converted into a type, such that all the types can be described in terms of the types converted from other rules of the grammar. This way, you can unambiguously re-create the grammar rules while guaranteeing [type safety](https://en.wikipedia.org/wiki/Type_safety). Some [dynamically typed languages](https://en.wikipedia.org/wiki/Dynamic_programming_language) have features to emulate type safety, such as [type hints for Python](https://docs.python.org/3/library/typing.html) or [JSDoc for JavaScript](https://jsdoc.app/), but they don't provide any real guarantees compared to statically typed languages.

If you re-create the grammar rules in a type safe fashion, you get the benefit that all type constructions are already constrained by the rules of that grammar. Concretely, this is equivalent to saying that you cannot create a number that starts with `0`, because the type hierarchy that you get from converting the grammar simply does not allow it. For more complex specifications, such as the JSON or YAML format, it means that you cannot accidentally create typos. Whatever output you create from the type hierarchy will be what's called *syntactically valid*.

Here's what the numbers grammar may look like when the BNF notation is converted to C#, which is a type-safe language:

```cs
// Interfaces represent BNF rule symbols
public interface INumber {}
public interface IDigit {}
public interface INonZeroDigit : INumber, IDigit {}

// Equivalent to the number ::= <number> <digit> definition part
public record NumberSymbol(INumber Number, IDigit Digit) : INumber; // The EBNF equivalent could be something like NumberSymbol(INonZeroDigit NonZeroDigit, List<IDigit> Digits)

// Equivalent to <digit> ::= "0"
public record Zero : IDigit;

// Equivalent to <non-zero-digit> ::= ...
public record One : INonZeroDigit;
public record Two : INonZeroDigit;
public record Three : INonZeroDigit;
public record Four : INonZeroDigit;
public record Five : INonZeroDigit;
public record Six : INonZeroDigit;
public record Seven : INonZeroDigit;
public record Eight : INonZeroDigit;
public record Nine : INonZeroDigit;
```

If you play around with this type hierarchy, you'll find that an instance of an `INumber` can be either an `INonZeroDigit` through the `One`, `Two`, ..., `Nine` types, or a `NumberSymbol` that wraps an `INumber` and an `IDigit`. This means it's impossible to create an `INumber` that is a `Zero` followed by one or more digits, even by accident.

## How to simplify constructing the types with dedicated builders?

Generating the types from a grammar can be valuable, but even for a relatively small grammar like the number example, constructing a number can get quite hairy. For example, to create the number `1230` using the C# types from before, you'd have to write something like this:

```cs
One one = new();
Two two = new();
Three three = new();
Zero zero = new();
INumber oneTwoThreeZero = 
    new NumberSymbol(
        new NumberSymbol(
            new NumberSymbol(
                one,
                two
            ),
            three
        ),
        zero
    );
```

Using a number builder that only exposes methods to build a syntactically valid number, the simplified code may look like this:

```cs
INumber oneTwoThreeZero = NumberBuilder
    .One() // First builder method call can not be 'Zero'
    .Two()
    .Three()
    .Zero() // Subsequent builder method calls can be 'Zero'
    .Build(); // Build to actually construct the number
```

This way to construct objects aligns more with [declarative programming](https://en.wikipedia.org/wiki/Declarative_programming), where you can focus on *what you want* and worry less about *how the code works*. It also means that you can build on top of the autogenerated builder classes to customize things even further to your own liking.