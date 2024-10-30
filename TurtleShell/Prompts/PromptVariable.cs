using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleShell.Prompts
{
    public class PromptVariable
    {
        public string Key { get; }
        public string Value { get; }

        public PromptVariable(string key, string value)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static implicit operator PromptVariable((string key, string value) tuple)
        {
            return new PromptVariable(tuple.key, tuple.value);
        }

        public static implicit operator KeyValuePair<string, string>(PromptVariable v)
        {
            return KeyValuePair.Create(v.Key, v.Value);
        }

        public static implicit operator PromptVariable(KeyValuePair<string, string> kvp)
        {
            return new PromptVariable(kvp.Key, kvp.Value);
        }

        public override string ToString()
        {
            return $"{{{Key}}} = {Value}";
        }

        public override bool Equals(object obj)
        {
            if (obj is PromptVariable other)
            {
                return Key == other.Key && Value == other.Value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }
    }
}
