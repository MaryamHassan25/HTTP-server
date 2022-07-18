using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{ // If error occured logger handle this error and threw exception to log.txt with details(Date,Message)
    class Logger

    {
        // TODO: Create log file named log.txt to log exception details in it
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            //Datetime:
            sr.WriteLine("DateTime: " + DateTime.Now);
            //message:
            sr.WriteLine("Message: " + ex.Message);
            sr.Close();
        }
    }
}