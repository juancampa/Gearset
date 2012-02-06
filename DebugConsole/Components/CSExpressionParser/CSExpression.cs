using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gearset;
using System.Collections.Specialized;
using System.Collections;
using System.Reflection;

namespace Gearset.Components.CSExpressionParser
{
    internal class CSExpression<T>
    {
        private delegate T ExpressionDelegate(Object[] parameters);

        private ListDictionary parameters;

        /// <summary>
        /// Delegate to evaluate expression without using MethodInfo.Invoke.
        /// </summary>
        private ExpressionDelegate ExpressionMethod;

        /// <summary>
        /// Evaluates the expression and return its value.
        /// </summary>
        public T Value { get { return Evaluate(); } }


        /// <summary>
        /// A c# expression that can be evaluated and return a value of type T.
        /// </summary>
        public String Expression { get { return expression; } set { OnExpressionChanged(value); } }
        private String expression = String.Empty;

        /// <summary>
        /// Construct a new CSExpression that returns default(T).
        /// Add parameters the this expression before changing the
        /// Expression, otherwise it will be invalid.
        /// </summary>
        public CSExpression()
        {
            parameters = new ListDictionary();
        }

        private void OnExpressionChanged(String expression)
        {
            MethodInfo method = null;
            try
            {
                String unfriendlyExpression = expression;
                foreach (var key in parameters.Keys)
                {
                    String name = key as String;
                    if (name != null)
                    {
                        // Regular expression that matches the name of the parameter and won't
                        // match if that parameters is a substring of something else. It will match
                        // the preceeding and succeeding character as well.
                        String regexp = String.Format(@"(?<![\.a-zA-Z0-9_]){0}(?![a-zA-Z0-9_])", name);

                        // Replace string that puts the preceeding and suceeding characters matched
                        // and changes the name of the variable to a reference in the paramter dict.
                        String replaceWith = String.Format("(({0})((IDictionary)p[0])[\"{1}\"])", parameters[key].GetType().FullName, key);

                        // Replace the parameter.
                        unfriendlyExpression = Regex.Replace(unfriendlyExpression, regexp, replaceWith);
                    }
                }
                method = ReflectionHelper.CompileCSharpMethod(unfriendlyExpression, typeof(T));
            }
            catch
            {
                GearsetResources.Console.Log("Gearset", "Error while compiling CSExpression.");
                GearsetResources.Console.Log("Gearset", "Invalid Expression: {0}", expression);
            }

            if (method != null)
            {
                ExpressionMethod = (ExpressionDelegate)Delegate.CreateDelegate(typeof(ExpressionDelegate), method);
                this.expression = expression;
            }
        }

        /// <summary>
        /// Evaluates the expression if any, else return de default <c>T</c>.
        /// </summary>
        /// <returns></returns>
        private T Evaluate()
        {
            if (ExpressionMethod != null)
                return ExpressionMethod(new Object[]{parameters});
            else
                return default(T);
        }

        /// <summary>
        /// Sets a parameter that will be used in the expression.
        /// </summary>
        public void SetParameter(String name, Object value)
        {
            if (name.Contains(" "))
            {
                throw new InvalidOperationException("The name of the parameter cannot contain whitespaces");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value", "The value of the parameter cannot be null");
            }
            if (!parameters.Contains(name))
            {
                parameters.Add(name, value);
            }
            else
            {
                parameters[name] = value;
            }
            
        }
    }
}
