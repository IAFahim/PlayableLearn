using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day08
{
    /// <summary>
    /// Extension methods for Day08NamedGraphData.
    /// Provides the high-level interface for graph naming operations.
    ///
    /// Day 08: The Director Name
    /// Adds naming functionality to PlayableGraphs for Profiler visibility.
    /// </summary>
    public static class Day08NamedGraphExtensions
    {
        private static int nextControllerId = 0;
        private const int MaxGraphNameLength = 128;

        /// <summary>
        /// Initializes a named graph handle.
        /// </summary>
        public static void Initialize(ref this Day08NamedGraphData namedGraph, in PlayableGraph graph, string name)
        {
            if (!graph.IsValid())
            {
                Debug.LogError("[Day 08] Cannot initialize named graph: Graph is not valid.");
                namedGraph = new Day08NamedGraphData { IsActive = false, IsValidControl = false };
                return;
            }

            string sanitizedName = GraphNamingOps.SanitizeName(name);
            sanitizedName = GraphNamingOps.TruncateName(sanitizedName, MaxGraphNameLength);

            // Set the name on the graph for Profiler visibility
            graph.SetName(sanitizedName);

            namedGraph = new Day08NamedGraphData
            {
                Graph = graph,
                GraphName = sanitizedName,
                PreviousName = string.Empty,
                IsActive = true,
                ControllerId = nextControllerId++,
                IsValidControl = true,
                IsNameSet = true,
                NameChangeCount = 1
            };

            Debug.Log($"[Day 08] Named graph initialized: '{sanitizedName}' (Controller ID: {namedGraph.ControllerId})");
        }

        /// <summary>
        /// Disposes the named graph handle.
        /// </summary>
        public static void Dispose(ref this Day08NamedGraphData namedGraph)
        {
            if (!namedGraph.IsActive)
                return;

            Debug.Log($"[Day 08] Named graph disposed: '{namedGraph.GraphName}' (Controller ID: {namedGraph.ControllerId})");

            namedGraph = new Day08NamedGraphData
            {
                IsActive = false,
                IsValidControl = false
            };
        }

        /// <summary>
        /// Sets a new name for the graph.
        /// </summary>
        public static void SetName(ref this Day08NamedGraphData namedGraph, string newName)
        {
            if (!namedGraph.IsValidControl())
            {
                Debug.LogError("[Day 08] Cannot set name: Named graph is not valid.");
                return;
            }

            string sanitizedName = GraphNamingOps.SanitizeName(newName);
            sanitizedName = GraphNamingOps.TruncateName(sanitizedName, MaxGraphNameLength);

            if (!GraphNamingOps.HasNameChanged(sanitizedName, namedGraph.GraphName))
            {
                // Name hasn't changed, no action needed
                return;
            }

            // Update previous name
            namedGraph.PreviousName = namedGraph.GraphName;

            // Set the new name on the graph
            namedGraph.Graph.SetName(sanitizedName);
            namedGraph.GraphName = sanitizedName;

            // Increment name change count
            namedGraph.NameChangeCount = GraphNamingOps.IncrementNameCount(namedGraph.NameChangeCount);

            Debug.Log($"[Day 08] Graph name changed: '{namedGraph.PreviousName}' -> '{sanitizedName}' (Change #{namedGraph.NameChangeCount})");
        }

        /// <summary>
        /// Gets the current name of the graph.
        /// </summary>
        public static string GetName(this in Day08NamedGraphData namedGraph)
        {
            if (!namedGraph.IsValidControl())
            {
                Debug.LogWarning("[Day 08] Cannot get name: Named graph is not valid.");
                return "InvalidGraph";
            }

            return namedGraph.GraphName;
        }

        /// <summary>
        /// Gets the previous name of the graph (before the last change).
        /// </summary>
        public static string GetPreviousName(this in Day08NamedGraphData namedGraph)
        {
            if (!namedGraph.IsValidControl())
            {
                Debug.LogWarning("[Day 08] Cannot get previous name: Named graph is not valid.");
                return string.Empty;
            }

            return namedGraph.PreviousName;
        }

        /// <summary>
        /// Checks if the graph name has changed.
        /// </summary>
        public static bool HasNameChanged(this in Day08NamedGraphData namedGraph)
        {
            if (!namedGraph.IsValidControl())
                return false;

            return GraphNamingOps.HasNameChanged(namedGraph.GraphName, namedGraph.PreviousName);
        }

        /// <summary>
        /// Gets the number of times the graph name has changed.
        /// </summary>
        public static int GetNameChangeCount(this in Day08NamedGraphData namedGraph)
        {
            if (!namedGraph.IsValidControl())
                return 0;

            return namedGraph.NameChangeCount;
        }

        /// <summary>
        /// Formats and sets a new name with prefix and suffix.
        /// </summary>
        public static void SetFormattedName(ref this Day08NamedGraphData namedGraph, string prefix, string baseName, string suffix)
        {
            if (!namedGraph.IsValidControl())
            {
                Debug.LogError("[Day 08] Cannot set formatted name: Named graph is not valid.");
                return;
            }

            string formattedName = GraphNamingOps.FormatName(prefix, baseName, suffix);
            namedGraph.SetName(formattedName);
        }

        /// <summary>
        /// Generates and sets a unique name with a counter suffix.
        /// </summary>
        public static void SetUniqueName(ref this Day08NamedGraphData namedGraph, string baseName, int counter)
        {
            if (!namedGraph.IsValidControl())
            {
                Debug.LogError("[Day 08] Cannot set unique name: Named graph is not valid.");
                return;
            }

            string uniqueName = GraphNamingOps.GenerateUniqueName(baseName, counter);
            namedGraph.SetName(uniqueName);
        }

        /// <summary>
        /// Renames the graph to a unique name based on the current name.
        /// </summary>
        public static void RenameWithCounter(ref this Day08NamedGraphData namedGraph)
        {
            if (!namedGraph.IsValidControl())
            {
                Debug.LogError("[Day 08] Cannot rename with counter: Named graph is not valid.");
                return;
            }

            namedGraph.SetUniqueName(namedGraph.GraphName, namedGraph.NameChangeCount);
        }

        /// <summary>
        /// Checks if the named graph is valid.
        /// </summary>
        public static bool IsValidControl(this in Day08NamedGraphData namedGraph)
        {
            return namedGraph.IsActive && namedGraph.IsValidControl && namedGraph.Graph.IsValid();
        }

        /// <summary>
        /// Checks if the graph name is set.
        /// </summary>
        public static bool IsNameSet(this in Day08NamedGraphData namedGraph)
        {
            if (!namedGraph.IsValidControl())
                return false;

            return namedGraph.IsNameSet && GraphNamingOps.IsValidName(namedGraph.GraphName);
        }

        /// <summary>
        /// Gets the controller ID for debugging.
        /// </summary>
        public static int GetControllerId(this in Day08NamedGraphData namedGraph)
        {
            return namedGraph.ControllerId;
        }

        /// <summary>
        /// Logs detailed information about the named graph.
        /// </summary>
        public static void LogNamedGraphInfo(this in Day08NamedGraphData namedGraph, string context)
        {
            if (!namedGraph.IsValidControl())
            {
                Debug.LogError($"[Day 08] {context}: Named graph is not valid.");
                return;
            }

            Debug.Log($"[Day 08] {context}:");
            Debug.Log($"  - Name: '{namedGraph.GraphName}'");
            Debug.Log($"  - Previous Name: '{namedGraph.PreviousName}'");
            Debug.Log($"  - Controller ID: {namedGraph.ControllerId}");
            Debug.Log($"  - Name Changes: {namedGraph.NameChangeCount}");
            Debug.Log($"  - Graph Valid: {namedGraph.Graph.IsValid()}");
            Debug.Log($"  - Is Playing: {namedGraph.Graph.IsPlaying()}");
        }

        /// <summary>
        /// Resets the static controller ID counter (useful for testing).
        /// </summary>
        public static void ResetControllerIdCounter()
        {
            nextControllerId = 0;
            Debug.Log("[Day 08] Controller ID counter reset.");
        }
    }
}
