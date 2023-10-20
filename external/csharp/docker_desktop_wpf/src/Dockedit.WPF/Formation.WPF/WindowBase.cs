using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Formation.WPF
{
    public  class WindowBase : Window
    {
        public WindowBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowBase), new FrameworkPropertyMetadata(typeof(WindowBase)));
        }

        //Relay Commands
        public RelayCommand MinimizeWindowCommand
        {
            get 
            {
                return new RelayCommand(param => OnMinimizeWindow());
            }
        }
        public RelayCommand MaximizeWindowCommand
        {
            get
            {
                return new RelayCommand(param => OnMaximizeWindow());
            }
        }
        public RelayCommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(param => OnCloseWindow());
            }
        }
        public RelayCommand AboutWindowCommand
        {
            get
            {
                return new RelayCommand(param => OnAbout());
            }
        }
        public RelayCommand PreferencesWindowCommand
        {
            get
            {
                return new RelayCommand(param => OnPreferences());
            }
        }

        private void OnMinimizeWindow()
        {
            SystemCommands.MinimizeWindow(this);
        }
        private void OnMaximizeWindow()
        {
            if ((WindowState == WindowState.Normal))
            {
                SystemCommands.MaximizeWindow(this);
            }
        }
        private void OnCloseWindow()
        {
            SystemCommands.CloseWindow(this);
        }
        private void OnAbout()
        {
            MessageBox.Show("A propos .....");
        }
        private void OnPreferences()
        {
            MessageBox.Show("Preferences ....");
        }

        //Dependency Properties
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register( "Header", typeof(String),typeof(WindowBase));
        public String Header
        {
            get { return (String)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
