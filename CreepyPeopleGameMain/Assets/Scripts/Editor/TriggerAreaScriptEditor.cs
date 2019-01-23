using Assets.Scripts.Main.Components;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    //[CustomEditor(typeof(TriggerAreaScript))]
    public class TriggerAreaScriptEditor : UnityEditor.Editor
    {
        //enum displayFieldType { DisplayAsAutomaticFields, DisplayAsCustomizableGUIFields }
        //displayFieldType DisplayFieldType;

        TriggerAreaScript t;
        SerializedObject GetTarget;
        SerializedProperty SoundList;
        SerializedProperty EnemyList;
        int ListSize;

        void OnEnable()
        {
            t = (TriggerAreaScript)target;
            GetTarget = new SerializedObject(t);
            SoundList = GetTarget.FindProperty("AudiosToTrigger"); // Find the List in our script and create a refrence of it
            EnemyList = GetTarget.FindProperty("EnemiesToTrigger");

        }

        public override void OnInspectorGUI()
        {
            //Update our list

            GetTarget.Update();

            //Choose how to display the list<> Example purposes only
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            //DisplayFieldType = (displayFieldType)EditorGUILayout.EnumPopup("", DisplayFieldType);

            //Resize our list
            EditorGUILayout.Space();
            ListSize = SoundList.arraySize;
            ListSize = EditorGUILayout.IntField("List Size", ListSize);

            if (ListSize != SoundList.arraySize)
            {
                while (ListSize > SoundList.arraySize)
                {
                    SoundList.InsertArrayElementAtIndex(SoundList.arraySize);
                }
                while (ListSize < SoundList.arraySize)
                {
                    SoundList.DeleteArrayElementAtIndex(SoundList.arraySize - 1);
                }
            }

            //EditorGUILayout.Space();
            //EditorGUILayout.Space();
            //EditorGUILayout.LabelField("Or");
            //EditorGUILayout.Space();
            //EditorGUILayout.Space();

            //Or add a new item to the List<> with a button
            EditorGUILayout.LabelField("Add a new item with a button");

            if (GUILayout.Button("Add New"))
            {
                t.AudiosToTrigger.Add(new TriggerAreaScript.AudioTrigger());
            }

            EditorGUILayout.Space();

            //Display our list to the inspector window

            for (int i = 0; i < SoundList.arraySize; i++)
            {
                SerializedProperty MyListRef = SoundList.GetArrayElementAtIndex(i);
                SerializedProperty AudioSource = MyListRef.FindPropertyRelative("AudioSource");
                SerializedProperty AudioClip = MyListRef.FindPropertyRelative("AudioClip");
                SerializedProperty TriggerOnce = MyListRef.FindPropertyRelative("TriggerOnce");


                // Display the property fields in two ways.

                //if (DisplayFieldType == 0)
                //{
                // Choose to display automatic or custom field types. This is only for example to help display automatic and custom fields.
                //1. Automatic, No customization <-- Choose me I'm automatic and easy to setup
                EditorGUILayout.LabelField("Automatic Field By Property Type");
                EditorGUILayout.PropertyField(AudioSource);
                EditorGUILayout.PropertyField(AudioClip);
                EditorGUILayout.PropertyField(TriggerOnce);
                //}
                //else
                //{
                //Or

                //2 : Full custom GUI Layout <-- Choose me I can be fully customized with GUI options.
                //EditorGUILayout.LabelField("Customizable Field With GUI");
                //AudioSource.objectReferenceValue = EditorGUILayout.ObjectField("Audio Source`", AudioSource.objectReferenceValue, typeof(AudioSource), true);
                //AudioClip.objectReferenceValue = EditorGUILayout.ObjectField("Audio Clip`", AudioClip.objectReferenceValue, typeof(AudioClip), true);
                //TriggerOnce.boolValue = EditorGUILayout.Toggle("Trigger Once", false, GUIStyle.none, null);
                //}

                EditorGUILayout.Space();

                //Remove this index from the List
                if (GUILayout.Button("Remove This Index (" + i.ToString() + ")"))
                {
                    SoundList.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }


            //EditorGUILayout.LabelField("Enemies to Trigger:");

            //for (int i = 0; i < SoundList.arraySize; i++)
            //{
            //    SerializedProperty MyListRef = SoundList.GetArrayElementAtIndex(i);
            //    SerializedProperty EnemyReference = MyListRef.FindPropertyRelative("AudioSource");


            //    EditorGUILayout.Space();
            //}

            //Apply the changes to our list
            GetTarget.ApplyModifiedProperties();
        }
    }
}
