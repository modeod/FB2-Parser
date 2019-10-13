﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace XML_Testing_on_FB2 
{
    delegate void howToShow(string s);
    abstract class PreNodes 
    {
        protected howToShow func;
        protected XmlElement xRoot;
        protected static XmlNode xmlnsAttr;
        protected XmlNodeList binaryNodes;
        protected XmlNode descriptionNode;
        protected XmlNode bodyNode;
        protected XmlDocument xDoc;
        protected XmlNamespaceManager nameSpace;

        /// <summary>
        /// Shows node.
        /// (If u inherit this function - first line must be:
        /// base.showNode(docPath) )
        /// </summary>
        /// <param name="docPath"> Path to Book </param>
        public virtual void showNode(string docPath, howToShow func)
        {
            //Select method to show all
            
            if (func == HelperFB2.showInTxtFile) { File.Delete(@"../../XML/formated text.txt"); }
               
            //TODO?: get path from console
            loadDocument(ref xDoc, docPath);
            
            //for(int i = 0, i <  )
            xRoot = xDoc.DocumentElement;
            xmlnsAttr = xRoot.Attributes.GetNamedItem("xmlns");
            nameSpace = new XmlNamespaceManager(xDoc.NameTable);
            nameSpace.AddNamespace("fb", xmlnsAttr.Value);

            getMainNodes
                (
                nameSpace,
                ref binaryNodes, 
                ref descriptionNode, 
                ref bodyNode, 
                xRoot
                );
        }

        
        protected void loadDocument(ref XmlDocument xDoc, string path)
        {
            xDoc = new XmlDocument();
            xDoc.Load(path);
        }

        //
        // All xRoot/xDoc functions lower 
        //
        protected void decodeImageFromBinaryTag(XmlNodeList binaryNodes, string imageID, string path = @"../../XML/pictures")
        {
            if ( !(binaryNodes != null || imageID != null) )
                return;

            Regex reg = new Regex("#*");
            string realName = null;
            bool isSharp = reg.IsMatch(imageID);
            string base64ImageString = null;
            foreach (XmlNode binaryNode in binaryNodes)
            {
                if (isSharp)
                {
                    if ( ("#" + binaryNode.Attributes.GetNamedItem("id").InnerText) == imageID )
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
                Base64ImageDecoder.decodeImage(base64ImageString, realName, path);
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
            XmlNodeList xMain = xRoot.SelectNodes("*"); //First node is always binary ._.

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
    }

    class HelperFB2
    {
        public static void showInConsole(string s)
        {
            Console.WriteLine(s);
        }
        public static void showInTxtFile(string s)
        {
            //File.AppendAllText(@"../../XML/formated text.txt", "\n");
            File.AppendAllText(@"../../XML/formated text.txt", s);
        }

        public static string checkAllP(XmlNodeList p, XmlNamespaceManager nameSpace, XmlNode xmlnsAttr)
        {
            string txt = "";
            foreach (XmlNode node in p)
            // IK about "one" ._.
            {
                XmlNodeList nextP = node.SelectNodes("fb:p", nameSpace);
                if (nextP.Count > 0)
                {
                    txt += checkAllP(nextP, nameSpace, xmlnsAttr);
                }
                else
                {
                    txt += "    ";
                    string preTxt = node.InnerText;
                    string outer = node.OuterXml;

                    string prepreTxt = findTags(preTxt, outer, xmlnsAttr); //Here
                    prepreTxt = prepreTxt.Trim();
                    txt += prepreTxt + "\n";
                }
            }

            return txt;
        }

        //REPLASE CAN BE CHANGED TO WPF CONSTRUCTION
        //REPLASE CAN BE CHANGED TO WPF CONSTRUCTION
        public static string findTags(string txt, string outer, XmlNode xmlnsAttr)
        {
            char charr = '"';
            string txtToReturn = null;
            // [A-Za-z0-9,^*: {charr}-=@]*
            Regex regexStart = new Regex($"<p xmlns=\"{xmlnsAttr}\">");

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

            txtToReturn = regexStartCenNone.Replace(txtToReturn, "Центровка слева,строка единственная\n\n\n\n\n\n\n\n");
            txtToReturn = regexStartLeftNone.Replace(txtToReturn, "Центровка слева,строка единственная\n\n\n\n\n\n\n\n ");
            txtToReturn = regexStartRightAll.Replace(txtToReturn, "Центровка справа,строка единственная\n\n\n\n\n\n\n\n ");

            //Тут строки
            txtToReturn = txtToReturn.Replace("<i>", "<i> ");
            txtToReturn = txtToReturn.Replace("</i>", " </i>");
            txtToReturn = txtToReturn.Replace("<p>", "\t");
            txtToReturn = txtToReturn.Replace("</p>", "\n");
            txtToReturn = txtToReturn.Replace("<strong>", "<strong> "); // -STRONG ={
            txtToReturn = txtToReturn.Replace("</strong>", " </strong>"); // }=

            return txtToReturn;
        }

        public static string structureAuthor(XmlNodeList authorNodes, XmlNamespaceManager nameSpace, bool ifTranslator)
        {
            string txtToReturn = "";
            if (ifTranslator) { txtToReturn = "==== Translator(s): "; }
            else { txtToReturn = "==== Author(s): "; }

            for (int i = authorNodes.Count; i > 0; i--)
                if (i > 1) txtToReturn += authorNodes[i - 1].InnerText + " / ";
                else txtToReturn += authorNodes[i - 1].InnerText;

            return txtToReturn;
        }
    }
}
