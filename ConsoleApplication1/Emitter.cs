using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Antlr.Runtime;
using Antlr.Runtime.Misc;

namespace ConsoleApplication1
{

    public class Emitter
    {
        
        //класс для хранения информации о переменных
        public class VariableInfo
        {
            public VariableInfo(string name) { Name= name; }
            public string Name { get; set; }
        }
        //таблица переменных
        IDictionary<string, VariableInfo> variableTable = new Dictionary<string, VariableInfo>();

        //буфер для формирования тела основоного метода
        StringBuilder methodBody = new StringBuilder();
        //-------------------------------------------------------
        // Запись в выходной поток шапки файла
		void WriteHeader(StreamWriter outWriter)
        {
            // Объявление сборки, модуля и подключаемых сборок
            outWriter.WriteLine(".assembly Program { }");
            outWriter.WriteLine(".module Program.exe");
            outWriter.WriteLine(".assembly extern mscorlib { }");
            outWriter.WriteLine();

            // Объявление основного метода - точки входа (стек ставим условно)
            outWriter.WriteLine(".method public static void Main() {");
            outWriter.WriteLine(".entrypoint");
            outWriter.WriteLine(".maxstack 300");
        }
        //-------------------------------------------------------
        // Запись окончания файла
		void WriteFooter(StreamWriter outWriter)
        {
            outWriter.WriteLine("ret");
            outWriter.WriteLine("}");
        }
        //-------------------------------------------------------
        //запись объявления всех встреченных локальных переменных
        void WriteLocals(StreamWriter outWriter) {
            if (variableTable.Count == 0) return;
            StringBuilder localsString = new StringBuilder();
            foreach (VariableInfo variable in variableTable.Values) {
                //localsString.Append(variable.Name + "  dw");
                //localsString.Append(" \n");
                localsString.AppendLine(variable.Name + "  dw");
            }
            localsString.Remove(localsString.Length - 1, 1);
            localsString.Append(" \n");
            outWriter.WriteLine(localsString.ToString());
        }
        //--------------------------------------------------------
        //запись ранее сгенерированного тела метода
        void WriteMethodBody(StreamWriter outWriter) {
            outWriter.WriteLine(methodBody.ToString());
        }
        //--------------------------------------------------------
        //запись всего выходного файла
        public void SaveMSIL(string fileName) {
            StreamWriter outWriter = new StreamWriter(File.Create(fileName), new System.Text.UTF8Encoding(true));
            WriteLocals(outWriter);
            WriteMethodBody(outWriter);
            outWriter.Flush();
        }
        //-------------------------------------------------------
        //добавление кода для оператора ввода
        /*
        public void AddInputStatement(string variableName) {
            methodBody.AppendLine("rvalue" + variableName + ":\"");
            if (!variableTable.Keys.Contains(variableName)) {
                variableTable.Add(variableName, new VariableInfo(variableName));
            }
            methodBody.AppendLine("in");
        }*/
        //-------------------------------------------------------
        //добавление кода для оператора печати 
        //Здесь формируется только операция вывода - само значение в этот момент
        //уже в стеке
        public void AddPrintStatement() {
            methodBody.AppendLine("out");
        }
        //-------------------------------------------------------
        //добавление кода для оператора присваивания
        //аналогично оператору печати здесь формируется только 
        //код загрузки переменной
        public void AddAssignStatement() {
            methodBody.AppendLine(":=");
        }
        //-------------------------------------------------------
        public void AddLValue(string variableName) {
            if (!variableTable.Keys.Contains(variableName)) {
                variableTable.Add(variableName, new VariableInfo(variableName));
            }
            methodBody.AppendLine("LVALUE " + variableName);
        }
        //------------------------------------------------------
        //Загрузка в стек значения локальной переменной
        public void AddLoadID(string variableName) {
            if (!variableTable.Keys.Contains(variableName)) {
                variableTable.Add(variableName, new VariableInfo(variableName));
            }
            methodBody.AppendLine("RVALUE " + variableName);
        }
        //------------------------------------------------------
        //загрузка в стек константы
        //+проверка константы
        public void AddLoadConst(string constant) {
            switch (constant) {
                case "true":
                    methodBody.AppendLine("PUSH 1");
                    break;
                case "false":
                    methodBody.AppendLine("PUSH 0");
                    break;
                default:
                    methodBody.AppendLine("PUSH " + constant);
                    break;
            }           
        }
        //------------------------------------------------------
        //Генерация кода операций
        public void AddOperation(string op)
        {
            switch (op)
            {
                case "+":
                    methodBody.AppendLine("add");
                    break;
                case "-":
                    methodBody.AppendLine("sub");
                    break;
                case "*":
                    methodBody.AppendLine("mult");
                    break;
                case "/":
                    methodBody.AppendLine("div");
                    break;
                case "||":
                    methodBody.AppendLine("or");
                    break;
                case "&&":
                    methodBody.AppendLine("and");
                    break;
                default:
                    methodBody.AppendLine(op);
                    break;
            }
        }
        //------------------------------------------------------
        //Добавить метку
        public void Add_Label_Condition()
        {
             methodBody.AppendLine("LABEL Condition");
        }
        //------------------------------------------------------
        //Добавить переход к метке
        public void Add_GoFalse_End_While()
        {
            methodBody.AppendLine("GOFALSE End_While");
        }
        //------------------------------------------------------
        //Добавить переход к метке
        public void Add_Goto_Condition()
        {
            methodBody.AppendLine("GOTO Condition");
        }
        //------------------------------------------------------
        //Добавить метку
        public void AddLabel_End_While()
        {
            methodBody.AppendLine("LABEL End_While");
        }
        //------------------------------------------------------
        //Добавить переход на метку
        public void Add_GoFalse_Else_or_EndIf()
        {
            methodBody.AppendLine("GOFALSE Else_or_EndIf");
        }
        //------------------------------------------------------
        //Добавить  метку
        public void Add_Label_Else_or_EndIf()
        {
            methodBody.AppendLine("LABEL Else_or_EndIf");
        }
        //------------------------------------------------------
        //Добавить  метку
        public void Add_Label_EndElse()
        {
            methodBody.AppendLine("LABEL EndElse");
        }
        //------------------------------------------------------
        //Добавить метку конца программы
        public void AddHalt()
        {
            methodBody.AppendLine("HALT");
        }
        //------------------------------------------------------
        //Знак равно
        public void AddEq()
        {
            methodBody.AppendLine("=");
        }
        //------------------------------------------------------
        //Добавить переход на метку
        public void AddGOFALSE_END_CASE()
        {
            methodBody.AppendLine("GOFALSE END_CASE");
        }
        //------------------------------------------------------
        //Добавить переход на метку
        public void AddLABEL_END_CASE()
        {
            methodBody.AppendLine("LABEL END_CASE");
        }

    }
 }
