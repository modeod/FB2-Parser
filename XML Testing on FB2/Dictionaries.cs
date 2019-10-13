using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XML_Testing_on_FB2
{
    enum Languages
    {
        RU,
        EN
    }

    class Dictionaries
    {
        public Dictionary<string, string> languages = new Dictionary<string, string>();
        public Dictionary<string, string> genres = new Dictionary<string, string>();
        public Dictionaries(Languages lang)
        {
            if(lang == Languages.EN)
            {
                //TODO >_<
            }
            else if(lang == Languages.RU)
            {
                InitializeRUDicts();
            }
        }

        private void InitializeRUDicts()
        {
            string[] lines = File.ReadAllLines("languages(ru).txt", Encoding.Default);
            foreach (string line in lines)
            {
                string[] el = line.Split(' ');
                string[] secEl = el[1].Split('/');
                foreach (string langSep in secEl)
                {
                    languages.Add(langSep, el[0]);
                }
            }

            lines = File.ReadAllLines("genres(ru).txt", Encoding.Default);
        }

    }
}
