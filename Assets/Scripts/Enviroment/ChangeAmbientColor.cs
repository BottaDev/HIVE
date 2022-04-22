using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeAmbientColor : MonoBehaviour
{
    public static ChangeAmbientColor i;

    [System.Serializable]
    public class VolumeStruct
    {
        public string name;
        public Colors color;
        public Volume volumeScript;

        private int _index;

        public int Index { get => _index; set { _index = value; } }
    }
    public enum Colors 
    {
        red, lightblue, green
    }
    public List<VolumeStruct> colors;

    public VolumeStruct current;

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        for (int i = 0; i < colors.Count; i++)
        {
            colors[i].Index = i;
        }
    }

    public void SetColor(Colors color)
    {
        SetColor(colors.Find(x => x.color == color).Index);
    }

    void SetColor(int index)
    {
        SetEnabledOfAll(false);

        VolumeStruct vol = colors[index];
        vol.volumeScript.enabled = true;

        current = vol;
    }

    public void IterateThroughColors()
    {
        int index = current.Index + 1;
        if(index >= colors.Count)
        {
            index = 0;
        }

        SetColor(index);
    }

    void SetEnabledOfAll(bool state)
    {
        foreach (var color in colors)
        {
            color.volumeScript.enabled = state;
        }
    }
}
