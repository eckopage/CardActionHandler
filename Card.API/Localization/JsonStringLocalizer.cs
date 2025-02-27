using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text.Json;

namespace Card.API.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly Dictionary<string, string> _localizationData;

        public JsonStringLocalizer()
        {
            _localizationData = new Dictionary<string, string>();
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var jsonFile = $"Resources/localization.{culture}.json";
            
            if (File.Exists(jsonFile))
            {
                var jsonData = File.ReadAllText(jsonFile);
                _localizationData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonData) ?? new();
            }
        }

        public LocalizedString this[string name]
        {
            get
            {
                return _localizationData.TryGetValue(name, out var value)
                    ? new LocalizedString(name, value)
                    : new LocalizedString(name, name, true);
            }
        }

        public LocalizedString this[string name, params object[] arguments] => this[name];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return new List<LocalizedString>();
        }
    }
}