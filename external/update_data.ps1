#Copyright (c) 2022 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.


$namespace = 'Utils'
$writer_class = 'JSONWriter'
if (-not ("${namespace}.${writer_class}" -as [type])) { 


# origin: https://github.com/zanders3/json

Add-Type -IgnoreWarnings @'
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

// origin: https://github.com/zanders3/json
namespace Utils {

    public static class JSONWriter {

	// extension method - will not work when called from Powershell
        public static string ToJsonExtensionMethod(this object item) {
            // Console.Error.WriteLine("called extension method with {0}", item.ToString());
            StringBuilder stringBuilder = new StringBuilder();
            AppendValue(stringBuilder, item);
            return stringBuilder.ToString();
        }

	// normal static method
        public static string ToJson(object item) {
            // Console.Error.WriteLine("called with {0}", item.ToString());
           string result = item.ToJsonExtensionMethod();
           // Console.Error.WriteLine("return {0}", result);
            return  result;
        }

        static void AppendValue(StringBuilder stringBuilder, object item) {
            if (item == null) {
                stringBuilder.Append("null");
                return;
            }

            Type type = item.GetType();
            if (type == typeof(string) || type == typeof(char)) {
                stringBuilder.Append('"');
                string str = item.ToString();
                for (int i = 0; i < str.Length; ++i)
                    if (str[i] < ' ' || str[i] == '"' || str[i] == '\\') {
                        stringBuilder.Append('\\');
                        int j = "\"\\\n\r\t\b\f".IndexOf(str[i]);
                        if (j >= 0)
                            stringBuilder.Append("\"\\nrtbf"[j]);
                        else
                            stringBuilder.AppendFormat("u{0:X4}", (UInt32)str[i]);
                    } else
                        stringBuilder.Append(str[i]);
                stringBuilder.Append('"');
            } else if (type == typeof(byte) || type == typeof(sbyte)) {
                stringBuilder.Append(item.ToString());
            } else if (type == typeof(short) || type == typeof(ushort)) {
                stringBuilder.Append(item.ToString());
            } else if (type == typeof(int) || type == typeof(uint)) {
                stringBuilder.Append(item.ToString());
            } else if (type == typeof(long) || type == typeof(ulong)) {
                stringBuilder.Append(item.ToString());
            } else if (type == typeof(float)) {
                stringBuilder.Append(((float)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            } else if (type == typeof(double)) {
                stringBuilder.Append(((double)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            } else if (type == typeof(decimal)) {
                stringBuilder.Append(((decimal)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            } else if (type == typeof(bool)) {
                stringBuilder.Append(((bool)item) ? "true" : "false");
            } else if (type == typeof(DateTime)) {
                stringBuilder.Append('"');
                stringBuilder.Append(((DateTime)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
                stringBuilder.Append('"');
            } else if (type.IsEnum) {
                stringBuilder.Append('"');
                stringBuilder.Append(item.ToString());
                stringBuilder.Append('"');
            } else if (item is IList) {
                stringBuilder.Append('[');
                bool isFirst = true;
                IList list = item as IList;
                for (int i = 0; i < list.Count; i++) {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    AppendValue(stringBuilder, list[i]);
                }
                stringBuilder.Append(']');
            } else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                Type keyType = type.GetGenericArguments()[0];

                //Refuse to output dictionary keys that aren't of type string
                if (keyType != typeof(string)) {
                    stringBuilder.Append("{}");
                    return;
                }

                stringBuilder.Append('{');
                IDictionary dict = item as IDictionary;
                bool isFirst = true;
                foreach (object key in dict.Keys) {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    stringBuilder.Append('\"');
                    stringBuilder.Append((string)key);
                    stringBuilder.Append("\":");
                    AppendValue(stringBuilder, dict[key]);
                }
                stringBuilder.Append('}');
            } else {
                stringBuilder.Append('{');

                bool isFirst = true;
                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                for (int i = 0; i < fieldInfos.Length; i++) {
                    if (fieldInfos[i].IsDefined(typeof(IgnoreDataMemberAttribute), true))
                        continue;

                    object value = fieldInfos[i].GetValue(item);
                    if (value != null) {
                        if (isFirst)
                            isFirst = false;
                        else
                            stringBuilder.Append(',');
                        stringBuilder.Append('\"');
                        stringBuilder.Append(GetMemberName(fieldInfos[i]));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value);
                    }
                }
                PropertyInfo[] propertyInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                for (int i = 0; i < propertyInfo.Length; i++) {
                    if (!propertyInfo[i].CanRead || propertyInfo[i].IsDefined(typeof(IgnoreDataMemberAttribute), true))
                        continue;

                    object value = propertyInfo[i].GetValue(item, null);
                    if (value != null) {
                        if (isFirst)
                            isFirst = false;
                        else
                            stringBuilder.Append(',');
                        stringBuilder.Append('\"');
                        stringBuilder.Append(GetMemberName(propertyInfo[i]));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value);
                    }
                }

                stringBuilder.Append('}');
            }
        }

        static string GetMemberName(MemberInfo member) {
            if (member.IsDefined(typeof(DataMemberAttribute), true)) {
                DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                    return dataMemberAttribute.Name;
            }

            return member.Name;
        }
    }
}

'@  -ReferencedAssemblies 'System.dll','System.Xml.dll','System.Data.dll','System.Core.dll','System.Runtime.Serialization.dll'

}

$parser_class = 'JSONParser'
if (-not ("${namespace}.${class}" -as [type])) { 

Add-Type -IgnoreWarnings @'
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
// origin: https://github.com/zanders3/json
namespace Utils {
    public static class JSONParser {
        [ThreadStatic] static Stack<List<string>> splitArrayPool;
        [ThreadStatic] static StringBuilder stringBuilder;
        [ThreadStatic] static Dictionary<Type, Dictionary<string, FieldInfo>> fieldInfoCache;
        [ThreadStatic] static Dictionary<Type, Dictionary<string, PropertyInfo>> propertyInfoCache;
	// regular static method
	public static Object FromJson(string json){
		return json.FromJson<object>();
	}
	// extension method
        public static T FromJson<T>(this string json) {
            // Initialize, if needed, the ThreadStatic variables
            if (propertyInfoCache == null) propertyInfoCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            if (fieldInfoCache == null) fieldInfoCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
            if (stringBuilder == null) stringBuilder = new StringBuilder();
            if (splitArrayPool == null) splitArrayPool = new Stack<List<string>>();

            //Remove all whitespace not within strings to make parsing simpler
            stringBuilder.Length = 0;
            for (int i = 0; i < json.Length; i++) {
                char c = json[i];
                if (c == '"') {
                    i = AppendUntilStringEnd(true, i, json);
                    continue;
                }
                if (char.IsWhiteSpace(c))
                    continue;

                stringBuilder.Append(c);
            }

            //Parse the thing!
            return (T)ParseValue(typeof(T), stringBuilder.ToString());
        }

        static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json) {
            stringBuilder.Append(json[startIdx]);
            for (int i = startIdx + 1; i < json.Length; i++) {
                if (json[i] == '\\') {
                    if (appendEscapeCharacter)
                        stringBuilder.Append(json[i]);
                    stringBuilder.Append(json[i + 1]);
                    i++;//Skip next character as it is escaped
                } else if (json[i] == '"') {
                    stringBuilder.Append(json[i]);
                    return i;
                } else
                    stringBuilder.Append(json[i]);
            }
            return json.Length - 1;
        }

        //Splits { <value>:<value>, <value>:<value> } and [ <value>, <value> ] into a list of <value> strings
        static List<string> Split(string json) {
            List<string> splitArray = splitArrayPool.Count > 0 ? splitArrayPool.Pop() : new List<string>();
            splitArray.Clear();
            if (json.Length == 2)
                return splitArray;
            int parseDepth = 0;
            stringBuilder.Length = 0;
            for (int i = 1; i < json.Length - 1; i++) {
                switch (json[i]) {
                    case '[':
                    case '{':
                        parseDepth++;
                        break;
                    case ']':
                    case '}':
                        parseDepth--;
                        break;
                    case '"':
                        i = AppendUntilStringEnd(true, i, json);
                        continue;
                    case ',':
                    case ':':
                        if (parseDepth == 0) {
                            splitArray.Add(stringBuilder.ToString());
                            stringBuilder.Length = 0;
                            continue;
                        }
                        break;
                }

                stringBuilder.Append(json[i]);
            }

            splitArray.Add(stringBuilder.ToString());

            return splitArray;
        }

        internal static object ParseValue(Type type, string json) {
            if (type == typeof(string)) {
                if (json.Length <= 2)
                    return string.Empty;
                StringBuilder parseStringBuilder = new StringBuilder(json.Length);
                for (int i = 1; i < json.Length - 1; ++i) {
                    if (json[i] == '\\' && i + 1 < json.Length - 1) {
                        int j = "\"\\nrtbf/".IndexOf(json[i + 1]);
                        if (j >= 0) {
                            parseStringBuilder.Append("\"\\\n\r\t\b\f/"[j]);
                            ++i;
                            continue;
                        }
                        if (json[i + 1] == 'u' && i + 5 < json.Length - 1) {
                            UInt32 c = 0;
                            if (UInt32.TryParse(json.Substring(i + 2, 4), System.Globalization.NumberStyles.AllowHexSpecifier, null, out c)) {
                                parseStringBuilder.Append((char)c);
                                i += 5;
                                continue;
                            }
                        }
                    }
                    parseStringBuilder.Append(json[i]);
                }
                return parseStringBuilder.ToString();
            }
            if (type.IsPrimitive) {
                var result = Convert.ChangeType(json, type, System.Globalization.CultureInfo.InvariantCulture);
                return result;
            }
            if (type == typeof(decimal)) {
                decimal result;
                decimal.TryParse(json, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result);
                return result;
            }
            if (type == typeof(DateTime)) {
                DateTime result;
                DateTime.TryParse(json.Replace("\"",""), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result);
                return result;
            }
            if (json == "null") {
                return null;
            }
            if (type.IsEnum) {
                if (json[0] == '"')
                    json = json.Substring(1, json.Length - 2);
                try {
                    return Enum.Parse(type, json, false);
                } catch {
                    return 0;
                }
            }
            if (type.IsArray) {
                Type arrayType = type.GetElementType();
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                List<string> elems = Split(json);
                Array newArray = Array.CreateInstance(arrayType, elems.Count);
                for (int i = 0; i < elems.Count; i++)
                    newArray.SetValue(ParseValue(arrayType, elems[i]), i);
                splitArrayPool.Push(elems);
                return newArray;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
                Type listType = type.GetGenericArguments()[0];
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                List<string> elems = Split(json);
                var list = (IList)type.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { elems.Count });
                for (int i = 0; i < elems.Count; i++)
                    list.Add(ParseValue(listType, elems[i]));
                splitArrayPool.Push(elems);
                return list;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                Type keyType, valueType;
                {
                    Type[] args = type.GetGenericArguments();
                    keyType = args[0];
                    valueType = args[1];
                }

                //Refuse to parse dictionary keys that aren't of type string
                if (keyType != typeof(string))
                    return null;
                //Must be a valid dictionary element
                if (json[0] != '{' || json[json.Length - 1] != '}')
                    return null;
                //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
                List<string> elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;

                var dictionary = (IDictionary)type.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { elems.Count / 2 });
                for (int i = 0; i < elems.Count; i += 2) {
                    if (elems[i].Length <= 2)
                        continue;
                    string keyValue = elems[i].Substring(1, elems[i].Length - 2);
                    object val = ParseValue(valueType, elems[i + 1]);
                    dictionary[keyValue] = val;
                }
                return dictionary;
            }
            if (type == typeof(object)) {
                return ParseAnonymousValue(json);
            }
            if (json[0] == '{' && json[json.Length - 1] == '}') {
                return ParseObject(type, json);
            }

            return null;
        }

        static object ParseAnonymousValue(string json) {
            if (json.Length == 0)
                return null;
            if (json[0] == '{' && json[json.Length - 1] == '}') {
                List<string> elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;
                var dict = new Dictionary<string, object>(elems.Count / 2);
                for (int i = 0; i < elems.Count; i += 2)
                    dict[elems[i].Substring(1, elems[i].Length - 2)] = ParseAnonymousValue(elems[i + 1]);
                return dict;
            }
            if (json[0] == '[' && json[json.Length - 1] == ']') {
                List<string> items = Split(json);
                var finalList = new List<object>(items.Count);
                for (int i = 0; i < items.Count; i++)
                    finalList.Add(ParseAnonymousValue(items[i]));
                return finalList;
            }
            if (json[0] == '"' && json[json.Length - 1] == '"') {
                string str = json.Substring(1, json.Length - 2);
                return str.Replace("\\", string.Empty);
            }
            if (char.IsDigit(json[0]) || json[0] == '-') {
                if (json.Contains(".")) {
                    double result;
                    double.TryParse(json, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result);
                    return result;
                } else {
                    int result;
                    int.TryParse(json, out result);
                    return result;
                }
            }
            if (json == "true")
                return true;
            if (json == "false")
                return false;
            // handles json == "null" as well as invalid JSON
            return null;
        }

        static Dictionary<string, T> CreateMemberNameDictionary<T>(T[] members) where T : MemberInfo {
            Dictionary<string, T> nameToMember = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < members.Length; i++) {
                T member = members[i];
                if (member.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                    continue;

                string name = member.Name;
                if (member.IsDefined(typeof(DataMemberAttribute), true)) {
                    DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                    if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                        name = dataMemberAttribute.Name;
                }

                nameToMember.Add(name, member);
            }

            return nameToMember;
        }

        static object ParseObject(Type type, string json) {
            object instance = FormatterServices.GetUninitializedObject(type);

            //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
            List<string> elems = Split(json);
            if (elems.Count % 2 != 0)
                return instance;

            Dictionary<string, FieldInfo> nameToField;
            Dictionary<string, PropertyInfo> nameToProperty;
            if (!fieldInfoCache.TryGetValue(type, out nameToField)) {
                nameToField = CreateMemberNameDictionary(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                fieldInfoCache.Add(type, nameToField);
            }
            if (!propertyInfoCache.TryGetValue(type, out nameToProperty)) {
                nameToProperty = CreateMemberNameDictionary(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                propertyInfoCache.Add(type, nameToProperty);
            }

            for (int i = 0; i < elems.Count; i += 2) {
                if (elems[i].Length <= 2)
                    continue;
                string key = elems[i].Substring(1, elems[i].Length - 2);
                string value = elems[i + 1];

                FieldInfo fieldInfo;
                PropertyInfo propertyInfo;
                if (nameToField.TryGetValue(key, out fieldInfo))
                    fieldInfo.SetValue(instance, ParseValue(fieldInfo.FieldType, value));
                else if (nameToProperty.TryGetValue(key, out propertyInfo))
                    propertyInfo.SetValue(instance, ParseValue(propertyInfo.PropertyType, value), null);
            }

            return instance;
        }
    }
}


'@  -ReferencedAssemblies 'System.dll','System.Xml.dll','System.Data.dll','System.Core.dll','System.Runtime.Serialization.dll'
}

function save_json {
param(
  [System.Collections.Hashtable] $data = @{},
  [String]$datafile = ''
  [String]$json_data = ''
)

  [System.Collections.Generic.Dictionary[String,Object]]$input_data = New-Object System.Collections.Generic.Dictionary'[String,Object]'
  if ($datafile -ne ''){
    $json_data = (get-content -path (resolve-path -path $datafile)) -join ''
  }
  if ($json_data -ne '') 
    $file_data = [Utils.JSONParser]::FromJson($json_data)
    $file_data.keys| foreach-object {
      $key = $_
      # NOTE: [System.Collections.Hashtable] does not contain a method named 'get'.
      # $input_data.Add($key, $data.get($key))
      $input_data.Add($key, $file_data[$key])
    }
  }
  $data.keys| foreach-object {
    $key = $_
    if ($input_data.ContainsKey($key)) {
      $input_data.Remove($key)
    }
    $input_data.Add($key, $data[$key])
  }
  try {
    $result = [Utils.JSONWriter]::ToJson($input_data)
    write-host $result
  } catch [Exception] { 
    # Parameter count mismatch
    $result = $null
    $e = $_[0].Exception
    write-host ('Exception:{0}{2}Message: {1}' -f $e.getType(), $e.Message,[char]10 )

  }
  return $result
}




function updateData{
  param(
    [System.Collections.Hashtable]$y = @{},
    [String] $data
  )

  [System.Collections.Hashtable]$x = @{}
  [System.Collections.Generic.Dictionary[String,Object]]$input_data = New-Object System.Collections.Generic.Dictionary'[String,Object]'
  $json_data = @'
{
  "CPU": 123,
  "HOSTNAME": "host",
  "MEMORY": "1 2 3"
}
'@

  $x = [Utils.JSONParser]::FromJson($json_data)

  if ($y -eq $null -or $y.Keys.Count -eq 0 ) {
    write-host $json_data
    return
  }

    # https://www.tutorialspoint.com/how-to-add-merge-two-hash-tables-in-powershell
  # https://stackoverflow.com/questions/8800375/merging-hashtables-in-powershell-how/32890418
  # cannot use simple $x + $y
  $y.keys | foreach-object {
    $k = $_
    $v = $y[$k]
    $x[$k] = $v
  }
  $result = save_json -data $x
}

write-output 'test 1: no arg'

&updateData

write-output 'test 2: new entry'
$entry = @{'new' = 'data'}
updateData -y $entry

write-output 'test 3: existing entry'
$entry = @{'HOSTNAME' = 'otherhost'}
updateData -y $entry

write-output 'test 4: multiple keys entry'

$entry = @{'HOSTNAME' = 'otherhost'; 'MORE'=  '1 2 3 4'}
updateData -y $entry
