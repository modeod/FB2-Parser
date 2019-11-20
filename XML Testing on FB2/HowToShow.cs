using System;
using System.IO;

namespace XML_Testing_on_FB2
{
    delegate void howToShow(string s);

    class HowToShow
    {
        public static void showInConsole(string txt)
        {
            Console.WriteLine(txt);
        }

        public static void writeInTxtBody(string txt)
        {
            File.AppendAllText(@"../../XML/BookMainText.txt", txt);
        }

        public static void writeInTxtDescription(string txt)
        {
            File.AppendAllText(@"../../XML/BookDescription.txt", txt);
        }
    }

    abstract class ParsersDecorator { }
}
