using System.Configuration;

namespace Reach.Configuration
{
   class ExternalLibraryElement : ConfigurationElement
   {
      [ConfigurationProperty("path", IsKey = true, IsRequired = true)]
      public string Path
      {
         get { return (string)this["path"]; }
      }
   }

   class ExternalLibraryCollection : ConfigurationElementCollection
   {
      protected override ConfigurationElement CreateNewElement()
      {
         return new ExternalLibraryElement();
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
         return ((ExternalLibraryElement)element).Path;
      }
   }

   class ExternalLibrariesSection : ConfigurationSection
   {
      [ConfigurationProperty("libraries")]
      public ExternalLibraryCollection Libraries
      {
         get { return (ExternalLibraryCollection)this["libraries"]; }
      }
   }
}
