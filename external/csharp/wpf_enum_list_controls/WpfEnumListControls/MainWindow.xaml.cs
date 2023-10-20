using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EnumListCtrls;
using System.Runtime.CompilerServices;

namespace WpfEnumListControls
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Notifies that the property changed, and sets IsModified to true.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		// The type or namespace name 'CallerMemberName' could not be found (CS0246)
		// The type or namespace name 'CallerMemberNameAttribute' could not be found (CS0246)
		// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callermembernameattribute?view=net-5.0&viewFallbackFrom=netframework-4.0
		// Allows you to obtain the method or property name of the caller to the method.		
        // protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        // does not recognize the notation
        protected void NotifyPropertyChanged( String propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

		#endregion INotifyPropertyChanged

		public EnumItemList Enum1 { get; set; }
		public EnumItemList Enum2 { get; set; }

		private string flagResult;
		public string FlagResult { get { return this.flagResult; } set { this.flagResult = value; this.NotifyPropertyChanged(); } }

		public MainWindow()
		{
			this.InitializeComponent();
			this.DataContext = this;
			this.CreateLocalEnumLists();
		}

		public void CreateLocalEnumLists()
		{
			this.Enum1 = new EnumItemList(typeof(EnumTest1), true);
			this.Enum2 = new EnumItemList(typeof(EnumFlagTest), true);
		}

		private void lbLocalEnum2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// this will work whether the listbox is multi-select or not
			EnumListBox lb = sender as EnumListBox;
			int flags = 0;
			foreach(object item in lb.SelectedItems)
			{
				flags |= (int)(((EnumItem)item).Value);
			}
			this.FlagResult = flags.ToString();
		}
	}
}
