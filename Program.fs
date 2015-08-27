// Learn more about F# at http://fsharp.net

open Microsoft.FSharp.Text.Lexing
open System
open System.IO
open System.Reflection
open System.Reflection.Emit
open System.Threading

let calcType value =
    match value with
        | Voidbred.Lisp.String(_) -> typeof<String>
        | _ -> failwith "not supported type"

let compileOp (car:String) cdr (ilGenerator:ILGenerator) typeBuilder =
    let splitName = car.Split [|'.'|]
    let typeName = (String.concat "." splitName.[0..splitName.Length - 2])
    let methodName = splitName.[splitName.Length - 1]
    let argTypes = List.map calcType cdr |> Array.ofList
    let typ = Type.GetType(typeName)
    let meth : MethodInfo = typ.GetMethod(methodName, argTypes)
    for arg in cdr do        
        match arg with
            | Voidbred.Lisp.String(x) -> ilGenerator.Emit(OpCodes.Ldstr, x)
            | _ -> failwithf "unsupported arg type:%A" arg    
    ilGenerator.Emit(OpCodes.Call, meth)    
    true

let compileSexp (sexp : Voidbred.Lisp.LispVal) ilGenerator typeBuilder =
    match sexp with
        | Voidbred.Lisp.List(Voidbred.Lisp.Atom(x)::y) -> compileOp x y ilGenerator typeBuilder
        | _ -> failwithf "don't know what to do with:%A" sexp

let compile (tree : seq<Voidbred.Lisp.LispVal>) =
    let assemblyName = new AssemblyName ()
    assemblyName.Name <- "HelloWorld"
    let assemblyBuilder =
        Thread.GetDomain().DefineDynamicAssembly(assemblyName,
                                                 AssemblyBuilderAccess.RunAndSave)
    let modl = assemblyBuilder.DefineDynamicModule("HelloWorld.exe")
    let typeBuilder = modl.DefineType("HelloWorldType",
                                      TypeAttributes.Public |||
                                      TypeAttributes.Class)
    let methodBuilder =
        typeBuilder.DefineMethod("Main",
                                 MethodAttributes.HideBySig |||
                                 MethodAttributes.Static |||
                                 MethodAttributes.Public,
                                 typeof<Void>,
                                 [|typeof<String>|])

    let ilGenerator = methodBuilder.GetILGenerator()
    for sexp in tree do
        compileSexp sexp ilGenerator typeBuilder |> ignore
    ilGenerator.Emit(OpCodes.Ret)

    let helloWorldType = typeBuilder.CreateType()
    helloWorldType.GetMethod("Main").Invoke(null, [|null|]) |> ignore
    assemblyBuilder

let testLexerAndParserFromString text = 
    let lexbuf = LexBuffer<char>.FromString text
    let parserOutput = Parser.parse Lexer.tokenstream lexbuf
    compile parserOutput |> ignore
    printfn "output:%A" parserOutput

let testLexerAndParserFromFile (fileName:string) = 
    use textReader = new System.IO.StreamReader(fileName)
    let lexbuf = LexBuffer<char>.FromTextReader textReader
    let parserOutput = Parser.parse Lexer.tokenstream lexbuf
    printfn "output:%A" parserOutput

testLexerAndParserFromString "(System.Console.WriteLine \"hello world\")"
