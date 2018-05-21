using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace RpgPdfParser
{
    public struct Differance
    {
        public string Field;
        public string LeftValue;
        public string RightValue;
    }

    public static class InstanceCompare
    {
        public static List<Differance> Compare(object leftInstance, object rightInstance)
        {
            var leftType = leftInstance.GetType();
            var rightType = leftInstance.GetType();

            if (leftType != rightType)
                throw new ArgumentException("Can't compare two instance of different types");

            var result = new List<Differance>();

            foreach (var property in leftType.GetProperties())
            {
                var leftValue = property.GetValue(leftInstance, null);
                var rightValue = property.GetValue(rightInstance, null);
                if (leftValue != rightValue)
                    result.Add(new Differance()
                    {
                        Field = property.Name,
                        LeftValue = leftValue.ToString(),
                        RightValue = rightValue.ToString()
                    });
            }
            
            foreach (var field in leftType.GetFields())
            {
                var leftValue = field.GetValue(leftInstance);
                var rightValue = field.GetValue(rightInstance);
                if (leftValue != rightValue)
                    result.Add(new Differance()
                    {
                        Field = field.Name,
                        LeftValue = leftValue.ToString(),
                        RightValue = rightValue.ToString()
                    });
            }

            foreach (var method in leftType.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public))
            {
                if (method.GetGenericArguments().Any())
                    continue;

                try
                {
                    var leftValue = method.Invoke(leftInstance, null);
                    var rightValue = method.Invoke(rightInstance, null);
                    if (leftValue != rightValue)
                        result.Add(new Differance()
                        {
                            Field = method.Name,
                            LeftValue = leftValue.ToString(),
                            RightValue = rightValue.ToString()
                        });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return result;
        }
    }
}