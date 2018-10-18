using UnityEngine;
using UnityEditor;

namespace AppCenterEditor
{
    public class AppCenterEditorHeader : Editor
    {
        public static void DrawHeader(float progress = 0f)
        {
            if (AppCenterEditorHelper.uiStyle == null)
                return;

            //using Begin Vertical as our container.
            using (new AppCenterGuiFieldHelper.UnityHorizontal(GUILayout.Height(52)))
            {
                EditorGUILayout.LabelField("", AppCenterEditorHelper.uiStyle.GetStyle("acLogo"), GUILayout.MaxHeight(60), GUILayout.Width(60));
                EditorGUILayout.LabelField("App Center", AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray2"), GUILayout.MinHeight(52));

                float gmAnchor = EditorGUIUtility.currentViewWidth - 30;


                if (EditorGUIUtility.currentViewWidth > 375)
                {
                    gmAnchor = EditorGUIUtility.currentViewWidth - 140;
                    GUILayout.BeginArea(new Rect(gmAnchor, 10, 140, 42));
                    GUILayout.BeginHorizontal();
                }
                else
                {
                    GUILayout.BeginArea(new Rect(gmAnchor, 10, EditorGUIUtility.currentViewWidth * .25f, 42));
                    GUILayout.BeginHorizontal();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                //end the vertical container
            }

            ProgressBar.Draw();
        }
    }
}
