using System;
using System.Collections.Generic;
using System.Text;

namespace Zaphyros.Core.Commands
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class CommandAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        /// <summary>
        /// An attribute that represents a command description.
        /// </summary>
        /// <param name="name">Name of the command.</param>
        /// <param name="description">Description of the command</param>
        public CommandAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

}
