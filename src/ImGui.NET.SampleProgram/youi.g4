grammar youi;

statements
    : statement+;

statement
    : screen
    | button 
    | check
    ;

control_arguments
    : IDENTIFIER
    ;

screen
    : SCREEN control_arguments SEMI
    | SCREEN SEMI
    ;
    
button 
    : '[' control_arguments ']'
    | '[' ']'
    | BUTTON SEMI
    | BUTTON control_arguments SEMI
    ;
    
check 
    : CHECK control_arguments SEMI
    ;

IDENTIFIER
   : LETTER CHARACTER*
   ;
         
SEMI : ';';
SCREEN: 'screen';
CHECK : 'check';
BUTTON : 'button';
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