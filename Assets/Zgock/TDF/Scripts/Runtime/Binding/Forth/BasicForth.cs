using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace TotalDialogue
{
    public class BasicForth
    {
        private readonly Stack<int> wordStack = new();
        private class LoopData
        {
            public int Counter;
            public int Limit;
            public int StartIndex;
        }
        private class Word
        {
            public List<string> tokens;
            public bool isAction;
            public Func<UniTask> action;
            public int index;
            public async UniTask Execute(BasicForth forth)
            {
                if (isAction)
                {
                    await action();
                }
                else
                {
                    BasicForth.Execution ex = new();
                    ex.tokens = forth.Tokens;
                    ex.index = forth.executeIndex + 1;
                    forth.Executions.Push(ex);
                    forth.m_tokens = tokens;
                    forth.executeIndex = 0;
                }
            }
        }
        private readonly Stack<LoopData> loopStack = new();
        protected readonly Stack<string> stack = new();
        public Stack<string> Stack {get => stack;}
        public class Execution
        {
            public List<string> tokens;
            public int index;
        }
        public readonly Stack<Execution> Executions = new();
        private int executeIndex;
        private List<string> m_tokens;
        public List<string> Tokens{get => m_tokens;}
        private readonly Dictionary<string, Word> words = new();
        private bool definingWord = false;
        private Word CurrentDefiningWord;

        protected bool tftMode = false;

        public BasicForth()
        {
            RegistWord("+", Add);
            RegistWord("-", Subtract);
            RegistWord("*", Multiply);
            RegistWord("/", Divide);
            RegistWord("if", If);
            RegistWord("else", Else);
            RegistWord("then", Then);
            RegistWord("begin", Begin);
            RegistWord("while", While);
            RegistWord("until", Until);
            RegistWord("repeat", Repeat);
            RegistWord("loop", Loop);
            RegistWord("do", Do);
            RegistWord("i", DoI);

            RegistWord(".", Print);

            RegistWord("dup", Dup);
            RegistWord("Swap", Swap);
            RegistWord("Drop", Drop);

            // Comparison operators
            RegistWord(">", GreaterThan);
            RegistWord("<", LessThan);
            RegistWord(">=", GreaterThanOrEqual);
            RegistWord("<=", LessThanOrEqual);
            RegistWord("==", Equal);
            RegistWord("!=", NotEqual);
        }
        public async UniTask Execute(string input)
        {
            await Execute(Tokenize(input));
        }
        public async UniTask Execute(List<string> tokens)
        {
            m_tokens = tokens;
            executeIndex = 0;
            for (; ; )
            {
                while (executeIndex < Tokens.Count)
                {
                    await ExecuteToken(Tokens[executeIndex]);
                    executeIndex++;
                }
                if (Executions.Count <= 0) break;
                Execution ex = Executions.Pop();
                executeIndex = ex.index;
                m_tokens = ex.tokens;
            }

        }
        public virtual void AddToken(List<string> tokens, string token)
        {
            if (token == String.Empty) return;
            tokens.Add(token);
        }
        public virtual List<string> Tokenize(string code, List<string> tokens = null)
        {
            code = code.Replace("\r\n", "\n");
            code = code.Replace("\r", "\n");
            char quatation;
            if (tokens == null) tokens = new();
            bool newline = true;
            int i = 0;
            char c = code[i];
            StringBuilder tftsb = new();
            while (i < code.Length)
            {
                c = code[i];
                if (tftMode){
                    if (c == '[' || c == ']')
                    {
                        tftsb.Insert(0,"\"");
                        tftsb.Append("\"");
                        AddToken(tokens, tftsb.ToString());
                        tftsb.Clear();
                        tftMode = false;
                        //Debug.Log("Exit tft mode");
                        i++;
                        continue;
                    }
                    if (newline && char.IsWhiteSpace(c))
                    {
                        i++;
                        continue;
                    }
                    if (c == '#'){
                        i++;
                        while(i < code.Length && code[i] != '\n'){
                            i++;
                        }
                        newline = true;
                        i++;
                        continue;
                    }
                    if (c == '\t'){
                        i++;
                        continue;
                    }
                    if (c == '\n'){
                        newline = true;
                        i++;
                        continue;
                    }
                    newline = false;
                    if (c == '*'){
                        tftsb.Insert(0,"\"");
                        tftsb.Append("\"");
                        AddToken(tokens, tftsb.ToString());
                        AddToken(tokens, ".");
                        tftsb.Clear();
                        i++;
                        continue;
                    }
                    tftsb.Append(code[i]);
                    i++;
                }
                else
                {
                    if (char.IsWhiteSpace(c))
                    {
                        i++;
                        continue;
                    }
                    else if (c == '(')
                    {
                        while (i < code.Length && code[i] != ')')
                        {
                            i++;
                        }
                        i++;
                        continue;
                    }
                    else if (c == '[' || c == ']')
                    {
                        tftMode = true;
                        //Debug.Log("Enter tft mode");
                        i++;
                        continue;
                    }
                    else if (c == '"' || c == '\'')
                    {
                        quatation = c;
                        StringBuilder sb = new();
                        i++; // skip opening quote
                        while (i < code.Length && code[i] != quatation)
                        {
                            sb.Append(code[i]);
                            i++;
                        }
                        AddToken(tokens, "\"" + sb.ToString() + "\"");
                        i++; // skip closing quote
                    }
                    else
                    {
                        StringBuilder sb = new();
                        while (i < code.Length && !char.IsWhiteSpace(code[i]) && code[i] != '[' && code[i] != ']')
                        {
                            sb.Append(code[i]);
                            i++;
                        }
                        AddToken(tokens, sb.ToString());
                    }
                }
            }
            return tokens;
        }


        private async UniTask ExecuteToken(string token)
        {
            if (definingWord)
            {
                if (token.ToLower() == ";")
                {
                    EndDefiningWord();
                }
                else if (token.ToLower() == ":")
                {
                    Debug.LogError("Error: Nested word is not supported");
                }
                else
                {
                    AddToken(CurrentDefiningWord.tokens, token);
                }
            }
            else if (float.TryParse(token, out float _))
            {
                stack.Push(token);
            }
            else if (words.ContainsKey(token.ToLower()))
            {
                await words[token.ToLower()].Execute(this);
            }
            else if (token.StartsWith("\"") && token.EndsWith("\""))
            {
                string unescapedToken = Regex.Unescape(token.Trim('\"'));
                stack.Push(unescapedToken);
            }
            else if (token.ToLower() == ":")
            {
                if (executeIndex >= Tokens.Count - 1)
                {
                    Debug.LogError($"Error: ':' found end of code");
                }
                StartDefiningWord();
            }
            else if (token.ToLower() == ";")
            {
                Debug.LogError($"Error: Found ';' without ';'.");
            }
            else
            {
                Debug.LogError($"Error: Unknown token '{token}'");
            }
        }

        private void StartDefiningWord()
        {
            definingWord = true;
            CurrentDefiningWord = new()
            {
                tokens = new(),
                isAction = false
            };
        }
        private void EndDefiningWord()
        {
            definingWord = false;
            RegistWord(CurrentDefiningWord);
        }

        void RegistWord(Word word)
        {
            string key = word.tokens[0];
            words[key.ToLower()] = word;
        }
        protected void RegistWord(string word, Func<UniTask> newAction)
        {
            words[word.ToLower()] = new Word()
            {
                isAction = true,
                action = newAction
            };
        }

        protected void DefineWord(string definition)
        {
            List<string> newToken = Tokenize(definition);
            if (newToken.Count < 2)
            {
                Debug.LogWarning("Forth: Tried to define empty word.");
                return;
            }
            string word = newToken[0];
            int newIndex = Tokens.Count + 2;
            newToken.Insert(0, ":");
            newToken.Add(";");
            Tokens.AddRange(newToken);
            RegistWord(new Word() { tokens = newToken, index = 0, isAction = false });
        }
        private UniTask Add()
        {
            if (stack.Count < 2)
            {
                Debug.LogError("Forth: Insufficient operands for + operation");
                return UniTask.CompletedTask;
            }
            string operand2 = stack.Pop();
            string operand1 = stack.Pop();

            if (float.TryParse(operand1, out float num1) && float.TryParse(operand2, out float num2))
            {
                stack.Push((num1 + num2).ToString());
            }
            else
            {
                stack.Push(operand1 + operand2);
            }
            return UniTask.CompletedTask;
        }

        private UniTask Subtract()
        {
            if (stack.Count < 2)
            {
                Debug.LogError("Error: Insufficient operands for - operation");
                return UniTask.CompletedTask;
            }
            string operand2 = stack.Pop();
            string operand1 = stack.Pop();

            if (float.TryParse(operand1, out float num1) && float.TryParse(operand2, out float num2))
            {
                stack.Push((num1 - num2).ToString());
            }
            else
            {
                stack.Push(operand1.Replace(operand2, ""));
            }
            return UniTask.CompletedTask;
        }

        private UniTask Multiply()
        {
            if (stack.Count < 2)
            {
                Debug.LogError("Error: Insufficient operands for * operation");
                return UniTask.CompletedTask;
            }
            string operand2 = stack.Pop();
            string operand1 = stack.Pop();

            bool parsedOperand1 = float.TryParse(operand1, out float floatNum);
            bool parsedOperand2 = float.TryParse(operand2, out float floatNum2);

            if (parsedOperand1 && parsedOperand2)
            {
                stack.Push((floatNum * floatNum2).ToString());
            }
            else if (parsedOperand1 && !parsedOperand2)
            {
                int repeatCount = (int)floatNum;
                stack.Push(string.Concat(Enumerable.Repeat(operand2, repeatCount)));
            }
            else if (!parsedOperand1 && parsedOperand2)
            {
                int repeatCount = (int)floatNum2;
                stack.Push(string.Concat(Enumerable.Repeat(operand1, repeatCount)));
            }
            else
            {
                Debug.LogError("Error: Invalid operands for * operation");
            }
            return UniTask.CompletedTask;
        }

        private UniTask Divide()
        {
            if (stack.Count < 2)
            {
                Debug.LogError("Error: Insufficient operands for / operation");
                return UniTask.CompletedTask;
            }
            string operand2 = stack.Pop();
            string operand1 = stack.Pop();

            if (!float.TryParse(operand1, out float num1) || !float.TryParse(operand2, out float num2))
            {
                Debug.LogError("Error: Invalid operands for / operation");
                return UniTask.CompletedTask;
            }

            if (num2 == 0)
            {
                Debug.LogError("Error: Division by zero");
                return UniTask.CompletedTask;
            }

            stack.Push((num1 / num2).ToString());
            return UniTask.CompletedTask;
        }

        private UniTask If()
        {
            int thenIndex = FindThenIndex(executeIndex + 1);
            int elseIndex = FindElseIndex(executeIndex + 1, thenIndex);

            bool condition = float.TryParse(stack.Pop(), out float result);
            condition = condition && result != 0;

            if (!condition)
            {
                if (elseIndex != -1)
                {
                    executeIndex = elseIndex;
                }
                else
                {
                    executeIndex = thenIndex;
                }
            }
            return UniTask.CompletedTask;
        }


        private UniTask Else()
        {
            executeIndex = FindThenIndex(executeIndex);
            return UniTask.CompletedTask;
        }

        private UniTask Then()
        {
            return UniTask.CompletedTask;
            // No action needed, used in conjunction with If
        }
        private int FindNestedIndex(int startIndex, string word, int endIndex = -1)
        {
            // 'then' トークンを探してその位置を返す
            if (endIndex < 0) endIndex = Tokens.Count;
            int nestingLevel = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                string token = Tokens[i].ToLower();
                if (token == word)
                {
                    if (nestingLevel == 0)
                    {
                        return i;
                    }
                }
                else if (token == "if" || token == "do")
                {
                    nestingLevel++;
                }
                else if (token == "then" || token == "loop")
                {
                    nestingLevel--;
                }
                else if (token == ":")
                {
                    SkipWordDefinition(ref i, ";");
                }
            }
            return -1;
        }

        private int FindReverseIndex(int startIndex, string word, int endIndex = -1)
        {
            // 'then' トークンを探してその位置を返す
            if (endIndex < 0) endIndex = 0;
            int nestingLevel = 0;
            for (int i = startIndex; i >= endIndex; i--)
            {
                string token = Tokens[i].ToLower();
                if (token == word)
                {
                    if (nestingLevel == 0)
                    {
                        return i;
                    }
                }
                else if (token == "repeat")
                {
                    nestingLevel++;
                }
                else if (token == "begin")
                {
                    nestingLevel--;
                }
                else if (token == ";")
                {
                    SkipReverseDefinition(ref i, ":");
                }
            }
            return -1;
        }

        private void SkipWordDefinition(ref int index, string target)
        {
            // ':' が見つかるまでトークンを読み飛ばす
            while (index < Tokens.Count && Tokens[index] != target)
            {
                index++;
            }
        }
        private void SkipReverseDefinition(ref int index, string target)
        {
            while (index >= 0 && Tokens[index] != target)
            {
                index--;
            }
        }
        private int FindThenIndex(int startIndex)
        {
            return FindNestedIndex(startIndex, "then");
        }

        private int FindElseIndex(int startIndex, int endIndex)
        {
            return FindNestedIndex(startIndex, "else", endIndex);
        }
        private int FindBeginIndex(int startIndex)
        {
            return FindReverseIndex(startIndex, "begin");
        }
        private int FindRepeatIndex(int startIndex)
        {
            return FindNestedIndex(startIndex, "repeat");
        }
        private UniTask Begin()
        {
            // No action needed, used in conjunction with Repeat
            return UniTask.CompletedTask;
        }
        private UniTask While()
        {
            string value = stack.Pop();
            bool condition = float.TryParse(value, out float result);
            condition = condition && result != 0;

            if (!condition)
            {
                int repeatIndex = FindRepeatIndex(executeIndex + 1);
                executeIndex = repeatIndex;
            }
            return UniTask.CompletedTask;
        }
        private UniTask Until()
        {
            string value = stack.Pop();
            bool condition = float.TryParse(value, out float result);
            condition = condition && result != 0;

            if (condition)
            {
                int repeatIndex = FindRepeatIndex(executeIndex + 1);
                executeIndex = repeatIndex;
            }
            return UniTask.CompletedTask;
        }

        private UniTask Repeat()
        {
            int beginIndex = FindBeginIndex(executeIndex - 1);
            if (beginIndex >= 0)
            {
                executeIndex = beginIndex;
            }
            else
            {
                Debug.LogError("Forth: Begin for Repeat not found.");
            }
            return UniTask.CompletedTask;
        }

        private UniTask Do()
        {
            int counter = int.Parse(stack.Pop());
            int limit = int.Parse(stack.Pop());
            LoopData loopData = new()
            {
                Counter = counter,
                Limit = limit,
                StartIndex = executeIndex
            };
            loopStack.Push(loopData);
            return UniTask.CompletedTask;
        }
        private UniTask DoI()
        {
            if (loopStack.Count < 1)
            {
                Debug.LogError("Error: Insufficient operands for I operation");
                return UniTask.CompletedTask;
            }
            stack.Push(loopStack.Peek().Counter.ToString());
            return UniTask.CompletedTask;
        }

        private UniTask Loop()
        {
            LoopData loopData = loopStack.Pop();
            loopData.Counter++;
            if (loopData.Counter < loopData.Limit)
            {
                loopStack.Push(loopData);
                executeIndex = loopData.StartIndex;
            }
            return UniTask.CompletedTask;
        }
        protected virtual UniTask Print()
        {
            if (stack.Count < 1)
            {
                Debug.LogError("Error: Insufficient operands for . operation");
                return UniTask.CompletedTask;
            }
            string topOfStack = stack.Pop();
            Debug.Log(topOfStack);
            return UniTask.CompletedTask;
        }
        private UniTask Dup()
        {
            if (stack.Count < 1)
            {
                Debug.LogError("Error: Insufficient operands for dup operation");
                return UniTask.CompletedTask;
            }
            string topOfStack = stack.Peek();
            stack.Push(topOfStack);
            return UniTask.CompletedTask;
        }
        private UniTask Swap(){
            if (stack.Count < 2)
            {
                Debug.LogError("Error: Insufficient operands for swap operation");
                return UniTask.CompletedTask;
            }
            string operand1 = stack.Pop();
            string operand2 = stack.Pop();
            stack.Push(operand1);
            stack.Push(operand2);
            return UniTask.CompletedTask;
        }
        private UniTask Drop()
        {
            if (stack.Count < 1)
            {
                Debug.LogError("Error: Insufficient operands for drop operation");
                return UniTask.CompletedTask;
            }
            stack.Pop();
            return UniTask.CompletedTask;
        }
        private UniTask GreaterThan()
        {
            if (stack.Count < 2)
            {
                Debug.LogError("Error: Insufficient operands for > operation");
                return UniTask.CompletedTask;
            }
            string operand2 = stack.Pop();
            string operand1 = stack.Pop();

            bool result = CompareOperands(operand1, operand2, (a, b) => a > b);
            stack.Push(result ? "1" : "0");
            return UniTask.CompletedTask;
        }

        private UniTask LessThan()
        {
            if (stack.Count < 2)
            {
                Debug.LogError("Error: Insufficient operands for < operation");
                return UniTask.CompletedTask;
            }
            string operand2 = stack.Pop();
            string operand1 = stack.Pop();

            bool result = CompareOperands(operand1, operand2, (a, b) => a < b);
            stack.Push(result ? "1" : "0");
            return UniTask.CompletedTask;
        }

        private UniTask GreaterThanOrEqual()
        {
            if (stack.Count < 2)
            {
                Debug.LogError("Error: Insufficient operands for >= operation");
                return UniTask.CompletedTask;
            }
            string operand2 = stack.Pop();
            string operand1 = stack.Pop();

            bool result = CompareOperands(operand1, operand2, (a, b) => a >= b);
            stack.Push(result ? "1" : "0");
            return UniTask.CompletedTask;
        }

        private UniTask LessThanOrEqual()
        {
            if (stack.Count < 2)
            {
                Debug.LogError("Error: Insufficient operands for <= operation");
                return UniTask.CompletedTask;
            }
            string operand2 = stack.Pop();
            string operand1 = stack.Pop();

            bool result = CompareOperands(operand1, operand2, (a, b) => a <= b);
            stack.Push(result ? "1" : "0");
            return UniTask.CompletedTask;
        }

        private UniTask Equal()
        {
            if (stack.Count < 2)
            {
                Debug.LogError("Error: Insufficient operands for == operation");
                return UniTask.CompletedTask;
            }
            string operand2 = stack.Pop();
            string operand1 = stack.Pop();

            bool result = CompareOperands(operand1, operand2, (a, b) => a == b);
            stack.Push(result ? "1" : "0");
            return UniTask.CompletedTask;
        }

        private UniTask NotEqual()
        {
            if (stack.Count < 2)
            {
                Debug.LogError("Error: Insufficient operands for != operation");
                return UniTask.CompletedTask;
            }
            string operand2 = stack.Pop();
            string operand1 = stack.Pop();

            bool result = CompareOperands(operand1, operand2, (a, b) => a != b);
            stack.Push(result ? "1" : "0");
            return UniTask.CompletedTask;
        }


        private bool CompareOperands(string operand1, string operand2, Func<float, float, bool> comparer)
        {
            if (float.TryParse(operand1, out float num1) && float.TryParse(operand2, out float num2))
            {
                return comparer(num1, num2);
            }
            else
            {
                Debug.LogError("Error: Invalid operands for comparison");
                return false;
            }
        }
    }
}
