using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XML_Testing_on_FB2
{
    abstract class ParserFormater
    {
        public abstract string formatBody(ParsersDecorator vars);
        public abstract string formatDescription(ParsersDecorator vars);
    }
}
