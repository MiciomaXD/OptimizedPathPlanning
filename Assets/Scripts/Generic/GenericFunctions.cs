using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class GenericFunctions
{
    public static int GetHashOfClass(Type myType)
    {
        int hash = 0;
        foreach (var f in myType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (f.MemberType == MemberTypes.Field ||
                (f.MemberType & MemberTypes.Property) != 0)
            {
                hash ^= CreateHash(f.Name);
            }

            if (f is FieldInfo fi)
            {
                hash ^= CreateHash(fi.FieldType.Name);
            }
        }

        return hash;
    }

    private static int CreateHash(string name)
    {
        throw new NotImplementedException();
        /*using (HashAlgorithm algorithm = SHA256.Create())
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(name));*/
    }
}
