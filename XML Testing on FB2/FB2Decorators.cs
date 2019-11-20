using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XML_Testing_on_FB2
{
    class FB2DecoratorHead : ParsersDecorator
    {
        public DecoratorTitleInfo titleVars;
        public DecoratorDocumentInfo documentVars;
        public DecoratorPublishInfo publishVars;

        public class DecoratorTitleInfo
        {
            public string author = "";
            public string description = null;
            public string bookName = null;
            public string dateValue = null;
            public string dateInner = null;
            public string lang = null;
            public string trans = "";
            public string posterId = "";

            public DecoratorTitleInfo (string au, string trans,
                string bookName, string lang, string dateValue,
                string dateInner, string description, string posterId)
            {
                this.author = au; this.trans = trans; this.description = description;
                this.lang = lang; this.bookName = bookName;
                this.dateInner = dateInner; this.dateValue = dateValue; this.posterId = posterId;
            }
        }

        public class DecoratorDocumentInfo
        {
            public string author = null;
            public string progUsed = null;
            public string date = null;
            public string srcU = null;
            public string srcO = null;
            public string id = null;
            public string version = null;
            public DecoratorDocumentInfo(string au, string progUsed,
                string date, string srcU, string srcO,
                string id, string version)
            {
                this.author = au; this.progUsed = progUsed; this.date = date;
                this.srcU = srcU; this.srcO = srcO;
                this.id = id; this.version = version;
            }
        }

        public class DecoratorPublishInfo
        {
            public string bookname = null;
            public string publisher = null;
            public string city = null;
            public string year = null;
            public DecoratorPublishInfo(string bookname, string publisher,
                string city, string year)
            {
                this.bookname = bookname;
                this.publisher = publisher;
                this.city = city;
                this.year = year; 
            }
        }
    }

    class FB2DecoratorBody : ParsersDecorator
    {
        public List<int> graph;
        public XmlNode bodyNode; 
    }
}
