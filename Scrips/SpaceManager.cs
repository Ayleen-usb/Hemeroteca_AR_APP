using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages availability status of computers and study rooms.
/// Attach this to a persistent GameObject called "AppManager" in the scene.
/// </summary>
public class SpaceManager : MonoBehaviour
{
    public static SpaceManager Instance { get; private set; }

    public enum SpaceType { Computer, StudyRoom }

    [System.Serializable]
    public class Space
    {
        public string id;           // e.g. "PC1", "Sala1"
        public string displayName;  // e.g. "Computador 1", "Sala de Estudio 1"
        public SpaceType type;
        public bool isAvailable;
        public Transform locationTransform; // Assign in Inspector: the 3D position of this space
    }

    [Header("Computers (PC1 - PC10)")]
    public List<Space> computers = new List<Space>();

    [Header("Study Rooms (Sala1 - Sala6)")]
    public List<Space> studyRooms = new List<Space>();

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSpaces();
    }

    void InitializeSpaces()
    {
        // Initialize 10 computers
        if (computers.Count == 0)
        {
            for (int i = 1; i <= 10; i++)
            {
                computers.Add(new Space
                {
                    id = "PC" + i,
                    displayName = "Computador " + i,
                    type = SpaceType.Computer,
                    isAvailable = true
                });
            }
        }

        // Initialize 6 study rooms
        if (studyRooms.Count == 0)
        {
            for (int i = 1; i <= 6; i++)
            {
                studyRooms.Add(new Space
                {
                    id = "Sala" + i,
                    displayName = "Sala " + i,
                    type = SpaceType.StudyRoom,
                    isAvailable = (i % 2 == 0) // Alternate for demo
                });
            }
        }
    }

    /// <summary>
    /// Get a space by its ID ("PC1", "Sala3", etc.)
    /// </summary>
    public Space GetSpace(string id)
    {
        foreach (var s in computers)
            if (s.id == id) return s;
        foreach (var s in studyRooms)
            if (s.id == id) return s;
        return null;
    }

    /// <summary>
    /// Set availability of a space. Call this to toggle status.
    /// </summary>
    public void SetAvailability(string id, bool available)
    {
        Space s = GetSpace(id);
        if (s != null)
        {
            s.isAvailable = available;
            Debug.Log($"[SpaceManager] {s.displayName} is now {(available ? "AVAILABLE" : "OCCUPIED")}");
        }
    }

    /// <summary>
    /// Toggle availability for testing purposes.
    /// </summary>
    public void ToggleAvailability(string id)
    {
        Space s = GetSpace(id);
        if (s != null) SetAvailability(id, !s.isAvailable);
    }

    public List<Space> GetAllComputers() => computers;
    public List<Space> GetAllStudyRooms() => studyRooms;
}
