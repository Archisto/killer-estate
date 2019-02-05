using System.IO;

namespace KillerEstate.Localization
{
    class LocalizationNotFoundException : FileNotFoundException
    {
        public LangCode Language { get; private set; }

        public LocalizationNotFoundException(LangCode language)
        {
            Language = language;
        }

        public override string Message
        {
            get
            {
                return "Localization cannot be found for language " +
                    Language;
            }
        }
    }
}
