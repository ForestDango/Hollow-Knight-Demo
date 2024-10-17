using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ArrayForEnumAttribute : PropertyAttribute
{
    public readonly Type EnumType;
    public readonly int EnumLength;

    public bool IsValid
    {
	get
	{
	    return EnumType != null && EnumType.IsEnum && EnumLength != 0;
	}
    }

    public ArrayForEnumAttribute(Type enumType)
    {
	EnumType = enumType;
	if (enumType != null && enumType.IsEnum)
	{
	    int num = 0;
	    Array values = Enum.GetValues(enumType);
	    for (int i = 0; i < values.Length; i++)
	    {
		int num2 = (int)values.GetValue(i);
		if (num2 >= 0 && num < num2)
		{
		    num = num2 + 1;
		}
	    }
	    EnumLength = num;
	    return;
	}
	EnumLength = 0;
    }

    public static void EnsureArraySize<T>(ref T[] array, Type enumType)
    {
	int num = Enum.GetNames(enumType).Length;
	if (array != null)
	{
	    if (array.Length != num)
	    {
		T[] array2 = array;
		array = new T[num];
		for (int i = 0; i < Mathf.Min(num, array2.Length); i++)
		{
		    array[i] = array2[i];
		}
		return;
	    }
	}
	else
	{
	    array = new T[num];
	}
    }
}
