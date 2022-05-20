using System;

namespace Metroidvania
{
    /// <summary>Set the path of a singleton asset located in resources</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ResourceObjectPathAttribute : Attribute
    {
        /// <summary>
        /// The path located in the resources should look like 'Data/Foo/Bar'.
        /// The path in the project should look like 'Assets/Resources/Data/Foo/Bar.asset'
        /// </summary>
        public readonly string path;

        public ResourceObjectPathAttribute(string path)
        {
            this.path = path;
        }
    }
}