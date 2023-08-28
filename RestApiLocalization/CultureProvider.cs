using System.Globalization;

namespace RestApiLocalization
{
    public class CultureProvider : ICultureProvider
    {
        private sealed class CultureItem : CultureDescription
        {
            internal CultureInfo Culture { get; }

            internal CultureItem(CultureInfo culture) :
                base(culture)
            {
                Culture = culture;
            }
        }

        private List<CultureItem> CultureItems { get; } = new();

        public CultureContext Context { get; }

        #region Constructor

        /// <summary>
        /// New default culture provider with the culture scope neutral, specific and installed
        /// </summary>
        public CultureProvider() :
            this(new CultureScope())
        {
        }

        /// <summary>
        /// New default culture provider with the culture scope
        /// </summary>
        /// <param name="cultureScope">The culture scope</param>
        /// <param name="defaultCultureName">The default culture</param>
        /// <param name="context">The culture context</param>
        public CultureProvider(CultureScope cultureScope,
            string? defaultCultureName = null, CultureContext context = CultureContext.Thread) :
            this(CultureInfo.GetCultures(cultureScope.GetCultureTypes()).Select(x => x.Name),
                cultureScope, defaultCultureName, context)
        {
        }

        /// <summary>
        /// New culture provider by culture names with the culture scope neutral, specific and installed
        /// </summary>
        /// <param name="supportedCultures">The culture names</param>
        /// <param name="defaultCultureName">The default culture</param>
        /// <param name="context">The culture context</param>
        public CultureProvider(IEnumerable<string> supportedCultures,
            string? defaultCultureName = null, CultureContext context = CultureContext.Thread) :
            this(supportedCultures, new CultureScope(), defaultCultureName, context)
        {
        }

        /// <summary>
        /// New culture provider by culture names
        /// </summary>
        /// <param name="supportedCultures">The culture names</param>
        /// <param name="cultureScope">The culture scope</param>
        /// <param name="defaultCultureName">The default culture</param>
        /// <param name="context">The culture context</param>
        public CultureProvider(IEnumerable<string> supportedCultures, CultureScope cultureScope,
            string? defaultCultureName = null, CultureContext context = CultureContext.Thread)
        {
            var uniqueCultureNames = new HashSet<string>(supportedCultures);
            if (!uniqueCultureNames.Any())
            {
                throw new ArgumentException("Missing cultures");
            }

            var availableCultureInfos = CultureInfo.GetCultures(cultureScope.GetCultureTypes());
            foreach (var cultureName in uniqueCultureNames)
            {
                var cultureInfo = availableCultureInfos.FirstOrDefault(
                    x => string.Equals(x.Name, cultureName, StringComparison.OrdinalIgnoreCase));
                if (cultureInfo == null)
                {
                    throw new ArgumentException($"Unknown culture {cultureName}");
                }
                if (!string.IsNullOrWhiteSpace(cultureInfo.Name))
                {
                    CultureItems.Add(new CultureItem(cultureInfo));
                }
            }

            // default culture
            defaultCultureName ??= context == CultureContext.Application
                ? CultureInfo.CurrentCulture.Name
                : CultureInfo.DefaultThreadCurrentCulture?.Name ?? CultureInfo.InvariantCulture.Name;
            var defaultCulture = CultureItems.FirstOrDefault(
                x => string.Equals(x.Name, defaultCultureName, StringComparison.OrdinalIgnoreCase))?.Culture;
            if (defaultCulture == null)
            {
                throw new LocalizationException($"Unknown default culture {defaultCultureName}");
            }
            DefaultCultureName = defaultCultureName;

            // culture context
            Context = context;
        }

        /// <summary>
        /// New culture provider
        /// </summary>
        /// <param name="supportedCultures">The cultures</param>
        /// <param name="defaultCultureName">The default culture</param>
        /// <param name="context">The culture context</param>
        public CultureProvider(IEnumerable<CultureInfo> supportedCultures,
            string? defaultCultureName = null, CultureContext context = CultureContext.Thread) :
            this(supportedCultures, new CultureScope(), defaultCultureName, context)
        {
        }

        /// <summary>
        /// New culture provider
        /// </summary>
        /// <param name="supportedCultures">The cultures</param>
        /// <param name="cultureScope">The culture scope</param>
        /// <param name="defaultCultureName">The default culture</param>
        /// <param name="context">The culture context</param>
        public CultureProvider(IEnumerable<CultureInfo> supportedCultures, CultureScope cultureScope,
            string? defaultCultureName = null, CultureContext context = CultureContext.Thread)
        {
            var availableCultureInfos = CultureInfo.GetCultures(cultureScope.GetCultureTypes());
            foreach (var cultureInfo in supportedCultures)
            {
                var availableCultureInfo = availableCultureInfos.FirstOrDefault(
                    x => string.Equals(x.Name, cultureInfo.Name, StringComparison.OrdinalIgnoreCase));
                if (availableCultureInfo == null)
                {
                    throw new ArgumentException($"Unknown culture {cultureInfo.Name}");
                }
                if (!string.IsNullOrWhiteSpace(availableCultureInfo.Name))
                {
                    CultureItems.Add(new CultureItem(availableCultureInfo));
                }
            }

            // default culture
            defaultCultureName ??= context == CultureContext.Application
                ? CultureInfo.CurrentCulture.Name
                : CultureInfo.DefaultThreadCurrentCulture?.Name ?? CultureInfo.InvariantCulture.Name;
            var defaultCulture = CultureItems.FirstOrDefault(
                x => string.Equals(x.Name, defaultCultureName, StringComparison.OrdinalIgnoreCase))?.Culture;
            if (defaultCulture == null)
            {
                throw new LocalizationException($"Unknown default culture {defaultCultureName}");
            }
            DefaultCultureName = defaultCultureName;

            // culture context
            Context = context;
        }

        #endregion

        /// <inheritdoc />
        public virtual string DefaultCultureName { get; }

        /// <inheritdoc />
        public virtual CultureInfo CurrentCulture =>
            Context switch
            {
                CultureContext.Application => CultureInfo.CurrentCulture,
                _ => CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.InvariantCulture
            };

        /// <inheritdoc />
        public CultureInfo CurrentUICulture =>
            Context switch
            {
                CultureContext.Application => CultureInfo.CurrentUICulture,
                _ => CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.InvariantCulture
            };

        /// <inheritdoc />
        public virtual void SetCurrentCulture(string cultureName)
        {
            if (string.IsNullOrWhiteSpace(cultureName))
            {
                throw new ArgumentException(nameof(cultureName));
            }

            // validate culture
            var culture = GetCulture(cultureName);
            if (culture == null)
            {
                throw new ArgumentException($"Unknown culture {cultureName}");
            }

            // apply culture
            switch (Context)
            {
                case CultureContext.Application:
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                    break;
                default:
                    CultureInfo.DefaultThreadCurrentCulture = culture;
                    CultureInfo.DefaultThreadCurrentUICulture = culture;
                    break;
            }
        }

        /// <inheritdoc />
        public virtual CultureInfo? GetCulture(string cultureName)
        {
            if (string.IsNullOrWhiteSpace(cultureName))
            {
                throw new ArgumentException(nameof(cultureName));
            }
            return CultureItems.FirstOrDefault(
                x => string.Equals(x.Name, cultureName, StringComparison.OrdinalIgnoreCase))?.Culture;
        }

        /// <inheritdoc />
        public virtual IList<CultureInfo> GetSupportedCultures() =>
            CultureItems.Select(x => x.Culture).ToList();

        /// <inheritdoc />
        public virtual IList<CultureDescription> GetSupportedCultureDescriptions() =>
            new List<CultureDescription>(CultureItems);
    }
}
