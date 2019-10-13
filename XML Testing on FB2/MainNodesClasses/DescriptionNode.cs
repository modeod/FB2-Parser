using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace XML_Testing_on_FB2
{
    class DescriptionNode : PreNodes
    {
        private interface IDescriptionNodeShowFuncs
        {
            /// <summary>
            /// Функция, в которой будут передаваться обьекты нодов в переменные,
            /// обозначеные вверху класса, и с которых значения внутри нода будут
            /// идти в переменные (текстовые), которые потом станут аргументами для метода, 
            /// принимающего их и выводящего с помощью делегата значения этих текстовых переменных.
            /// </summary>
            /// <param name="descriptionNode"> Экземпляр обьекта XmlNode с нодом description</param>
            /// <param name="nameSpace"> Экземпляр обьекта XmlNamespaceManager </param>
            /// <param name="func"> Функция делегата </param>
            void showNode(XmlNode descriptionNode, XmlNamespaceManager nameSpace, howToShow func);
        }
        static string posterID = null;

        //TODO: вывести в тайтле жанры (необязон)
        //TODO: Переписать этот класс в нормальный вид
        public class TitleInfoNode : IDescriptionNodeShowFuncs
        {
            XmlNode titleInfoNode;
            XmlNodeList authorNodes;
            XmlNodeList translatorNodes;
            XmlNodeList genreNodes; //TODO: realise this node
            XmlNode annotationNode;
            XmlNode dateNode;
            XmlNode coverPageNode;
            XmlNode imageNode;
            XmlNode xlinkAttr;
            XmlNode xlinkHrefAttr;

            public void showNode(
                XmlNode descriptionNode,
                XmlNamespaceManager nameSpace,
                howToShow func)
            {
                titleInfoNode = descriptionNode.SelectSingleNode("fb:title-info", nameSpace);

                 authorNodes = titleInfoNode.SelectNodes("fb:author", nameSpace);
                 translatorNodes = titleInfoNode.SelectNodes("fb:translator", nameSpace);
                 genreNodes = titleInfoNode.SelectNodes("fb:genre", nameSpace);
                //TODO:FictionBook Site get all variants of innerText for annotation
                 annotationNode = titleInfoNode.SelectSingleNode("fb:annotation", nameSpace);
                 dateNode = titleInfoNode.SelectSingleNode("fb:date", nameSpace);

                //Poster
                 coverPageNode = titleInfoNode.SelectSingleNode("fb:coverpage", nameSpace);
                 imageNode = coverPageNode.SelectSingleNode("fb:image", nameSpace);
                 xlinkAttr = imageNode.Attributes.GetNamedItem("xlink");
                 xlinkHrefAttr = imageNode.Attributes.GetNamedItem("xlink:href");
                if (xlinkHrefAttr != null) posterID = xlinkHrefAttr.InnerText;
                else if (xlinkAttr != null) posterID = xlinkAttr.InnerText;
                else posterID = null;

                string s = structureAnnotation(annotationNode, nameSpace); // Here

                string bookName = null;
                try
                {
                    if (titleInfoNode.SelectSingleNode("fb:book-title", nameSpace) != null)
                    { bookName = titleInfoNode.SelectSingleNode("fb:book-title", nameSpace).InnerText; }
                }
                catch { }

                //TODO?: parse keywords 
                string keywords = null;
                try
                {
                    if (titleInfoNode.SelectSingleNode("fb:keyword", nameSpace) != null)
                    { keywords = titleInfoNode.SelectSingleNode("fb:keyword", nameSpace).InnerText; }
                }
                catch { keywords = null; }

                string dateValue = null;
                string dateInner = null;
                try
                {
                    if (dateNode != null)
                    {
                        if (dateNode.Attributes.GetNamedItem("value") != null)
                        {
                            dateValue = dateNode.Attributes.GetNamedItem("value").InnerText;
                        }
                        dateInner = dateNode.InnerText;
                    }
                }
                catch { dateValue = null; dateInner = null; }

                string publisher = null;
                try
                {
                    if (titleInfoNode.SelectSingleNode("fb:publisher", nameSpace) != null)
                    { publisher = titleInfoNode.SelectSingleNode("fb:publisher", nameSpace).InnerText; }
                }
                catch { publisher = null; }

                string lang = null;
                try
                {
                    if (titleInfoNode.SelectSingleNode("fb:lang", nameSpace) != null)
                    {
                        lang = titleInfoNode.SelectSingleNode("fb:lang", nameSpace).InnerText;
                        Dictionaries dict = new Dictionaries(Languages.RU);
                        lang = dict.languages[lang.ToLower()];
                    }
                }
                catch { lang = null; }

                string src_lang = null;
                try
                {
                    if (titleInfoNode.SelectSingleNode("fb:src-lang", nameSpace) != null)
                    { src_lang = titleInfoNode.SelectSingleNode("fb:src-lang", nameSpace).InnerText; }
                }
                catch { src_lang = null; }

                string trans = "";
                string au = "";
                try
                {
                    if (translatorNodes != null && (translatorNodes.Count > 0))
                    { trans = HelperFB2.structureAuthor(translatorNodes, nameSpace, true); }
                    if (authorNodes != null && (authorNodes.Count > 0))
                    { au = HelperFB2.structureAuthor(authorNodes, nameSpace, false); }
                }
                catch { trans = ""; au = ""; }


                writeNodeText(func, au, trans, bookName, lang, dateValue, dateInner, s, posterID);
                HelperFB2.ButtonInfo(annotationNode/*,authorNodes*/,bookName);
            }

            

            private string structureAnnotation(XmlNode annotation, XmlNamespaceManager nameSpace)
            {
                string txt;
                XmlNodeList p = annotation.SelectNodes("//fb:annotation/fb:p", nameSpace);
                txt = HelperFB2.checkAllP(p, nameSpace, xmlnsAttr); //Here
                return txt;
            }

            /// <summary>
            /// Shows TitleInfo inner text (referense only in "showTitleInfoNode")
            /// </summary>
            /// <param name="func"> delegate function </param>
            /// <param name="au"> author </param>
            /// <param name="trans">translator</param>
            /// <param name="bookName">book name</param>
            /// <param name="lang">language</param>
            /// <param name="dateValue">date from attribute value in "date" tag </param>
            /// <param name="dateInner">date from inner text in "date" tag</param>
            /// <param name="s"> string from structureAnnotation() </param>
            private void writeNodeText(
                howToShow func, string au, string trans,
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

                func(writeTxt);
                //func("==== Description: =================");
                //func(s);
                //func("===================================");
                //func("-----------------------------------");
            }

        }

        public class DocumentInfoNode : IDescriptionNodeShowFuncs
        {
            XmlNode documentInfoNode;

            //В массив передаю только сингловые ноды
            XmlNodeList authorNode;
            XmlNode programUsedNode;//1
            XmlNode dateNode;//2
            XmlNodeList srcUrlNode;
            XmlNode srcOcrNode;//3
            XmlNode idNode;//4
            XmlNode versionNode;//5
            //XmlNode historyNode; //щас не в счет

            // 5 нодов = 5 размер массива
            XmlNode[] massNodes = new XmlNode[5];
            string[] massVars = new string[5];

            public void showNode(XmlNode descriptionNode, XmlNamespaceManager nameSpace, howToShow func)
            {
                documentInfoNode = descriptionNode.SelectSingleNode("fb:document-info", nameSpace);

                authorNode = documentInfoNode.SelectNodes("fb:author", nameSpace);
                programUsedNode = documentInfoNode.SelectSingleNode("fb:program-used", nameSpace);
                dateNode = documentInfoNode.SelectSingleNode("fb:date", nameSpace);
                srcUrlNode = documentInfoNode.SelectNodes("fb:src-url", nameSpace);
                srcOcrNode = documentInfoNode.SelectSingleNode("fb:src-ocr", nameSpace);
                //TODO: разобраться с айди
                idNode = documentInfoNode.SelectSingleNode("fb:id", nameSpace);
                versionNode = documentInfoNode.SelectSingleNode("fb:version", nameSpace);

                //В массив передаю только сингловые ноды
                massNodes[0] = programUsedNode; massNodes[1] = dateNode;
                massNodes[2] = srcOcrNode; massNodes[3] = idNode; massNodes[4] = versionNode;

                //Cоздаем переменніе для каждого нода
                string authorr = null; string progUsed = null;
                string date = null; string srcU = null;
                string srcO = null; string id = null; string version = null;

                //В массив помещаем только переменные для сингл нодов!
                massVars[0] = progUsed; massVars[1] = date;
                massVars[2] = srcO; massVars[3] = id; massVars[4] = version;

                //Заменил миллионы трай на 1 в цикле. 
                //ПОРЯДОК НОДОВ В МАСИВЕ ДОЛЖЕН СОВПАДАТЬ С ПОРЯДКОМ ПЕРЕМЕННЫХ К НИМ ВО ВТОРОМ МАССИВЕ!!!!!
                for (int i = 0; i < massNodes.Length; i++)
                {
                    try
                    {
                        if(massNodes[i] != null)
                        {
                            massVars[i] = massNodes[i].InnerText;
                        }
                    }
                    catch { }
                }

                authorr = HelperFB2.structureAuthor(authorNode, nameSpace, false);
                //TODO: realise srcU node (not important)

                writeNodeText(func, authorr, massVars[0], massVars[1], srcU, massVars[2], massVars[3], massVars[4]);
            }

            private void writeNodeText(
                howToShow func, string au, string progUsed,
                string date, string srcU, string srcO, string id, string version)
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

                func(txt);
            }
        }

        public class PublishInfoNode : IDescriptionNodeShowFuncs
        {
            XmlNode publishInfoNode;

            XmlNode booknameNode;
            XmlNode publisherNode;
            XmlNode cityNode;
            XmlNode yearNode;

            XmlNode[] massNodes = new XmlNode[4];
            string[] massVars = new string[4];
            public void showNode(XmlNode descriptionNode, XmlNamespaceManager nameSpace, howToShow func)
            {
                publishInfoNode = descriptionNode.SelectSingleNode("fb:publish-info", nameSpace);

                booknameNode = publishInfoNode.SelectSingleNode("fb:book-name", nameSpace);
                publisherNode = publishInfoNode.SelectSingleNode("fb:publisher", nameSpace);
                cityNode = publishInfoNode.SelectSingleNode("fb:city", nameSpace);
                yearNode = publishInfoNode.SelectSingleNode("fb:year", nameSpace);

                massNodes[0] = booknameNode; massNodes[1] = publisherNode;
                massNodes[2] = cityNode; massNodes[3] = yearNode;

                string bookname = null; string publisher = null;
                string city = null; string year = null;

                massVars[0] = bookname; massVars[1] = publisher;
                massVars[2] = city; massVars[3] = year;

                for (int i = 0; i < massNodes.Length; i++)
                {
                    if(massNodes[i] != null)
                    {
                        massVars[i] = massNodes[i].InnerText;
                    }
                }

                writeNodeText(func, massVars[0], massVars[1], massVars[2], massVars[3]);
            }

            private void writeNodeText(howToShow func, string bn, string pub, string city, string year)
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

                func(txt);
            }
        }

        public override void showNode(string docPath, howToShow funct)
        {
            base.showNode(docPath, funct);
            // NOT FUNC - FUNCT !!!
            if (descriptionNode != null)
            {
                TitleInfoNode titleInfoNode = new TitleInfoNode();
                titleInfoNode.showNode(descriptionNode, nameSpace, funct);
                DocumentInfoNode docInfoNode = new DocumentInfoNode();
                docInfoNode.showNode(descriptionNode, nameSpace, funct);
                PublishInfoNode pubInfoNode = new PublishInfoNode();
                pubInfoNode.showNode(descriptionNode, nameSpace, funct);
            }
            decodeImageFromBinaryTag(binaryNodes, posterID, @"../../XML/pictures");
        }
    }
}
