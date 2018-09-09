using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;

namespace PrintReloader1
{
    class Program
    {
        static void Main(string[] args)
            //Alright so here's how this program works:
            //Step 1. Stop Print Spooler service
            //Step 2. Stop "printfilterpipelinesvc.exe" task
            //Step 3. Delete contents of C:\Windows\System32\spool\PRINTERS
            //Step 4. Start the Print Spooler service back up
            //Note that this program has to be run as administrator or it won't work.
        {
            bool noErrors = true;//Let the program know if it encountered errors. If it encountered errors, it will tell you what went wrong before closing
                                 // Check whether the Spooler service is started and if it is, stop it.
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (isElevated == false) {
                Console.WriteLine("It has been detected that this program is not being run as administrator. It is unlikely that this will work.");
                Console.WriteLine("For best results, please run this as administrator.");
            }

            ServiceController sc = new ServiceController();
            sc.ServiceName = "Spooler";
            Console.WriteLine("The Spooler service status is currently set to {0}",
                               sc.Status.ToString());

            if (sc.Status == ServiceControllerStatus.Running)
            {
                // Stop the Spooler service if it's running.
                Console.WriteLine("Stopping the Spooler service");
                try
                {
                    // Start the service, and wait until its status is "Running".
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);

                    // Display the current service status.
                    Console.WriteLine("The Spooler service status is now stopped.",
                                       sc.Status.ToString());
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Could not stop the Spooler service.");
                    noErrors = false;
                }
            }

            //This below is the code having to do with stopping the "printerfilterpipelinesvc.exe" executable.
            //The print spooler service has to be killed before you do this. Note how the code for stopping it is above.
            Console.WriteLine("Stopping printfilterpipelinesvc.exe");
            Process.Start("taskkill", "/F /IM iexplore.exe");

            System.IO.DirectoryInfo di = new DirectoryInfo("C:\\Windows\\System32\\spool\\PRINTERS");
            Console.WriteLine("Deleting all the contents of C:\\Windows\\System32\\spool\\PRINTERS");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                // Start the service if the current status is stopped.
                Console.WriteLine("Starting the Spooler service...");
                try
                {
                    // Start the service, and wait until its status is "Running".
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);

                    // Display the current service status.
                    Console.WriteLine("The Spooler service status is now set to {0}.",
                                       sc.Status.ToString());
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Could not start the Spooler service.");
                    noErrors = false;
                }
            }


            if (noErrors == false) {
                Console.WriteLine("There were errors btw");
            }
            Console.WriteLine("Press ENTER to close this Window.");
            Console.ReadLine();
        }
    }
}
