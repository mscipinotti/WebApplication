using WebAPP.Infrastructure.GlobalParameters;
using WebAPP.Infrastructure.Models.enums;

namespace WebAPP.Services;

public class Language : ILanguage
{
    public Languages UserLanguage { get; } = Program.Language;
}
