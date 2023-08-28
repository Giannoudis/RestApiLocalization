using System.Globalization;

namespace RestApiLocalization
{
    public class CultureScope
    {
        public bool Neutral { get; set; }
        public bool Specific { get; set; }
        public bool Installed { get; set; }
        public bool Custom { get; set; }
        public bool Replacement { get; set; }

        public CultureScope() :
            this(true, true, true)
        {
        }

        public CultureScope(bool neutral, bool specific, bool installed,
            bool custom = false, bool replacement = false)
        {
            Neutral = neutral;
            Specific = specific;
            Installed = installed;
            Custom = custom;
            Replacement = replacement;
        }

        public CultureTypes GetCultureTypes()
        {
            if (!Neutral && !Specific && !Installed && !Custom && !Replacement)
            {
                throw new InvalidOperationException("Missing culture type selection");
            }

            CultureTypes types = CultureTypes.AllCultures |
                                 CultureTypes.UserCustomCulture |
                                 CultureTypes.ReplacementCultures;
            if (!Neutral)
            {
                types &= ~CultureTypes.NeutralCultures;
            }
            if (!Specific)
            {
                types &= ~CultureTypes.SpecificCultures;
            }
            if (!Installed)
            {
                types &= ~CultureTypes.InstalledWin32Cultures;
            }
            if (!Custom)
            {
                types &= ~CultureTypes.UserCustomCulture;
            }
            if (!Replacement)
            {
                types &= ~CultureTypes.ReplacementCultures;
            }

            return types;
        }
    }
}
