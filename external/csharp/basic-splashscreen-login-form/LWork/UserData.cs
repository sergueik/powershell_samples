#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

#endregion

namespace LWork
{
  /// <summary>
  /// This structure need for store the user login data
  /// we use the serialization for store data
  /// </summary>
  [Serializable]
  public struct UserData
  {
    private string _Name;

    public string Name
    {
      get { return _Name; }
      set { _Name = value; }
    }

    private string _Nikname;

    public string Nikname
    {
      get { return _Nikname; }
      set { _Nikname = value; }
    }

    private string _Password;

    public string Password
    {
      get { return _Password; }
      set { _Password = value; }
    }

  }
}
