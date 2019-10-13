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
            NodeReplace(bodyNode);
            func(text);
        }

        private void NodeReplace(XmlNode bodyNode)
        {
            text = HelperFB2.findTags(bodyNode.OuterXml, xmlnsAttr);
        }

    }
}
