using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PS.Event
{
    [CustomPropertyDrawer(typeof(CutSceneData))]
    public class CutSceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = EditorGUIUtility.singleLineHeight;

                var halfWidth = position.width * 0.5f;

                var indexRect = new Rect(position) { width = halfWidth };
                var actionCountRect = new Rect(position) { width = halfWidth, x = position.x + halfWidth + 2 };
                //var actionCountRect = new Rect(position) { y = indexRect.y + EditorGUIUtility.singleLineHeight + 2 };
                var actionTimeRect = new Rect(position) { y = actionCountRect.y + EditorGUIUtility.singleLineHeight + 2 };
                var skipableRect = new Rect(position) { y = actionTimeRect.y + EditorGUIUtility.singleLineHeight + 2, width = halfWidth };
                var autopassRect = new Rect(position) { x = position.x + halfWidth + 2, y = actionTimeRect.y + EditorGUIUtility.singleLineHeight + 2, width = halfWidth };

                var IndexProperty = property.FindPropertyRelative("cutSceneIndex");
                var nameProperty = property.FindPropertyRelative("cutSceneName");
                var actionTimeProperty = property.FindPropertyRelative("actionTime");
                var skipableProperty = property.FindPropertyRelative("skipable");
                var autopassProperty = property.FindPropertyRelative("autopass");
                var actionsProperty = property.FindPropertyRelative("actions");

                EditorGUI.BeginChangeCheck();

                EditorGUIUtility.labelWidth = 40;
                IndexProperty.stringValue = EditorGUI.TextField(indexRect, "컷 번호", IndexProperty.stringValue);
                //nameProperty.stringValue = EditorGUI.TextField(nameRect, "이름", nameProperty.stringValue);
                EditorGUI.LabelField(actionCountRect, "포함 연출 수 " + actionsProperty.arraySize.ToString());
                EditorGUIUtility.labelWidth = 80;
                actionTimeProperty.floatValue = EditorGUI.FloatField(actionTimeRect, "연출시간", actionTimeProperty.floatValue);
                skipableProperty.boolValue = EditorGUI.Toggle(skipableRect, "스킵 가능", skipableProperty.boolValue);
                autopassProperty.boolValue = EditorGUI.Toggle(autopassRect, "자동 넘기기", autopassProperty.boolValue);

                position.x += 4;
                float newY = skipableRect.y + EditorGUIUtility.singleLineHeight + 2;
                for (int i = 0; i < actionsProperty.arraySize; i++)
                {
                    EditorGUI.LabelField(new Rect(position) { y = newY}, "   #" + (i + 1).ToString());
                    newY += EditorGUIUtility.singleLineHeight;
                    var action = actionsProperty.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(new Rect(position) { y = newY, height = EditorGUI.GetPropertyHeight(action) }, action);
                    newY += EditorGUI.GetPropertyHeight(action);
                }
                position.x -= 4;
                if (GUI.Button(new Rect(position) { y = newY, width = halfWidth }, "새 연출"))
                {
                    actionsProperty.InsertArrayElementAtIndex(actionsProperty.arraySize);
                }
                if (GUI.Button(new Rect(position) { y = newY, width = halfWidth, x = position.x + halfWidth }, "연출삭제"))
                {
                    if (actionsProperty.arraySize > 0)
                        actionsProperty.DeleteArrayElementAtIndex(actionsProperty.arraySize - 1);
                }

                EditorGUI.EndChangeCheck();
            }
        }
    }
}