code  ::= stmts
stmts ::= stmt ( "\n" stmt)*
stmt  ::= [ expr ] [ "#" cmt ]
expr  ::= [ rslts "=" ] opcode "(" [ args ] ")" 
rslts ::= rslt ( "," rslt )*
rslt  ::= ident | "_"
args  ::= arg ( "," arg )*
arg   ::= ident | lit
lit   ::= "\"" str "\"" | "true" | "false" | num