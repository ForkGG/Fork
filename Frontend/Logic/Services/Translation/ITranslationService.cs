namespace ForkFrontend.Logic.Services.Translation;

public interface ITranslationService
{
    /// <summary>
    ///     Translate the given variable in the configured language
    /// </summary>
    /// <param name="variable">i.e. "common.navbar.servers"</param>
    /// <returns></returns>
    Task<string> Translate(string variable);
}