using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PS.Event
{
    [CustomEditor(typeof(PS.Event.Event))]
    public class EventEditor : Editor
    {
        PS.Event.Event _event;
        ReorderableList reorderableList;

        private void OnEnable()
        {
            _event = target as PS.Event.Event;
            var cutScenes = serializedObject.FindProperty("cutScenes");
            reorderableList = new ReorderableList(serializedObject, cutScenes);

            reorderableList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "컷씬"); };
            reorderableList.onAddCallback += AddNewCutScene;
            reorderableList.elementHeightCallback = (index) => 
            {
                //return EditorGUI.GetPropertyHeight(cutScenes.GetArrayElementAtIndex(index)); 
                float height = EditorGUIUtility.singleLineHeight * 5;
                var actionsProperty = cutScenes.GetArrayElementAtIndex(index).FindPropertyRelative("actions");
                height += EditorGUI.GetPropertyHeight(actionsProperty);
                height += actionsProperty.arraySize * EditorGUIUtility.singleLineHeight;
                return height;
            };
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = cutScenes.GetArrayElementAtIndex(index);
                rect.height -= 4;
                rect.y += 2;
                EditorGUI.PropertyField(rect, element);
            };
        }

        public override void OnInspectorGUI()
        {
            Draw();
        }

        private void Draw()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("이벤트 번호");
            _event.eventIndex = EditorGUILayout.TextField(_event.eventIndex);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("이벤트 이름");
            _event.name = EditorGUILayout.TextField(_event.name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("이벤트 종류");
            _event.eventType = (EventType)EditorGUILayout.EnumPopup(_event.eventType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("시작조건");
            _event.triggerType = (TriggerType)EditorGUILayout.EnumPopup(_event.triggerType);
            EditorGUILayout.EndHorizontal();

            switch (_event.triggerType)
            {
                case TriggerType.Arrive:
                    break;
                case TriggerType.Come:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("X");
                    EditorGUILayout.BeginHorizontal();
                    if (CmpType.None != (_event.trigger.xCmp = (CmpType)EditorGUILayout.EnumPopup(_event.trigger.xCmp)))
                        _event.trigger.xValue = EditorGUILayout.IntField(_event.trigger.xValue);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Y");
                    EditorGUILayout.BeginHorizontal();
                    if (CmpType.None != (_event.trigger.yCmp = (CmpType)EditorGUILayout.EnumPopup(_event.trigger.yCmp)))
                        _event.trigger.yValue = EditorGUILayout.IntField(_event.trigger.yValue);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    break;
                case TriggerType.Talk:
                    break;
                case TriggerType.Next:
                    break;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("종료조건");
            EditorGUILayout.TextField("");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("재생횟수");
            if (PlayAmount.Finite == (_event.playAmount = (PlayAmount)EditorGUILayout.EnumPopup(_event.playAmount)))
                _event.playCount = EditorGUILayout.IntField(_event.playCount);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("시간 정지");
            _event.stoptime = EditorGUILayout.Toggle(_event.stoptime);
            EditorGUILayout.PrefixLabel("스킵 가능");
            _event.skipable = EditorGUILayout.Toggle(_event.skipable);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("다음 이벤트");
            _event.followingEvent = EditorGUILayout.IntField(_event.followingEvent);
            EditorGUILayout.EndHorizontal();


            reorderableList.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void AddNewCutScene(ReorderableList list)
        {
            var cutScenes = serializedObject.FindProperty("cutScenes");
            cutScenes.arraySize++;

            list.index = cutScenes.arraySize - 1;

            var element = cutScenes.GetArrayElementAtIndex(list.index);
            element.FindPropertyRelative("cutSceneIndex").stringValue = _event.eventIndex + "_" + list.index.ToString();
            element.FindPropertyRelative("actions").ClearArray();
        }
    }

}