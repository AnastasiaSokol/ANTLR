grammar test;
options { 
	language = CSharp3; 
} 
//header Добавляет код в самое начало файла 
@header {
	using System;
	using ConsoleApplication1;
}
//members Добавляет код в определение классов для анализаторов. Обычно это объявление вспомогательных свойств и методов.
@members {
  Emitter emitter; 
  public testParser(ITokenStream input, Emitter emitter)
  	: this(input) 
  {
    this.emitter = emitter;
  }
}
public
program	: (stmt)+ {emitter.AddHalt();};
public
stmt:	declar_stmt|assign_stmt|cycle_stmt|if_stmt|switch_stmt;
//int  a11e;
public
declar_stmt: DECLAR SPACE operand ENDOP;
public
assign_stmt: ID  {emitter.AddLValue($ID.text); }  INITIALIZE  expr {emitter.AddAssignStatement();} ENDOP;
//while(<ЛВ>) do<ОБ>
//while(a11w||a11e)do{a11q=a22d;}
public
cycle_stmt
	:  WHILE {emitter.Add_Label_Condition();} log_operation {emitter.Add_GoFalse_End_While();} DO  OPENFIGUREBRACKET stmt_or_block CLOSEFIGUREBRACKET 
	{emitter.Add_Goto_Condition(); emitter.AddLabel_End_While();};
//if <ЛВ> <ОБ> [else<ОБ>]
//if_stmt ? K_IF BRACKET_11 log_operation BRACKET_11 stmt_or_block | K_IF BRACKET_11 log_operation BRACKET_11 stmt_or_block K_ELSE stmt_or_blok
//if (a11s||a22s){a11q=a1e;}else{a11d=a11g;}
public
if_stmt	: IF log_operation {emitter.Add_GoFalse_Else_or_EndIf();} OPENFIGUREBRACKET stmt_or_block CLOSEFIGUREBRACKET {emitter.Add_Label_Else_or_EndIf();} (ELSE OPENFIGUREBRACKET stmt_or_block CLOSEFIGUREBRACKET {emitter.Add_Label_EndElse();})?
	;
//----------------------------------------------------------------------------
public
switch_stmt
	:SWITCH ID OPENFIGUREBRACKET ( {emitter.AddLoadID($ID.text); } cases)+ CLOSEFIGUREBRACKET;
public
cases	: BY  const {emitter.AddLoadConst($const.value); emitter.AddEq(); emitter.AddGOFALSE_END_CASE(); } DOUBLEPOINT stmt_or_block {emitter.AddLABEL_END_CASE();}
	;

//----------------------------------------------------------------------------	
//(a11w+a11e)
//((a11w*f11r)+a11e)

term 	:	OPENBRACKET factor MULTOP factor {emitter.AddOperation($MULTOP.text);}  CLOSEBRACKET
	| 	factor ;
factor	: 	operand;
expr	:	term  
	|       OPENBRACKET term   ADDOP   term {emitter.AddOperation($ADDOP.text);}CLOSEBRACKET
	;
  
operand	:	ID {emitter.AddLoadID($ID.text); } |const{emitter.AddLoadConst($const.value); };
//------------------------------------------------------------------------
//(a11w&&a11e)
//(a11w<=a11e)
stmt_or_block
	:	 stmt | OPENFIGUREBRACKET stmt CLOSEFIGUREBRACKET;

log_operation
	:	operand
	|	OPENBRACKET operand LOGIC_OP   operand {emitter.AddOperation($LOGIC_OP.text);}CLOSEBRACKET
	|	OPENBRACKET operand OPSRAVN operand {emitter.AddOperation($OPSRAVN.text);}CLOSEBRACKET
	;
//const	:	integer|float|char|bool;
const returns[String value]:	
		INT{$value=$INT.text}
	|	FLOAT{$value=$FLOAT.text}
	|	CHAR{$value=$CHAR.text}
	|	BOOL{$value=$BOOL.text}
	;
//-----------------------------------------------------------------------------------------------
WS  :   ( 
		  ' '
	        | '\t'
	        | '\r'
	        | '\n'
        ) 
        {$channel = Hidden; }
    ;

fragment LETTER	:	'A'..'Z'|'a'..'z';
fragment DIGIT	: 	'0'..'3';
ID	:	LETTER (DIGIT)+ LETTER; //<Б><пЦ><Б>
//------------------------------------------------------------------
DECLAR:		'int'('e'('g'('e'('r')?)?)?)? 
	|	'float'
	|	'char'
	|	'bool';
//------------------------------------------------------------------
INITIALIZE:	'=';
OPENBRACKET:		'(';
OPENFIGUREBRACKET:	'{';
CLOSEBRACKET:		')';
CLOSEFIGUREBRACKET:	'}';
ENDOP	: 	';';
DOUBLEPOINT:	':';
SPACE	:	'  ';
//------------------------------------------------------------------
OPSRAVN:	'<'|'>'|'=='|'<='|'>='|'<>';
ADDOP 	:	('+'|'-');
MULTOP 	:	('*'|'/');
LOGIC_OP:	'||'|'&&';
//------------------------------------------------------------------
//while(<ЛВ>) do<ОБ>
WHILE	: 	'while'|'WHILE';
DO	:	'do'|'DO';
IF	:	'if '|'IF ';//if <ЛВ> <ОБ> [else<ОБ>]
ELSE 	:	'else'|'ELSE';
SWITCH	:	'switch '|'SWITCH ';
BY	:	'by '|'BY ';
//------------------------------------------------------------------
INT	:	DIGIT+;
FLOAT   :	DIGIT+ ('.' DIGIT+ EXPONENT)?
	;
EXPONENT:	('e'|'E') ('+'|'-')? DIGIT+ ;
CHAR    :	'"' LETTER '"';
BOOL	:	'true'|'false';
	
//------------------------------------------------------------------








