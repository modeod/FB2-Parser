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
    class BodyFormatingProgram : PreNodes
    {

        private string text;

        public override void showNode(string docPath, howToShow func)
        {
            base.showNode(docPath, func);


            List<int> graph = checkAllSection(bodyNode.SelectNodes("fb:section", nameSpace), 0);
            graph = graph;
            NodeReplace(bodyNode);
            func(text);
        }

        private void NodeReplace(XmlNode bodyNode)
        {
            text = HelperFB2.findTags(bodyNode.OuterXml, xmlnsAttr);
        }

        private List<int> checkAllSection(XmlNodeList list, int zero)
        {
            List<int> toReturn = new List<int>();

            foreach(XmlNode node in list)
            {
                XmlNodeList secNodeList = node.SelectNodes("fb:section", nameSpace);

                if (secNodeList.Count > 0)
                {
                    toReturn.Add(zero);
                    List<int> asd = checkAllSection(secNodeList, zero + 1);
                    foreach(int i in asd)
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
}
