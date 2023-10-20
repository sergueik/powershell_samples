using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace EnumListCtrls
{
	public class EnumItem
	{
		public object Value               { get; set; }
		public string Name                { get; set; }
		public Type   EnumType            { get; set; }
		public Type   UnderlyingType      { get; set; }
		public bool   ShowOrdinalWithName { get; set; }

		public EnumItem()
		{
			this.ShowOrdinalWithName = false;
		}

		public override string ToString()
		{
			return (this.ShowOrdinalWithName) ? string.Format("({0}) {1}", Convert.ChangeType(this.Value, this.UnderlyingType), Name)
											  : this.Name;
		}
	}

	public class EnumItemList : ObservableCollection<EnumItem>
	{
		public bool CanMultiSelect { get; set; }

		public EnumItemList(Type enumType, bool showOrd)
		{
			// this.CanMultiSelect = enumType.GetCustomAttributes<FlagsAttribute>().Any();
			// The non-generic method 'System.Reflection.MemberInfo.GetCustomAttributes(bool)' cannot be used with type arguments (CS0308)
			// this.CanMultiSelect = (FlagsAttribute) enumType.GetCustomAttributes().Any();
			// type cast expression of incompatible type
			// this.CanMultiSelect = enumType.GetCustomAttributes().Any();
			// No overload for method 'GetCustomAttributes' takes 0 arguments (CS1501)
			this.CanMultiSelect = enumType.GetCustomAttributes(typeof(System.FlagsAttribute), false).Any();
			this.AsObservableEnum(enumType, showOrd);
		}

		public void AsObservableEnum(Type enumType, bool showOrd)
		{
			// if the specified type is not null AND it is actually an enum type, 
			// we can create the collection
			if (enumType != null && enumType.IsEnum)
			{
				// discover the underlying type (int, long, byte, etc)
				Type underlyingType = Enum.GetUnderlyingType(enumType);
				// get each enum item and add it to the list
				foreach (Enum item in enumType.GetEnumValues())
				{
					this.Add(new EnumItem()
					{ 
						// the name that will probably be displayed in the UI component
						Name           = item.ToString(), 
						// the actual enum value (DayofWeek.Monday)
						Value          = item, 
						// the enum type
						EnumType       = enumType,
						// the underlying type (int, long, byte, etc)
						UnderlyingType = underlyingType,
						ShowOrdinalWithName = showOrd,
					});
				}
			}
		}
	}
	
}
