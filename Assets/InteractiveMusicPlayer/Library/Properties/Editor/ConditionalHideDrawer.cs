﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
class ConditionalHideDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //get the attribute data
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        //check if the propery we want to draw should be enabled
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        //Enable/disable the property
        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;

        //Check if we should draw the property
        if (!condHAtt.HideInInspector || enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        //Ensure that the next property that is being drawn uses the correct settings
        GUI.enabled = wasEnabled;
    }


    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        
        bool enabled = true;
        
        string[] variableName = property.propertyPath.Split('.');
        if (variableName.Length <= 0) Debug.LogError("Property Path of EnumHide Attribute is not valid.");
        SerializedObject sourceObject = property.serializedObject;
        SerializedProperty sourceProperty = sourceObject.FindProperty(variableName[0]);
        string sourcePropertyPath = sourceProperty.propertyPath; 
        
        string conditionPath = sourcePropertyPath.Replace(sourceProperty.name, condHAtt.ConditionalSourceField);
        SerializedProperty resultProperty = sourceObject.FindProperty(conditionPath);
        
        if (variableName.Length > 1 && variableName[1] == "Array") // Property is an element insied an array
        {
            // WARNING: ConditionalHide is not fully supported on Array objects.
            
        }
        if (resultProperty != null)
        {
            enabled = resultProperty.boolValue;
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        return enabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            //The property is not being drawn
            //We want to undo the spacing added before and after the property
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }


}