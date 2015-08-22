namespace Voidbred.Lisp

open System.Reflection.Emit
open System.Reflection
open System

type LispVal =
  | ArgRef of int
  | Atom of string
  | Bool of bool
  | IfPrimitive of LispVal * LispVal * LispVal
  | LambdaDef of string list * LispVal
  | LambdaRef of MethodInfo * bool * Type list
  | List of LispVal list
  | ListPrimitive of ListOp * LispVal list
  | Number of int
  | QuotePrimitive of LispVal
  | String of string
  | VariableDef of string * LispVal
  | VariableRef of LocalBuilder
