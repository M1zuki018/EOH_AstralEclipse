using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class RegexValidationAttribute : PropertyAttribute
{
    public string Pattern;
    public string ErrorMessage;

    public RegexValidationAttribute(string pattern, string errorMessage = "Invalid input")
    {
        Pattern = pattern;
        ErrorMessage = errorMessage;
    }
}
