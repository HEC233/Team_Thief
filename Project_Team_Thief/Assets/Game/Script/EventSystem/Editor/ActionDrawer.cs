using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PS.Event
{
    [CustomPropertyDrawer(typeof(ActionData))]
    public class ActionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                var height = EditorGUIUtility.singleLineHeight;
                position.height = EditorGUIUtility.singleLineHeight;
                var actionLength = property.FindPropertyRelative("actionLength");
                var enterDelay = property.FindPropertyRelative("enterDelay");

                var rect = new Rect(position);

                EditorGUI.PropertyField(position, property.FindPropertyRelative("type"), new GUIContent() { text = "연출방법" });

                rect.y += height + 2;
                actionLength.floatValue = EditorGUI.FloatField(rect, "연출 시간", actionLength.floatValue);
                rect.y += height;
                enterDelay.floatValue = EditorGUI.FloatField(rect, "딜레이", enterDelay.floatValue);

                switch (property.FindPropertyRelative("type").enumValueIndex)
                {
                    case (int)ActionType.Dialog:
                        var dialogName = property.FindPropertyRelative("dialogName");
                        rect.y += height;
                        dialogName.stringValue = EditorGUI.TextField(rect, "대화 번호", dialogName.stringValue);
                        break;
                    case (int)ActionType.CutScene:
                        var image = property.FindPropertyRelative("image");
                        rect.y += height;
                        EditorGUI.PropertyField(rect, image, new GUIContent() { text = "이미지" });
                        break;
                    case (int)ActionType.RestrictStart:
                        var leftDown = property.FindPropertyRelative("leftDown");
                        var rightUp = property.FindPropertyRelative("rightUp");
                        rect.y += height;
                        leftDown.vector2IntValue = EditorGUI.Vector2IntField(rect, "왼쪽아래", leftDown.vector2IntValue);
                        rect.y += height;
                        rightUp.vector2IntValue = EditorGUI.Vector2IntField(rect, "오른쪽위", rightUp.vector2IntValue);
                        break;
                    case (int)ActionType.Spawn:
                        var unitName = property.FindPropertyRelative("unitName");
                        var spawnPos = property.FindPropertyRelative("spawnPos");
                        var count = property.FindPropertyRelative("count");
                        rect.y += height;
                        unitName.stringValue = EditorGUI.TextField(rect, "유닛 이름", unitName.stringValue);
                        rect.y += height;
                        spawnPos.vector2IntValue = EditorGUI.Vector2IntField(rect, "생성 위치", spawnPos.vector2IntValue);
                        rect.y += height;
                        count.intValue = EditorGUI.IntField(rect, "생성 수", count.intValue);
                        break;
                    case (int)ActionType.CameraChange:
                        var cameraName = property.FindPropertyRelative("cameraName");
                        rect.y += height;
                        cameraName.stringValue = EditorGUI.TextField(rect, "카메라", cameraName.stringValue);
                        break;
                    case (int)ActionType.CameraShake:
                        var intensity = property.FindPropertyRelative("intensity");
                        rect.y += height;
                        EditorGUI.PropertyField(rect, intensity, new GUIContent() { text = "강도" });
                        break;
                    case (int)ActionType.CameraZoom:
                        var cameraSize = property.FindPropertyRelative("cameraSize");
                        rect.y += height;
                        cameraSize.intValue = EditorGUI.IntField(rect, "시야", cameraSize.intValue);
                        break;
                    case (int)ActionType.CharacterMove:
                        var movePos = property.FindPropertyRelative("movePos");
                        rect.y += height;
                        movePos.vector2IntValue = EditorGUI.Vector2IntField(rect, "목표 좌표", movePos.vector2IntValue);
                        break;
                    case (int)ActionType.CharacterAnimation:
                        var animationName = property.FindPropertyRelative("animationName");
                        rect.y += height;
                        animationName.stringValue = EditorGUI.TextField(rect, "애니메이션", animationName.stringValue);
                        break;
                    case (int)ActionType.Effect:
                        var effectName = property.FindPropertyRelative("effectName");
                        var effectPos = property.FindPropertyRelative("effectPos");
                        rect.y += height;
                        effectName.stringValue = EditorGUI.TextField(rect, "이펙트", effectName.stringValue);
                        rect.y += height;
                        effectPos.vector2IntValue = EditorGUI.Vector2IntField(rect, "목표 좌표", effectPos.vector2IntValue);
                        break;
                    case (int)ActionType.SFX:
                        var sfxName = property.FindPropertyRelative("sfxName");
                        rect.y += height;
                        sfxName.stringValue = EditorGUI.TextField(rect, "사운드", sfxName.stringValue);
                        break;
                    case (int)ActionType.BGMChange:
                        var bgmName = property.FindPropertyRelative("bgmName");
                        rect.y += height;
                        bgmName.stringValue = EditorGUI.TextField(rect, "사운드", bgmName.stringValue);
                        break;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var returnValue = EditorGUIUtility.singleLineHeight * 3 + 4;

            switch (property.FindPropertyRelative("type").enumValueIndex)
            {
                case (int)ActionType.Dialog:
                case (int)ActionType.CutScene:
                case (int)ActionType.CameraChange:
                case (int)ActionType.CameraShake:
                case (int)ActionType.CameraZoom:
                case (int)ActionType.CharacterMove:
                case (int)ActionType.CharacterAnimation:
                case (int)ActionType.SFX:
                case (int)ActionType.BGMChange:
                    returnValue += EditorGUIUtility.singleLineHeight;
                    break;
                case (int)ActionType.RestrictStart:
                case (int)ActionType.Effect:
                    returnValue += EditorGUIUtility.singleLineHeight * 2;
                    break;
                case (int)ActionType.Spawn:
                    returnValue += EditorGUIUtility.singleLineHeight * 3;
                    break;
            }

            return returnValue;
        }
    }
}