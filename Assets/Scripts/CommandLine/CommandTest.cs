using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
public delegate bool ParameterParser(string args, out object value);
namespace CommandLine.Core
{
    public class CommandCore : MonoBehaviour
    {

        [Command("pow")]
        static void Add(int a, int b)
        {
            Debug.Log(Mathf.Pow(a, b));
        }
        void Start()
        {

            CommandSystem.SetOutputFunc(UnityEngine.Debug.Log);
            CommandSystem.SetOutputErrFunc(UnityEngine.Debug.Log);
            CommandSystem.Execute("pow 2 3");
            CommandSystem.Execute("pow 2 test");
        }
    }
    #region 命令属性 用来标记一个方法是命令
    public class CommandAttribute : Attribute
    {

        public string Name { get; private set; }
        public string Desc { get; set; }

        public CommandAttribute(string name)
        {

            this.Name = name;
            this.Desc = "command has no description";
        }
        public CommandAttribute()
        {

            this.Name = null;
            this.Desc = "command has no description";
        }
    }
    public class CommandParameterParserAttribute : Attribute
    {

        public readonly Type type;
        public CommandParameterParserAttribute(Type type)
        {
            this.type = type;
        }
    }
    #endregion
    public static class CommandSystem
    {
        static readonly Dictionary<string, Command> commands = new(); //存储命令的字典
 
        static readonly CommandParser parser = new(); //命令解析器
                                                      //这里的两个函数指针使得输出函数更加灵活
        static Action<string> output;
        static Action<string> outputErr;
        static void Output(string message) => output?.Invoke(message); //输出正常信息 
        static void OutputErr(string message) => outputErr?.Invoke(message); //输出错误信息

        public static IEnumerable<Command> TotalCommands
        {
            get 
            {
                return commands.Values;
            }
        }
        static CommandSystem() //在构造函数中添加已有的所有命令
        {

            foreach (var command in CommandCreator.CollectCommands<CommandAttribute>()) //获取含有CommandAttribute的所有方法
            {
                if (commands.ContainsKey(command.name))
                {
                    commands[command.name] = command;
                    continue;
                }
                commands.Add(command.name, command);
            }
        }
        /// <summary>execute given command</summary>
        /// <param name="input">the command you want to execute</param>
        /// <returns>if success</returns>
        public static bool Execute(string input)
        {

            if (!parser.Parse(input, out string[] result))
            {
                OutputErr($"invalid command: {input} ");
                return false;
            }
            string commandName = result[0]; //第一个part是命令名
            if (!commands.TryGetValue(commandName, out Command command))
            {
                OutputErr($"unknown command name: {commandName}");
                return false;
            }
            Exception e;
            if (result.Length == 1) //如果只有命令没有参数
            {
                e = command.Execute(new string[] { });
            }
            else
            {
                string[] args = new string[result.Length - 1]; //part作为参数传递给args
                for (int i = 1; i < result.Length; i++) //从序号1开始 因为0是命令名
                {
                    args[i - 1] = result[i];
                }
                e = command.Execute(args);
            }
            //如果 e为null 则表示执行成功
            if (e != null)
            {
                OutputErr($"command error : {e.Message}");
                OutputErr(e.StackTrace);
                return false;
            }
            return true;
        }
        /// <summary>execute given command</summary>
        /// <param name="input">the command you want to execute</param>
        /// <returns>if success</returns>
        public static bool ExecuteSilence(string input) //不输出任何信息的Excute函数
        {

            if (!parser.Parse(input, out string[] result)) return false;
            if (!commands.TryGetValue(result[0], out Command command)) return false;
            Exception e;
            if (result.Length == 1)
            {
                e = command.Execute(new string[] { });
            }
            else
            {
                string[] args = new string[result.Length - 1];
                for (int i = 1; i < result.Length; i++)
                {
                    args[i - 1] = result[i];
                }
                e = command.Execute(args);
            }
            return e == null;
        }
        /// <summary>you can set a output function of CommandSystem
        /// to receive normal message </summary>
        public static void SetOutputFunc(Action<string> func) => output = func;

        /// <summary>you can set a output function of CommandSystem
        /// to receive error message </summary>
        public static void SetOutputErrFunc(Action<string> func) => outputErr = func;

    }
    static class CommandCreator
    {
        /// <summary>create a command from a method</summary>
        /// <param name="methodInfo">method info defined <b>CommandAttribute</b></param>
        /// <param name="attr">the target command attribute</param>
        /// <param name="command">result command</param>
        /// <returns>return true if success</returns>
        static bool CreateCommand(MethodInfo methodInfo, CommandAttribute attr, out Command command) //封装创建Command的逻辑
        {
            /* check if target type has been supported by invoker */
            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters.Length > 0)
            {
                foreach (ParameterInfo parameter in parameters)
                {
                    if (CommandParameterHandle.ContainsParser(parameter.ParameterType)) continue;
                    command = default; //如果有一个参数类型不支持就返回false
                    return false;
                }
            }
            /* command name should be the one defined in attribute, if client not set a name
               then would use method name as default */
            string commandName = attr.Name ?? methodInfo.Name;
            command = new Command(commandName, attr.Desc, methodInfo);
            return true;
        }
        /// <summary>collect all commands from execution position</summary>
        /// <returns>return all commands</returns>
        public static IEnumerable<Command> CollectCommands<T>() where T : CommandAttribute
        {

            Type[] totalTypes = Assembly.GetExecutingAssembly().GetTypes(); //获取当前执行的程序集中的所有类型
            foreach (Type type in totalTypes) //遍历每个type
            {

                MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic); //获取type的所有静态方法
                foreach (MethodInfo method in methods)//遍历每个type的method
                {
                    var attr = method.GetCustomAttribute<T>(); //获取方法上的CommandAttribute
                    if (attr == null) continue;
                    if (CreateCommand(method, attr, out Command command))
                    {
                        yield return command;
                    }
                }
            }
        }
        /// <summary>get all parameter parser function from given types</summary>
        static void RegisterParameterParsers(Type[] totalTypes) //注册用户自定义的参数解析器
        {

            foreach (Type type in totalTypes)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (methods.Length == 0) continue;
                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<CommandParameterParserAttribute>();
                    if (attr == null || attr.type == null) continue;
                    if (TryConvertDelegate<ParameterParser>(method, out var pp))
                    {
                        CommandParameterHandle.RegisterParser(attr.type, pp);
                    }
                }
            }
        }
        /// <summary>check if target methodInfo have same signature with given delegate type</summary>
        static bool TryConvertDelegate<T>(MethodInfo methodInfo, out T result) where T : Delegate
        {
            // Check if the return types match
            result = null;
            MethodInfo delegateMethodInfo = typeof(T).GetMethod("Invoke");
            if (methodInfo.ReturnType != delegateMethodInfo.ReturnType) return false;

            // Check if the parameter types match
            var methodParams = methodInfo.GetParameters();
            var delegateParams = delegateMethodInfo.GetParameters();

            if (methodParams.Length != delegateParams.Length) return false;

            for (int i = 0; i < methodParams.Length; i++)
            {
                if (methodParams[i].ParameterType != delegateParams[i].ParameterType)
                    return false;
            }

            result = Delegate.CreateDelegate(typeof(T), null, methodInfo) as T;
            return true;
        }
    }
    public readonly struct Command //用户可以注册解析函数
    {

        public readonly string name;
        public readonly string description;
        public readonly MethodInfo method;
        public readonly ParameterInfo[] parameters;

        public int ParameterCount => parameters.Length;

        public Command(string name, string description, MethodInfo method)
        {

            this.name = name;
            this.description = description;
            this.method = method;
            parameters = method.GetParameters();
        }
        public Exception Execute(string[] args)
        {
            try
            {
                /* parse all parameters from args and load them into an object array */
                object[] loadedParams = new object[parameters.Length];
                for (int i = 0; i < loadedParams.Length; i++)
                {
                    /* check if has default parameter value */
                    if (i >= args.Length)
                    {
                        if (parameters[i].HasDefaultValue) //如果用户没有传入参数但是有默认值
                        {
                            loadedParams[i] = parameters[i].DefaultValue;
                            continue;
                        }
                        throw new Exception($"parameter {parameters[i].Name} is missing");
                    }
                    /* parse parameter from string */
                    if (CommandParameterHandle.ParseParameter(args[i], parameters[i].ParameterType, out loadedParams[i])) continue;
                    throw new Exception($"parameter {parameters[i].Name} is invalid");
                }
                /* call the method */
                method.Invoke(null, loadedParams);
                return null;
            }
            catch (TargetInvocationException ex)
            {
                return ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                return ex.InnerException ?? ex;
            }
            //这个函数把catch到的异常返回给调用者 让调用者来决定异常输出方式
        }
    }
    static class CommandParameterHandle //命令参数解析器 用来解析命令中参数类型并调用对应的解析函数
    {

        static readonly Dictionary<Type, ParameterParser> DefaultParseFunctions;
        static readonly Dictionary<Type, ParameterParser> CustomParseFunctions;
        static CommandParameterHandle()
        {

            DefaultParseFunctions = new Dictionary<Type, ParameterParser>();

            /* register default parameter parsers */
            DefaultParseFunctions.Add(typeof(int), ParseInt);
            DefaultParseFunctions.Add(typeof(string), ParseString);
            DefaultParseFunctions.Add(typeof(float), ParseFloat);
            DefaultParseFunctions.Add(typeof(double), ParseDouble);
            DefaultParseFunctions.Add(typeof(bool), ParseBool);
            DefaultParseFunctions.Add(typeof(char), ParseChar);
            DefaultParseFunctions.Add(typeof(byte), ParseByte);
            DefaultParseFunctions.Add(typeof(short), ParseShort);
            DefaultParseFunctions.Add(typeof(long), ParseLong);
            DefaultParseFunctions.Add(typeof(ushort), ParseUShort);
            DefaultParseFunctions.Add(typeof(uint), ParseUInt);
            DefaultParseFunctions.Add(typeof(ulong), ParseULong);
            DefaultParseFunctions.Add(typeof(decimal), ParseDecimal);
            DefaultParseFunctions.Add(typeof(sbyte), ParseSByte);
        }
        #region 调用系统默认的解析器
        static bool ParseString(string args, out object value)
        {
            value = args ?? string.Empty;
            return true;
        }
        static bool ParseInt(string args, out object value)
        {
            if (int.TryParse(args, out int result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseFloat(string args, out object value)
        {
            if (float.TryParse(args, out float result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseDouble(string args, out object value)
        {
            if (double.TryParse(args, out double result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseBool(string args, out object value)
        {
            if (bool.TryParse(args, out bool result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseChar(string args, out object value)
        {
            if (char.TryParse(args, out char result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseByte(string args, out object value)
        {
            if (byte.TryParse(args, out byte result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseShort(string args, out object value)
        {
            if (short.TryParse(args, out short result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseLong(string args, out object value)
        {
            if (long.TryParse(args, out long result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseUShort(string args, out object value)
        {
            if (ushort.TryParse(args, out ushort result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseUInt(string args, out object value)
        {
            if (uint.TryParse(args, out uint result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseULong(string args, out object value)
        {
            if (ulong.TryParse(args, out ulong result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseDecimal(string args, out object value)
        {
            if (decimal.TryParse(args, out decimal result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        static bool ParseSByte(string args, out object value)
        {
            if (sbyte.TryParse(args, out sbyte result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
        #endregion

        public static bool RegisterParser(Type type, ParameterParser parser)
        {
            if (type == null || parser == null) return false;
            if (DefaultParseFunctions.ContainsKey(type)) return false;
            if (CustomParseFunctions.ContainsKey(type))
            {
                CustomParseFunctions[type] = parser;
            }
            else
            {
                CustomParseFunctions.Add(type, parser);
            }
            return true;
        }
        public static bool ContainsParser(Type type)
        {
            if (type == null) return false;
            return DefaultParseFunctions.ContainsKey(type) || CustomParseFunctions.ContainsKey(type);
        }

        public static bool ParseParameter(string args, Type type, out object value) //解析参数
        {
            value = null;
            if (DefaultParseFunctions.TryGetValue(type, out var parser))
            {
                return parser(args, out value);
            }
            if (!CustomParseFunctions.TryGetValue(type, out parser)) return false; //如果也不是用户自定义类型则返回false
                                                                                   //对用户自定义的解析器进行异常捕获
            try
            {
                return parser(args, out value);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class CommandParser //命令解析器 用来把用户输入的字符串解析分割成独立的part
    {
        const char EOL = '\0';
        const char SPACE = ' ';
        const char DBL_QUOTATION = '"'; //双引号
        const char SGL_QUOTATION = '\''; //单引号

        private string input; //记录输入的字符串
        private int index; //记录当前解析到的位置
        private bool HasMore => index < input.Length; //用来判断是否还有更多字符

        /// <summary>parse command with given input string</summary>
        public bool Parse(string input, out string[] result)
        {
            result = null;
            if (input == null || input.Length == 0) return false;
            /* pad EOL in the end for we known where to leave state machine */
            this.input = input.Trim() + EOL;
            index = 0;
            try
            {
                var list = new List<string>();
                foreach (var part in Walk())
                {
                    list.Add(part); //把解析出来的每一部分放到list里
                }
                result = list.ToArray();
                return list.Count > 0;
            }
            catch (Exception)
            {

                result = null;
                return false;
            }
        }
        IEnumerable<string> Walk() //字符自动机
        {

            while (HasMore)
            {
                char c = input[index++];
                Console.WriteLine(c);
                switch (c)
                {
                    case DBL_QUOTATION:
                        yield return NextString(index, DBL_QUOTATION);
                        if (HasMore && input[index] == SPACE) index++;
                        break;

                    case SGL_QUOTATION:
                        yield return NextString(index, SGL_QUOTATION);
                        if (HasMore && input[index] == SPACE) index++;
                        break;

                    case EOL:
                        break;

                    default:
                        yield return NextId(index - 1);
                        break;
                }
            }
        }
        string NextId(int start, int length = 0)
        {

            while (HasMore)
            {
                char c = input[index++];
                if (char.IsWhiteSpace(c))
                {
                    return input.Substring(start, length + 1);
                }
                if (c == DBL_QUOTATION || c == SGL_QUOTATION)
                {
                    index--; //回退一个字符，因为这个引号是下一个token的开始
                    return input.Substring(start, length + 1);
                }
                length++;
            }
            return input.Substring(start, length);
        }
        string NextString(int start, char quotation, int length = 0)
        {

            while (HasMore)
            {
                char c = input[index++];
                if (c == quotation) return input.Substring(start, length);
                length++;
            }
            throw new Exception("string parser error");
        }
    }
}
