grammar youi;

statements
    : statement+;

statement
    : screen
    | button 
    ;

control_arguments
    : IDENTIFIER?
    ;

screen
    : '|' control_arguments '|' SEMI
    ;
    
button 
    : '[' control_arguments ']' SEMI
    ;

IDENTIFIER
   : LETTER CHARACTER*
   ;
         
SEMI : ';';
WS : [\u0020\u0009\u000C]+ -> skip;
NEWLINE: '\n' -> skip;

fragment CHARACTER
   : (LETTER | DIGIT)
   ;

fragment LETTER
   : ('a' .. 'z' | 'A' .. 'Z' | '_' | '/' | '#' | '@' | '(' | ')')
   ;
   
fragment DIGIT
  : '0' .. '9' | '-' | '+' | '.' | ','
  ;