using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace WindowsFormsApplication2
{
    class FileRtns
    {
        StreamReader sr;
        /// <summary>
        /// Open the file for reading. 
        /// </summary>
        /// <returns></returns>
        public int FileOpen()
        {
            int iret = 0;
            try
            {
                // Open the file and read it back.
                sr = System.IO.File.OpenText("DBIns.sql");
            }
            catch
            {
                iret = -1;  // error opps. 
            }
            return (iret);
        }
        /// <summary>
        /// Read the frist line from the file.
        /// We will read until the first semicolon since that ends the sql statement. 
        /// </summary>
        /// <returns>a string that contains the sql statement or empty</returns>
        public string FileRead()
        {
            string  strResult = "";
            string  strBuffer = "";

            while ((sr.EndOfStream)!= true) {
                strBuffer = sr.ReadLine();
                strBuffer = strBuffer.Trim();
                if (strBuffer.EndsWith(";"))
                {
                    strResult += strBuffer;
                    break;
                }
                strResult += strBuffer;
            }
            return (strResult);
        }
        /// <summary>
        /// Close the file. 
        /// </summary>
        /// <returns>o if success or -1 if error.</returns>
        public int FileClose()
        {
            int iret = 0;
            try
            {
                // Open the file and read it back.
                sr.Close();
            }
            catch
            {
                iret = -1;
            }
            return (iret);
        }
    }
}
