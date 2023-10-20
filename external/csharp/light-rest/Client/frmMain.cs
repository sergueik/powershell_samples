using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using LightCaseClient;
using ShareLibraries;
using System.Net;

namespace Client
{
	public partial class frmMain : Form
	{
		private List<Student> students;
		private string serviceUrl;

		public frmMain()
		{
			InitializeComponent();

			// Get the service url
			serviceUrl = ConfigurationManager.AppSettings["ServiceUrl"];

			// Initialize a list of students
			students = new List<Student>();
			for (var i = 1; i <= 10; i++) {
				var student = new Student()
                { Fact = "Fact" + i.ToString() };

				students.Add(student);
			}
		}

		private void btnSynchronousCall_Click(object sender, EventArgs e)
		{
			dgStudents.DataSource = null;
			dgStudents.Refresh();

			// Make a synchronous POST Rest service call
			try {
				// added for debugging only	- unfinished
				HttpWebRequest httpWebRequest = GenericProxies.CreateRequest(serviceUrl, GenericProxies.DefaultConfiguration);
				httpWebRequest.Accept = "application/json";
				httpWebRequest.Method = "GET";

				var evaluatedStudent = GenericProxies.ReceiveData<Root>(httpWebRequest, GenericProxies.DefaultConfiguration);
				/*
			
				evaluatedStudent = GenericProxies
                    .RestPost<List<Student>, List<Student>>(serviceUrl, students);
                    */
				var students = new List<Node>();
				students.AddRange(evaluatedStudent.value.nodes);
				// need List<Node>  
				dgStudents.DataSource = students;
				dgStudents.Refresh();
			} catch (Exception ex) {
				MessageBox.Show("Failed to call the service - " + ex.Message);
			}
		}

		private void btnSynchronousGetCall_Click(object sender, EventArgs e)
		{
			dgStudents.DataSource = null;
			dgStudents.Refresh();

			// Make a synchronous GET Rest service call
			try {
				HttpWebRequest httpWebRequest = GenericProxies.CreateRequest(serviceUrl, GenericProxies.DefaultConfiguration);
				httpWebRequest.Accept = "application/json";
				httpWebRequest.Method = "GET";

				var data = GenericProxies.ReceiveData<Root>(httpWebRequest, GenericProxies.DefaultConfiguration);
				// extract List<Node>  
				var nodes = new List<Node>();
				nodes.AddRange(data.value.nodes);
				dgStudents.DataSource = nodes;
				dgStudents.Refresh();
			} catch (Exception ex) {
				MessageBox.Show("Failed to call the service - " + ex.Message);
			}
		}
		private void btnSynchronousPostCall_Click(object sender, EventArgs e)
		{
			dgStudents.DataSource = null;
			dgStudents.Refresh();

			// Make a synchronous POST Rest service call. Will return the empty List<Student>  if the JSON is not 
			try {
			
				var data = GenericProxies
                    .RestPost<List<Student>, List<Student>>(serviceUrl, students);
				dgStudents.DataSource = data;
				dgStudents.Refresh();
			} catch (Exception ex) {
				MessageBox.Show("Failed to call the service - " + ex.Message);
			}
		}

		private void btnAsynchronousCall_Click(object sender, EventArgs e)
		{
			dgStudents.DataSource = null;
			dgStudents.Refresh();

			// Make an asynchronous POST Rest service call
			GenericProxies.RestPostAsync<List<Student>, List<Student>>(serviceUrl, students,
				(ex, evaluatedStudent) => {
					if (ex != null)
						MessageBox.Show("Failed to call the service - " + ex.Message);
					else
						dgStudents.Invoke((Action)delegate {
							dgStudents.DataSource = evaluatedStudent;
							dgStudents.Refresh();
						});
				}
			);
		}


		private void btnClickAnyway_Click(object sender, EventArgs e)
		{
			MessageBox.Show("I am clicked!");
		}
	}
}
