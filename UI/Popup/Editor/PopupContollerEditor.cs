using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Waker.Popups.Editors
{
    [CustomEditor(typeof(PopupController))]
    public class PopupControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate MyPopups.cs"))
            {
                PopupController instance = target as PopupController;

                var popups = instance.GetComponentsInChildren<PopupBase>();
                string code = CreateCode(popups);
                CreateScript(code);
            }
        }

        private string CreateCode(IEnumerable<PopupBase> popups)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("using Waker.Popups;");
            sb.AppendLine("using GoStopLandlord;");
            sb.AppendLine("// PopupController.cs에 의해 자동으로 생성된 스크립트입니다. 네임스페이스를 추가하거나 에러를 수정하는 등 자유롭게 수정 가능합니다.");
            sb.AppendLine("public static class MyPopups");
            sb.AppendLine("{");

            foreach (var popup in popups)
            {
                string typeName = popup.GetType().Name;
                string popupName = popup.PopupName;
                string propertyName = String.Concat(popupName.Where(c => !Char.IsWhiteSpace(c)));
                sb.AppendLine($"\tpublic static {typeName} {propertyName} => PopupController.Instance.Get<{typeName}>(\"{popupName}\");");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        private void CreateScript(string code)
        {
            File.WriteAllText("Assets/MyPopups.cs", code);
            AssetDatabase.Refresh();
        }
    }
    
}