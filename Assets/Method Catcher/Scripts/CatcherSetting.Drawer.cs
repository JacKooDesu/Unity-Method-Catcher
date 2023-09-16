using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MethodCatcher
{
    public partial class CatcherSetting
    {
#if UNITY_EDITOR
        [SerializeField] int selectedAssembly = 0;
        [SerializeField] int selectedType = 0;
        [SerializeField] int selectedMethod = 0;

        [CustomPropertyDrawer(typeof(CatcherSetting))]
        public class Drawer : PropertyDrawer
        {
            float height;

            string[] assemblies;
            string[] types;
            string[] methods;

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
                height;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                height = 0;
                EditorGUI.BeginProperty(position, label, property);
                {
                    var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    position.y += EditorGUIUtility.singleLineHeight;
                    height += EditorGUIUtility.singleLineHeight;

                    property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, property.displayName);
                    if (!property.isExpanded)
                        goto End;

                    EditorGUI.indentLevel++;
                    {
                        var serializedAssembly = property.FindPropertyRelative(nameof(TargetAssembly));
                        var serializedType = property.FindPropertyRelative(nameof(TargetType));
                        var serializedMethod = property.FindPropertyRelative(nameof(TargetMethod));

                        var serializedSelectedAssembly = property.FindPropertyRelative(nameof(selectedAssembly));
                        var serializedSelectedType = property.FindPropertyRelative(nameof(selectedType));
                        var serializedSelectedMethod = property.FindPropertyRelative(nameof(selectedMethod));

                        assemblies = _FlattenDict.Keys.ToArray();
                        if (assemblies.Length == 0)
                        {
                            var warningRect = CalRect(EditorGUIUtility.singleLineHeight, ref height, ref position);
                            EditorGUI.LabelField(warningRect, "Assembly batching is not complete!!");
                            goto EndBlock;
                        }

                        serializedAssembly.stringValue = assemblies[serializedSelectedAssembly.intValue];
                        if (_FlattenDict.TryGetValue(assemblies[serializedSelectedAssembly.intValue], out var tDict))
                        {
                            types = tDict.Keys.ToArray();
                            serializedType.stringValue = types[serializedSelectedType.intValue];
                            if (tDict.TryGetValue(types[serializedSelectedType.intValue], out var mArr))
                            {
                                methods = mArr;
                                serializedMethod.stringValue = methods[serializedSelectedMethod.intValue];
                            }
                        }

                        // var assemblyRect = CalRect(serializedAssembly, ref height, ref position);
                        // var typeRect = CalRect(serializedType, ref height, ref position);
                        // var methodRect = CalRect(serializedMethod, ref height, ref position);

                        var selectedAssemblyRect = CalRect(EditorGUIUtility.singleLineHeight, ref height, ref position);
                        var selectedTypeRect = CalRect(EditorGUIUtility.singleLineHeight, ref height, ref position);
                        var selectedMethodRect = CalRect(EditorGUIUtility.singleLineHeight, ref height, ref position);

                        serializedSelectedAssembly.intValue =
                            EditorGUI.Popup(selectedAssemblyRect, "Select Assembly", serializedSelectedAssembly.intValue, assemblies);
                        serializedSelectedType.intValue =
                            EditorGUI.Popup(selectedTypeRect, "Select Type", serializedSelectedType.intValue, types);
                        serializedSelectedMethod.intValue =
                            EditorGUI.Popup(selectedMethodRect, "Select Method", serializedSelectedMethod.intValue, methods);

                        // EditorGUI.PropertyField(assemblyRect, serializedAssembly, true);
                        // EditorGUI.PropertyField(typeRect, serializedType, true);
                        // EditorGUI.PropertyField(methodRect, serializedMethod, true);
                    }
                EndBlock:
                    EditorGUI.indentLevel--;
                }

            End:
                EditorGUI.EndProperty();
            }

            Rect CalRect(SerializedProperty sp, ref float height, ref Rect position) =>
                CalRect(EditorGUI.GetPropertyHeight(sp), ref height, ref position);
            Rect CalRect(float h, ref float height, ref Rect position)
            {
                var rect = new Rect(
                    position.x, position.y,
                    position.width, h);
                position.y += h;
                height += h;
                return rect;
            }
        }
#endif
    }
}
