using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;


namespace XML_Testing_on_FB2
{
    class FB2ParserFormater : ParserFormater
    {
        public override string formatDescription(ParsersDecorator vars)
        {
            string toReturn = "";

            FB2DecoratorHead headInfo = (FB2DecoratorHead)vars;

            FB2DecoratorHead.DecoratorTitleInfo titleInfoVars = headInfo.titleVars;
            FB2DecoratorHead.DecoratorDocumentInfo docInfoVars = headInfo.documentVars;
            FB2DecoratorHead.DecoratorPublishInfo pubInfoVars = headInfo.publishVars;


            toReturn += writeTitleInfo(titleInfoVars.author, titleInfoVars.trans, 
                titleInfoVars.bookName, titleInfoVars.lang, titleInfoVars.dateValue,
                titleInfoVars.dateInner, titleInfoVars.description, titleInfoVars.posterId);

            toReturn += writeDocInfo(docInfoVars.author, docInfoVars.progUsed,
                docInfoVars.date, docInfoVars.srcU, docInfoVars.srcO, docInfoVars.id, docInfoVars.version);

            toReturn += writePubInfo(pubInfoVars.bookname, pubInfoVars.publisher, pubInfoVars.city, pubInfoVars.year);

            return toReturn;

            // Локальные методы, которые используются вэтой функции, но не надо где-либо еще
            string writeTitleInfo(
                string au, string trans,
                string bookName, string lang, string dateValue,
                string dateInner, string s, string posterID
                )
            {
                string writeTxt =
                    "-------------BOOK-INFO-------------\n" +
                    $"==== Book name - {bookName}\n";

                if (au != null && au != "" && au != " ")
                    writeTxt += au + "\n";

                if (trans != null && trans != "" && trans != " ")
                    writeTxt += trans + "\n";

                if (lang != null && lang != "" && lang != " ")
                    writeTxt += $"==== Language - {lang}\n";

                if (
                    (dateValue != null && dateValue != "" && dateValue != " ")
                    &&
                    (dateInner != null && dateInner != "" && dateInner != " ")
                   )
                {
                    writeTxt += $"==== Date: {dateValue} ({dateInner})\n";
                }
                else if (dateValue != null && dateValue != "" && dateValue != " ")
                {
                    writeTxt += $"==== Date: {dateValue}\n";
                }
                else if (dateInner != null && dateInner != "" && dateInner != " ")
                {
                    writeTxt += $"==== Date: {dateInner}\n";
                }

                if (s != null && s != "" && s != " ")
                {
                    writeTxt +=
                        "==== Description: =================\n" +
                        s +
                        "===================================\n";

                }

                return writeTxt;
                //func("==== Description: =================");
                //func(s);
                //func("===================================");
                //func("-----------------------------------");
            }

            string writeDocInfo(
                string au, string progUsed,
                string date, string srcU, string srcO, 
                string id, string version
                )
            {
                string txt = "-----------DOCUMENT-INFO-----------\n";
                if (au != null && au != "" && au != " ")
                    txt += au + "\n";
                if (progUsed != null && progUsed != "" && progUsed != " ")
                    txt += $"==== Program: {progUsed}\n";
                if (date != null && date != "" && date != " ")
                    txt += $"==== Date: {date}\n";
                if (srcU != null && srcU != "" && srcU != " ") { }

                if (srcO != null && srcO != "" && srcO != " ")
                    txt += $"==== Editor: {srcO}\n";
                if (id != null && id != "" && id != " ")
                    txt += $"==== Book ID: {id}\n";
                if (version != null && version != "" && version != " ")
                    txt += $"==== Book version: {version}\n";

                return txt;
            }

            string writePubInfo(string bn, string pub, string city, string year)
            {
                string txt = "------------PUBLISH-INFO-----------\n";
                if (bn != null && bn != "" && bn != " ")
                    txt += $"==== Paper book name: {bn}\n";
                if (pub != null && pub != "" && pub != " ")
                    txt += $"==== Publisher: {pub}\n";
                if (city != null && city != "" && city != " ")
                    txt += $"==== City: {city}\n";
                if (year != null && year != "" && year != " ")
                    txt += $"==== Year: {year}\n";

                return txt;
            }

        }

        public override string formatBody(ParsersDecorator vars)
        {
            string toReturn = "";

            FB2DecoratorBody bodDec = (FB2DecoratorBody)vars;

            toReturn += findTags(bodDec.bodyNode.OuterXml);

            return toReturn;

            
        }

        // Костыль в виде статика для checkAllP в FB2ParserCore
        // Я в рот ебал переписывать ту функцию, пошло оно нахуц
        public static string findTags(string outer) 
        {
            char charr = '"';
            string txtToReturn = null;
            // [A-Za-z0-9,^*: {charr}-=@]*
            Regex regexStart = new Regex($"<p xmlns=\"[A-Za-z0-9,^* :{charr}-=@]*\">");

            //TODO: надо как-то этот регекс оптимизировать, но пока я не придумал как ._.

            Regex regexStartCenAll = new Regex($"<p [A-Za-z0-9,^* :{charr}-=@]* style=\"text-align: center\" [A-Za-z0-9,^*:{charr}-=@]*>");
            Regex regexStartCenRight = new Regex($"<p style=\"text-align: center\" [A-Za-z0-9,^*: {charr}-=@]*>");
            Regex regexStartCenLeft = new Regex($"<p [A-Za-z0-9,^* :{charr}-=@]* style=\"text-align: center\">");

            Regex regexStartLeftAll = new Regex($"<p [A-Za-z0-9,^* :{charr}-=@]* style=\"text-align: left\" [A-Za-z0-9,^*:{charr}-=@]*>");
            Regex regexStartLeftRight = new Regex($"<p style=\"text-align: left\" [A-Za-z0-9,^*: {charr}-=@]*>");
            Regex regexStartLeftLeft = new Regex($"<p [A-Za-z0-9,^* :{charr}-=@]* style=\"text-align: left\">");

            Regex regexStartRightAll = new Regex($"<p [A-Za-z0-9,^* :{charr}-=@]* style=\"text-align: right\" [A-Za-z0-9,^*:{charr}-=@]*>");
            Regex regexStartRightRight = new Regex($"<p style=\"text-align: right\" [A-Za-z0-9,^*: {charr}-=@]*>");
            Regex regexStartRightLeft = new Regex($"<p [A-Za-z0-9,^* :{charr}-=@]* style=\"text-align: right\">");

            Regex regexStartCenNone = new Regex($"<p style=\"text-align: center\">");
            Regex regexStartLeftNone = new Regex($"<p style=\"text-align: left\">");
            Regex regexStartRightNone = new Regex($"<p style=\"text-align: right\">");


            //Регексом искать теги с параметрами (как в реге выше) 
            //Ебанина только начинается!

            txtToReturn = regexStart.Replace(outer, "\t");

            txtToReturn = regexStartLeftAll.Replace(txtToReturn, "Центровка слева,строка по центру\n\n\n\n\n\n\n\n\n");
            txtToReturn = regexStartLeftLeft.Replace(txtToReturn, "Центровка слева,строка слева \n\n\n\n\n\n\n\n");
            txtToReturn = regexStartLeftRight.Replace(txtToReturn, "Центровка слева,строка справа \n\n\n\n\n\n\n\n\n");

            txtToReturn = regexStartCenAll.Replace(txtToReturn, "Центровка по центру,строка по центру\n\n\n\n\n\n\n\n");
            txtToReturn = regexStartCenLeft.Replace(txtToReturn, "Центровка по центру,строка слева\n\n\n\n\n\n\n\n");
            txtToReturn = regexStartCenRight.Replace(txtToReturn, "Центровка по центру,строка справа\n\n\n\n\n\n\n\n");


            txtToReturn = regexStartRightAll.Replace(txtToReturn, "Центровка справа,строка по центру \n\n\n\n\n\n\n\n");
            txtToReturn = regexStartRightRight.Replace(txtToReturn, "Центровка справа,строка справа \n\n\n\n\n\n\n\n");
            txtToReturn = regexStartRightLeft.Replace(txtToReturn, "Центровка справа,строка слева \n\n\n\n\n\n\n\n\n");

            txtToReturn = regexStartCenNone.Replace(txtToReturn, "Центровка по центру,строка единственная\n\n\n\n\n\n\n\n");
            txtToReturn = regexStartLeftNone.Replace(txtToReturn, "Центровка слева,строка единственная\n\n\n\n\n\n\n\n ");
            txtToReturn = regexStartRightAll.Replace(txtToReturn, "Центровка справа,строка единственная\n\n\n\n\n\n\n\n ");

            //Тут строки
            txtToReturn = txtToReturn.Replace("<i>", "<i> ");
            txtToReturn = txtToReturn.Replace("</i>", " </i>");
            txtToReturn = txtToReturn.Replace("<p>", "\t");
            txtToReturn = txtToReturn.Replace("</p>", "\n");
            txtToReturn = txtToReturn.Replace("<strong>", "<strong> HUIIIIIIIII "); // -STRONG ={
            txtToReturn = txtToReturn.Replace("</strong>", " IUUUUHHHHHH </strong>"); // }=

            return txtToReturn;
        }
    }
}
