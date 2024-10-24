using UnityEditor;

namespace Cainos.LucidEditor
{
    public static class SerializedPropertyUtility
    {
        public static System.Type GetUnderlyingType(this SerializedProperty property)
        {
            var parentType = property.serializedObject.targetObject.GetType();
            var fi = parentType.GetFieldViaPath(property.propertyPath);
            return fi.FieldType;
        }

        public static System.Reflection.FieldInfo GetFieldViaPath(this System.Type type, string path)
        {
            var parentType = type;
            var fi = type.GetField(path);
            var perDot = path.Split('.');
            foreach (var fieldName in perDot)
            {
                fi = parentType.GetField(fieldName);
                if (fi != null)
                    parentType = fi.FieldType;
                else
                    return null;
            }
            if (fi != null)
                return fi;
            else return null;
        }
    }
}