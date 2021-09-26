using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

namespace NPlatform
{
    /// <summary>
    /// ������������---��̬������ѧ���ʽ��������ֵ
    /// ���ʽʹ�� C# �﷨���ɴ�һ�����Ա���(x)��
    /// ���ʽ���Ա�����ֵ��Ϊ(double)���͡�
    /// </summary>
    /// <example>
    /// <code>
    /// Expression expression = new Expression("Math.Sin(x)"); 
    /// Console.WriteLine(expression.Compute(Math.PI / 2)); 
    /// expression = new Expression("double u = Math.PI - x;" + 
    /// "double pi2 = Math.PI * Math.PI;" + 
    /// "return 3 * x * x + Math.Log(u * u) / pi2 / pi2 + 1;"); 
    /// Console.WriteLine(expression.Compute(0)); 
    /// 
    /// Expression expression = new Expression("return 10*(5+5)/10;");
    /// Response.Write(expression.Compute(0));
    /// Response.End();
    /// </code>
    /// </example>
    public class MathExpression
    {
        object instance;
        MethodInfo method;
        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="expression">�����</param>
        public MathExpression(string expression)
        {
            if (expression.IndexOf("return") < 0) expression = "return " + expression + ";";
            string className = "Expression";
            string methodName = "Compute";
            CompilerParameters p = new CompilerParameters();
            p.GenerateInMemory = true;
            CompilerResults cr = new CSharpCodeProvider().CompileAssemblyFromSource(p, string.
              Format("using System;sealed class {0}{{public double {1}(double x){{{2}}}}}",
              className, methodName, expression));
            if (cr.Errors.Count > 0) {
                string msg = "Expression(\"" + expression + "\"): \n";
                foreach (CompilerError err in cr.Errors) msg += err.ToString() + "\n";
                throw new Exception(msg);
            }
            instance = cr.CompiledAssembly.CreateInstance(className);
            method = instance.GetType().GetMethod(methodName);
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="x"></param>
        /// <returns>���ؼ���ֵ</returns>
        public double Compute(double x)
        {
            return (double)method.Invoke(instance, new object[] { x });
        }
    }
}
