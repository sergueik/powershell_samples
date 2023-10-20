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
	/// Represents a list box that specifically supports enumerators
	/// </summary>
	public class EnumListBox : ListBox
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
			DependencyProperty.Register("EnumTypeName", typeof(string), typeof(EnumListBox), new PropertyMetadata(null));
		public string EnumTypeName
		{
			get { return (string)GetValue(EnumTypeNameProperty); }
			set { SetValue(EnumTypeNameProperty, value); }
		}

		public static DependencyProperty AutoSelectionModeProperty = 
			DependencyProperty.Register("AutoSelectionMode", typeof(bool), typeof(EnumListBox), new PropertyMetadata(true));
		public bool AutoSelectionMode
		{
			get { return (bool)GetValue(AutoSelectionModeProperty); }
			set { SetValue(AutoSelectionModeProperty, value); }
		}

		public static DependencyProperty ShowOrdinalWithNameProperty = 
			DependencyProperty.Register("ShowOrdinalWithName", typeof(bool), typeof(EnumListBox), new PropertyMetadata(false));
		public bool ShowOrdinalWithName
		{
			get { return (bool)GetValue(ShowOrdinalWithNameProperty); }
			set { SetValue(ShowOrdinalWithNameProperty, value); }
		}

		public EnumListBox():base()
		{
			this.DataContext = this;
			this.Loaded += this.EnumListBox_Loaded;
		}

		private void EnumListBox_Loaded(object sender, RoutedEventArgs e)
		{
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
					Binding binding    = new Binding() { Source=this.EnumList };
					this.SetBinding(ListBox.ItemsSourceProperty, binding);
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
				// no matter what happens, see if we can set the list to mult5iple selection
				if (this.ItemsSource != null)
				{
					if (this.AutoSelectionMode)
					{
						this.SelectionMode = (((EnumItemList)(this.ItemsSource)).CanMultiSelect) ? SelectionMode.Multiple : SelectionMode.Single;
					}
				}
			}
		}

	}

}
