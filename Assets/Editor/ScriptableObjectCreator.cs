using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
    public class ScriptableObjectCreator : EditorWindow
    {
        [MenuItem("MatchThree/Create ScriptableObject")]
        static void Init()
        {
            ScriptableObjectCreator window = EditorWindow.GetWindow<ScriptableObjectCreator>("Create ScriptableObject", true);
            window.Show();
        }

        private Vector2 scrollPosition = Vector2.zero;
        private List<Assembly> selectedAssemblies = new List<Assembly>();
        void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.HelpBox("Compiling....", MessageType.Warning, true);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (Assembly A in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] scriptableObjects = Array.FindAll<Type>(A.GetTypes(), x => x.IsSubclassOf(typeof(ScriptableObject)));
                if (scriptableObjects.Length > 0)
                {
                    GUILayout.BeginHorizontal();
                    bool oldState = selectedAssemblies.Contains(A);
                    bool newState = EditorGUILayout.Toggle(oldState, GUILayout.Width(20f));
                    if (newState && !oldState) { selectedAssemblies.Add(A); }
                    if (!newState && oldState) { selectedAssemblies.Remove(A); }
                    EditorGUILayout.LabelField(A.CodeBase);
                    GUILayout.EndHorizontal();
                }
            }

            foreach (Assembly A in selectedAssemblies)
            {
                foreach (Type type in A.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(ScriptableObject)))
                    {
                        if (GUILayout.Button(type.FullName))
                        {
                            CreateScriptableObject(type);
                            Close();
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }

        void CreateScriptableObject(Type type)
        {
            ScriptableObject asset = ScriptableObject.CreateInstance(type);
            
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/" + type.Name + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }