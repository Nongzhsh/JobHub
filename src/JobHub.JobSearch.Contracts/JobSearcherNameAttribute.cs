using System;
using System.Linq;

namespace JobHub.JobSearch.Contracts
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class JobSearcherNameAttribute : Attribute
    {
        public string Name { get; }

        public JobSearcherNameAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            Name = name;
        }

        public static string GetName(Type filterType)
        {
            var filterNameAttribute = filterType
                .GetCustomAttributes(true)
                .OfType<JobSearcherNameAttribute>()
                .FirstOrDefault();

            if (filterNameAttribute != null)
            {
                return filterNameAttribute.Name;
            }

            return filterType.Name;
        }
    }
}
