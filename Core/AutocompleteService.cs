using AutocompleteMenuNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester.Core
{
    public class AutocompleteService
    {
        private readonly List<string> _snippets;
        private readonly List<string> _keywords;

        public AutocompleteService()
        {
            _snippets = new List<string> { "if(^)\n{\n}", "for(^;;)\n{\n}" };
            _keywords = new List<string> { "abstract", "bool", "break" };
        }

        public List<AutocompleteItem> GetAutocompleteItems()
        {
            var items = new List<AutocompleteItem>();
            foreach (var snippet in _snippets)
                items.Add(new SnippetAutocompleteItem(snippet));

            foreach (var keyword in _keywords)
                items.Add(new AutocompleteItem(keyword));

            return items;
        }
    }

}
