using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif


public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator i;
    public LayerMask boundingBoxMask;
    public int size;
    
    public DungeonRoom startRoom;
    public DungeonRoom endRoom;
    public DungeonRoom verticalToHorizontalHelper;
    public bool useVerticalToHorizontalHelper = true;
    public Vector3 startingPoint;
    public GameObject deadEnd;
    public bool generatedEnd;
    public bool shouldGenerateEnd;
    public List<DungeonRoom> horizontalRooms;
    public List<DungeonRoom> verticalRooms;
    public float validationTime;

    private List<DungeonRoom> dungeon = new List<DungeonRoom>();
    private List<DungeonRoom> ungeneratedRooms = new List<DungeonRoom>();
    
    
    //InfiniteRoomStuff
    public int roomAmountBeforeAndAfterPlayer;

    public UnityEvent onFinishedGeneration;
    private void Awake()
    {
        i = this;
    }

    public void Start()
    {
        ReGenerateDungeon();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ReGenerateDungeon();
            }
        }
        
    }

    private IEnumerator GenerateDungeon()
    {
        DungeonRoom startingRoom = Instantiate(startRoom, startingPoint, Quaternion.identity, transform);
        AddToDungeon(startingRoom);
        startingRoom.DistanceFromStartingPoint = 0;
        
        while (ungeneratedRooms.Count > 0)
        {
            DungeonRoom room = ungeneratedRooms[0];
            
            yield return room.Generate();
            
            Debug.Log("Room generated.");
            
            ungeneratedRooms = dungeon.Where(x => !x.generated).ToList();
        }
        
        Debug.Log("Dungeon generated.");
    }

    public void AddToDungeon(DungeonRoom room)
    {
        dungeon.Add(room);
        ungeneratedRooms = dungeon.Where(x => !x.generated).ToList();
    }
    public void RemoveFromDungeon(DungeonRoom room)
    {
        try
        {
            Destroy(room.gameObject);
            dungeon.Remove(room);
            ungeneratedRooms = dungeon.Where(x => !x.generated).ToList();
        }
        catch
        {
            Debug.LogWarning("The room you were trying to access was already destroyed.");
        }
        
    }

    public List<DungeonRoom> GetRoomsOfType(DungeonRoomConnection.ConnectionType type)
    {
        List<DungeonRoom> result = new List<DungeonRoom>();
        switch (type)
        {
            case DungeonRoomConnection.ConnectionType.Horizontal:
                result.AddRange(horizontalRooms);
                break;
            
            case DungeonRoomConnection.ConnectionType.Vertical:
                result.AddRange(verticalRooms);
                break;
        }
        return result;
    }

    private void ReGenerateDungeon()
    {
        StopAllCoroutines();
        foreach (var room in dungeon)
        {
            Destroy(room.gameObject);
        }
        dungeon.Clear();
        generatedEnd = false;
        
        StartCoroutine(GenerateDungeon());
    }

    public void StopInfiniteGeneration()
    {
        shouldGenerateEnd = true;
    }
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(DungeonGenerator))]
public class KamCustomEditor_DungeonGenerator : KamCustomEditor
{
    private DungeonGenerator editorTarget;
    
    private SerializedObject mySO;
    private SerializedProperty horizontalRooms;
    private SerializedProperty verticalRooms;

    private void OnEnable()
    {
        editorTarget = (DungeonGenerator)target;

        mySO = new SerializedObject(editorTarget);
        horizontalRooms = mySO.FindProperty("horizontalRooms");
        verticalRooms = mySO.FindProperty("verticalRooms");
    }

    public override void GameDesignerInspector()
    {
        EditorGUILayout.LabelField("Parameters", EditorStyles.centeredGreyMiniLabel);
         
        editorTarget.size = EditorGUILayout.IntField(
            new GUIContent(
                "Dungeon Size",
                "The amount of modules that are generated between the start point and end point of the dungeon."),
            editorTarget.size);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Rooms", EditorStyles.centeredGreyMiniLabel);

        editorTarget.startRoom = (DungeonRoom)EditorGUILayout.ObjectField(new GUIContent(
            "Starting Room",
            "The very first room that is generated in the dungeon.")
            , editorTarget.startRoom, typeof(DungeonRoom), false);
        editorTarget.endRoom = (DungeonRoom)EditorGUILayout.ObjectField(new GUIContent(
                "End Room",
                "The end room of the dungeon.")
            , editorTarget.endRoom, typeof(DungeonRoom), false);
        
        EditorGUILayout.PropertyField(horizontalRooms, true);
        EditorGUILayout.PropertyField(verticalRooms, true);
    }
}
#endif
#endregion