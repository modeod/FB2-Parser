﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace XML_Testing_on_FB2
{
    class Program
    {
        static void Main(string[] args)
        {

            DescriptionNode description = new DescriptionNode();
            description.showNode(@"../../XML/Resurrected Demon King[1,2].txt", HelperFB2.showDescriptionInTxt);
            BodyFormatingProgram body = new BodyFormatingProgram();
            body.showNode(@"../../XML/Resurrected Demon King[1,2].txt", HelperFB2.showBodyInTxt);


            Console.WriteLine("done");

            Console.ReadKey();


        }
    }
}
