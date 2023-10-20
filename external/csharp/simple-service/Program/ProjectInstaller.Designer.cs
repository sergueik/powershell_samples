namespace WindowsService.NET {
    partial class ProjectInstaller {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent() {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;

            this.serviceInstaller1.ServiceName = "WindowsService.NET";
            this.serviceInstaller1.DisplayName = "WindowsService.NET";
            this.serviceInstaller1.Description = "A sample Windows Service boilerplate written in C# using the .NET Framework and Visual Studio 2019";

            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
                this.serviceProcessInstaller1,
                this.serviceInstaller1});
        }

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}