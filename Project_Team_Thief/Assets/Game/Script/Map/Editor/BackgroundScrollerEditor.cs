using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BackgroundScroller.ScrollLayer))]
public class ScrollLayerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            var height = EditorGUIUtility.singleLineHeight;
            position.height = EditorGUIUtility.singleLineHeight;
            var rect = new Rect(position);

            var layerTag = property.FindPropertyRelative("layerTag");
            var vertical = property.FindPropertyRelative("verticalScrolling");
            var dist = property.FindPropertyRelative("distance");

            layerTag.stringValue = EditorGUI.TextField(rect, "태그 이름", layerTag.stringValue);
            rect.y += height;
            rect.width /= 2;
            vertical.boolValue = EditorGUI.Toggle(rect, "수직 스크롤 여부", vertical.boolValue);
            rect.x += rect.width;
            EditorGUIUtility.labelWidth = 30;
            dist.floatValue = EditorGUI.FloatField(rect, "거리", dist.floatValue);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + 2;
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(BackgroundScroller))]
public class BackgroundScrollerEditor : Editor
{
    SerializedProperty layersProperty;
    SerializedProperty dampProperty;

    private void OnEnable()
    {
        layersProperty = serializedObject.FindProperty("layers");
        dampProperty = serializedObject.FindProperty("damping");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.HelpBox("각 레이어는 따로 스크롤 되는 배경의 집합입니다.\n" +
        "초점은 10이며 거리를 10 보다 크게 줄시 멀리 있는 것처럼 행동하고 10 보다 작게 줄시 가까이에 있는 것처럼 행동합니다.\n" +
        "태그 이름에는 배경 오브젝트들이 속한 태그 이름을 넣어주면 됩니다. 그럼 컴포넌트가 알아서 배경들을 수집합니다.", MessageType.Info);

        dampProperty.floatValue = EditorGUILayout.Slider("댐프 값",dampProperty.floatValue, 0.0f, 1.0f);

        for (int i = 0; i < layersProperty.arraySize; i++)
        {
            EditorGUILayout.LabelField("#" + (i + 1).ToString());
            EditorGUILayout.PropertyField(layersProperty.GetArrayElementAtIndex(i));
        }

        GUILayout.BeginHorizontal();
        
        if(GUILayout.Button("레이어 생성"))
        {
            layersProperty.InsertArrayElementAtIndex(layersProperty.arraySize);
        }
        if(GUILayout.Button("레이어 삭제"))
        {
            if(layersProperty.arraySize > 0)
            {
                layersProperty.DeleteArrayElementAtIndex(layersProperty.arraySize - 1);
            }
        }

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}

