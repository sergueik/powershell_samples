using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

// origin: https://github.com/zanders3/json
namespace Utils {

	public static class JSONWriter {
		public static string ToJsonExtensionMethod(this object item) {
			var stringBuilder = new StringBuilder();
			AppendValue(stringBuilder, item);
			return stringBuilder.ToString();
		}
		// normal static method
		public static string ToJson(object item) {
			var stringBuilder = new StringBuilder();
			AppendValue(stringBuilder, item);
			return stringBuilder.ToString();
		}

		private static Dictionary<K,V> HashtableToDictionary<K,V>(Hashtable table){
			return table.Cast<DictionaryEntry>().ToDictionary(kvp => (K)kvp.Key, kvp => (V)kvp.Value);
		}

    	// for Powershell caller
		public static string ToJson(Hashtable newdata) {
			return ToJson(HashtableToDictionary<string,object>(newdata));
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
				var list = item as IList;
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
				var dict = item as IDictionary;
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
					// for item of Hashtable type, the second argument should not be null - leading to
					// System.Reflection.TargetParameterCountException
					// same exception when removing the argument from the invocation
					//
					// replacing the null argument with new object[1] {null}
					// leads to System.ArgumentNullException : Key cannot be null
					// https://stackoverflow.com/questions/26382810/how-do-you-get-the-value-of-a-property-from-propertyinfo
					// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo.getvalue?view=netframework-4.5
					// https://csharp.hotexamples.com/examples/System.Reflection/PropertyInfo/GetValue/php-propertyinfo-getvalue-method-examples.html
					// object value = propertyInfo[i].GetValue(item, new object[1] {null});
					//
					// object value = propertyInfo[i].GetValue(item);
					// 
					//
					// NOTE: this is issue  https://github.com/zanders3/json/issues/35
					// resolved effectively as won't fix by upstream project author in Apr 16 2020
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

