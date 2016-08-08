using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;
using TransformationTimelineTool.Helpers;

namespace SendSubscriberNotificationConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            ChangeNotify job = new ChangeNotify();
            job.ManualExecute();
            namesTest();

        }

        private static void namesTest() {
            TimelineContext db = new TimelineContext();
            Console.WriteLine("Welcome to the Subscriber notification console app!");

            List<Initiative> initiatives = db.Initiatives.ToList();
            foreach( var init in initiatives)
            {
                Console.WriteLine("Name => {0}", init.NameE);
            }
/*
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
*/
        }
    }
}
