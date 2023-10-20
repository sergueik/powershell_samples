using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace EnumListCtrls
{
	/// <summary>
	/// Represents a combo box that specifically supports enumerators
	/// </summary>
	public class EnumComboBox : ComboBox
	{
		public EnumItemList EnumList { get; set; }

		public Type EnumType
		{
			get 
			{ 
				Type value = (string.IsNullOrEmpty(this.EnumTypeName)) ? null : Type.GetType(this.EnumTypeName);  
				return value;
			}
		}

		public static DependencyProperty EnumTypeNameProperty = 
			DependencyProperty.Register("EnumTypeName", typeof(string), typeof(EnumComboBox), new PropertyMetadata(null));
		public string EnumTypeName
		{
			get { return (string)GetValue(EnumTypeNameProperty); }
			set { SetValue(EnumTypeNameProperty, value); }
		}

		public static DependencyProperty AutoSelectionModeProperty = 
			DependencyProperty.Register("AutoSelectionMode", typeof(bool), typeof(EnumComboBox), new PropertyMetadata(true));
		public bool AutoSelectionMode
		{
			get { return (bool)GetValue(AutoSelectionModeProperty); }
			set { SetValue(AutoSelectionModeProperty, value); }
		}

		public static DependencyProperty ShowOrdinalWithNameProperty = 
			DependencyProperty.Register("ShowOrdinalWithName", typeof(bool), typeof(EnumComboBox), new PropertyMetadata(false));
		public bool ShowOrdinalWithName
		{
			get { return (bool)GetValue(ShowOrdinalWithNameProperty); }
			set { SetValue(ShowOrdinalWithNameProperty, value); }
		}

		public EnumComboBox():base()
		{
			this.DataContext = this;
			this.Loaded += this.EnumComboBox_Loaded;
		}

		private void EnumComboBox_Loaded(object sender, RoutedEventArgs e)
		{
			//// create the list of enums
			//this.EnumList      = new EnumItemList(this.EnumType, this.ShowOrdinalWithName);

			//// create and set the binding
			//Binding binding    = new Binding() {Source=this.EnumList };
			//this.SetBinding(ListBox.ItemsSourceProperty, binding);

			// avoid errors being displayed in designer
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				// if the enum type is not null, the enum must be a system enum, so we can 
				// populate/bind automatically
				if (this.EnumType != null)
				{
					// create the list of enums
					this.EnumList      = new EnumItemList(this.EnumType, this.ShowOrdinalWithName);

					// create and set the binding
					Binding binding    = new Binding() {Source=this.EnumList };
					this.SetBinding(ComboBox.ItemsSourceProperty, binding);
				}
				else
				{
					// otherwise, the developer specifically set the binding, so we have to get the 
					// datacontext from the parent content control (window or usercontrol) so we can 
					// use the specified collection
					this.DataContext = EnumGlobal.FindParent<ContentControl>(this).DataContext;
					// before we use it, make sure it's the correct type (it must be a EnumItemList object)
					if (!(this.ItemsSource is EnumItemList))
					{
						throw new InvalidCastException("The bound collection must be of type EnumItemList.");
					}
				}
			}
		}

	}
}
