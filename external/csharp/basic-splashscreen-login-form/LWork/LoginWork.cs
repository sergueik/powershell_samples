#region File Description
//LoginWork.cs
//Helper for login do
//Copyright 2010 Vladimir Knyakzov E-mail: vladisvit@nvsoftpro.com
//Copyright (C) NVSoftPro. All rights reserved. site: www.nvsoftpro.com
#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Authentication;
using System.Xml.Serialization;
using System.IO;

#endregion

namespace LWork
{

  /// <summary>
  /// The LoginWork class
  /// Here doing hashing, saving, loading the user data
  /// doing login user
  /// </summary>
  public static class LoginWork
  {

    #region Fields & Properties

    //this field should set in settings application
    private static string _FileName = @"UserData.xml";

    //the login flag (true if the user is logged)
    private static bool _Logged;
    public static bool Logged
    {
      get { return _Logged; }
    }

    // this list store the all registered users
    private static List<UserData> _UserDataList = new List<UserData>();

    // the user data who is logged
    private static UserData _user = new UserData();

    public static UserData User
    {
      get { return _user; }
    }


    #endregion

    #region Private Procedures

    /// <summary>
    /// Check exist this nikname in the users list
    /// </summary>
    /// <param name="NikName">Nikname of the user</param>
    /// <returns></returns>
    private static bool IsExistNikName( string NikName )
    {

      if (_UserDataList.Count == 0)
      {
        return false;
      }
      else
      {
        // here I use a lambda expression for the searching the nikname in the user data list
        _user = _UserDataList.FirstOrDefault(UserData => UserData.Nikname == NikName);

        if (String.IsNullOrEmpty(_user.Nikname))
        {
          return false;
        }
      }

      return true;

    }

    /// <summary>
    /// Load  from the file registered users data in the list
    /// </summary>
    /// <returns></returns>
    private static List<UserData> Load()
    {

      List<UserData> returnUserData = new List<UserData>();

      if (File.Exists(_FileName))
      {
        using (Stream fileStream = File.OpenRead(_FileName))
        {
          XmlSerializer serializer = new XmlSerializer(typeof(List<UserData>));
          returnUserData = (List<UserData>)serializer.Deserialize(fileStream);
        }
      }

      return returnUserData;
    }

    #endregion

    #region Public Procedures
    /// <summary>
    /// The hash function
    /// returns the hash as a 32-character, hexadecimal-formatted string
    /// for more information see ms-help
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string HashString( string s )
    {
      //encode string to the array of bytes
      byte[] data = Encoding.Default.GetBytes(s);

      //hashing
      MD5 md5 = new MD5CryptoServiceProvider();
      byte[] result = md5.ComputeHash(data);

      //do hexadecimal-formatted string
      StringBuilder sb = new StringBuilder();
      foreach (byte item in result)
      {
        sb.Append(item.ToString("X"));
      }

      return sb.ToString();

    }

    public static void Initialization()
    {
      _UserDataList = Load();
    }

    /// <summary>
    /// Check the nikname and the password in the users list
    /// </summary>
    /// <param name="NikName"></param>
    /// <param name="Password"></param>
    public static void DoLogin( string NikName, string Password )
    {

      if (IsExistNikName(NikName))
      {
        if (_user.Password == HashString(Password))
        {
          _Logged = true;
        }
      }
      else
        _Logged = false;

    }

    /// <summary>
    /// Saving the new user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static bool Save( UserData user )
    {

      if (!IsExistNikName(user.Nikname))
      {

        _UserDataList.Add(user);

        using (Stream fileStream = File.Create(_FileName))
        {
          XmlSerializer serializer = new XmlSerializer(typeof(List<UserData>));
          serializer.Serialize(fileStream, _UserDataList);
        }

        return true;
      }

      return false;

    }

    #endregion

  }
}
