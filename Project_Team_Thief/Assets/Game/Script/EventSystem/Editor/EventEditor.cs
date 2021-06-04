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
                for (int i = 0; i < actionsProperty.arraySize; i++)
                {
                    height += EditorGUI.GetPropertyHeight(actionsProperty.GetArrayElementAtIndex(i));
                }
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

            float originalWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("이벤트 이름");
            _event.name = EditorGUILayout.TextField(_event.name);
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("이벤트 종류");
            _event.eventType = (EventType)EditorGUILayout.EnumPopup(_event.eventType);
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = originalWidth;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("시작조건");
            _event.triggerType = (TriggerType)EditorGUILayout.EnumPopup(_event.triggerType);
            _event.bFollowed = EditorGUILayout.Toggle("다른 이벤트 이후에 실행되는지", _event.bFollowed);
            EditorGUILayout.EndHorizontal();

            PS.Event.TriggerCondition newTrigger = new TriggerCondition();
            switch (_event.triggerType)
            {
                case TriggerType.TRUE:
                    break;
                case TriggerType.Come:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("X");
                    EditorGUILayout.BeginHorizontal();
                    var xType = (newTrigger.xCmp = (CmpType)EditorGUILayout.EnumPopup(_event.trigger.xCmp));
                    if (CmpType.None != xType)
                        newTrigger.xValue = EditorGUILayout.IntField(_event.trigger.xValue);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Y");
                    EditorGUILayout.BeginHorizontal();
                    var yType = (newTrigger.yCmp = (CmpType)EditorGUILayout.EnumPopup(_event.trigger.yCmp));
                    if (CmpType.None != yType)
                        newTrigger.yValue = EditorGUILayout.IntField(_event.trigger.yValue);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    break;
                case TriggerType.Queue:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("이름");
                    newTrigger.NPCname = EditorGUILayout.TextField(_event.trigger.NPCname);
                    EditorGUILayout.EndHorizontal();
                    break;
            }
            _event.trigger = newTrigger;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("종료조건");
            _event.stopCondition = EditorGUILayout.TextField(_event.stopCondition);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("재생횟수");
            var playType = (_event.playAmount = (PlayAmount)EditorGUILayout.EnumPopup(_event.playAmount));
            if (PlayAmount.Finite == playType)
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
            _event.followingEvent = EditorGUILayout.TextField(_event.followingEvent);
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