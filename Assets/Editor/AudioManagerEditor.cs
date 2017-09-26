using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;

public class AudioManagerEditor : EditorWindow
{
/// <summary>
/// 点击UnityEditor/AudioManager打开面板，拖着Project下的文件到面板上松手
/// 会统计名称和路径，并存储在文件中。下次打开会从文件中读取。
/// </summary>
    private Dictionary<string, string> AudioClipPathDic = new Dictionary<string, string>();
    private bool isRemove;
    [MenuItem("UnityEditor/AudioManager")]
    public static void CreateWindow()
    {
        AudioManagerEditor window = EditorWindow.GetWindow<AudioManagerEditor>();
        window.titleContent.text = "AudioManager";
        window.Show();
    }

    void Awake()
    {
        AudioClipPathDic.Clear();
        isRemove = false;
        Debug.Log("Application.persistentDataPath:" + Application.persistentDataPath);
        //读文件
        string jsonReadString = FileHelper.LoadAndJsonNameFile();
        Dictionary<string, string> loadDic = JsonMapper.ToObject<Dictionary<string, string>>(jsonReadString);//读取Json文件直接放到字典中
        if (loadDic != null)
        {
            AudioClipPathDic = loadDic;
        }
    }

    private Vector2 ScrollPos;//拿到返回值才能拉的动
    void OnGUI()
    {
        if (isRemove)
        {//只有操作的时候才写文件
            WriteJsonFile();
        }
        GUILayout.BeginHorizontal();//1

        GUILayout.BeginVertical();//2

        GUILayout.BeginHorizontal();//3
        GUILayout.Label("AudioName");
        GUILayout.Label("AudioPath");
        GUILayout.Label("Handler");
        GUILayout.EndHorizontal();//3

        ScrollPos = GUILayout.BeginScrollView(ScrollPos,false,true);
        if (AudioClipPathDic.Count > 0)
        {
            foreach (var eachKey in AudioClipPathDic.Keys)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(eachKey);
                string eachValue;
                if (AudioClipPathDic.TryGetValue(eachKey,out eachValue))
                {
                    GUILayout.Label(eachValue);
                }
                if (GUILayout.Button("delete"))
                {
                    AudioClipPathDic.Remove(eachKey);
                    isRemove = true;
                    return;//直接返回，下一帧来的时候会刷新字典的，否则会对字典造成影响。
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();

        GUILayout.EndVertical();//2
        GUILayout.Space(50f);

        GUILayout.BeginVertical();//4
        GUILayout.Label("Drag Audio Clip To This");
        EditorGUILayout.GetControlRect(GUILayout.Width(200f), GUILayout.Height(200f));//占位
        GUILayout.Label("Drag Audio Clip Up");
        if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
        {
            foreach (var eachPath in DragAndDrop.paths)
            {
                if (!string.IsNullOrEmpty(eachPath) && Event.current.type == EventType.DragExited)//在面板内释放
                {
                    int lastSplite = eachPath.LastIndexOf("/");
                    int lastDot = eachPath.LastIndexOf(".");
                    string audioName = eachPath.Substring(lastSplite+1, (lastDot - lastSplite)-1);
                    if (!AudioClipPathDic.ContainsKey(audioName))
                    {
                        AudioClipPathDic.Add(audioName, eachPath);
                    }
                    else
                    {
                        Debug.Log("Key is Exists!");
                    }
                }
            }
            WriteJsonFile();
        }
        

        GUILayout.EndVertical();//4

        GUILayout.EndHorizontal();//1
    }

    private void WriteJsonFile()
    {
//写json文件
        string jsonString = JsonMapper.ToJson(AudioClipPathDic);
        FileHelper.createOrWriteJsonFile(jsonString);
    }
}
