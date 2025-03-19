using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FrameChecker))]
public class FrameCheckerDrawer : PropertyDrawer
{
    private const int yDistance = 20;
    private const int fieldHeight = 16;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty hitFrameStart = property.FindPropertyRelative("hitFrameStart");
        SerializedProperty hitFrameEnd = property.FindPropertyRelative("hitFrameEnd");
        SerializedProperty totalFrames = property.FindPropertyRelative("totalFrames");
        SerializedProperty animationFrameInfo = property.FindPropertyRelative("animationFrameInfo");

        Rect nameRect = new Rect(position.x, position.y, position.width, fieldHeight);
        Rect framesRect = new Rect(position.x, position.y + yDistance, position.width, fieldHeight);
        Rect hitFramesRect = new Rect(position.x, position.y + yDistance * 2, position.width, fieldHeight);
        Rect sliderRect = new Rect(position.x, position.y + yDistance * 3, position.width, fieldHeight);
        Rect animationFrameInfoRect = new Rect(position.x, position.y + yDistance * 4, position.width,EditorGUI.GetPropertyHeight(animationFrameInfo));

        label = EditorGUI.BeginProperty(position, label, property);
        EditorGUI.LabelField(nameRect, property.displayName);
        EditorGUI.indentLevel++;
        FrameRangeSlider(ref hitFrameStart, ref hitFrameEnd, totalFrames.intValue, framesRect, hitFramesRect, sliderRect);
        EditorGUI.indentLevel--;
        EditorGUI.PropertyField(animationFrameInfoRect, animationFrameInfo, true);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty animationFrameInfo = property.FindPropertyRelative("animationFrameInfo");
        
        return yDistance * 4 + EditorGUI.GetPropertyHeight(animationFrameInfo);
    }

    public void FrameRangeSlider(ref SerializedProperty hitFrameStart, ref SerializedProperty hitFrameEnd,
        int totalFrames, Rect framesRect, Rect hitRect, Rect sliderRect)
    {
        //Must be a float to use the MinMaxSlider
        float start = hitFrameStart.intValue;
        float end = hitFrameEnd.intValue;

        GUIStyle style = new GUIStyle(EditorStyles.helpBox);
        style.richText = true; //enables more text options like color or format

        string frames = "<b>" + totalFrames + "</b> frames";
        string startUp = "<color=green><b>" + (start - 1) + "</b> startup</color>";
        string active = "<color=red><b>" + (end - start + 1) + "</b> active</color>";
        string recovery = "<color=blue><b>" + (totalFrames - end) + "</b> recovery</color>";
        
        EditorGUI.LabelField(framesRect, frames + ": " + startUp + " | " + active + " | " + recovery, style);
        EditorGUI.LabelField(hitRect, "Hits in:  <b>" + start + "</b> to <b>" + end + "</b>", style);
        
        //and here we add the MinMaxSlider with our values
        EditorGUI.BeginChangeCheck();
        EditorGUI.MinMaxSlider(sliderRect, ref start, ref end, 1, totalFrames);
        if (EditorGUI.EndChangeCheck())
        {
            hitFrameStart.intValue = Mathf.RoundToInt(start);
            hitFrameEnd.intValue = Mathf.RoundToInt(end);
        }
    }
}
