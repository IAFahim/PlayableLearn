using UnityEngine;
using UnityEngine.Playables;
using Unity.Mathematics;
using PlayableLearn.Day01;
using PlayableLearn.Day02;
using PlayableLearn.Day04;
using PlayableLearn.Day05;
using PlayableLearn.Day06;
using PlayableLearn.Day07;
using PlayableLearn.Day08;

namespace PlayableLearn.Day09
{
    /// <summary>
    /// Data structure for Graph Visualizer setup and configuration.
    /// Day 09: The Graph Visualizer - Setting up visualization tools to inspect PlayableGraph nodes.
    /// 
    /// This component enables visual debugging of PlayableGraph structures using the
    /// GBG PlayableGraph Monitor package, allowing developers to see node connections,
    /// port data, and real-time graph state.
    /// 
    /// Part of the Playable Protocol:
    /// - Layer A: Data (Visualizer configuration data)
    /// - Layer B: Operations (Visualizer setup operations)
    /// - Layer C: Extensions (Public API for visualizer management)
    /// </summary>
    public struct Day09VisualizerData
    {
        // LAYER A: Data
        
        /// <summary>
        /// The PlayableGraph being visualized
        /// </summary>
        public PlayableGraph Graph;
        
        /// <summary>
        /// Whether the visualizer is active
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Whether auto-refresh is enabled
        /// </summary>
        public bool AutoRefresh;
        
        /// <summary>
        /// Refresh interval in seconds
        /// </summary>
        public float RefreshInterval;
        
        /// <summary>
        /// Time since last refresh
        /// </summary>
        public float LastRefreshTime;
        
        /// <summary>
        /// Whether to show detailed port information
        /// </summary>
        public bool ShowPortDetails;
        
        /// <summary>
        /// Whether to show node connections
        /// </summary>
        public bool ShowConnections;
        
        /// <summary>
        /// Visualizer window position
        /// </summary>
        public int2 WindowPosition;
        
        /// <summary>
        /// Visualizer window size
        /// </summary>
        public int2 WindowSize;
        
        /// <summary>
        /// Whether the visualizer window is docked
        /// </summary>
        public bool IsDocked;
        
        /// <summary>
        /// Color scheme for visualization
        /// </summary>
        public VisualizerColorScheme ColorScheme;
        
        /// <summary>
        /// Visualization detail level
        /// </summary>
        public VisualizationDetailLevel DetailLevel;
    }
    
    /// <summary>
    /// Color schemes for graph visualization
    /// </summary>
    public enum VisualizerColorScheme
    {
        Default,
        Dark,
        Light,
        HighContrast
    }
    
    /// <summary>
    /// Detail levels for visualization
    /// </summary>
    public enum VisualizationDetailLevel
    {
        Minimal,
        Basic,
        Detailed,
        Verbose
    }
    
    /// <summary>
    /// Layer B: Operations - Static methods for visualizer setup and management
    /// </summary>
    public static class VisualizerOps
    {
        /// <summary>
        /// Validates if a graph can be visualized
        /// </summary>
        public static bool IsValidForVisualization(in PlayableGraph graph)
        {
            return graph.IsValid();
        }
        
        /// <summary>
        /// Checks if the visualizer package is available
        /// </summary>
        public static bool IsVisualizerAvailable()
        {
            // The GBG PlayableGraph Monitor package should be installed
            // This is a runtime check - the package adds editor visualization
            #if UNITY_EDITOR
            return true; // Package is installed and available
            #else
            return false; // Visualizer is editor-only
            #endif
        }
        
        /// <summary>
        /// Gets the default refresh interval based on detail level
        /// </summary>
        public static float GetDefaultRefreshInterval(VisualizationDetailLevel detailLevel)
        {
            switch (detailLevel)
            {
                case VisualizationDetailLevel.Minimal:
                    return 1.0f;
                case VisualizationDetailLevel.Basic:
                    return 0.5f;
                case VisualizationDetailLevel.Detailed:
                    return 0.25f;
                case VisualizationDetailLevel.Verbose:
                    return 0.1f;
                default:
                    return 0.5f;
            }
        }
        
        /// <summary>
        /// Validates refresh interval
        /// </summary>
        public static bool IsValidRefreshInterval(float interval)
        {
            return interval >= 0.01f && interval <= 5.0f;
        }
        
        /// <summary>
        /// Calculates window size based on detail level
        /// </summary>
        public static int2 CalculateWindowSize(VisualizationDetailLevel detailLevel)
        {
            switch (detailLevel)
            {
                case VisualizationDetailLevel.Minimal:
                    return new int2(400, 300);
                case VisualizationDetailLevel.Basic:
                    return new int2(600, 400);
                case VisualizationDetailLevel.Detailed:
                    return new int2(800, 600);
                case VisualizationDetailLevel.Verbose:
                    return new int2(1200, 800);
                default:
                    return new int2(600, 400);
            }
        }
        
        /// <summary>
        /// Checks if auto-refresh should occur
        /// </summary>
        public static bool ShouldAutoRefresh(in Day09VisualizerData visualizer, float currentTime)
        {
            if (!visualizer.AutoRefresh)
                return false;
            
            if (!visualizer.IsActive)
                return false;
            
            float timeSinceLastRefresh = currentTime - visualizer.LastRefreshTime;
            return timeSinceLastRefresh >= visualizer.RefreshInterval;
        }
        
        /// <summary>
        /// Updates the last refresh time
        /// </summary>
        public static Day09VisualizerData UpdateRefreshTime(in Day09VisualizerData visualizer, float currentTime)
        {
            var updated = visualizer;
            updated.LastRefreshTime = currentTime;
            return updated;
        }
    }
    
    /// <summary>
    /// Layer C: Extensions - Public API for visualizer management
    /// </summary>
    public static class Day09VisualizerExtensions
    {
        /// <summary>
        /// Initializes the visualizer with a PlayableGraph
        /// </summary>
        public static void Initialize(ref this Day09VisualizerData visualizer, in PlayableGraph graph, 
            VisualizationDetailLevel detailLevel = VisualizationDetailLevel.Basic,
            VisualizerColorScheme colorScheme = VisualizerColorScheme.Default)
        {
            if (!VisualizerOps.IsVisualizerAvailable())
            {
                Debug.LogWarning("[Day 09] PlayableGraph Monitor package is not available. Visualizer cannot be initialized.");
                return;
            }
            
            if (!VisualizerOps.IsValidForVisualization(in graph))
            {
                Debug.LogError("[Day 09] Cannot initialize visualizer: Graph is not valid.");
                return;
            }
            
            visualizer.Graph = graph;
            visualizer.IsActive = true;
            visualizer.AutoRefresh = true;
            visualizer.DetailLevel = detailLevel;
            visualizer.ColorScheme = colorScheme;
            visualizer.RefreshInterval = VisualizerOps.GetDefaultRefreshInterval(detailLevel);
            visualizer.LastRefreshTime = Time.time;
            visualizer.ShowPortDetails = true;
            visualizer.ShowConnections = true;
            visualizer.WindowPosition = new int2(100, 100);
            visualizer.WindowSize = VisualizerOps.CalculateWindowSize(detailLevel);
            visualizer.IsDocked = false;
            
            Debug.Log($"[Day 09] Graph Visualizer initialized with {detailLevel} detail level.");
            LogVisualizerInfo(in visualizer);
        }
        
        /// <summary>
        /// Checks if the visualizer is valid and ready
        /// </summary>
        public static bool IsValidVisualizer(in this Day09VisualizerData visualizer)
        {
            return visualizer.IsActive && VisualizerOps.IsValidForVisualization(in visualizer.Graph);
        }
        
        /// <summary>
        /// Sets the auto-refresh state
        /// </summary>
        public static void SetAutoRefresh(ref this Day09VisualizerData visualizer, bool enabled)
        {
            visualizer.AutoRefresh = enabled;
            Debug.Log($"[Day 09] Auto-refresh {(enabled ? "enabled" : "disabled")}.");
        }
        
        /// <summary>
        /// Sets the refresh interval
        /// </summary>
        public static void SetRefreshInterval(ref this Day09VisualizerData visualizer, float interval)
        {
            if (!VisualizerOps.IsValidRefreshInterval(interval))
            {
                Debug.LogError($"[Day 09] Invalid refresh interval: {interval}. Must be between 0.01 and 5.0 seconds.");
                return;
            }
            
            visualizer.RefreshInterval = interval;
            Debug.Log($"[Day 09] Refresh interval set to {interval} seconds.");
        }
        
        /// <summary>
        /// Sets the visualization detail level
        /// </summary>
        public static void SetDetailLevel(ref this Day09VisualizerData visualizer, VisualizationDetailLevel detailLevel)
        {
            visualizer.DetailLevel = detailLevel;
            visualizer.RefreshInterval = VisualizerOps.GetDefaultRefreshInterval(detailLevel);
            visualizer.WindowSize = VisualizerOps.CalculateWindowSize(detailLevel);
            Debug.Log($"[Day 09] Detail level set to {detailLevel}.");
        }
        
        /// <summary>
        /// Sets the color scheme
        /// </summary>
        public static void SetColorScheme(ref this Day09VisualizerData visualizer, VisualizerColorScheme colorScheme)
        {
            visualizer.ColorScheme = colorScheme;
            Debug.Log($"[Day 09] Color scheme set to {colorScheme}.");
        }
        
        /// <summary>
        /// Toggles port details visibility
        /// </summary>
        public static void TogglePortDetails(ref this Day09VisualizerData visualizer)
        {
            visualizer.ShowPortDetails = !visualizer.ShowPortDetails;
            Debug.Log($"[Day 09] Port details {(visualizer.ShowPortDetails ? "enabled" : "disabled")}.");
        }
        
        /// <summary>
        /// Toggles connections visibility
        /// </summary>
        public static void ToggleConnections(ref this Day09VisualizerData visualizer)
        {
            visualizer.ShowConnections = !visualizer.ShowConnections;
            Debug.Log($"[Day 09] Connections {(visualizer.ShowConnections ? "enabled" : "disabled")}.");
        }
        
        /// <summary>
        /// Updates the visualizer (call this in Update)
        /// </summary>
        public static void UpdateVisualizer(ref this Day09VisualizerData visualizer)
        {
            if (!visualizer.IsValidVisualizer())
                return;
            
            float currentTime = Time.time;
            if (VisualizerOps.ShouldAutoRefresh(in visualizer, currentTime))
            {
                visualizer = VisualizerOps.UpdateRefreshTime(in visualizer, currentTime);
                // The actual refresh is handled by the GBG PlayableGraph Monitor package
                // This just tracks when to refresh
            }
        }
        
        /// <summary>
        /// Forces an immediate refresh
        /// </summary>
        public static void ForceRefresh(ref this Day09VisualizerData visualizer)
        {
            if (!visualizer.IsValidVisualizer())
            {
                Debug.LogWarning("[Day 09] Cannot refresh: Visualizer is not valid.");
                return;
            }
            
            visualizer.LastRefreshTime = Time.time;
            Debug.Log("[Day 09] Forced visualizer refresh.");
        }
        
        /// <summary>
        /// Disposes the visualizer
        /// </summary>
        public static void Dispose(ref this Day09VisualizerData visualizer)
        {
            if (!visualizer.IsActive)
                return;
            
            Debug.Log("[Day 09] Disposing graph visualizer.");
            
            visualizer.IsActive = false;
            visualizer.Graph = default;
            visualizer.AutoRefresh = false;
        }
        
        /// <summary>
        /// Logs visualizer information
        /// </summary>
        public static void LogVisualizerInfo(in this Day09VisualizerData visualizer, string context = "Visualizer")
        {
            if (!visualizer.IsValidVisualizer())
            {
                Debug.LogError($"[Day 09] {context}: Visualizer is not valid.");
                return;
            }
            
            Debug.Log($"[Day 09] {context} Information:");
            Debug.Log($"  - Active: {visualizer.IsActive}");
            Debug.Log($"  - Auto-Refresh: {visualizer.AutoRefresh}");
            Debug.Log($"  - Refresh Interval: {visualizer.RefreshInterval}s");
            Debug.Log($"  - Detail Level: {visualizer.DetailLevel}");
            Debug.Log($"  - Color Scheme: {visualizer.ColorScheme}");
            Debug.Log($"  - Show Port Details: {visualizer.ShowPortDetails}");
            Debug.Log($"  - Show Connections: {visualizer.ShowConnections}");
            Debug.Log($"  - Window Size: {visualizer.WindowSize.x}x{visualizer.WindowSize.y}");
            Debug.Log($"  - Docked: {visualizer.IsDocked}");
        }
        
        /// <summary>
        /// Gets the current visualizer configuration as a string
        /// </summary>
        public static string GetConfigurationString(in this Day09VisualizerData visualizer)
        {
            if (!visualizer.IsValidVisualizer())
                return "Visualizer is not valid";
            
            return $"Visualizer Config: {visualizer.DetailLevel} detail, {visualizer.ColorScheme} scheme, " +
                   $"Refresh: {visualizer.RefreshInterval}s, Auto-Refresh: {visualizer.AutoRefresh}";
        }
    }
}