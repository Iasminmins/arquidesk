using ArquiDesk.Domain.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArquiDesk.Web.Helpers;

public static class EnumSelectListHelper
{
    public static IEnumerable<SelectListItem> ToSelectList<TEnum>() where TEnum : struct, Enum =>
        Enum.GetValues<TEnum>().Select(value => new SelectListItem
        {
            Value = value.ToString(),
            Text = value.GetDisplayName()
        });
}
