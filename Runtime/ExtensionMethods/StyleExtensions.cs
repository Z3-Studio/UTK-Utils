using UnityEngine.UIElements;

namespace Z3.Utils.ExtensionMethods
{
    public static class StyleExtensions
    {
        public static void AddToClassList(this VisualElement visualElement, string[] classes)
        {
            foreach (string className in classes)
            {
                visualElement.AddToClassList(className);
            }
        }

        public static void AddAlignedStyle(this VisualElement visualElement)
        {
            visualElement.AddToClassList(BaseField<object>.alignedFieldUssClassName);
        }

        public static void SetDisplay(this VisualElement visualElement, bool visible)
        {
            SetDisplay(visualElement.style, visible);
        }

        public static void SetDisplay(this IStyle style, bool visible)
        {
            style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static void SetFullVisibility(this IStyle style, bool visible)
        {
            style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        public static void SetBorderColor(this IStyle style, StyleColor color)
        {
            style.borderTopColor = color;
            style.borderBottomColor = color;
            style.borderLeftColor = color;
            style.borderRightColor = color;
        }

        public static void SetBorderWidth(this IStyle style, StyleFloat size)
        {
            style.borderTopWidth = size;
            style.borderBottomWidth = size;
            style.borderLeftWidth = size;
            style.borderRightWidth = size;
        }

        public static void SetBorderRadius(this IStyle style, StyleLength size)
        {
            style.borderTopLeftRadius = size;
            style.borderTopRightRadius = size;
            style.borderBottomLeftRadius = size;
            style.borderBottomRightRadius = size;
        }

        public static void SetPosition(this IStyle style, StyleLength size)
        {
            style.left = size;
            style.right = size;
            style.top = size;
            style.bottom = size;
        }

        /// <summary> Outside of content </summary>
        public static void SetMargin(this IStyle style, StyleLength size)
        {
            style.marginTop = size;
            style.marginBottom = size;
            style.marginLeft = size;
            style.marginRight = size;
        }

        /// <summary> Contents inside </summary>
        public static void SetPadding(this IStyle style, StyleLength size)
        {
            style.paddingTop = size;
            style.paddingBottom = size;
            style.paddingLeft = size;
            style.paddingRight = size;
        }

        public static void SetAsExpanded(this IStyle style)
        {
            style.flexBasis = new Length(100, LengthUnit.Percent);
            style.flexShrink = new StyleFloat(StyleKeyword.Initial);
        }
    }
}