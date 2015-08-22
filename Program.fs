// Learn more about F# at http://fsharp.net

open System.IO
open Microsoft.FSharp.Text.Lexing

let testLexerAndParserFromString text = 
    let lexbuf = LexBuffer<char>.FromString text
    let parserOutput = Parser.parse Lexer.tokenstream lexbuf
    printfn "output:%A" parserOutput

let testLexerAndParserFromFile (fileName:string) = 
    use textReader = new System.IO.StreamReader(fileName)
    let lexbuf = LexBuffer<char>.FromTextReader textReader
    let parserOutput = Parser.parse Lexer.tokenstream lexbuf
    printfn "output:%A" parserOutput

testLexerAndParserFromString "1234"
testLexerAndParserFromString "(define (foo x y) (+ x y))"

