namespace CFG.Parsing

module SingleLineString =
    /// <summary>A string that does not contain any new lines.</summary>
    type SingleLineString = private SingleLineString of string
    type CreationError =
        | ContainsLineFeed
        | ContainsCarriageReturn
    let create (input : string) =
        match input with
        | s when s.Contains '\n' -> Error ContainsLineFeed
        | s when s.Contains '\r' -> Error ContainsCarriageReturn
        | _ -> Ok (SingleLineString input)