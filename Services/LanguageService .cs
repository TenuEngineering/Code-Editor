using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Tester.Services
{
    public class LanguageService
    {
        private readonly ResourceManager _resManager;
        private CultureInfo _cultureInfo;

        public LanguageService(string baseName, Assembly assembly)
        {
            _resManager = new ResourceManager(baseName, assembly);
            _cultureInfo = CultureInfo.CurrentCulture;
        }

        public void ChangeLanguage(string langCode)
        {
            _cultureInfo = new CultureInfo(langCode);
            CultureInfo.CurrentCulture = _cultureInfo;
            CultureInfo.CurrentUICulture = _cultureInfo;
        }

        public string GetString(string key)
        {
            return _resManager.GetString(key, _cultureInfo) ?? $"[{key}]";
        }
    }
}
