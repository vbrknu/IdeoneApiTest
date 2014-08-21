/*
 * ideone.com
 * API sample
 * 
 * This program shows how to use ideone api.
 * 
 * How to run it?
 *  1. Create C# Windows Console Application project in the Visual Studio;
 *  2. Include Program.cs and Ideone_Service.cs files to the project
 *      (you can generate the stub - Ideone_ServiceService.cs - by yourself 
 *      using wsdl.exe tool from Microsoft SDK);
 *  3. Add System.Web.Services reference to the project (right click on the
 *      project name in the Solution Explorer -> click Add Reference ...);
 *  4. Run the project.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace csharp_test
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceCode = @"use strict;use warnings;print ""No"";";
            sourceCode = sourceCode.Replace("@", System.Environment.NewLine);
            Ideone_Service client = new Ideone_Service();   // instantiating the stub
            Dictionary<string, string> result = new Dictionary<string, string>();
            
            // Creating a submission for the sourceCode
            Object[] ret = client.createSubmission("ptusk", "password", sourceCode, 3, "4", true, false);

            Stopwatch timer = new Stopwatch();

            result = GetXmlData(ret);

            // checking if everything went OK. If it did the program could be in various stages of the compilation process
            if (result["error"] == "OK")
            {
                Object[] status = client.getSubmissionStatus("ptusk", "password", result["link"]);
                Dictionary<string, string> status_result = new Dictionary<string, string>();
                Dictionary<string, string> submission_result = new Dictionary<string, string>();
                status_result = GetXmlData(status);
 
                if (status_result["error"] == "OK")
                {
                    int current_status = Int32.Parse(status_result["status"]);
                    timer.Start();
                    
                    //when status is lower than 0 it means that the compilation is not ready, yet
                    while (current_status != 0)
                    {
                        System.Threading.Thread.Sleep(5000);
                        Object[] statuss = client.getSubmissionStatus("ptusk", "password", result["link"]);
                        Dictionary<string, string> statuss_result = new Dictionary<string, string>();
                        statuss_result = GetXmlData(statuss);
                        current_status = Int32.Parse(statuss_result["status"]);
                        
                        // The program is compiled
                        if (current_status == 0)
                        {
                            Object[] objj = client.getSubmissionDetails("ptusk", "password", result["link"], true, true, true, true, true);                            
                            submission_result = GetXmlData(objj);
                            timer.Stop();
                            break;
                        }
                    }
                    
                    foreach (KeyValuePair<string, string> kk in submission_result)
                    {
                        Console.WriteLine(kk.Key + " : " + kk.Value);
                    }

                    Console.WriteLine("Time elapsed: {0}", timer.Elapsed);
                }
                
            }
            else
            {
                Console.WriteLine("Error occured: " + result["error"]);
            }

            Console.Read(); 
        }

        static Dictionary<String, String> GetXmlData(Object[] obj)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (object o in obj)
            {
                if (o is XmlElement)
                {
                    XmlNodeList x = ((XmlElement)o).ChildNodes;
                    result.Add(x.Item(0).InnerText, x.Item(1).InnerText);
                }
            }
            return (Dictionary<String,String>)result;
        }
    }
}
