using System;
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
            //TODO: Сделать систему с делегатами более эластичной (чтобы можно было добавлять и удалять делегаты
            // Или переделать делегаты на события

            FB2ParserCore fb2Parser = new FB2ParserCore(@"../../XML/Resurrected Demon King[1,2].txt");
            fb2Parser.realiseDescription(HowToShow.showInConsole);
            fb2Parser.realiseBody(HowToShow.showInConsole);
            Console.WriteLine("done");

            Console.ReadKey();


        }
    }
}
