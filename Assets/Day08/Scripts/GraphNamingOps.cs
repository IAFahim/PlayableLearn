using Unity.Burst;
using Unity.Mathematics;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day08
{
    /// <summary>
    /// Burst-compiled operations for graph naming.
    /// Pure computation. No side effects.
    ///
    /// Day 08: The Director Name
    /// Operations for naming and managing PlayableGraph names for Profiler visibility.
    /// </summary>
    [BurstCompile]
    public static class GraphNamingOps
    {
        /// <summary>
        /// Checks if a graph name is valid (not null or empty).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidName(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        /// <summary>
        /// Checks if two graph names are equal (case-insensitive).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreNamesEqual(string name1, string name2)
        {
            if (string.IsNullOrEmpty(name1) && string.IsNullOrEmpty(name2))
                return true;

            if (string.IsNullOrEmpty(name1) || string.IsNullOrEmpty(name2))
                return false;

            return name1.Equals(name2, System.StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if the name has changed from the previous name.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNameChanged(string currentName, string previousName)
        {
            return !AreNamesEqual(currentName, previousName);
        }

        /// <summary>
        /// Generates a unique graph name by appending a counter.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GenerateUniqueName(string baseName, int counter)
        {
            return $"{baseName}_{counter}";
        }

        /// <summary>
        /// Sanitizes a graph name by removing invalid characters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SanitizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "UnnamedGraph";

            // Replace invalid characters with underscores
            char[] invalidChars = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
            string sanitized = name;

            for (int i = 0; i < invalidChars.Length; i++)
            {
                sanitized = sanitized.Replace(invalidChars[i], '_');
            }

            // Remove leading/trailing whitespace
            sanitized = sanitized.Trim();

            // Ensure name is not empty after sanitization
            if (string.IsNullOrEmpty(sanitized))
                return "UnnamedGraph";

            return sanitized;
        }

        /// <summary>
        /// Checks if a graph name exceeds the maximum length.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNameTooLong(string name, int maxLength)
        {
            return !string.IsNullOrEmpty(name) && name.Length > maxLength;
        }

        /// <summary>
        /// Truncates a graph name to the maximum length.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TruncateName(string name, int maxLength)
        {
            if (string.IsNullOrEmpty(name))
                return "UnnamedGraph";

            if (name.Length <= maxLength)
                return name;

            return name.Substring(0, maxLength);
        }

        /// <summary>
        /// Formats a graph name with a prefix and suffix.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatName(string prefix, string baseName, string suffix)
        {
            string name = baseName ?? "UnnamedGraph";

            if (!string.IsNullOrEmpty(prefix))
                name = $"{prefix}_{name}";

            if (!string.IsNullOrEmpty(suffix))
                name = $"{name}_{suffix}";

            return name;
        }

        /// <summary>
        /// Increments the name change count.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IncrementNameCount(int currentCount)
        {
            return currentCount + 1;
        }

        /// <summary>
        /// Validates a controller ID.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidControllerId(int id)
        {
            return id >= 0;
        }

        /// <summary>
        /// Generates a controller ID.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GenerateControllerId()
        {
            return 0; // Will be set by the extension method
        }
    }
}
