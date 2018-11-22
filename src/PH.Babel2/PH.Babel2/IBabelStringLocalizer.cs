using Microsoft.Extensions.Localization;

namespace PH.Babel2
{
    public interface IBabelStringLocalizer : IBabel, IStringLocalizer
    {

    }

    public interface IBabelStringLocalizer<TResource> : IBabel, IStringLocalizer<TResource>
    {

    }
}