using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using PlayerSystem.State.Attack;
using PlayerSystem.State.Base;

/// <summary>
/// ステートマシンのための各enumごとのクラスのテンプレートを生成する機能
/// </summary>
public class StateTemplateGenerator : EditorWindow
{
    private enum StateCategory
    {
        Base,
        Attack,
    }
    
    private StateCategory _selectedCategory = StateCategory.Base;
    private BaseStateEnum _selectedBaseStateEnum;
    private AttackStateEnum _selectedAttackStateEnum;

    [MenuItem("Tools/State Template Generator")]
    public static void ShowWindow()
    {
        GetWindow<StateTemplateGenerator>("State Template Generator");
    }

    private void OnGUI()
    {
        _selectedCategory = (StateCategory)EditorGUILayout.EnumPopup("Select Category", _selectedCategory);

        switch (_selectedCategory)
        {
            case StateCategory.Base:
                _selectedBaseStateEnum = (BaseStateEnum)EditorGUILayout.EnumPopup("Select Base Enum", _selectedBaseStateEnum);
                break;
            case StateCategory.Attack:
                _selectedAttackStateEnum = (AttackStateEnum)EditorGUILayout.EnumPopup("Select Attack Enum", _selectedAttackStateEnum);
                break;
        }
        
        if (GUILayout.Button("Generate State Template"))
        {
            switch (_selectedCategory)
            {
                case StateCategory.Base:
                    GenerateStateTemplate(_selectedBaseStateEnum.ToString(),"PlayerSystem.State.Base", "BaseStateEnum");
                    break;
                case StateCategory.Attack:
                    GenerateStateTemplate(_selectedAttackStateEnum.ToString(), "PlayerSystem.State.Attack", "AttackStateEnum");
                    break;
            }
            
        }
    }

    private void GenerateStateTemplate(string stateEnumName, string namespaceName, string enumType)
    {
        string stateClassName = $"{stateEnumName}State";
        string stateNamespace = namespaceName;

        string scriptContent = $@"
using Cysharp.Threading.Tasks;

namespace {stateNamespace} 
{{
    /// <summary>
    /// {stateEnumName}状態 
    /// </summary>
    public class {stateClassName} : PlayerBaseState<{enumType}> 
    {{
        public {stateClassName}(IPlayerStateMachine stateMachine) : base(stateMachine) {{ }}

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {{
            await UniTask.Yield();
        }}

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {{
            while (StateMachine.CurrentState.Value == {enumType}.{stateEnumName})
            {{
                await UniTask.Yield();
            }}
        }}

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {{
            await UniTask.Yield();
        }}
    }}
}}";

        // 保存先のディレクトリを選択
        string path = _selectedCategory == StateCategory.Attack 
            ? $"Assets/Scripts/Character/PlayerSystem/State/Attack/{stateClassName}.cs"
            : $"Assets/Scripts/Character/PlayerSystem/State/Base/{stateClassName}.cs";

        // スクリプトをファイルに書き出し
        File.WriteAllText(path, scriptContent);
        AssetDatabase.Refresh();
        Debug.Log($"State template for {stateEnumName} generated at {path}");
    }
}
