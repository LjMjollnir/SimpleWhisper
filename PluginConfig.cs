using Rocket.API;

namespace LjMjollnir
{
    // This will create a AssemblyName.configuration.xml file in the plugins\AssemblyName folder that people can easily edit
    public class PluginConfig : IRocketPluginConfiguration
    {
        public string message = "Hello World"; // Set them here and in LoadDefaults incase of changes to your Config 

        public void LoadDefaults()
        {
            message = "Hello World";
        }
    }
}