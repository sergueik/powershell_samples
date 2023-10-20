// Author - Anshu Dutta
// Contact - anshu.dutta@gmail.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company
{
    public class Employee
    {
        private string _firstName;
        private string _lastName;
        private int _empCode;
        private string _designation;

        public Employee()
        { }
        /// <summary>
        /// Property First Name
        /// </summary>
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        /// <summary>
        /// Property Last Name
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        /// <summary>
        /// Property Employee Code
        /// </summary>
        public int EmpCode
        {
            get { return _empCode; }
            set { _empCode = value; }
        }
        /// <summary>
        /// Property Designation
        /// </summary>
        public string Designation
        {
            get { return _designation; }
            set {_designation = value;}
        }
        /// <summary>
        /// Method - Returns Employee Full Name
        /// </summary>
        /// <returns></returns>
        public string getEmployeeName()
        {
            string fullName = FirstName + ' ' + LastName;
            return fullName;
        }

    }
}
