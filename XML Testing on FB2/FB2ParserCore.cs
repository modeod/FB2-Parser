using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace XML_Testing_on_FB2
{
    class FB2ParserCore : ParsersCore
    {
        protected howToShow func;
        protected XmlElement xRoot;
        protected static XmlNode xmlnsAttr;
        protected XmlNodeList binaryNodes;
        protected XmlNode descriptionNode;
        protected XmlNode bodyNode;
        protected XmlDocument xDoc;
        protected XmlNamespaceManager nameSpace;

        protected string bookPath;
        protected FB2DecoratorHead headDecor;
        protected FB2DecoratorBody bodyDecor;

        class DescriptionNode
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
                void initializeNode(XmlNode descriptionNode, XmlNamespaceManager nameSpace, ref FB2DecoratorHead head);
            }

            //TODO: вывести в тайтле жанры (необязон)
            //TODO: Переписать этот класс в нормальный вид
            private class TitleInfoNode : IDescriptionNodeShowFuncs
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

                public void initializeNode(
                    XmlNode descriptionNode,
                    XmlNamespaceManager nameSpace,
                    ref FB2DecoratorHead head)
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

                    string posterID = null;
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
                            lang = lang;
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
                        { trans = structureAuthor(translatorNodes, nameSpace, true); }
                        if (authorNodes != null && (authorNodes.Count > 0))
                        { au = structureAuthor(authorNodes, nameSpace, false); }
                    }
                    catch { trans = ""; au = ""; }

                    head.titleVars = new FB2DecoratorHead.DecoratorTitleInfo(au, trans, bookName,
                        lang, dateValue, dateInner, s, posterID);
                }

                private string structureAnnotation(XmlNode annotation, XmlNamespaceManager nameSpace)
                {
                    string txt;
                    XmlNodeList p = annotation.SelectNodes("//fb:annotation/fb:p", nameSpace);
                    txt = checkAllP(p, nameSpace, xmlnsAttr); //Here
                    return txt;
                }
            }

            private class DocumentInfoNode : IDescriptionNodeShowFuncs
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

                public void initializeNode(XmlNode descriptionNode, XmlNamespaceManager nameSpace, ref FB2DecoratorHead head)
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

                    // Ниже тут будет какая-то ебаная ссанина, которую я написал в припадке ночью еще хуй пойми когда,
                    // но исправлять мне ее лень, ибо она работает. Просто забей и знай, что это работает

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
                            if (massNodes[i] != null)
                            {
                                massVars[i] = massNodes[i].InnerText;
                            }
                        }
                        catch { }
                    }

                    authorr = structureAuthor(authorNode, nameSpace, false);
                    //TODO: realise srcU node (not important)

                    head.documentVars = new FB2DecoratorHead.DecoratorDocumentInfo(authorr, massVars[0], massVars[1], srcU, massVars[2], massVars[3], massVars[4]);
                }
            }

            private class PublishInfoNode : IDescriptionNodeShowFuncs
            {
                XmlNode publishInfoNode;

                XmlNode booknameNode;
                XmlNode publisherNode;
                XmlNode cityNode;
                XmlNode yearNode;

                XmlNode[] massNodes = new XmlNode[4];
                string[] massVars = new string[4];
                public void initializeNode(XmlNode descriptionNode, XmlNamespaceManager nameSpace, ref FB2DecoratorHead head)
                {
                    publishInfoNode = descriptionNode.SelectSingleNode("fb:publish-info", nameSpace);

                    booknameNode = publishInfoNode.SelectSingleNode("fb:book-name", nameSpace);
                    publisherNode = publishInfoNode.SelectSingleNode("fb:publisher", nameSpace);
                    cityNode = publishInfoNode.SelectSingleNode("fb:city", nameSpace);
                    yearNode = publishInfoNode.SelectSingleNode("fb:year", nameSpace);

                    // Тут я томной ночью тоже решил повторить эту поебень, а исправлять мне лень, так что пойдет

                    massNodes[0] = booknameNode; massNodes[1] = publisherNode;
                    massNodes[2] = cityNode; massNodes[3] = yearNode;

                    string bookname = null; string publisher = null;
                    string city = null; string year = null;

                    massVars[0] = bookname; massVars[1] = publisher;
                    massVars[2] = city; massVars[3] = year;

                    for (int i = 0; i < massNodes.Length; i++)
                    {
                        if (massNodes[i] != null)
                        {
                            massVars[i] = massNodes[i].InnerText;
                        }
                    }

                    head.publishVars = new FB2DecoratorHead.DecoratorPublishInfo(massVars[0], massVars[1], massVars[2], massVars[3]);

                }
            }

            public void initializeAllDescriptionNodes(
                XmlNode descriptionNode, 
                XmlNamespaceManager nameSpace,
                ref FB2DecoratorHead head)
            {
                // NOT FUNC - FUNCT !!!
                if (descriptionNode != null)
                {
                    TitleInfoNode titleInfoNode = new TitleInfoNode();
                    titleInfoNode.initializeNode(descriptionNode, nameSpace, ref head);
                    DocumentInfoNode docInfoNode = new DocumentInfoNode();
                    docInfoNode.initializeNode(descriptionNode, nameSpace, ref head);
                    PublishInfoNode pubInfoNode = new PublishInfoNode();
                    pubInfoNode.initializeNode(descriptionNode, nameSpace, ref head);
                }
                else { return; }
            }
        }

        class BodyNode
        {
            public void initializeBodyNode(XmlNode bodyNode, XmlNamespaceManager nameSpace, ref FB2DecoratorBody bodyDec)
            {
                if (bodyNode != null)
                {
                    List<int> graph = checkAllSection(bodyNode.SelectNodes("fb:section", nameSpace), 0, nameSpace);

                    graph = graph; // Debug point

                    bodyDec.bodyNode = bodyNode;
                    bodyDec.graph = graph;
                }
                else { return; }
            }

            private List<int> checkAllSection(XmlNodeList list, int zero, XmlNamespaceManager nameSpace)
            {
                List<int> toReturn = new List<int>();

                foreach (XmlNode node in list)
                {
                    XmlNodeList secNodeList = node.SelectNodes("fb:section", nameSpace);

                    if (secNodeList.Count > 0)
                    {
                        toReturn.Add(zero);
                        List<int> asd = checkAllSection(secNodeList, zero + 1, nameSpace);
                        foreach (int i in asd)
                        {
                            toReturn.Add(i);
                        }
                    }
                    else
                    {
                        toReturn.Add(zero);
                    }
                }

                return toReturn;
            }
        }

        // Инициализация всех нужній переменніх для работы в конструкторе
        public FB2ParserCore(string docPath)
        {
            bookPath = docPath;
            loadDocument(ref xDoc, docPath);

            xRoot = xDoc.DocumentElement;
            xmlnsAttr = xRoot.Attributes.GetNamedItem("xmlns");
            nameSpace = new XmlNamespaceManager(xDoc.NameTable);
            nameSpace.AddNamespace("fb", xmlnsAttr.Value);

            headDecor = new FB2DecoratorHead();
            bodyDecor = new FB2DecoratorBody();

            getMainNodes
                (
                nameSpace,
                ref binaryNodes,
                ref descriptionNode,
                ref bodyNode,
                xRoot
                );

            // Сперва мы инициализируем все переменные и запихиваем их в класс декоратора
            DescriptionNode decNodeClass = new DescriptionNode();
            decNodeClass.initializeAllDescriptionNodes(descriptionNode, nameSpace, ref headDecor);

            BodyNode bodyNodeClass = new BodyNode();
            bodyNodeClass.initializeBodyNode(bodyNode, nameSpace, ref bodyDecor);
            // А теперь мы можем вызывать методы realiseDesk / Body, которые будут брать декоратор и передавать его в 
            // класс FB2Formatter, где он все запишет и отфррматирует, после чего готовая строка 
            // врзвращается в метод реалайс и с помощью делегата funk выведится тудо, куда тебе надо


            //Decode poster
            decodeImageFromBinaryTag(binaryNodes, headDecor.titleVars.posterId, @"../../XML/pictures");
        }

        public void realiseDescription(howToShow funk)
        {
            if (func == HowToShow.writeInTxtBody) { return;  }
            if (func == HowToShow.writeInTxtDescription) { File.Delete(@"../../XML/BookDescription.txt"); }

            FB2ParserFormater dFormater = new FB2ParserFormater();
            string toShow = dFormater.formatDescription(headDecor); // Обьект декоратора зоздался в конструкторе ПарсерКор

            funk(toShow);

        }
        public void realiseBody(howToShow funk)
        {
            if (func == HowToShow.writeInTxtDescription) { return; }
            if (func == HowToShow.writeInTxtBody) { File.Delete(@"../../XML/BookMainText.txt"); }

            FB2ParserFormater bFormater = new FB2ParserFormater();
            string toShow = bFormater.formatBody(bodyDecor); // Обьект декоратора зоздался в конструкторе ПарсерКор
            // 
            funk(toShow);
        } 

        private void getMainNodes
            (
            XmlNamespaceManager nameSpace,
            ref XmlNodeList binaryNodes,
            ref XmlNode descriptionNode,
            ref XmlNode bodyNode,
            XmlElement xRoot
            )
        {
            binaryNodes = null;
            descriptionNode = null;
            bodyNode = null;
            XmlNodeList xMain = xRoot.SelectNodes("*");

            for (int i = 0; i < xMain.Count; i++)
            {
                if (xMain[i].Name == "description")
                {
                    //Console.WriteLine("ЄТО опЫсанЫэ!!!");
                    descriptionNode = xMain[i];
                }
                else if (xMain[i].Name == "body")
                {
                    //Console.WriteLine("HERE WE COME 'BODY' ._.");
                    bodyNode = xMain[i];
                }

                //Console.WriteLine(xMain[i].Name + "\n");
            }

            binaryNodes = xRoot.SelectNodes("fb:binary", nameSpace);
        }

        private static void decodeImageFromBinaryTag(XmlNodeList binaryNodes, string imageID, string path = @"../../XML/pictures")
        {
            if (!(binaryNodes != null || imageID != null)) // Why not 
                return;

            Regex reg = new Regex("#*");
            string realName = null;
            bool isSharp = reg.IsMatch(imageID);
            string base64ImageString = null;
            
            foreach (XmlNode binaryNode in binaryNodes)
            {
                if (isSharp)
                {
                    if (("#" + binaryNode.Attributes.GetNamedItem("id").InnerText) == imageID)
                    {
                        base64ImageString = binaryNode.InnerText;
                        realName = binaryNode.Attributes.GetNamedItem("id").InnerText;
                    }
                }
                else
                {
                    if (binaryNode.Attributes.GetNamedItem("id").InnerText == imageID)
                    {
                        base64ImageString = binaryNode.InnerText;
                        realName = binaryNode.Attributes.GetNamedItem("id").InnerText;
                    }
                }

            }

            if (base64ImageString != null || base64ImageString != "" || base64ImageString != " ")
            {
                ParsersCore.decodeBinaryImage(base64ImageString, realName, path);
            }
        }

        // Pizdec bad function ik
        private static string checkAllP(XmlNodeList nodeS, XmlNamespaceManager nameSpace, XmlNode xmlnsAttr)
        {

            string txt = "";
            foreach (XmlNode node in nodeS)
            // IK about "one" ._.
            {
                XmlNodeList nextNode = null;
                if (node.Name == "p")
                {
                    nextNode = node.SelectNodes("fb:p", nameSpace);
                }
                else if (node.Name == "section")
                {
                    nextNode = node.SelectNodes("fb:section", nameSpace);
                }

                if (nextNode.Count > 0)
                {
                    txt += checkAllP(nextNode, nameSpace, xmlnsAttr); // Вот тут костіль ебаный иди нахуй
                }
                else
                {
                    txt += "    ";
                    string outer = node.OuterXml;

                    string prepreTxt = FB2ParserFormater.findTags(outer); //Here
                    prepreTxt = prepreTxt.Trim();
                    txt += prepreTxt + "\n";
                }
            }

            return txt;
        } 

        private static string structureAuthor(XmlNodeList authorNodes, XmlNamespaceManager nameSpace, bool ifTranslator)
        {
            string txtToReturn = "";
            if (ifTranslator) { txtToReturn = "==== Translator(s): "; }
            else { txtToReturn = "==== Author(s): "; }

            for (int i = authorNodes.Count; i > 0; i--)
                if (i > 1) txtToReturn += authorNodes[i - 1].InnerText + " / ";
                else txtToReturn += authorNodes[i - 1].InnerText;

            return txtToReturn;
        }

        private void loadDocument(ref XmlDocument xDoc, string path)
        {
            xDoc = new XmlDocument();
            xDoc.Load(path);
        }
    }
}
